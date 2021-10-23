using System;
using System.Collections;
using System.Collections.Generic;
using Spirometry.Debugging;
using Spirometry.ScriptableObjects;
using Spirometry.Statics;
using UnityEngine;
using UnityEngine.SceneManagement;
using System.Threading;
using Menus.Overlay;
using UnityEngine.Events;
using UnityEngine.UI;
using Event = Spirometry.ScriptableObjects.Event;
using Math = System.Math;
using Random = UnityEngine.Random;

namespace Spirometry.SpiroController
{
    /// <summary>
    /// Spiro-Play project by University of Twente and MST
    /// Made by Koen Vogel - k.a.vogel@student.utwente.nl
    /// 
    /// Spiro_Manager class:
    /// Important, manages a complete spirometry test with the supplied data from RealtimeDataProcessor class, detects useful events in data like inhalation and exhalation; should be inherited from by metaphors)
    /// </summary>

    public abstract class SpiroManager : MonoBehaviour
    {
        #region variables
        #pragma warning disable 649

        //enums and delegates
        public enum State { Prep, Inhaling, Exhaling, Done }
        private State _devState;
        
        [Header("References")]
        [Tooltip("This data is being used for the actual test, probably in assets/resources/persistent variables")]
        [SerializeField] private Datastream incomingData;
        [Tooltip("Event triggered when test starts")]
        [SerializeField] private Event StartTriggered;
        [Tooltip("Event triggered when test ends")]
        [SerializeField] private Event EndTriggered;
        [Tooltip("Overlay that is present in every metaphor")]
        [SerializeField] protected Overlay generalOverlay;
        
        [Header("Important Parameters (Read Only)")]
        [Tooltip("Here you can check the current state of testing (Prep, Inhaling, Exhaling or Done")]
        public State gameState = State.Prep;
        [Tooltip("Realtime representation of flow")]
        public float currentFlow;
        public float currentMaxFlow;
        [Tooltip("Current amount of liters of air inhaled, with the predicted maximum volume of 100, this goal does not necessarily have to be reached")]
        public float inspirationProgressLong;
        [Tooltip("Current amount of liters of air exhaled, with the predicted maximum volume of 100, this goal does not necessarily have to be reached")]
        public float expirationProgressLong;
        [Tooltip("Current flow, with the predicted maximum flow of 100, this goal does not necessarily have to be reached")]
        public float expirationProgressHard;
        public SpirometryEvents exposedBreathEvents;

        private static Settings Settings => SessionManager.Instance == null ? null : SessionManager.Instance.settings;

        private float _currentVolume = 0f;
        private float _goalVolume = 0f;
        private float _goalFlow = 0f;
        private bool _reachedPef = false;
        private float TestDuration
        {
            get
            {
                if (startingPoint == null || startingPoint == default(SpiroData)) return 0;
                DateTime startingTime = startingPoint.Timestamp;
                TimeSpan duration = incomingData.Values[incomingData.Count - 1].Timestamp - startingTime;
                return (float) duration.TotalMilliseconds / 1000;
            }
        }
        private SpiroData startingPoint, switchingPoint, endingPoint;
        
        //struct for accessing events in the inspector
        [System.Serializable]
        public struct SpirometryEvents
        {
            [Tooltip("Triggered when the user starts inhaling")]
            public UnityEvent onStartTest;
            [Tooltip("Triggered when the user starts exhaling")]
            public UnityEvent onSwitchToExpiration;
            [Tooltip("Triggered when the user stops exhaling")]
            public UnityEvent onEndTest;
            [Tooltip("Triggered when the user reaches their target peak flow")]
            public UnityEvent onReachedPEF;
            [Tooltip("Triggered when the test is stopped during the inspiration phase")]
            public UnityEvent onInterruptionDuringInspiration;
        }
        
        #pragma warning restore 649
        #endregion

        #region Singleton pattern
        //singleton pattern
        public static SpiroManager Instance { get; private set; }

        private void Awake()
        {
            //singleton pattern (https://wiki.unity3d.com/index.php/Singleton)
            if (Instance != null && Instance != this)
                Destroy(this.gameObject);
            else
                Instance = this;
        }
        #endregion

        protected virtual void Start()
        {
            //warning for using metaphors without spirocontroller
            if (RealtimeDataProcessor.Instance == null)
                Debug.LogError("SPIROPLAY SYSTEM: spiromanager cannot function on its own, please start metaphor from main menu to include background spirometry processes");
            generalOverlay.gameObject.SetActive(true);
            
            //clear data
            inspirationProgressLong = 0;
            expirationProgressLong = 0;
            currentFlow = 0;
            _currentVolume = 0;
            
            incomingData.newValueReceived.AddListener(delegate() { UpdateData(incomingData.Values[incomingData.Values.Count-1]); });

            DetermineGoals();
        }

