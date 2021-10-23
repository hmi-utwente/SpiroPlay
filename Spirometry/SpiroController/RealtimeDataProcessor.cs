using System;
using System.Collections;
using System.Collections.Generic;
using Spirometry.Debugging;
using Spirometry.ScriptableObjects;
using Spirometry.Statics;
using UnityEngine;
using Event = Spirometry.ScriptableObjects.Event;

namespace Spirometry.SpiroController
{
    /// <summary>
    /// Spiro-Play project by University of Twente and MST
    /// Made by Koen Vogel - k.a.vogel@student.utwente.nl
    /// 
    /// RealtimeDataProcessor class:
    /// Imports data from files and parses it into usable variables
    /// </summary>

    public class RealtimeDataProcessor : MonoBehaviour
    {

        #region variables
        #pragma warning disable 649
    
        #region Singleton Pattern

        //singleton pattern
        private static RealtimeDataProcessor _instance;
        public static RealtimeDataProcessor Instance { get { return _instance; } }

        private void Awake()
        {
            //singleton pattern (https://wiki.unity3d.com/index.php/Singleton)
            if (_instance != null && _instance != this)
                Destroy(this.gameObject);
            else
                _instance = this;
        }
        
        #endregion
    
        //public variables
        [Header("References")]
        [SerializeField] private Datastream RTInput;
        [SerializeField] private Event StartDetected;
        [SerializeField] private Event EndDetected;
        [SerializeField] private Datastream RTOutput;
        [SerializeField] public Datastream TestFile;

        private enum LogLevel { NoLogging, UnfilteredToConsole, UnfilteredToDebugger, FilteredToConsole, FilteredToDebugger }
        [Header("Settings")]
        [SerializeField] private LogLevel loggingLevel;
        [Tooltip("Amount of time, in milliseconds, for the async data processor to compensate for, increase this for unstable connections")]
        [SerializeField] private int AsyncDelay = 100;
    
        #pragma warning restore 649
        #endregion

        private void Start()
        {
            const int amountOfExtraIndexes = 50;
            int startingIndex = 0;
            StartDetected.value.AddListener( (delegate
            {
                startingIndex = RTInput.Values.Count - 1 < amountOfExtraIndexes
                    ? 0
                    : RTInput.Values.Count - 1 - amountOfExtraIndexes;
            }));
            
            EndDetected.value.AddListener((delegate
            {
                Debug.Log("test recording...");
                int endingIndex = RTInput.Values.Count - 1;
                RecordTest(startingIndex, endingIndex);
            }));
        }

        //recording test
        private void RecordTest(int startIndex, int endIndex)
        {
            TestFile.Clear();
            for (int i = startIndex; i < endIndex; i++)
            {
                TestFile.Values.Add(RTInput.Values[i]);
            }
        }
        
        //IMPORTANT: call this from realtime emulator or realtime web socket to create the datastream
        public void ImportDataRT(DateTime newTimestamp, float newFlow, float newMaxFlow)
        {
            //debug
            if (loggingLevel == LogLevel.UnfilteredToConsole)
                Debug.Log("Index: " + RTInput.Count + "   <b>Flow:</b> " + RTOutput.Values[RTOutput.Count - 1].Timestamp + "     Flow: " + RTInput.Values[RTInput.Count - 1].Flow + "     Volume: " + RTInput.Values[RTInput.Count - 1].Volume);
            else if (loggingLevel == LogLevel.UnfilteredToDebugger && Debugger.Instance != null)
            {
                List<SpiroData> data = RTInput.Values;
                data.Reverse();
                Debugger.Instance.Display(data, "Incoming and unfiltered Realtime data");
            }

            //process timings and export to scriptableObject
            var point = new SpiroData(newTimestamp, 999, newFlow);
            point.MaxFlow = newMaxFlow;
            StartCoroutine(AsyncExport(point, AsyncDelay));
        }

        private IEnumerator AsyncExport(SpiroData point, int delayInMilliseconds)
        {
            //if there is no delay, immediately export point
            if (delayInMilliseconds == 0)
            {
                ExportDataRT(point);
            }
            else
            {
                //calculate delay and time to export
                TimeSpan delay = new TimeSpan(0, 0, 0, 0, delayInMilliseconds);
                DateTime timeToExport = point.Timestamp.Add(delay);

                //cancel if timeout is reached or timestamp is in the past
                if ((DateTime.Now - timeToExport) < new TimeSpan(0, 0, 0, 1))
                {
                    if (DateTime.Now > timeToExport)
                    {
                        Debug.Log("Exporting data point directly, because it was in the past");
                        ExportDataRT(point);
                    }
                    else
                    {
                        //wait for the right time
                        yield return new WaitUntil(() => DateTime.Now < timeToExport);

                        //when the right time has been reached
                        ExportDataRT(point);
                    }
                }
                else
                    Debug.Log("Discarded data point because the timestamp was too far in the future");
            }
            yield return null;
        }
    
        private void ExportDataRT(SpiroData filteredDataPoint)
        {
            //save to scriptableObject
            RTOutput.Add(filteredDataPoint);
        
            //debug
            if (loggingLevel == LogLevel.FilteredToConsole)
                Debug.Log("<b>Index:</b> " + RTOutput.Count + "   <b>Timestamp:</b> " + RTOutput.Values[RTOutput.Count - 1].Timestamp + "   <b>Flow:</b> " + RTOutput.Values[RTOutput.Count - 1].Flow + "   <b>Volume:</b> " + RTOutput.Values[RTOutput.Count - 1].Volume);
            else if (loggingLevel == LogLevel.FilteredToDebugger && Debugger.Instance != null)
            {
                List<SpiroData> data = RTOutput.Values;
                data.Reverse();
                Debugger.Instance.Display(data, "Processed and filtered Realtime data");
            }
        }
    }
}
