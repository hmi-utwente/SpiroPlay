using System;
using System.Collections.Generic;
using Spirometry.Statics;
using UnityEngine;
using System.IO;
using System.Linq;
using Logger = Spirometry.SpiroController.Logger;

namespace Spirometry.ScriptableObjects
{
    /// <summary>
    /// Spiro-Play project by University of Twente and MST
    /// Made by Koen Vogel - k.a.vogel@student.utwente.nl
    /// 
    /// Storage class:
    /// File on disk to store users, results etc
    /// </summary>

    //[CreateAssetMenu(fileName = "Storage", menuName = "Spiro-Play/Storage Object", order = 0)]
    public class UserStorage : MonoBehaviour
    {
        //public List<SpiroResult> results = new List<SpiroResult>();
        public static List<User> Users = new List<User>();
        public const int AmountOfParams = 44;

        [System.Serializable]
        public class User
        {
            //user specific parameters
            public string userName;
            public float age;
            public float predictedFvc;
            public float predictedPef;
            public int coins;
            public DateTime creationDate;
            public bool[] metaphorStatus;
            public DateTime lastPlayedDate;
            public DateTime lastUnlockDate;
            public int lastPlayedAmountOfSessions;
            public int currentSessionID;
            
            //automatic shortcut properties
            public int DaysSinceCreation => (int)(DateTime.Now - creationDate).TotalDays;
            public int WeeksSinceCreation => (int)((DateTime.Now - creationDate).TotalDays / 7f);
            public int DaysSinceLastPlayed => (int)(DateTime.Now - lastPlayedDate).TotalDays;
            public int DaysSinceLastUnlock => (int)(DateTime.Now - lastUnlockDate).TotalDays;

            public User(string name, float age, float predictedFvc, float predictedPef, int coins, DateTime creationDate, bool[] metaphorInfo, DateTime lastPlayedDate, int lastPlayedAmountOfSessions, DateTime lastUnlockDate, int sessionID)
            {
                //user variables
                this.userName = name;
                this.age = age;
                this.predictedFvc = predictedFvc;
                this.predictedPef = predictedPef;
                this.coins = coins;
                this.creationDate = creationDate;
                this.metaphorStatus = metaphorInfo;
                this.lastPlayedDate = lastPlayedDate;
                this.lastPlayedAmountOfSessions = lastPlayedAmountOfSessions;
                this.lastUnlockDate = lastUnlockDate;
                this.currentSessionID = sessionID;
           }

            public void Save()
            {
                SaveUserToDisk(this);
            }
        }

        //save user info to file system
        private static void SaveUserToDisk(User user)
        {
            //get folder based on username
            var userFolder = Logger.RootFolder + "/" + user.userName;
            if (!Directory.Exists(userFolder)) Directory.CreateDirectory(userFolder);
            
            //create custom file type with extension of 'gebruiker'
            var userFilePath = userFolder + "/" + user.userName + ".gebruiker";
            if (File.Exists(userFilePath)) File.Delete(userFilePath);
            var sr = File.CreateText(userFilePath);
            var contents = "";

            //add all user parameters to savefile
            contents += user.userName + ":" + user.age + ":" + user.predictedFvc + ":" + user.predictedPef + ":" + user.coins;
            contents += ":" + user.creationDate.Year + ":" + user.creationDate.Month + ":" + user.creationDate.Day;
            contents += ":" + user.lastPlayedDate.Year + ":" + user.lastPlayedDate.Month + ":" + user.lastPlayedDate.Day;
            contents += ":" + user.lastPlayedAmountOfSessions;
            contents += ":" + user.lastUnlockDate.Year + ":" + user.lastUnlockDate.Month + ":" + user.lastUnlockDate.Day;
            contents += ":" + user.currentSessionID;
            contents += user.metaphorStatus.Aggregate(contents, (current, status) => current + (":" + status));
            
            //finish writing
            sr.Write(contents);
            sr.Close();
        }

        public static List<User> LoadUsers()
        {
            var output = new List<User>();
            var dir = new DirectoryInfo(Logger.RootFolder);

            var userFolders = dir.GetDirectories();
            Debug.Log("Attempting user load, found " + userFolders.Length + " subdirectories");
            foreach (var userFolder in userFolders)
            {
                /*var files = userFolder.GetFiles("*", SearchOption.AllDirectories);
                foreach (var file in files)
                {
                   Debug.Log("for folder " + userFolder.Name + ", found file " + file.Name + " with path " + file.Directory); 
                }*/
                var userFiles = userFolder.GetFiles("*" + ".gebruiker");
                Debug.Log("subFolder " + userFolder.Name + " found, with " + userFiles.Length + " user files in it");
                foreach (var file in userFiles)
                {
                    Debug.Log("user " + file.Name + " found on disk");
                    var reader = new StreamReader(file.ToString());
                    var text = reader.ReadToEnd();
                    reader.Close();
                    var data = text.Split(':');
                    Debug.Log("Users loaded from disk with " + data.Length + " parameters, content: " + text);
                    
                    //up to date user, loading in...
                    if (data.Length == AmountOfParams)
                    {
                        //parse parameters from string file
                        var username = data[0];
                        var age = float.Parse(data[1]);
                        var predFvc = float.Parse(data[2]);
                        var predPef = float.Parse(data[3]);
                        var coins = int.Parse(data[4]);
                        var creationDate = new DateTime(int.Parse(data[5]), int.Parse(data[6]), int.Parse(data[7]));
                        //print("loadUser params 8-10: " + data[8] + " "+ data[9] + " "+ data[10]);
                        var lastPlayedDate = new DateTime(int.Parse(data[8]), int.Parse(data[9]), int.Parse(data[10]));
                        var lastPlayedAmountOfSessions = int.Parse(data[11]);
                        var lastUnlockDate = new DateTime(int.Parse(data[12]), int.Parse(data[13]), int.Parse(data[14]));
                        //var currentSessionID = int.Parse(data[15]);
                        var currentSessionID = int.Parse(data[30]);
                        
                        //parse the unlock status for every metaphor
                        var metaphorStatus = new List<bool>();
                        for (var i = 31; i < 44; i++)
                        {
                            var value = bool.Parse(data[i]);
                            metaphorStatus.Add(value);
                        }
                        
                        Debug.Log("Loaded user " + username + "with " + metaphorStatus.Count + " metaphor data points");
                        
                        var newUser = new User(username, age, predFvc, predPef, coins, creationDate, metaphorStatus.ToArray(), lastPlayedDate, lastPlayedAmountOfSessions, lastUnlockDate, currentSessionID);
                        output.Add(newUser);
                    }
                    //out of date user file, removing user
                    else
                    {
                        var userName = data[0];
                        Debug.Log(userName + " has been deleted due to incompatible app update");
                        ConnectionCanvas.Instance.Warn_Custom("Gebruiker '" + userName + "' is verwijderd door een update van de app");
                        Logger.DeleteFolder(userName);
                    }
                }
            }
            return output;
        }
    }
}