        private void UpdateData(SpiroData newpoint)
        {
            switch (gameState)
            {
                case State.Inhaling:
                    _currentVolume += currentFlow * 0.1f;
                    DetermineProgress(_currentVolume, currentFlow);
                    break;
                case State.Exhaling:
                    _currentVolume += currentFlow * 0.1f;
                    DetermineProgress(_currentVolume, currentMaxFlow);
                    print("exp progress hard: " + expirationProgressHard);
                    if (expirationProgressHard >= 100 & !_reachedPef)
                    {
                        //SpiroReceiver.Instance.peakFlow = 0;
                        _reachedPef = true;
                        Debug.Log("Reached predicted PEF...");
                        exposedBreathEvents.onReachedPEF.Invoke();
                        OnReachedProficientFlow();
                    }
                    break;
                case State.Prep:
                    break;
                case State.Done:
                    break;
                default:
                    throw new ArgumentOutOfRangeException();
            }
        }

        protected virtual void Update()
        {
            //fallback developer settings
            if (Application.platform == RuntimePlatform.WindowsEditor ||
                Application.platform == RuntimePlatform.WindowsPlayer ||
                Application.platform == RuntimePlatform.OSXEditor)
            {

                //detect changes in gamestate and trigger event calls
                if (gameState == _devState) return;
                _devState = gameState;
                switch (gameState)
                {
                    case State.Prep:
                        break;
                    case State.Inhaling:
                        exposedBreathEvents.onStartTest.Invoke();
                        OnStartTest();
                        break;
                    case State.Exhaling:
                        OnSwitchToExpiration();
                        exposedBreathEvents.onSwitchToExpiration.Invoke();
                        break;
                    case State.Done:
                        exposedBreathEvents.onEndTest.Invoke();
                        OnEndTest();
                        break;
                    default:
                        throw new ArgumentOutOfRangeException();
                }
            }

            if (incomingData.Values == null) return;
        
            if (StartDetected(incomingData.Values))
                StartTest();

            if (gameState == State.Prep || gameState == State.Done || incomingData.Count <= 0) return;
            var lastIndex = incomingData.Count - 1;
            currentFlow = incomingData.Values[lastIndex].Flow;
            currentMaxFlow = incomingData.Values[lastIndex].MaxFlow;

            if (gameState == State.Inhaling && incomingData.Values[incomingData.Count - 1].Flow > Settings.SwitchToExpirationFlowThreshold)
                InhaleToExhaleSwitch();

            //endtest
            if (InterruptionDetectedInInspiration(incomingData.Values))
            {
                exposedBreathEvents.onInterruptionDuringInspiration.Invoke();
                StartCoroutine(EndTest(false));
            }

            if (EndDetected(incomingData.Values))
                StartCoroutine(EndTest(true));
        }

        #region Events

        private void StartTest()
        {
            if (gameState != State.Prep) return;
            Debug.Log("<color=green><b>SPIROMANAGER:</b> TEST HAS STARTED </color>");
            gameState = State.Inhaling;
            StartTriggered.value.Invoke();
            _currentVolume = 0f;
            generalOverlay.OnStartTest();
            SpiroReceiver.Instance.peakFlow = 0;
            startingPoint = incomingData.Values[incomingData.Count - 1];
            exposedBreathEvents.onStartTest.Invoke();
            OnStartTest();
        }

        private void InhaleToExhaleSwitch()
        {
            //switch from inhale to exhale
            Debug.Log("<color=green><b>SPIROMANAGER:</b> TEST HAS SWITCHED TO EXPIRATION </color>");
            gameState = State.Exhaling;
            _currentVolume = 0;
            //record point at which signal switches to expiration
            switchingPoint = incomingData.Values[incomingData.Count - 1];
            exposedBreathEvents.onSwitchToExpiration.Invoke();
            OnSwitchToExpiration();
        }

