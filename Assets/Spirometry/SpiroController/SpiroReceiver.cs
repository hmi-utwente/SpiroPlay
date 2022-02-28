using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text.RegularExpressions;
using System.Threading;
using Fleck;
using Spirometry.ScriptableObjects;
using Spirometry.Statics;
using UnityEngine;
using UnityEngine.Serialization;
using Event = Spirometry.ScriptableObjects.Event;
using Math = Spirometry.Statics.Math;

namespace Spirometry.SpiroController
{
    public class SpiroReceiver : MonoBehaviour
    {
        /// <summary>
        /// Spiro-Play project by University of Twente, MST and Deventer ziekenhuis
        /// Made by Koen Vogel - k.a.vogel@student.utwente.nl
        /// 
        /// SpiroReceiver class:
        /// Socket connection with spirometers, includes functionality like timeouts, parsing, and averaging of values
        /// </summary>
        
        #region variables
        #pragma warning disable 649
        
        [Header("Settings")]
        [SerializeField] private bool averageBatchValuesRt = true;
        [SerializeField] private string socketAdres = "ws://0.0.0.0:8181";
        [SerializeField] private bool loggingEnabled;
        
        [Header("References")]
        [SerializeField] private Datastream RTInput;
        [SerializeField] private Event onDisconnect;
        [SerializeField] private Event onConnect;
        
        //private variables
        private Thread _thread;
        private DateTime _lastReceivedValue;
        private bool _connected = false;
        private readonly TimeSpan _diconnectTimeout = new TimeSpan(0, 0, 0 ,0, 750);
        
        //publicly accessible peak flow with large sample rate
        public float peakFlow;

        #region Singleton pattern
        //singleton pattern
        public static SpiroReceiver Instance { get; private set; }

        private void Awake()
        {
            //singleton pattern (https://wiki.unity3d.com/index.php/Singleton)
            if (Instance != null && Instance != this)
                Destroy(this.gameObject);
            else
                Instance = this;
        }
        #endregion
        
        #pragma warning restore 649
        #endregion
        
        private void Start()
        {
            _thread = new Thread(InitWebsocket);
            _thread.Start();
        }

        //this functions starts up the web socket in another thread
        private void InitWebsocket()
        {
            var allSockets = new List<Fleck.IWebSocketConnection>();
            var server = new WebSocketServer(socketAdres);
            Debug.Log("Receiver: New thread started, waiting for socket connection...");
            server.Start(socket =>
            {
                socket.OnOpen = () =>
                {
                    Debug.Log("Opened websocket connection on port <i>" + socketAdres + "</i>");
                    allSockets.Add(socket);
                };
                socket.OnClose = () =>
                {
                    Debug.Log("Closed websocket connection");
                    allSockets.Remove(socket);
                };
                socket.OnMessage = message =>
                {
                    if (loggingEnabled) Debug.Log(message);
                    allSockets.ToList().ForEach(s => s.Send("Echo: " + message ));
                    ParseInput(message);
                };
            });
            
            //Transmit console logs back to socket
            var input = Console.ReadLine();
            while (input != "exit")
            {
                foreach (var socket in allSockets.ToList())
                {
                    socket.Send(input);
                }
                input = Console.ReadLine();
            }
        }

        private void ParseInput(string message)
        {
            //record timestamp
            _lastReceivedValue = DateTime.Now;
            
            //parse string
            var parsedMessage1 = message.Replace("[", "");
            var parsedMessage = parsedMessage1.Replace("]", "");
            string[] stringValues = Regex.Split(parsedMessage, ", ");
            var values = new List<float>();

            //report inconsistent batch sizes
            if (values.Count != 10 && loggingEnabled)
                Debug.Log("SpiroReceiver received " + values.Count + " values instead of 10");
            
            //parse string to float
            foreach (var thing in stringValues)
            {
                if (decimal.TryParse(thing, NumberStyles.Any, CultureInfo.InvariantCulture, out decimal result))
                    values.Add((float) result);
                else
                    Debug.Log("Receiver: " + thing + "was not able to be parsed");
            }
            
            var t = 100;
            foreach (var flow in values)
            {
                //calculate correct timestamp
                t -= 10;
                var span = new TimeSpan(0, 0, 0, 0, t);
                var time = DateTime.Now - span;
                
                //save imported unfiltered data to scriptableObject
                RTInput.Values.Add(new SpiroData(time, 999, flow));
                
                //check for peak flow in unaveraged data, this way the error detection can make use of high sample rates
                if (flow > peakFlow)
                    peakFlow = flow;
                
                //push raw unaveraged data to realtime if configured (watch out for performance loss)
                if (!averageBatchValuesRt)
                    RealtimeDataProcessor.Instance.ImportDataRT(time, flow, flow);
            }

            //if averaging batch values is enabled
            if (!averageBatchValuesRt) return;
            //import the data
            var averageFlow = Math.Average(values);
            if (loggingEnabled) Debug.Log("Average Flow: " + averageFlow);
            RealtimeDataProcessor.Instance.ImportDataRT(DateTime.Now, averageFlow, values.Max());
        }

        //detect timeout in spirometer connection
        private void Update()
        {
            //get the time since last value received
            var now = DateTime.Now;
            var idleDuration = now - _lastReceivedValue;

            //if long enough, trigger timeout and set connection state
            var timedOut = idleDuration > _diconnectTimeout;
            if (timedOut && _connected)
                onDisconnect.value.Invoke();
            if (!timedOut && !_connected)
                onConnect.value.Invoke();
            _connected = !timedOut;
        }
    }
}