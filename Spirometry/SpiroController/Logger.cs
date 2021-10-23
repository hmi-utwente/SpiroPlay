using System;
using System.Collections;
using System.Collections.Generic;
using System.Globalization;
using Spirometry.Statics;
using UnityEngine;
using System.IO;
using System.Linq;
using Application = UnityEngine.Application;

namespace Spirometry.SpiroController
{
    public static class Logger
    {
        /// <summary>
        /// Spiro-Play project by University of Twente, MST and Deventer ziekenhuis
        /// Made by Koen Vogel - k.a.vogel@student.utwente.nl
        /// 
        /// Logger class:
        /// Responsible for parsing and logging files with raw data and metadata
        /// </summary>

        #region variables

        //public const string RootFolder = "/storage/emulated/0/Download/SpiroPlay";
        public static readonly string RootFolder = Application.persistentDataPath;
        
        private static string _userFolder;
        private static string _lastFileName;
        
        #endregion

        public static void SaveLog(SpiroResult result)
        {
            _userFolder = RootFolder + "/" + result.User.userName;
            var name = result.User.userName + "_" + result.TimeOfTest.Year + "-" + result.TimeOfTest.Month + "-" + result.TimeOfTest.Day + "_" + result.TimeOfTest.Hour + "-" + result.TimeOfTest.Minute + "-" + result.TimeOfTest.Second + "_SID" + result.SessionID;
            _lastFileName = name;
            
            try
            {
                WriteString(name, ConstructText(result), ".txt");
            }
            catch (Exception e)
            {
                Debug.LogError("Logger failed to write text file of name " + name + ", error message: " + e.Message);
                throw;
            }
            
            try
            {
                WriteString(name, ConstructCsv(result), ".csv");
            }
            catch (Exception e)
            {
                Debug.LogError("Logger failed to write csv file of name " + name + ", error message: " + e.Message);
                throw;
            }
        }
        
        private static string ConstructCsv(SpiroResult result)
        {
            var output = "Time,Flow";
            for (var i = 0; i < result.Validation.FullFunction.Count; i++)
            {
                /*
                //optional detailed timestamp information
                output += result.raw[i].Timestamp.Hour.ToString() + "/";
                output += result.raw[i].Timestamp.Minute.ToString() + "/";
                output += result.raw[i].Timestamp.Second.ToString() + "/";
                output += result.raw[i].Timestamp.Millisecond.ToString();
                */
                
                output += "\r\n";
                output += i;
                output += ",";
                
                //also print correct values if flow is 0
                var flow = result.Validation.FullFunction[i].Flow;
                output += flow != 0 ? flow.ToString(CultureInfo.InvariantCulture) : "0";
            }
            return output;
        }

        private static string ConstructText(SpiroResult result)
        {
            string output = "";
            //output += "\n";
            output += "-------------------- General ----------------------------" + "\n";
            output += "Subject nr: " + result.User.userName + "\n";
            output += "Date & Time: " + result.TimeOfTest + "\n";
            output += "Metaphor: " + result.Metaphor + "\n";
            output += "Predicted FVC: " + result.PredictedFVC + "\n";
            output += "Predicted PEF: " + result.PredictedPEF + "\n";
            output += "Session ID: " + result.SessionID + "\n";
            output += "Number of test in current session: " + result.IndexInSession + "\n";
            output += "\n";
            output += "-------- Error Criteria (true is no error) --------------" + "\n";
            output += "Criteria 1A (EV): " + result.Validation.Crit1A + "\n";
            output += "Criteria 1B (First peak): " + result.Validation.Crit1B + "\n";
            output += "Criteria 2A (Plateau): " + result.Validation.Crit2A + "\n";
            output += "Criteria 2B (3 sec): " + result.Validation.Crit2B + "\n";
            output += "Criteria 2C (Early termination): " + result.Validation.Crit2C + "\n";
            output += "Criteria 3 (Cough): " + result.Validation.Crit3 + "\n";
            output += "Criteria 4 (Gap inspir expir): " + result.Validation.Crit4 + "\n";
            output += "\n";
            output += "---- Recognized Errors (true is error detected) ---------" + "\n";
            output += "Errors recognized: " + !result.Validation.NoErrors + "\n";
            output += "Unsatisfactory start of expiration: " + result.Validation.UnsatisfactoryStart + "\n";
            output += "Premature end of expiration: " + result.Validation.PrematureEnd + "\n";
            output += "Cough detected: " + result.Validation.CoughDetected + "\n";
            output += "Given Feedback: " + result.FeedbackText + "\n";
            output += "\n";
            output += "-------------------- Parameters -------------------------" + "\n";
            output += "FIVC " + result.Validation.FIVC + " litres " + "\n";
            output += "FVC: " + result.Validation.FVC + " litres " + "\n";
            output += "FET: " + result.Validation.FET + " seconds " + "\n";
            output += "FEV1: " + result.Validation.FEV1 + " litres " + "\n";
            output += "FEV0.5: " + result.Validation.FEV05 + " litres " + "\n";
            output += "PEF: " + result.Validation.PEF + " litres per second" + "\n";
            output += "EV: " + result.Validation.EV + "\n";
            output += "FEF2575: " + result.Validation.FEF2575 + "\n";
            output += "Tiffeneau Index: " + result.Validation.TiffeneauIndex;
            return output;
        }

