using System;
using System.Collections;
using System.Collections.Generic;
using System.Globalization;
using AwesomeCharts;
using Spirometry.ScriptableObjects;
using Spirometry.SpiroController;
using Spirometry.Statics;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.Serialization;
using UnityEngine.UI;
using Logger = Spirometry.SpiroController.Logger;
using Math = Spirometry.Statics.Math;

namespace Menus.Overlay
{
    public class Overlay : MonoBehaviour
    {
        /// <summary>
        /// Spiro-Play project by University of Twente, MST and Deventer ziekenhuis
        /// Made by Koen Vogel - k.a.vogel@student.utwente.nl
        /// 
        /// Overlay class:
        /// Canvas controller to visualize feedback, instructions and interaction during metaphors
        /// </summary>
        
        #region variables
        #pragma warning disable 649

        [Header("General")]
        [SerializeField] private Settings settings;
        [SerializeField] private Animator animator;
        [SerializeField] public AudioSource audioSource;
        [SerializeField] private Image overlayBackground;
        private enum State { None, Graph, Feedback, SessionCheck }
        private State _currentState = State.None;

        [Header("Buttons")]
        [SerializeField] private GameObject backButton;
        [SerializeField] private GameObject nextButton;

        [Header("Text Elements")]
        [SerializeField] private TMPro.TextMeshProUGUI prepText;
        [SerializeField] private TMPro.TextMeshProUGUI feedbackText;
        [SerializeField] private TMPro.TextMeshProUGUI sessionCheckText;
        [SerializeField] private LineChart firstGraph;
        [SerializeField] private LineChart secondGraph;
        [SerializeField] private TMPro.TextMeshProUGUI parameterFvc;
        [SerializeField] private TMPro.TextMeshProUGUI parameterFev1;

        //hashes
        private static readonly int DimScreen = Animator.StringToHash("DimScreen");
        private static readonly int FadePlotterIn = Animator.StringToHash("FadePlotterIn");
        private static readonly int FadeButtonIn = Animator.StringToHash("FadeButtonIn");
        private static readonly int FadePlotterOut = Animator.StringToHash("FadePlotterOut");
        private static readonly int FadeFeedbackIn = Animator.StringToHash("FadeFeedbackIn");
        private static readonly int FadeFeedbackOut = Animator.StringToHash("FadeFeedbackOut");
        
        [Header("Settings")]
        [SerializeField] private float instructionDuration;
        [SerializeField] private float delayBeforeFeedback = 0f;
        [SerializeField] private bool enableVoiceOver = true;
        [SerializeField] private bool enableText = true;
        private bool _showingPrep = true;
        private SessionManager.Check _sessionCheckData;
        private static bool _succes = false;
        private AudioClip _feedbackAudio;
        private AudioClip _sessionCheckAudio;
        private static readonly int FadeSessionCheckIn = Animator.StringToHash("FadeSessionCheckIn");
        private static readonly int LightenScreen = Animator.StringToHash("LightenScreen");
        private static readonly int FadeInstructionsOut = Animator.StringToHash("FadeInstructionsOut");
        private static readonly int FadeInstructionsIn = Animator.StringToHash("FadeInstructionsIn");

        public bool IsTesting => SceneManager.GetActiveScene().name == "M0_Test" || SceneManager.GetActiveScene().name == "M12_Test2";
        public bool HideFeedback => SceneManager.GetActiveScene().name == "M12_Test2";
#pragma warning restore 649
        #endregion

        private void Start()
        {
            backButton.SetActive(true);
            if (SceneManager.GetActiveScene().name != "M0_Test" && SceneManager.GetActiveScene().name != "M12_Test2")
                StartCoroutine(VisualizeInstructions());
        }
        
        private IEnumerator VisualizeInstructions()
        {
            //instructions during preperation
            _showingPrep = true;
            prepText.gameObject.SetActive(true);
            prepText.text = settings.Feedback.instructions;
            yield return new WaitForSeconds(0.5f);
            animator.SetTrigger(FadeInstructionsIn);
            yield return new WaitForSeconds(0.5f);
            
            //play audio
            audioSource.clip = settings.Feedback.instructionsAudio;
            audioSource.Play();
            
            yield return new WaitForSeconds(instructionDuration);
            if (_showingPrep)
            {
                StartCoroutine(EndInstructions(false));
            }
            yield return null;
        }

