using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ApplicationHandler : MonoBehaviour
{
    private GameObject renderHandler;

    //called before start
    void Awake()
    {
        Debug.Log("Application started");
        LogHandler.WriteMessage("Application started");
        LogHandler.WriteMessage("Begin initialization");
        var settingsHandler = GameObject.Find("ApplicationHandlerObject").GetComponent<SettingsHandler>();
        settingsHandler.LoadSettings();
        //renderHandler = GameObject.Find("RenderingObject");
        //renderHandler.SetActive(false);
        //renderHandler.SetActive(true);
    }

    // Start is called before the first frame update
    void Start()
    {
        InitializeApplication();
    }

    private void InitializeApplication()
    {
        var settingsHandler = GameObject.Find("ApplicationHandlerObject").GetComponent<SettingsHandler>();
        settingsHandler.ApplySettingsToInternalObjects();
        if (AnimationsHandler.CheckAnimationDataAvailability())
        {
            AnimationsHandler.GetAnimationsSummary();
        }
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    
}
