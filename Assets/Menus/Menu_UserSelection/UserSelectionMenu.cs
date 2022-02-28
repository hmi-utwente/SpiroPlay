using System;
using System.Collections;
using System.Linq;
using Spirometry.ScriptableObjects;
using Spirometry.SpiroController;
using UnityEngine;
using UnityEngine.UI;
using Logger = Spirometry.SpiroController.Logger;
using Object = UnityEngine.Object;

namespace Menus.Menu_UserSelection
{
    /// <summary>
    /// Spiro-Play project by University of Twente and MST
    /// Made by Koen Vogel - k.a.vogel@student.utwente.nl
    /// 
    /// UserSelection class:
    /// Manages UI, scene changes and login system
    /// </summary>

    public class UserSelectionMenu : MonoBehaviour
    {
        #region variables
#pragma warning disable 649
    
        [Header("References")]
        [SerializeField] private GameObject ButtonPrefab;
        [SerializeField] private Transform ButtonParent;
        [SerializeField] private Settings settings;
        [SerializeField] private Transform inputWindowUser;
        [SerializeField] private GameObject InputWindow;
        [SerializeField] private TMPro.TMP_InputField inputFieldName;
        [SerializeField] private TMPro.TMP_InputField inputFieldAge;
        [SerializeField] private TMPro.TMP_InputField inputFieldFVC;
        [SerializeField] private TMPro.TMP_InputField inputFieldPEF;
        [SerializeField] private RectTransform blocker;
        [SerializeField] private Color semiTrans;
        [SerializeField] private Transform inputWindowSession;
        [SerializeField] private TMPro.TextMeshProUGUI sessionSubtext;

        private bool IsLogginIn = false;

#pragma warning restore 649
        #endregion
    
        // Start is called before the first frame update
        private void Start()
        {
            StartCoroutine(Setup());
        }

        private IEnumerator Setup()
        {
            InputWindow.SetActive(false);
            
            //on boot, find all users in storage and add button on screen for them
            UserStorage.Users = UserStorage.LoadUsers();
            yield return new WaitForSeconds(1f);
            
            //if there are no users, automatically let the input window popup
            if (UserStorage.Users.Count == 0)
                InitializeNewUserButton();
            
            //spawn button objects with small delay in between
            foreach (var user in UserStorage.Users)
            {
                AddButtton(user);
                yield return new WaitForSeconds(0.2f);
            }
            yield return null;
        }

        public void InitializeNewUserButton()
        {
            InputWindow.SetActive(true);
            inputFieldName.text = "";
            inputFieldAge.text = "";
            inputFieldFVC.text = "";
            inputFieldPEF.text = "";
            inputWindowUser.localScale = Vector3.zero;
            blocker.gameObject.SetActive(true);
            blocker.GetComponent<Image>().color = Color.clear;
            LeanTween.color(blocker, semiTrans, 0.5f);
            LeanTween.scale(inputWindowUser.gameObject, new Vector3(5, 5, 5), 0.5f);
        }
    
        public void SubmitNewUser()
        {
            //text validation
            if (string.IsNullOrEmpty(inputFieldName.text)) {ConnectionCanvas.Instance.Warn_Custom("Vul een naam in"); return; }
            if (string.IsNullOrEmpty(inputFieldAge.text)) {ConnectionCanvas.Instance.Warn_Custom("Vul een leeftijd in"); return;}
            if (string.IsNullOrEmpty(inputFieldFVC.text)) {ConnectionCanvas.Instance.Warn_Custom("Vul een voorspelde FVC in"); return;}
            if (string.IsNullOrEmpty(inputFieldPEF.text)) {ConnectionCanvas.Instance.Warn_Custom("Vul een voorspelde PEF in"); return;}
            
            //parse input fields
            var username = inputFieldName.text;
            var age = float.Parse(inputFieldAge.text);
            var fvc = float.Parse(inputFieldFVC.text);
            var pef = float.Parse(inputFieldPEF.text);
            
            //value validation
            if (fvc < settings.MinimumFVC) {ConnectionCanvas.Instance.Warn_Custom("FVC is te laag"); return;}
            if (fvc > settings.MaximumFVC) {ConnectionCanvas.Instance.Warn_Custom("FVC is te hoog"); return;}
            if (pef < settings.MinimumPEF) {ConnectionCanvas.Instance.Warn_Custom("PEF is te laag"); return;}
            if (pef > settings.MaximumPEF) {ConnectionCanvas.Instance.Warn_Custom("PEF is te hoog"); return;}

            //name validation
            if (UserStorage.Users.Any(user => user.userName == username))
            {
                ConnectionCanvas.Instance.Warn_Custom("Die naam bestaat al");
                Debug.Log("Cannot add " + username + " because it already exists in the database");
                return;
            }
            
            //save user to storage
            var metaphorStatus = settings.MetaphorInfo.Select(metaphor => metaphor.unlockedPerDefault).ToArray();
            var newUser = new UserStorage.User(username, age, fvc, pef, settings.startingCoinAmount, DateTime.Now, metaphorStatus, DateTime.Now, 0, (DateTime.Now - TimeSpan.FromDays(8)), 0);
            newUser.Save();
            UserStorage.Users.Add(newUser);
            Debug.Log("<b>USER SELECTION:</b> User added: " + newUser.userName);

            //visualize new user
            AddButtton(newUser);
            LeanTween.scale(inputWindowUser.gameObject, Vector3.zero, 0.5f);
            blocker.gameObject.SetActive(false);
        }
    
