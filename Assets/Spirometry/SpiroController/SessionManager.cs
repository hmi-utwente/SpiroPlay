using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using Spirometry.ScriptableObjects;
using Spirometry.Statics;
using UnityEngine;
using UnityEngine.Analytics;
using UnityEngine.UIElements;
using Math = Spirometry.Statics.Math;
using Random = UnityEngine.Random;
#if PLATFORM_ANDROID
using UnityEngine.Android;
#endif

/// <summary>
/// Spiro-Play project by University of Twente and MST
/// Made by Koen Vogel - k.a.vogel@student.utwente.nl
/// 
/// SessionManager class:
/// Manages complete session from boot to shutdown, should manage multiple tests and be able to draw conclusions from multiple tests such as determining how many more tests need to be completed
/// </summary>

namespace Spirometry.SpiroController
{
    [RequireComponent(typeof(RealtimeDataProcessor))]
    [RequireComponent(typeof(Validation))]
    public class SessionManager : MonoBehaviour
    {
        #region variables
        
        public static List<SpiroResultSimple> cachedResults = new List<SpiroResultSimple>();
        public static UserStorage.User CurrentUser;

        public enum Check
        {
            Restart, EndSuccess, EndLimit
        }
        
        //singleton pattern
        public static SessionManager Instance { get; private set; }

        [Header("Persistent Objects")]
        [SerializeField] public Settings settings;

        #endregion

        private void Awake()
        {
            DontDestroyOnLoad(this.gameObject);
            PersistentVariables.Reset();
            {
                #if PLATFORM_ANDROID
                if (!Permission.HasUserAuthorizedPermission(Permission.ExternalStorageWrite))
                    Permission.RequestUserPermission(Permission.ExternalStorageWrite);
                if (!Permission.HasUserAuthorizedPermission(Permission.ExternalStorageRead))
                    Permission.RequestUserPermission(Permission.ExternalStorageRead);
                #endif

                //singleton pattern (https://wiki.unity3d.com/index.php/Singleton)
                if (Instance != null && Instance != this)
                    Destroy(this.gameObject);
                else
                    Instance = this;
            }
        }

        #region session checks
        
        public static void ClearCache()
        {
            cachedResults.Clear();
        }

        //This function will be called when a test is completed and validated, it will compare it to previously completed tests
        // in order to determine if another needs to be done
        public static Check SaveAndCheck(SpiroResult result)
        {
            //do compile multiple tests
            cachedResults.Add(result.SimpleVersion);
            var amountOfTests = cachedResults.Count;

            //criteria 1: no more than 8 completed tests
            if (amountOfTests >= 8)
            {
                Debug.Log("SessionManager: more than 8 tests done, return end of session");
                return Check.EndLimit;
            }

            //criteria 2: three or more 'good' tests
            var acceptableTests = GetAcceptableTests(cachedResults);
            var Crit2 = acceptableTests.Count >= 3;
            
            //calculate highest and next to highest FVC values within the list of acceptable tests
            float highestFvc = 0, oneToHighestFvc = 0;
            foreach (var test in acceptableTests)
            {
                if (test.fvc > highestFvc)
                    highestFvc = test.fvc;
                if (test.fvc != highestFvc && test.fvc > oneToHighestFvc)
                    oneToHighestFvc = test.fvc;
            }
            
            //criteria 3A: two or more 'reproducible' fvc
            //base threshold on volume (children should have a lower threshold)
            var isChild = CurrentUser.age < Instance.settings.ageThreshold;
            var diffThreshold = isChild ? Mathf.Max(0.1f, 0.1f * highestFvc) : 0.15f;
            var diff_fvc = Mathf.Abs(highestFvc - oneToHighestFvc);
            var crit3A = diff_fvc <= diffThreshold;

            //criteria 3B: two or more 'reproducable' fev1
            float highestFev1 = 0, oneToHighestFev1 = 0;
            foreach (var test in acceptableTests)
            {
                if (test.fev1 > highestFev1)
                    highestFev1 = test.fev1;
                if (test.fev1 != highestFev1 && test.fev1 > oneToHighestFev1)
                    oneToHighestFev1 = test.fev1;
            }
            var diff_fev1 = Mathf.Abs(highestFev1 - oneToHighestFev1);
            var crit3B = diff_fev1 <= 0.15;
            
            //combine criteria
            if (Crit2 && crit3A && crit3B)
            {
                Debug.Log("SessionManager: returning successful end of session, crit2: " + Crit2 + ", crit3a: " + crit3A + ", crit3b: " + crit3B);
                cachedResults.Clear();
                
                //return succesfull and set predicted fvc and pef
                CalibratePredictedValues(acceptableTests);
                return Check.EndSuccess;
            }
            else
                Debug.Log("SessionManager: returning unsuccessfull end of session, crit2: " + Crit2 + ", crit3a: " + crit3A + ", crit3b: " + crit3B);
            
            //otherwise, do another test
            return Check.Restart;
        }

        //override the default predicted fvc and pef values with averaged values from acceptable tests
        private static void CalibratePredictedValues(List<SpiroResultSimple> tests)
        {
            //get the correct parameters from tests
            var fvcValues = tests.Select(test => test.fvc).ToList();
            var pefValues = tests.Select(test => test.pef).ToList();

            //average values
            var avrFvc = Math.Average(fvcValues);
            var avrPef = Math.Average(pefValues);

            //override user parameters and save to filesystem
            var user = SessionManager.CurrentUser;
            user.predictedFvc = avrFvc;
            user.predictedPef = avrPef;
            user.Save();
            
            //message console
            Debug.Log("Calibrating and overriding predicted user values, FVC: " + avrFvc + ", PEF: " + avrPef);
        }
        