        public static List<SpiroResultSimple> GetSessionLogs(int sessionID)
        {
            //find correct user folder
            _userFolder = RootFolder + "/" + SessionManager.CurrentUser.userName;
            var dir = new DirectoryInfo(_userFolder);
            Debug.Log("scanning for logs in folder: " + dir.Name);
            
                //find logs for specific session ID
                var sessionFiles = dir.GetFiles(("*" + sessionID + ".txt"));
                Debug.Log("scanning for logs, found " + sessionFiles.Length + " logs");
                return sessionFiles.Select(file => ReadLogText(file)).ToList();
        }

        //read a log text and parse it to struct
        public static SpiroResultSimple ReadLogText(FileInfo file)
        {
            //read contents of log from disk
            var reader = new StreamReader(file.ToString());
            var text = reader.ReadToEnd();
            reader.Close();

            //preprocess
            text = text.Replace(" ", "");
            var data = text.Split('\n');
            
            //testing
            //Debug.Log("split read log file into " + data.Length + " pieces");
            //Debug.Log("fvc parsing: " + data[27].Split(':')[1].Split('l')[0]);
            //Debug.Log("fev1 parsing: " + data[29].Split(':')[1].Split('l')[0]);
            //Debug.Log("pef parsing: " + data[31].Split(':')[1].Split('l')[0]);
            //Debug.Log("crit1a parsing: " + data[10].Split(':')[1]);
            
            var fvc = float.Parse(data[27].Split(':')[1].Split('l')[0]);
            var fev1 = float.Parse(data[29].Split(':')[1].Split('l')[0]);
            var pef = float.Parse(data[31].Split(':')[1].Split('l')[0]);
            var metaphor = data[3].Split(':')[1];
            var crit1a = bool.Parse(data[10].Split(':')[1]);
            var crit1b = bool.Parse(data[11].Split(':')[1]);
            var crit2a = bool.Parse(data[12].Split(':')[1]);
            var crit2b = bool.Parse(data[13].Split(':')[1]);
            var crit2c = bool.Parse(data[14].Split(':')[1]);
            var crit3 = bool.Parse(data[15].Split(':')[1]);
            return new SpiroResultSimple(fvc, fev1, pef, metaphor, crit1a, crit1b, crit2a, crit2b, crit2c, crit3);
        }
        
        private static void WriteString(string filename, string data, string extension)
        {
            if (Application.platform != RuntimePlatform.Android) return;
            
            //create path name
            var path = _userFolder;

            //make directory if needed
            if (!Directory.Exists(path))
                Directory.CreateDirectory(path);
            
            //create file path
            path += "/";
            path += filename;
            path += extension;

            //make file if needed
            if (File.Exists(path))
                Debug.Log("LOGGER: <i>" + path + "</i> already exists. Overriding...");
            var sr = File.CreateText(path);
            
            //actually writing string to disk
            Debug.Log("LOGGER: writing file to " + path);
            sr.Write(data);
            sr.Close();
        }

        public static void DeleteFolder(string userName)
        {
            var folderName = RootFolder + "/" + userName;
            if (Directory.Exists(folderName))
            {
                Debug.Log("Removed folder " + folderName);
                
                //clear out all logs inside folder
                var logs = Directory.GetFiles(folderName);
                if (logs.Length > 0)
                    foreach (var log in logs)
                        File.Delete(log);

                //delete the folder itself
                Directory.Delete(folderName);
            }
            else
                Debug.Log("failed to delete " + folderName + "because it could not be found");
        }

        public static void DeleteAllFolders()
        {
            Debug.Log("Deleting all folders");
            foreach (var folder in new DirectoryInfo(RootFolder).GetDirectories())
            {
                var files = folder.GetFiles();
                foreach (var file in files)
                {
                    file.Delete();
                }
            }
            Directory.Delete(RootFolder);
            Directory.CreateDirectory(RootFolder);
        }

        //this function takes a screenshot of the entire screen, including UI
        public static IEnumerator TakeScreenshot()
        {
            Debug.Log("Capturing screenshot...");
            
            //get correct folder to store screenshot in
            _userFolder = RootFolder + "/" + SessionManager.CurrentUser.userName;
            
            //construct name based on last pushed log
            var finalName =  _lastFileName  + ".jpg";
            
            //capture screenshot to persistent data path
            ScreenCapture.CaptureScreenshot(finalName);

            //wait before file system finishes processing
            yield return new WaitForSeconds(1f);
            
            //get current and destination paths of screenshot
            var sourceLocation = Application.persistentDataPath + "/" + finalName;
            var destination = _userFolder + "/" + finalName;
            
            //move screenshot from persistent datapath to correct folder
            System.IO.File.Move(sourceLocation, destination);
            
            yield return null;
        }

        public static IEnumerable<string> GetAllLogs()
        {
            var output = new List<string>();

            var rootDir = new DirectoryInfo(Logger.RootFolder);

            var userFolders = rootDir.GetDirectories();
            foreach (var userFolder in userFolders)
            {
                var userFiles = userFolder.GetFiles("*" + ".gebruiker");
                if (userFiles.Length <= 0) continue;
                output.AddRange(userFolder.GetFiles().Select(logFile => logFile.FullName));
            }

            return output.ToArray();
        }

        public static string[] GetUserLogs()
        {
            var output = new List<string>();
            var userFolder = new DirectoryInfo(Logger.RootFolder + "/" + SessionManager.CurrentUser.userName);
            output.AddRange(userFolder.GetFiles().Select(logFile => logFile.FullName));
            return output.ToArray();
        }
    }
}