using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using Event = Spirometry.ScriptableObjects.Event;

[RequireComponent(typeof(Animator))]
public class ConnectionCanvas : MonoBehaviour
{
    /// <summary>
    /// Spiro-Play project by University of Twente, MST and Deventer ziekenhuis
    /// Made by Koen Vogel - k.a.vogel@student.utwente.nl
    /// 
    /// ConnectionCanvas class:
    /// Canvas controller to visualize state of spirometer connection, driven bij SpiroReceiver
    /// </summary>
    
    #region variables
    #pragma warning disable 649
    
    private Animator _animator;
    [SerializeField] private Event onConnectedToSpirometer;
    [SerializeField] private Event onDisconnectedFromSpirometer;
    [SerializeField] private Image statusIcon;
    [SerializeField] private TMPro.TextMeshProUGUI warningText;
    
    private static readonly int WarningConnected = Animator.StringToHash("WarningConnected");

#pragma warning restore 649
    #endregion

    //singleton pattern
    public static ConnectionCanvas Instance { get; private set; }
    private bool spiroConnected = false;
    
    private void Awake()
    {
        //singleton pattern (https://wiki.unity3d.com/index.php/Singleton)
        if (Instance != null && Instance != this)
            Destroy(this.gameObject);
        else
            Instance = this;
        DontDestroyOnLoad(this.gameObject);
    }

    // Start is called before the first frame update
    private void Start()
    {
        _animator = GetComponent<Animator>();
        
        //subscribe to connection events and trigger animations
        onConnectedToSpirometer.value.AddListener(() => spiroConnected = true);
        onDisconnectedFromSpirometer.value.AddListener(() => spiroConnected = false);
    }

    private void Update()
    {
        //refresh UI
        if (SceneManager.GetActiveScene().name == "UserSelection") return;
        if (statusIcon.gameObject.activeSelf && spiroConnected) Warn_Connect();
        else if (!statusIcon.gameObject.activeSelf && !spiroConnected) Warn_Disconnect();
    }


    private void Warn_Connect()
    {
        statusIcon.gameObject.SetActive(false);
        _animator.SetTrigger(WarningConnected);
        warningText.text = "Spirometer gekoppeld";
    }

    private void Warn_Disconnect()
    {
        statusIcon.gameObject.SetActive(true);
        _animator.SetTrigger(WarningConnected);
        warningText.text = "Spirometer ontkoppeld";
    }

    public void Warn_Custom(string message)
    {
        _animator.SetTrigger(WarningConnected);
        warningText.text = message;
    }

    public void DownloadBackgroundApp()
    {
        Application.OpenURL("https://drive.google.com/file/d/1zjMCJuxjLz3wzkb-ZFItA8gAL_6hhOla/view?usp=sharing");
    }
    
    public void ExitGame()
    {
        Application.Quit();
    }
}