        //reward the user with coins if the session ends
        public static string RewardUser(Check sessionCheck)
        {
            var rewardFeedback = "";
            CurrentUser.lastPlayedDate = DateTime.Now;
            
            //only reward users if the session is completed
            if (sessionCheck != Check.Restart)
            {
                //determine the amount of coins rewarded
                var rewardAmount = sessionCheck == Check.EndSuccess
                    ? Instance.settings.coinWinAmountSuccessfull
                    : Instance.settings.coinWinAmountUnsuccessfull;

                //CHECK HOW MANY SESSIONS PER DAY THE USER HAS DONE
                //if it is the first session this day, reset amount of sessions per day
                if (CurrentUser.lastPlayedDate.Day != DateTime.Now.Day) CurrentUser.lastPlayedAmountOfSessions = 0;
                CurrentUser.lastPlayedAmountOfSessions++;

                //only reward if the session is within limits
                if (CurrentUser.lastPlayedAmountOfSessions <= Instance.settings.sessionLimitPerDay)
                {
                    CurrentUser.coins += rewardAmount;
                    rewardFeedback = "\n Je krijgt " + rewardAmount + " munten!";
                }
                else { rewardFeedback = ".\n De munten zijn helaas op vandaag!"; }

                //save to storage
                Debug.Log("COMPLETED SESSION (SID: " + CurrentUser.currentSessionID + "), resetting SID and saving user");
                CurrentUser.currentSessionID = 0;
                cachedResults.Clear();
                cachedResults = new List<SpiroResultSimple>();
                //CurrentUser
                CurrentUser.Save();
            }
            else
            {
                CurrentUser.Save();
            }
            return rewardFeedback;
        }

        //determine which tests in cached results classify as 'acceptable'
        private static List<SpiroResultSimple> GetAcceptableTests(IReadOnlyList<SpiroResultSimple> cachedResults)
        {
            var output = new List<SpiroResultSimple>();
            var acceptableIndexes = new List<int>();
            
            //determine highest fvc
            var highestFvc = -999f;
            foreach (var result in cachedResults)
                if (result.fvc > highestFvc)
                     highestFvc = result.fvc;

            //go through every test done in the current moment
            for (var index = 0; index < cachedResults.Count; index++)
            {
                var result = cachedResults[index];

                //most important criteria, if one of these has a detected error, definetely not acceptable
                if (!result.crit1a || !result.crit1b || !result.crit2c || !result.crit3 /* || !val.Crit4 */ ) continue; //criteria 4 not yet finalized

                if (result.crit2a || result.crit2b)
                {
                    output.Add(result);
                    acceptableIndexes.Add(index);
                    continue;
                }

                //if there is only one test one, it should be counted as acceptable
                if (cachedResults.Count == 1)
                {
                    output.Add(result);
                    acceptableIndexes.Add(index);
                    continue;
                }
                
                //lastly, if the fvc should be within range of highest fvc
                var isChild = CurrentUser.age < Instance.settings.ageThreshold;;
                var diffThreshold = isChild ? Mathf.Max(0.1f, 0.1f * highestFvc) : 0.15f;
                if (Mathf.Abs(highestFvc - result.fvc) < diffThreshold)
                {
                    output.Add(result);
                    acceptableIndexes.Add(index);
                    continue;
                }
            }

            //return output
            var log = "There are " + output.Count + " acceptable tests with indexes ";
            foreach (var index in acceptableIndexes)
            {
                log += index + ", ";
            }
            Debug.Log(log);
            return output;
        }

        public static string RememberSession()
        {
            //if there is no active session, do nothing
            var session = CurrentUser.currentSessionID;
            if (session == 0)
            {
                Debug.Log("No active session found");
                return "";
            }
            
            //if last session was long ago, do nothing
            var timeNotPlayed = DateTime.Now - CurrentUser.lastPlayedDate;
            if (timeNotPlayed.TotalDays >= 1)
            {
                Debug.Log("You have not played in " + timeNotPlayed.TotalDays + " days, don't scan for old session");
                CurrentUser.currentSessionID = 0;
                return "";
            }
            
            //load logs from disk
            cachedResults.Clear();
            cachedResults = Logger.GetSessionLogs(session);
            if (cachedResults.Count <= 0)
            {
                Debug.Log("trying to remember session but no logs found...");
                CurrentUser.currentSessionID = 0;
                return"";
            }
            
            //parse last played metaphor
            Debug.Log("RESUMING SESSION WITH METAPHOR: "+ cachedResults[0].metaphor);
            return cachedResults[0].metaphor;
        }
        #endregion
        
        
        
        #region Persistent Variable Management

        private void OnApplicationQuit()
        {
            //log out
            CurrentUser = null;
            PersistentVariables.Reset();
        }

        [Tooltip("Reset these variables to their original state at game start")]
        public ScriptableObjectCollection PersistentVariables;

        [System.Serializable]
        public class ScriptableObjectCollection
        {
            public Datastream[] datastreams;
            public float[] floats;
            public string[] strings;

            //clear scriptableobjects
            public void Reset()
            {
                if (datastreams != null)
                    foreach (var t in datastreams)
                        if (t.Count > 0)
                            t.Clear();
                if (floats != null)
                    for (int i = 0; i < floats.Length; i++)
                        floats[i] = 0;
                if (strings != null)
                    for (int i = 0; i < strings.Length; i++)
                        strings[i] = "";
            }
        }

        #endregion
    }
}