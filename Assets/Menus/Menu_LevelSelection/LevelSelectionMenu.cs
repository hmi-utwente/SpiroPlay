using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Spirometry.ScriptableObjects;
using Spirometry.SpiroController;
using Spirometry.Statics;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Video;

namespace Menus.Menu_LevelSelection
{
    public class LevelSelectionMenu : MonoBehaviour
    {

        [Header("Config")]
        public VideoPlayer video;
        public CurrencyCounter coinCounter;
        public Transform unlockedMetaphorParent;
        public Transform lockedMetaphorParent;

        [Header("Metafors")]
        public MetaphorButton[] metaphors;

        private static int Coins
        {
            get
            {
                if (SessionManager.CurrentUser == null) return -99;
                return SessionManager.CurrentUser.coins;
            }
            set => SessionManager.CurrentUser.coins = value;
        }

        private void Start()
        {
            CloseGif();
            FetchMetaphorStatus();
            coinCounter.UpdateCurrency(Coins);
            StartCoroutine(ButtonEntrance());
        }

        private void Update()
        {
            //catch android back button action, and reroute it to touch back button action
            if (Input.GetKeyDown(KeyCode.Escape)) BackButton();
        }

        private void FetchMetaphorStatus()
        {
            //go through saved user data
            var status = SessionManager.CurrentUser.metaphorStatus;
            foreach (var metaphor in metaphors)
            {
                //if metaphors have been unlocked in save file, unlock them directly with special permissions
                if (!metaphor.Unlocked && status[metaphor.index] == true) UnlockMetaphor(metaphor, true);
            }
        }
        
        private IEnumerator ButtonEntrance()
        {
            foreach (var metaphor in metaphors)
            {
                if (!metaphor.isTest) metaphor.transform.SetParent(metaphor.Unlocked ? unlockedMetaphorParent : lockedMetaphorParent);
                metaphor.Appear();
                metaphor.button.onClick.AddListener(() => this.StartMetaphor(metaphor));
                metaphor.gifButton.onClick.AddListener(() => this.ViewGif(metaphor.gifClip));
                yield return new WaitForSeconds(0.15f);
            }
            yield return null;
        }
        
        //overloading methods for starting methafor levels
        public void StartMetaphor(MetaphorButton button)
        {
            //unlock button if locked
            if (!button.Unlocked)
            {
                UnlockMetaphor(button, false);
                return;
            }
            //reset to new session and fade to level
            SessionManager.CurrentUser.currentSessionID = 0;
            SessionManager.cachedResults.Clear();
            SessionManager.cachedResults = new List<SpiroResultSimple>();
            LevelChanger.FadeToLevel(button.gameObject.name);
        }
        
        public void BackButton()
        {
            LevelChanger.FadeToLevel("MainMenu");
        }

        public void ViewGif(VideoClip clip)
        {
            video.transform.parent.gameObject.SetActive(true);
            video.clip = clip;
        }

        public void CloseGif()
        {
            video.transform.parent.gameObject.SetActive(false);
        }

        public void UnlockMetaphor(MetaphorButton metaphor, bool specialPermission)
        {
            var user = SessionManager.CurrentUser;

            //special permission means the metaphor is directly unlocked from storage or from time requirements set by special metaphors
            if (!specialPermission)
            {
                //check if user has played this week
                if (user.DaysSinceLastUnlock <= 7)
                {
                    ConnectionCanvas.Instance.Warn_Custom("Je hebt deze week al een level ontgrendeld, probeer het volgende week weer");
                    return;
                }
                
                //check if user has played this week
                if (user.DaysSinceLastPlayed > 7)
                {
                    ConnectionCanvas.Instance.Warn_Custom("Je hebt deze week nog niet gespeeld, probeer het nog keer na het spelen");
                    return;
                }
                
                //check if the user has enough coins
                if (metaphor.cost > Coins)
                {
                    //show warning if the user does not have enough coins
                    ConnectionCanvas.Instance.Warn_Custom("Je hebt nog " + (metaphor.cost - Coins) + " munten nodig");
                    return;
                }
                
                //deduct coins
                Coins -= metaphor.cost;
                coinCounter.UpdateCurrency(Coins);
                SessionManager.CurrentUser.lastUnlockDate = DateTime.Now;
            }

            //unlock metaphor, then update users save file
            metaphor.Unlocked = true;
            if (!metaphor.isTest) metaphor.transform.SetParent(metaphor.Unlocked ? unlockedMetaphorParent : lockedMetaphorParent);
            SessionManager.CurrentUser.metaphorStatus[metaphor.index] = true;
            SessionManager.CurrentUser.Save();
        }

        /*
        public void UnlockSpecial(MetaphorButton metaphor)
        {
            //give a popup message and unlock the metaphor
            ConnectionCanvas.Instance.Warn_Custom("Kadootje! je kan nu " + metaphor.name + " spelen!");
            UnlockMetaphor(metaphor, true);
        }
        */
    }
}
