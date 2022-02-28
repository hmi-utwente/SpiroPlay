using System;
using Menus.Menu_UserSelection;
using Spirometry.Debugging;
using Spirometry.ScriptableObjects;
using Spirometry.SpiroController;
using UnityEngine;

namespace Menus.Menu_Main
{
    /// <summary>
    /// Spiro-Play project by University of Twente and MST
    /// Made by Koen Vogel - k.a.vogel@student.utwente.nl
    /// 
    /// MainMenuBehaviour class:
    /// Manages UI, scene changes and settings
    /// </summary>

    public class MainMenuBehaviour : MonoBehaviour
    {
        #region variables
        #pragma warning disable 649

        [SerializeField] private Settings settings;
        [SerializeField] private FeedbackConfig Vivianne;
        [SerializeField] private FeedbackConfig Matienne;
        [SerializeField] private TMPro.TextMeshProUGUI voiceStateText;
        
        private enum VoiceType { Vivianne, Matienne }
        private VoiceType voiceState = VoiceType.Vivianne;

#pragma warning restore 649
        #endregion

        /// <summary>
        /// Button Handlers
        /// </summary>

        private void Start()
        {
            if (UserSelectionMenu.RememberedSession != "")
            {
                LevelChanger.FadeToLevel(UserSelectionMenu.RememberedSession);
                UserSelectionMenu.RememberedSession = "";
            }
            DisplayUser();
            UpdateVoice();
        }

        private int buttonTaps = 0;
        private DateTime firstTap;
        private readonly TimeSpan window = TimeSpan.FromSeconds(5);
        public void OpenSettings()
        {
            var now = DateTime.Now;
            if (now - firstTap > window)
            {
                firstTap = now;
                buttonTaps = 1;
            }
            else
            {
                buttonTaps++;
                if (buttonTaps >= 5) LevelChanger.FadeToLevel("OptionsMenu");
            }
        }

        public void Play()
        {
            LevelChanger.FadeToLevel("LevelSelection");
        }

        #region users
        public TMPro.TextMeshProUGUI UserText;

        public void WatchTutorial()
        {
            LevelChanger.FadeToLevel("Tutorial");
        }

        private void UpdateVoice()
        {
            voiceStateText.text = settings.Feedback == Vivianne ? "Huidige stem: Vrouw" : "Huidige stem: Man";
        }
        public void ToggleVoice()
        {
            if (settings.Feedback == Matienne)
            {
                settings.Feedback = Vivianne;
                UpdateVoice();
            }
            else
            {
                settings.Feedback = Matienne;
                UpdateVoice();
            }
        }
        
        private void DisplayUser()
        {
            UserText.text = "Je bent niet ingelogd";
            if (SessionManager.CurrentUser == null) return;
            if (SessionManager.CurrentUser.userName == "") return;
            UserText.text = "Ingelogd als " + SessionManager.CurrentUser.userName;
        }

        private void Update()
        {
            //catch android back button action, and reroute it to touch back button action
            if (Input.GetKeyDown(KeyCode.Escape)) Logout();
        }

        private void Logout()
        {
            SessionManager.CurrentUser = null;
            SessionManager.ClearCache();
            LevelChanger.FadeToLevel("UserSelection");
        }

        public void GoToHistory()
        {
            LevelChanger.FadeToLevel("HistoryMenu");
        }
        #endregion
    
    }
}