        public void CancelNewUser()
        {
            blocker.gameObject.SetActive(false);
            LeanTween.scale(inputWindowUser.gameObject, Vector3.zero, 0.5f);
        }

        //spawn in a new button
        private void AddButtton(UserStorage.User user)
        {
            //spawn button prefab
            var button = Instantiate(ButtonPrefab, ButtonParent);
            button.transform.SetSiblingIndex(ButtonParent.childCount - 2);
            
            //set button size and intro animation
            var localScale = button.transform.localScale;
            var originalScale = new Vector3(localScale.x, localScale.y, localScale.z);
            button.transform.localScale = Vector3.zero;
            LeanTween.scale(button, originalScale, 0.4f);

            //assign functions to the two button elements
            button.transform.GetChild(0).GetComponent<Button>().onClick.AddListener(delegate { Login(user); });
            button.transform.GetChild(1).GetComponent<Button>().onClick.AddListener(delegate { RemoveUser(button, user); });
            button.GetComponentInChildren<TMPro.TextMeshProUGUI>().text = user.userName;
        }

        private static void RemoveUser(Object button, UserStorage.User user)
        {
            Debug.Log("<b>USER SELECTION:</b> User removed: " + user.userName);
            Logger.DeleteFolder(user.userName);
            Destroy(button);
            UserStorage.Users.Remove(user);
        }

        public static string RememberedSession = "";
        private void Login(UserStorage.User user)
        {
            if (IsLogginIn) return;
            IsLogginIn = true;
            if (SessionManager.Instance != null)
                SessionManager.ClearCache();
            SessionManager.CurrentUser = user;

            //try to remember previous session
            RememberedSession = SessionManager.RememberSession();
            if (RememberedSession == "")
            {
                LevelChanger.FadeToLevel("MainMenu");
                SessionManager.CurrentUser.currentSessionID = 0;
            }
            //if session found, prompt user to resume
            else
            {
                inputWindowSession.gameObject.SetActive(true);
                inputWindowSession.localScale = Vector3.zero;
                LeanTween.scale(inputWindowSession.gameObject, new Vector3(5, 5, 5), 0.5f);
                
                //display metaphor
                sessionSubtext.text = "";
                foreach (var metaphor in settings.MetaphorInfo)
                {
                    if (metaphor.id == RememberedSession) sessionSubtext.text = "Level: " + metaphor.displayName;
                }

                //background
                blocker.gameObject.SetActive(true);
                blocker.GetComponent<Image>().color = Color.clear;
                LeanTween.color(blocker, semiTrans, 0.5f);
                
            }
        }

        public void SessionResumeButton()
        {
            
            LevelChanger.FadeToLevel("MainMenu");
        }

        public void SessionCancelButton()
        {
            SessionManager.CurrentUser.currentSessionID = 0;
            RememberedSession = "";
            LevelChanger.FadeToLevel("MainMenu");
        }
        
        private void Update()
        {
            //catch android back button action, and reroute it to touch back button action
            if (Input.GetKeyDown(KeyCode.Escape)) QuitGame();
        }

        public void QuitGame()
        {
            /*if (Application.platform == RuntimePlatform.WindowsEditor) UnityEditor.EditorApplication.isPlaying = false;
            else */Application.Quit();
        }

        public void OpenHelpPage()
        {
            Application.OpenURL("http://gli-calculator.ersnet.org/index.html");
        }
    }
}
