using System;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using Spirometry.Statics;
using UnityEngine;

namespace Spirometry.SpiroController
{
    
    /// <summary>
    /// Spiro-Play project by University of Twente, MST and Deventer ziekenhuis
    /// Made by Koen Vogel - k.a.vogel@student.utwente.nl
    /// 
    /// FakeDataImporter class:
    /// Reads csv data of stored fake data files which can be used to simulate a spirometer
    /// </summary>
    
    public class FakeDataImporter : MonoBehaviour
    {
    
        #region Singleton Pattern

        //singleton pattern
        private static FakeDataImporter _instance;
        public static FakeDataImporter Instance { get { return _instance; } }

        private void Awake()
        {
            //singleton pattern (https://wiki.unity3d.com/index.php/Singleton)
            if (_instance != null && _instance != this)
                Destroy(this.gameObject);
            else
                _instance = this;
        }

        #endregion
    
        //data file paths
        [Header("Fake Data")]
        [Tooltip("Exact file name of the csv file contain fake data parameters, located in assets/resources")]
        public string ParametersFileName;
        [Tooltip("Exact file name of the csv file contain fake raw data, located in assets/resources")]
        public string RawDataFileName;

        #region parameters
        //parsing paramters of interest
        private List<float> FEV1 = new List<float>();
        private List<float> FEV05 = new List<float>();
        private List<float> PEF = new List<float>();
        private List<float> FEF2575 = new List<float>();
        //criteria 1a
        [HideInInspector]
        public List<float> FVC = new List<float>();
        [HideInInspector]
        public List<float> EV = new List<float>();
        //criteria 3a
        [HideInInspector]
        public List<float> FET = new List<float>();
        #endregion

        //the actual data
        [HideInInspector]
        public List<SpiroData>[] RawFunctions = new List<SpiroData>[10];


        // Start is called before the first frame update
        private void Start()
        {
            ImportFakeParameters();
            ImportFakeRawData();
        }
    
        //import all relevant data from the csv file into variables
        public void ImportFakeParameters()
        {
            //importing whole csv files into rows
            TextAsset DataFile = Resources.Load(ParametersFileName) as TextAsset;
            string[] Rows = Regex.Split(DataFile.text, RowSplitter);

            //parse relevant parameters into float lists;
            FEV1 = ParseRow(Rows[4], true);
            FEV05 = ParseRow(Rows[25], true);
            PEF = ParseRow(Rows[6], true);
            FEF2575 = ParseRow(Rows[8], true);
            //criteria 1a
            FVC = ParseRow(Rows[3], true);
            EV = ParseRow(Rows[10], true);
            //criteria 3a
            FET = ParseRow(Rows[9], true);
        }

        //import all relevant raw data from csv files into variables;
        public void ImportFakeRawData()
        {
            //import raw data file from filesystem
            TextAsset DataFileRaw = Resources.Load(RawDataFileName) as TextAsset;

            //initialize final output file
            List<SpiroData>[] output = new List<SpiroData>[10];
            for (int i = 0; i < output.Length; i++)
                output[i] = new List<SpiroData>();

            //split raw file into rows
            string[] rows = Regex.Split(DataFileRaw.text, RowSplitter);
            
            TimeSpan interval = new TimeSpan(0, 0, 0, 0, 10);
            DateTime time = DateTime.Now;
            
            for (int h = 1; h < rows.Length; h++)
            {
                //split every row into entries
                string[] row = Regex.Split(rows[h], EntrySplitter);

                int index = 0;
                //go through the row
                for (int v = 1; v < row.Length - 1; v += 2)
                {

                    //disregard empty and null values
                    if (row[v] != null && row[v + 1] != null)
                    {
                        if (row[v] != "" && row[v + 1] != "")
                        {
                            float volume, flow;
                            bool error = false;
                            //filter out NaN values
                            if (!float.TryParse(row[v], out volume))
                                error = true;
                            if (!float.TryParse(row[v + 1], out flow))
                                error = true;

                            //combine the parsed floats into a vector2(x, y)
                            if (!error)
                                output[index].Add(new SpiroData(time, -999, flow));
                        }
                    }
                    index++;
                }
                time += interval;
            }
            //set output
            for (int i = 0; i < RawFunctions.Length; i++)
            {
                RawFunctions[i] = new List<SpiroData>();
                RawFunctions[i] = output[i];
            }
        }

        //parsing formatters
        private const string RowSplitter = "\n";
        private const string EntrySplitter = ";";

        //filter out single rows from a raw csv string
        private List<float> ParseRow(string file, bool Trimmed)
        {
            string[] stringArray = Regex.Split(file, EntrySplitter);
            List<float> output = new List<float>();

            for (int i = 0; i < stringArray.Length; i++)
            {
                if (Trimmed)
                {
                    if (i != 0 && i != 1 && i != stringArray.Length - 1 && i != stringArray.Length - 2 && i != stringArray.Length - 3)
                        output.Add(float.Parse(stringArray[i]));
                }
                else
                    output.Add(float.Parse(stringArray[i]));
            }
            return output;
        }
    
    }
}