        public IEnumerator EndInstructions(bool quick)
        {
            if (!_showingPrep) yield return null;
            _showingPrep = false;
            if (quick)
            {
                prepText.gameObject.SetActive(false);
                var color = overlayBackground.color;
                color = new Color(color.r, color.g, color.b, 9.6f);
                overlayBackground.color = color;
                audioSource.Stop();
            }
            else
            {
                animator.SetTrigger(FadeInstructionsOut);
                for (int i = 0; i < 4; i++)
                {
                    audioSource.volume -= 0.15f;
                    yield return new WaitForSeconds(0.1f);
                }
                animator.SetTrigger(LightenScreen);
            }
            audioSource.Stop();
            audioSource.volume = 1f;
            yield return null;
        }
        
        private void Update()
        {
            if (!enableVoiceOver)
                audioSource.volume = 0;
            
            //testing
            if (Input.GetKeyDown(KeyCode.Space))
                GiveFeedback_Interrupted();
        }

        public void OnStartTest()
        {
            if (_showingPrep) StartCoroutine(EndInstructions(false));
            backButton.SetActive(false);
            prepText.gameObject.SetActive(false);
        }

        public void GiveFeedback_Succesfull(SpiroResult result, SessionManager.Check sessionCheck, string rewardFeedback)
        {
            //enable objects
            nextButton.SetActive(true);
            if (firstGraph != null)
                firstGraph.gameObject.SetActive(true);
            if (secondGraph != null)
                secondGraph.gameObject.SetActive(true);
            feedbackText.gameObject.SetActive(true);
            _succes = true;
            _sessionCheckData = sessionCheck;
            switch (_sessionCheckData)
            {
                case SessionManager.Check.Restart:
                    sessionCheckText.text = settings.Feedback.restartMetaphor + rewardFeedback;
                    _sessionCheckAudio = settings.Feedback.restartMetaphorAudio;
                    break;
                case SessionManager.Check.EndLimit:
                    sessionCheckText.text = settings.Feedback.endSessionLimit + rewardFeedback;
                    _sessionCheckAudio = settings.Feedback.endSessionLimitAudio;
                    break;
                case SessionManager.Check.EndSuccess:
                    sessionCheckText.text = settings.Feedback.endSessionSucces + rewardFeedback;
                    _sessionCheckAudio = settings.Feedback.endSessionSuccesAudio;
                    break;
                default:
                    sessionCheckText.text = settings.Feedback.endSessionSucces + rewardFeedback;
                    _sessionCheckAudio = settings.Feedback.endSessionSuccesAudio;
                    break;
            }

            //generate fitting text and audio
            if (enableText)
                feedbackText.GetComponent<TMPro.TextMeshProUGUI>().text = result.FeedbackText;
            else
            {
                feedbackText.GetComponent<TMPro.TextMeshProUGUI>().text = "";
                sessionCheckText.text = "";
            }

            _feedbackAudio = result.FeedbackAudio;

            //plot firstGraph
            if (firstGraph != null)
                PlotGraph(firstGraph, Math.GetFlowTime(result.Validation.FullFunction));
            if (secondGraph != null)
                PlotGraph(secondGraph, Math.GetFlowVolume(result.Validation.Expiration));
            parameterFev1.text = "FEV1: " + result.Validation.FEV1.ToString("F2");
            parameterFvc.text = "FVC: " + result.Validation.FVC.ToString("F2");
            
            //start animations
            StartCoroutine(VisualizeGraph());
        }

        public void GiveFeedback_Interrupted()
        {
            //enable objects
            nextButton.SetActive(true);
            feedbackText.gameObject.SetActive(true);
            _succes = false;
            
            //generate fitting text
            feedbackText.GetComponent<TMPro.TextMeshProUGUI>().text = settings.Feedback.testInterrupted;
            _feedbackAudio = settings.Feedback.testInterruptedAudio;
            
            //start animations
            StartCoroutine(VisualizeFeedback());
        }

