using System;
using System.Collections.Generic;
using System.IO;
using System.Text.RegularExpressions;
using AwesomeCharts;
using Spirometry.ScriptableObjects;
using Spirometry.Statics;
using UnityEngine;
using UnityEngine.Assertions.Comparers;
using UnityEngine.UI;
using Logger = Spirometry.SpiroController.Logger;
using Math = Spirometry.Statics.Math;

namespace Menus.Menu_History
{
    public class HistoryInfoPanel : MonoBehaviour
    {
        /// <summary>
        /// Spiro-Play project by University of Twente, MST and Deventer ziekenhuis
        /// Made by Koen Vogel - k.a.vogel@student.utwente.nl
        /// 
        /// HistoryInfoPanel class:
        /// Canvas controller to visualize content of logs in the form of metadata text and graph of raw data
        /// </summary>
        
        #region variables
        #pragma warning disable 649

        [Header("Panel Elements")]
        [SerializeField] private GameObject Panel;
        [SerializeField] private TMPro.TextMeshProUGUI text;
        [SerializeField] private RawImage image;
        [SerializeField] private TMPro.TextMeshProUGUI headerText;

        #pragma warning restore 649
        #endregion

        private void Start()
        {
            //clear panel
            image.gameObject.SetActive(false);
            text.gameObject.SetActive(false);
            headerText.gameObject.SetActive(false);
        }

        //switch right panel view to graph screenshot
        public void NewGraph(string filename)
        {
            //setup panel
            headerText.gameObject.SetActive(true);
            image.gameObject.SetActive(true);
            text.gameObject.SetActive(false);
            headerText.text = "Grafiek";

            //get image from disk and display it on UI
            var pathImage = filename + ".jpg";
            Vector2 size = new Vector2(Screen.width, Screen.height); // image size
            var texture = LoadImage(size, Path.GetFullPath(pathImage));
            image.texture = texture;
        }
        
        private static Texture2D LoadImage(Vector2 size, string filePath) {
         
            //Load file into memory
            byte[] bytes = File.ReadAllBytes(filePath);
            Texture2D texture = new Texture2D((int)size.x, (int)size.y, TextureFormat.RGB24, false);
            texture.filterMode = FilterMode.Trilinear;
            texture.LoadImage(bytes);
            return texture;
        }
        
        public void NewPanel(string filename)
        {
            //create path name
            var path_txt = filename + ".txt";
            var path_csv = filename + ".csv";
            var reader = new StreamReader(path_txt);
            var txt = reader.ReadToEnd();

            //set up panel
            headerText.gameObject.SetActive(true);
            text.gameObject.SetActive(true);
            image.gameObject.SetActive(false);
            headerText.text = "Informatie";
            text.text = txt;

            /*
            //read and parse csv
            reader = new StreamReader(path_csv);
            var csv = reader.ReadToEnd();
            string[] rows = Regex.Split(csv, "\r\n");
            var data = new List<float>();

            for (var index = 1; index < rows.Length; index++)
            {
                var row = rows[index];
                string[] stringFloats = Regex.Split(row, ",");
                string stringFloat = stringFloats[1];
                float realFloat = float.Parse(stringFloat);
                data.Add(realFloat);
            }

            //parse relevant data
            var parsedData = new List<SpiroData>();
            foreach (var point in data)
            {
                parsedData.Add(new SpiroData(default(DateTime), 999, point));
            }
            */
        }
    }
}