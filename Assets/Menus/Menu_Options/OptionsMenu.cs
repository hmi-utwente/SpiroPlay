using System;
using System.Collections;
using System.Collections.Generic;
using Spirometry.Debugging;
using Spirometry.ScriptableObjects;
using Spirometry.SpiroController;
using UnityEngine;
using Logger = Spirometry.SpiroController.Logger;

public class OptionsMenu : MonoBehaviour
{
    /// <summary>
    /// Spiro-Play project by University of Twente, MST and Deventer ziekenhuis
    /// Made by Koen Vogel - k.a.vogel@student.utwente.nl
    /// 
    /// OptionsMenu class:
    /// Canvas controller to visualize and control several settings
    /// </summary>
    
    #region variables
    #pragma warning disable 649

    [SerializeField] private Settings settings;
    [SerializeField] private TMPro.TextMeshProUGUI versionNr;
    private bool _debuggingEnabled = false;
    
    #pragma warning restore 649
    #endregion
    
    // Start is called before the first frame update
    private void Start()
    {
        versionNr.text = "Versie nummer: " + Application.version;
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

    public void ClearLogsUser()
    {
        ConnectionCanvas.Instance.Warn_Custom("Logs en gebruiker verwijderd");
        Logger.DeleteFolder(SessionManager.CurrentUser.userName);
        LevelChanger.FadeToLevel("UserSelection");
    }
    
    public void ClearLogsAll()
    {
        ConnectionCanvas.Instance.Warn_Custom("Alle logs en gebruikers verwijderd");
        Logger.DeleteAllFolders();
        LevelChanger.FadeToLevel("UserSelection");
    }

    public void ExportUserLogs()
    {
        if (Application.platform != RuntimePlatform.Android || Application.platform == RuntimePlatform.IPhonePlayer)
        {
            Debug.Log("Cannot export logs on non-mobile platforms");
            return;
        }
        Debug.Log("Attempting export of user logs");
        
        var shareInfo = new NativeShare();
        foreach (var log in Logger.GetUserLogs())
        {
            shareInfo.AddFile(log);
        }    
        shareInfo.SetSubject("Spiroplay logs");
        shareInfo.SetCallback((result, shareTarget) => Debug.Log("Share result: " + result + ", selected app: " + shareTarget));
        shareInfo.Share();
    }
    
    public void ExportLogs()
    {
        if (Application.platform != RuntimePlatform.Android || Application.platform == RuntimePlatform.IPhonePlayer)
        {
            Debug.Log("Cannot export logs on non-mobile platforms");
            return;
        }
        Debug.Log("Attempting export of all logs");
        
        var shareInfo = new NativeShare();
        foreach (var log in Logger.GetAllLogs())
        {
            shareInfo.AddFile(log);
        }    
        shareInfo.SetSubject("Spiroplay logs");
        shareInfo.SetCallback((result, shareTarget) => Debug.Log("Share result: " + result + ", selected app: " + shareTarget));
        shareInfo.Share();
    }

    public void UnlockAllMetaphors()
    {
        //count and unlock locked metaphors
        var unlocked = 0;
        for (var index = 0; index < SessionManager.CurrentUser.metaphorStatus.Length; index++)
        {
            if (SessionManager.CurrentUser.metaphorStatus[index] == true) continue;
            SessionManager.CurrentUser.metaphorStatus[index] = true;
            unlocked++;
        }
        
        //if all metaphors are already unlocked
        if (unlocked <= 0)
        {
            ConnectionCanvas.Instance.Warn_Custom("Alle metaforen zijn al speelbaar voor " + SessionManager.CurrentUser.userName);
            return;
        }
        
        //finalize and save
        SessionManager.CurrentUser.lastUnlockDate = DateTime.Now;
        SessionManager.CurrentUser.Save();
        ConnectionCanvas.Instance.Warn_Custom(unlocked + " metaforen ongrendeld; Alles is nu speelbaar voor " + SessionManager.CurrentUser.userName);
    }
    
    public void LockAllMetaphors()
    {
        //count and unlock locked metaphors
        var locked = 0;
        for (var index = 0; index < SessionManager.CurrentUser.metaphorStatus.Length; index++)
        {
            if (SessionManager.CurrentUser.metaphorStatus[index] == false) continue;
            SessionManager.CurrentUser.metaphorStatus[index] = false;
            locked++;
        }
        
        //if all metaphors are already unlocked
        if (locked <= 0)
        {
            ConnectionCanvas.Instance.Warn_Custom("Alle levels zijn al vergrendeld voor " + SessionManager.CurrentUser.userName);
            return;
        }
        
        //finalize and save
        SessionManager.CurrentUser.lastUnlockDate = DateTime.Now;
        SessionManager.CurrentUser.Save();
        ConnectionCanvas.Instance.Warn_Custom(locked + " levels vergrendeld; Alles is nu vergrendeld voor " + SessionManager.CurrentUser.userName);
    }

    public void UnlockSingleMetaphor(int index)
    {
        var oldValue = SessionManager.CurrentUser.metaphorStatus[index];
        SessionManager.CurrentUser.metaphorStatus[index] = true;
        
        //if all metaphors are already unlocked
        if (oldValue == true)
        {
            ConnectionCanvas.Instance.Warn_Custom("Level is al speelbaar voor " + SessionManager.CurrentUser.userName);
            return;
        }
        
        //finalize and save
        SessionManager.CurrentUser.lastUnlockDate = DateTime.Now;
        SessionManager.CurrentUser.Save();
        ConnectionCanvas.Instance.Warn_Custom("Level ongrendeld en speelbaar voor " + SessionManager.CurrentUser.userName);
    }
    
    public void LockSingleMetaphor(int index)
    {
        var oldValue = SessionManager.CurrentUser.metaphorStatus[index];
        SessionManager.CurrentUser.metaphorStatus[index] = false;
        
        //if all metaphors are already unlocked
        if (oldValue == false)
        {
            ConnectionCanvas.Instance.Warn_Custom("Level is al vergrendeld voor " + SessionManager.CurrentUser.userName);
            return;
        }
        
        //finalize and save
        SessionManager.CurrentUser.lastUnlockDate = DateTime.Now;
        SessionManager.CurrentUser.Save();
        ConnectionCanvas.Instance.Warn_Custom("Level nu vergrendeld voor " + SessionManager.CurrentUser.userName);
    }
}