        private IEnumerator EndTest(bool succesfull)
        {
            gameState = State.Done;
            OnEndTest();
            exposedBreathEvents.onEndTest.Invoke();
            Debug.Log("<color=green><b>SPIROMANAGER:</b> END OF TEST DETECTED... </color>");

            //wait some time after end is detected to capture extra data
            var delay = Settings.DelayAfterEndDetected;
            yield return new WaitForSeconds(delay);
            
            //trigger end of test
            SpiroReceiver.Instance.peakFlow = 0;
            currentFlow = 0f;
            currentMaxFlow = 0f;
            EndTriggered.value.Invoke();
            endingPoint = incomingData.Values[incomingData.Count - 1];

            //only start error recognition if test was succesful
            if (!succesfull)
            {
                Debug.Log("<color=red><b>SPIROMANAGER:</b> TEST HAS ENDED UNSUCCESFULLY </color>");
                generalOverlay.GiveFeedback_Interrupted();
            }
            else
            {
                Debug.Log("<color=green><b>SPIROMANAGER:</b> TEST HAS ENDED SUCCESFULLY, COMPILING RESULTS... </color>");
               
                //compile data from begin to end of test into a complete result
                if (SessionManager.CurrentUser != null)
                {
                    //add a delay before ending test
                    yield return new WaitForSeconds(0.1f);
                    
                    if (SessionManager.CurrentUser.currentSessionID == 0) SessionManager.CurrentUser.currentSessionID = (int)Random.Range(1, 1000000);
                    
                    var result = new SpiroResult(RealtimeDataProcessor.Instance.TestFile.Values, SessionManager.CurrentUser, SceneManager.GetActiveScene().name, SessionManager.cachedResults.Count + 1, Settings);
                    yield return new WaitForSeconds(0.1f);

                    //save result to file system and check if more tests need to be completed
                    var sessionCheck = SessionManager.SaveAndCheck(result);
                    yield return new WaitForSeconds(0.3f);
                    Logger.SaveLog(result);
                    
                    //reward user with coins and get result for feedback purposes
                    var rewardFeeback = SessionManager.RewardUser(sessionCheck);
                    generalOverlay.GiveFeedback_Succesfull(result, sessionCheck, rewardFeeback);
                
                    //debug displaying feedback
                    Debug.Log("<color=green><b>SPIROMANAGER:</b> DISPLAYING FEEDBACK... </color>");
                } else Debug.Log("<color=green><b>SPIROMANAGER:</b> No logged in user found, failed to compile test... </color>");
            }
            yield return null;
        }

        private bool InterruptionDetectedInInspiration(List<SpiroData> function)
        {
            if (gameState != State.Inhaling) return false;
            
            //inspiration has to be longer than 0.5 seconds
            float inspirDuration = (float)(function[function.Count - 1].Timestamp - startingPoint.Timestamp).TotalMilliseconds / 1000;
            if (inspirDuration < 0.5) return false;
            
            //inspiration has to be shorter than 6 seconds
            return inspirDuration > 6;
        }
        #endregion

        #region Subclass Events
        protected abstract void OnStartTest();
        protected abstract void OnEndTest();
        protected abstract void OnSwitchToExpiration();
        protected abstract void OnReachedProficientFlow();

        #endregion

        #region Calculate progress

        private void DetermineGoals()
        {
            //get appropriate goal volume
            if (Settings == null) return;
            if (SessionManager.Instance == null)
            {
                _goalVolume = Settings.DefaultPredictedFVC;
                _goalFlow = Settings.DefaultPredictedPEF;
            }
            else if (SessionManager.CurrentUser == null)
            {
                _goalVolume = Settings.DefaultPredictedFVC;
                _goalFlow = Settings.DefaultPredictedPEF;
            }
            else if (SessionManager.CurrentUser.userName == "")
            {
                _goalVolume = Settings.DefaultPredictedFVC;
                _goalFlow = Settings.DefaultPredictedPEF;
            }
            else
            {
                _goalVolume = SessionManager.CurrentUser.predictedFvc;
                _goalFlow = SessionManager.CurrentUser.predictedPef;
            }

            //modify goal according to settings
            _goalVolume *= Settings.GoalModifier;
            _goalVolume = Mathf.Abs(_goalVolume);
            _goalFlow *= Settings.GoalModifier;
            _goalFlow = Mathf.Abs(_goalFlow);
        }
        private void DetermineProgress(float currentVolume, float currentFlowPoint)
        {
            //only determine progress if goal volume and flow have set values
            if (Math.Abs(_goalFlow) < 0.1 || Math.Abs(_goalVolume) < 0.1) return;
            
            //calculate progress based on game state
            switch (gameState)
            {
                case State.Inhaling:
                    inspirationProgressLong = Mathf.Abs(100 * (Mathf.Abs(currentVolume) / _goalVolume));
                    //Debugger.Instance.Display(_goalVolume + "   " + inspirationProgressLong, "GoalVolume + Inspiration progress");
                    break;
                case State.Exhaling:
                    expirationProgressLong = Mathf.Abs(100 * (Mathf.Abs(currentVolume) / _goalVolume));
                    
                    //retrieve peak flow from unaveraged values in receiver class
                    //var peak = SpiroReceiver.Instance.peakFlow;
                    var peak = currentFlowPoint;
                    expirationProgressHard = Mathf.Abs(100 * (Mathf.Abs(peak) / _goalFlow));
                    //Debugger.Instance.Display(currentVolume + "   " + expirationProgressLong + "\n" + currentFlowPoint + "   " + expirationProgressHard, "Current Volume, Current Flow, Expiration progress Long + Hard");
                    break;
                case State.Prep:
                    break;
                case State.Done:
                    break;
                default:
                    throw new ArgumentOutOfRangeException();
            }
        }
        #endregion