        private IEnumerator VisualizeGraph()
        {
            yield return new WaitForSeconds(delayBeforeFeedback);
            _currentState = State.Graph;
            animator.SetTrigger(DimScreen);
            yield return new WaitForSeconds(0.5f);
            
            //animate next button and plotter in
            if (!HideFeedback) animator.SetTrigger(FadePlotterIn);
            animator.SetTrigger(FadeButtonIn);
            nextButton.GetComponentInChildren<TMPro.TextMeshProUGUI>().text = IsTesting ? "Menu" : "Verder";
            
            //take screenshot once graph has faded in
            yield return new WaitForSeconds(1f);
            StartCoroutine(Logger.TakeScreenshot());
            yield return null;
        }

        private IEnumerator VisualizeFeedback()
        {
            if (_currentState == State.Graph)
            {
                if (!HideFeedback) animator.SetTrigger(FadePlotterOut);
                _currentState = State.Feedback;
                yield return new WaitForSeconds(0.7f);
            }
            else
            {
                _currentState = State.Feedback;
                animator.SetTrigger(DimScreen);
                yield return new WaitForSeconds(0.5f);
                animator.SetTrigger(FadeButtonIn);
            }
            if (!HideFeedback)animator.SetTrigger(FadeFeedbackIn);
            yield return new WaitForSeconds(0.5f);
            audioSource.clip = _feedbackAudio;
            audioSource.volume = 1f;
            audioSource.Play();
        }

        private IEnumerator VisualizeSessionCheck()
        {
            if (HideFeedback) yield return null;
            _currentState = State.SessionCheck;
            animator.SetTrigger(FadeFeedbackOut);
            yield return new WaitForSeconds(0.9f);
            animator.SetTrigger(FadeSessionCheckIn);
            yield return new WaitForSeconds(0.5f);
            audioSource.clip = _sessionCheckAudio;
            audioSource.volume = 1f;
            audioSource.Play();
            yield return null;
        }
        
        #region button actions

        public void SkipButtonAction()
        {
            if (_showingPrep)
                StartCoroutine(EndInstructions(false));
        }
        
        public void BackButtonAction()
        {
            LevelChanger.FadeToLevel("MainMenu");
        }

        public void NextButtonAction()
        {
            if (_showingPrep)
            {
                StartCoroutine(EndInstructions(false));
                return;
            }
            
            //skip button presses if testing
            if (IsTesting) {LevelChanger.FadeToLevel("MainMenu");
                return;
            }
            
            if (!_succes)
                LevelChanger.FadeToLevel(SceneManager.GetActiveScene().name);
            else
            {
                switch (_currentState)
                {
                    case State.Graph:
                        StartCoroutine(VisualizeFeedback());
                        break;
                    case State.Feedback:
                        StartCoroutine(VisualizeSessionCheck());
                        break;
                    case State.None:
                        StartCoroutine(VisualizeGraph());
                        break;
                    case State.SessionCheck:
                        LevelChanger.FadeToLevel(_sessionCheckData == SessionManager.Check.Restart
                            ? SceneManager.GetActiveScene().name
                            : "MainMenu");
                        break;
                    default:
                        StartCoroutine(VisualizeGraph());
                        break;
                }
            }
        }

        private static void PlotGraph(LineChart chart, List<Vector2> values)
        {
            if (!chart.gameObject.activeSelf) return;
            chart.GetChartData().DataSets[0].Clear();
            
            //get window size
            var maxValueX = Math.FindHighestXvalue(values);
            var maxValueY = Math.FindHighestYvalue(values);
            if (maxValueY < 2 * maxValueX)
            {
                chart.XAxis.MinAxisValue = 0;
                chart.XAxis.MaxAxisValue = maxValueX + 0.5f;
                chart.YAxis.MinAxisValue = 0;
                chart.YAxis.MaxAxisValue = maxValueX * 2;
            }
            else
            {
                chart.XAxis.MinAxisValue = 0;
                chart.XAxis.MaxAxisValue = maxValueY / 2;
                chart.YAxis.MinAxisValue = 0;
                chart.YAxis.MaxAxisValue = maxValueY + 0.5f;
            }

            //plot entries
            for (var i = 0; i < values.Count-1; i++)
            {
                chart.GetChartData().DataSets[0].AddEntry(new LineEntry(values[i].x, values[i].y));
            }
            
            //finalyze
            chart.SetDirty();
        }

        #endregion
    }
}
