using System;
using System.Collections.Generic;
using Spirometry.Statics;
using TMPro;
using UnityEngine;

namespace Spirometry.Debugging
{
    /// <summary>
    /// Spiro-Play project by University of Twente and MST
    /// Made by Koen Vogel - k.a.vogel@student.utwente.nl
    /// 
    /// Debugger class:
    /// Visualizing data for debugging purposes
    /// </summary>

    public class Debugger : MonoBehaviour
    {
        #region variables
        #pragma warning disable 649

        [Header("References")]
        [SerializeField] private TextMeshProUGUI displayText;
        [SerializeField] private TextMeshProUGUI consoleText;

        [Header("Settings")]
        [SerializeField] private bool ResetOnNewDisplay;
        [SerializeField] private bool DrawConsoleToDebugger;
        [SerializeField] private int amountOfMessages;

        #pragma warning restore 649
        #endregion

        #region Singleton
    
        //singleton pattern
        private static Debugger _instance;
        public static Debugger Instance { get { return _instance; } }
        private void Awake()
        {
            //singleton pattern (https://wiki.unity3d.com/index.php/Singleton)
            if (_instance != null && _instance != this)
                Destroy(this.gameObject);
            else
                _instance = this;
            
            DontDestroyOnLoad(this.gameObject);
        }
        #endregion

    
        private List<string> consoledata = new List<string>();

        private void Start()
        {
            //handle console messages
            Application.logMessageReceived += Instance.HandleConsoleMessages;
        }

        private void HandleConsoleMessages(string message, string trace, LogType type)
        {
            if (!DrawConsoleToDebugger) return;
            consoleText.text = "";
        
            consoledata.Insert(0, message);
        
            if (consoledata.Count > amountOfMessages + 1)
                consoledata.RemoveRange(amountOfMessages, consoledata.Count - amountOfMessages);
            
            foreach (var v in consoledata)
            {
                consoleText.text += v + "\n";
            }
        }
    
        //testing methods
        public void Display(List<float> list, string name)
        {
            //clear display
            if (ResetOnNewDisplay)
                ClearDisplay();

            string FinalOutput = "";
            foreach (var f in list)
            {
                FinalOutput += f.ToString();
                FinalOutput += " - ";
            }

            //header text
            if (name != "")
                displayText.text += "\n" + "Displaying: " + name;

            displayText.text += "\n" + FinalOutput;
        }

        public void Display(string text, string name)
        {
            //clear display
            if (ResetOnNewDisplay)
                ClearDisplay();

            //header text
            if (name != "")
                displayText.text += "\n" + "Displaying: " + name;

            displayText.text += "\n" + text + "\n";
        }

        public void Display(string text)
        {
            //clear display
            if (ResetOnNewDisplay)
                ClearDisplay();
            displayText.text += "\n" + text + "\n";
        }

        public void Display(float number, string name)
        {
            //clear display
            if (ResetOnNewDisplay)
                ClearDisplay();

            //header text
            if (name != "")
                displayText.text += "\n" + "Displaying: " + name;

            displayText.text += "\n" + number + "\n";
        }

        public void Display(int number, string name)
        {
            //clear display
            if (ResetOnNewDisplay)
                ClearDisplay();

            //header text
            if (name != "")
                displayText.text += "\n" + "Displaying: " + name;

            displayText.text += "\n" + number + "\n";
        }

        public void Display(string[] array, string name)
        {
            //clear display
            if (ResetOnNewDisplay)
                ClearDisplay();

            //header text
            if (name != "")
                displayText.text += "\n" + "Displaying: " + name;

            displayText.text += "\n";
            for (int i = 0; i < array.Length; i++)
                displayText.text += array[i] + "\n";
        }

        public void Display(List<string> array, string name)
        {
            //clear display
            if (ResetOnNewDisplay)
                ClearDisplay();

            //header text
            if (name != "")
                displayText.text += "\n" + "Displaying: " + name;

            displayText.text += "\n";
            for (int i = 0; i < array.Count; i++)
                displayText.text += array[i] + "\n";
        }

        public void Display(List<Vector2> list, string name)
        {
            //clear display
            if (ResetOnNewDisplay)
                ClearDisplay();

            //header text
            if (name != "")
                displayText.text += "\n" + "Displaying: " + name;

            displayText.text += "\n";
            for (int i = 0; i < list.Count; i++)
                displayText.text += list[i].x + " - " + list[i].y + "\n";
            displayText.text += "\n";
        }

        public void Display(List<SpiroData> list, string name)
        {
            //clear display
            if (ResetOnNewDisplay)
                ClearDisplay();

            //header text
            if (name != "")
                displayText.text += "\n" + "Displaying: " + name;

            //data display
            displayText.text += "\n";
            foreach (var point in list)
            {
                if (point.Volume != 999) displayText.text += "Volume: " + point.Volume;
                if (point.Timestamp != null && point.Timestamp != default(DateTime)) displayText.text += " - " + "Timestamp: " + point.Timestamp;
                if (point.Flow != 999) displayText.text += " - " + "Flow: " + point.Flow;
                displayText.text += "\n";
            }
            displayText.text += "\n";
        }

        public void Display(bool[] array, string name)
        {
            //clear display
            if (ResetOnNewDisplay)
                ClearDisplay();

            //header text
            if (name != "")
                displayText.text += "\n" + "Displaying: " + name;

            displayText.text += "\n";
            for (int i = 0; i < array.Length; i++)
                displayText.text += array[i] + "\n";
        }

        public void Display(List<bool> array, string name)
        {
            //clear display
            if (ResetOnNewDisplay)
                ClearDisplay();

            //header text
            if (name != "")
                displayText.text += "\n" + "Displaying: " + name;

            displayText.text += "\n";
            for (int i = 0; i < array.Count; i++)
                displayText.text += array[i] + "\n";
        }

        public void ClearDisplay()
        {
            displayText.text = "";
        }
    }
}