        #region Event detection
        private bool StartDetected(IReadOnlyList<SpiroData> function)
        {
            var flowThreshold = Settings != null ? Settings.StartOfTestFlowThreshold : -0.2f;
            if (gameState != State.Prep) return false;
            if (function == null) return false;
            if (function.Count <= 0) return false;
            var enoughFlow = function[function.Count - 1].Flow <= flowThreshold;
            return enoughFlow;
        }
        
        //detect if breath signal is idle
        private bool EndDetected(List<SpiroData> function)
        {
            var flowThreshold = Settings.EndOfTestFlowThreshold;
            if (gameState != State.Exhaling) return false;
            
            //expiration has to be longer than 1 second
            var expirDuration = (float)(function[function.Count - 1].Timestamp - switchingPoint.Timestamp).TotalMilliseconds / 1000;
            if (expirDuration < 1) return false;
            
            //expiration has to be shorter than 8 seconds
            if (expirDuration > 8f) return true;

            if (gameState != State.Exhaling || !(function[function.Count - 1].Flow < flowThreshold) ||
                !(function[function.Count - 1].Volume < 1)) return false;
            Debug.Log("LAST RECORDED VOLUME: " + function[function.Count -1].Volume); return true;
        }
        #endregion

        #region Dev Functionality
        
        //force a "OnReachedProficientFlow" event call for devs to test with
        public void ReachedPeakFlowOverride()
        {
            if (Application.isPlaying)
            {
                _reachedPef = true;
                OnReachedProficientFlow();
            }
            else
                Debug.LogWarning("This button only works in runtime");
        }

        //simulate a breath test for the devs to work with
        private bool _simulatingFakeBreath = false;
        public IEnumerator SimulatePerfectBreath()
        {
            //fallback for pressing button outside of runtime
            if (!Application.isPlaying) {
                Debug.LogWarning("This button only works in runtime");
                yield return null;
            }

            //fallback for pressing button while already simulating
            if (_simulatingFakeBreath)
            {
                Debug.LogWarning("Already simulating fake breath... wait for it to complete");
                yield return null;
            }
            _simulatingFakeBreath = true;
            
            
            //simulate inspiration 
            gameState = State.Inhaling;
            currentFlow = 0;
            while (inspirationProgressLong < 100)
            {
                inspirationProgressLong += 3f;
                
                //force flow values
                if (inspirationProgressLong < 50)
                    currentFlow -= 0.2f;
                else
                    currentFlow += 0.2f;
                
                yield return new WaitForSeconds(0.1f);
            }
            
            //simulate expiration 
            currentFlow = 0;
            gameState = State.Exhaling;
            while (expirationProgressLong < 100)
            {
                expirationProgressLong += 1.5f;

                if (!_reachedPef)
                {
                    //increase relative flow
                    expirationProgressHard += 5;
                    //simulate peak flow
                    if (expirationProgressHard >= 100)
                        ReachedPeakFlowOverride();
                }
                else
                {
                    //decrease relative flow
                    if (expirationProgressHard > 0)
                        expirationProgressHard -= 3.6f;
                }
                
                //force flow values
                if (expirationProgressLong < 50)
                    currentFlow += 0.2f;
                else
                    currentFlow -= 0.2f;
                
                yield return new WaitForSeconds(0.1f);
            }

            //finish coroutine & reset values
            gameState = State.Done;
            currentFlow = 0f;
            inspirationProgressLong = 0;
            expirationProgressHard = 0;
            expirationProgressLong = 0;
            _reachedPef = false;
            yield return null;
        }
        #endregion
    }
}
