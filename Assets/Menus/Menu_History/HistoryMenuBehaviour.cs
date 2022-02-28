using System;
using System.Collections.Generic;
using Spirometry.ScriptableObjects;
using Spirometry.Statics;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;
using Spirometry.SpiroController;
using Logger = Spirometry.SpiroController.Logger;

namespace Menus.Menu_History
{
    public class HistoryMenuBehaviour : MonoBehaviour
    {
        /// <summary>
        /// Spiro-Play project by University of Twente, MST and Deventer ziekenhuis
        /// Made by Koen Vogel - k.a.vogel@student.utwente.nl
        /// 
        /// HistoryMenuBehaviour class:
        /// Canvas controller to visualize logs in the form of a scrollable list with indexed logs per user
        /// </summary>
        
        #region variables
        #pragma warning disable 649

        [Header("General")]
        [SerializeField] private UserStorage userStorage;
        [SerializeField] private Settings Settings;

        [Header("UI")]
        [SerializeField] private TMPro.TextMeshProUGUI TitleHeader;
        [SerializeField] private TMPro.TextMeshProUGUI StatusText;
        [SerializeField] private HistoryInfoPanel Panel;
        [SerializeField] private Transform ButtonParent;
        [SerializeField] private GameObject ButtonPrefab;
        [SerializeField] private GameObject DividerPrefab;

        #pragma warning restore 649
        #endregion

        private void Start()
        {
            //clear the page
            StatusText.text = "";

            if (SessionManager.CurrentUser == null || SessionManager.CurrentUser.userName == "")
            {
                StatusText.text = "Er is niemand ingelogd";
                return;
            }
            
            //set title
            TitleHeader.text = "Geschiedenis van " + SessionManager.CurrentUser.userName;

            var folderPath = Logger.RootFolder + "/" + SessionManager.CurrentUser.userName;
            var dir = new DirectoryInfo(folderPath);

            //list all files in the directory sorted by creation time
            var allFiles = dir.GetFiles().OrderByDescending(f => f.CreationTime).ToArray();

            //if empty array
            if (allFiles.Length <= 1)
            {
                StatusText.text = "Je hebt nog geen tests gedaan";
                return;
            }

            //fetch and display all logs
            GenerateList(allFiles);
        }

        private void GenerateList(FileInfo[] allFiles)
        {
            //filter out the textual logs
            var fileList = (from file in allFiles where file.ToString().Contains(".txt") select file).ToList();
            print("history menu first scan found logs: " + fileList.Count);
            var separator = new string[] {"_"};
            //sort the logs based on SID
            fileList = fileList.OrderBy(s => int.Parse(s.Name.Split(separator, StringSplitOptions.None)[3].Replace("SID", "").Replace(".txt",""))).ToList();
            
            /*var split = titleList[0].Split(separator, StringSplitOptions.None);
            var printitem = "history item log split into " + split.Length + ", content:";
            printitem = split.Aggregate(printitem, (current, s) => current + " " + s);
            Debug.Log(printitem);
            Debug.Log("parsing log title, sid: " + titleList[0].Split(separator, StringSplitOptions.None)[3].Replace("SID", ""));*/
            
            //split file list into groups based on SID
            var sessionGroup = new List<FileInfo[]>();
            var currSid = "";
            var currSesh = new List<FileInfo>();
            for (var index = 0; index < fileList.Count; index++)
            {
                var sid = fileList[index].Name.Split(separator, StringSplitOptions.None)[3].Replace("SID", "").Replace(".txt", "");
                if (sid != currSid)
                {
                    if (index != 0) sessionGroup.Add(currSesh.ToArray());
                    currSesh.Clear();
                }
                currSid = sid;
                currSesh.Add(fileList[index]);
            }
            sessionGroup.Add(currSesh.ToArray());

            //sort both sessions and logs within sessions based on creation time
            sessionGroup = sessionGroup.OrderByDescending(s => s[0].CreationTime).ToList();

            print("history menu second scan (after grouping), found sessions: " + sessionGroup.Count);
            for (var index = 0; index < sessionGroup.Count; index++)
            {
                print("history menu second scan (after grouping), found session: " + index + " with logs: " + sessionGroup[index].Length);
                sessionGroup[index] = sessionGroup[index].OrderByDescending(s => s.CreationTime).ToArray();
            }

            //spawning of items
            foreach (var session in sessionGroup)
            {
                //get information like SID and metaphor to create header above each session
                var sid = session[0].Name.Split(separator, StringSplitOptions.None)[3].Replace("SID", "").Replace(".txt", "");
                var metaphor = Settings.GetDisplayName(Logger.ReadLogText(session[0]).metaphor);
                
                var div = Instantiate(DividerPrefab, ButtonParent);
                div.GetComponent<TMPro.TextMeshProUGUI>().text = metaphor + " (SID: " + sid + ")";

                //spawn buttons for all logs
                foreach (var file in session)
                {
                    Debug.Log("history button found with name: " + file.Name);
                    SpawnButton(file);
                }
            }
        }
        
        private void SpawnButton(FileInfo file)
        {
            //remove end of file name
            var title = file.FullName.Replace(".txt", "");

            //instantiating new history listItem button en assigning function to it
            GameObject button = GameObject.Instantiate(ButtonPrefab, ButtonParent);
            button.GetComponent<Button>().onClick.AddListener(delegate { Panel.NewPanel(title); });
            button.transform.GetChild(2).GetComponent<Button>().onClick.AddListener(delegate { Panel.NewGraph(title); });

            string[] data = Regex.Split(title, "_");
            //assign text elements of the button
            var dateText = button.transform.GetChild(0).GetComponent<TextMeshProUGUI>();
            var timeText = button.transform.GetChild(1).GetComponent<TextMeshProUGUI>();
            var date = data[1].Split('-');
            dateText.text = date[2] + "-" + date[1] + "-" + date[0];
            timeText.text = data[2].Replace("-", ":");
        }
        
        private void Update()
        {
            //catch android back button action, and reroute it to touch back button action
            if (Input.GetKeyDown(KeyCode.Escape)) BackToMenu();
        }

        public void BackToMenu()
        {
            LevelChanger.FadeToLevel("MainMenu");
        }

        public void ShareButton()
        {
            if (Application.platform != RuntimePlatform.Android || Application.platform == RuntimePlatform.IPhonePlayer)
            {
                Debug.Log("Cannot export logs on non-mobile platforms");
                return;
            }
            Debug.Log("Attempting export of user logs");
        
            //gather and check logs
            var logs = Logger.GetUserLogs();
            if (logs.Length <= 1)
            {
                ConnectionCanvas.Instance.Warn_Custom("Er zijn nog geen tests gedaan");
                return;
            }
            
            //make share data and export
            var shareInfo = new NativeShare();
            foreach (var log in logs) shareInfo.AddFile(log);
            shareInfo.SetSubject("Spiroplay logs");
            shareInfo.SetCallback((result, shareTarget) => Debug.Log("Share result: " + result + ", selected app: " + shareTarget));
            shareInfo.Share();
        }
    }
}
