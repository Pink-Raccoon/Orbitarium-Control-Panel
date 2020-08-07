using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

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

    public void Load()
    {
        var animationsDropdown = GameObject.Find("animationsDropdown").GetComponent<Dropdown>();
        var selectedAnimation = animationsDropdown.value;
        var animationInformation = AnimationsHandler.AnimationSummary[animationsDropdown.options[selectedAnimation].text];
        var initUri = animationInformation.InitUri;
        var runCommand = animationInformation.RunCommand;
        Debug.Log(initUri);
        BrowserHandler.StartDisplayDriver();
        BrowserHandler.StartContentBrowser(initUri);
        BrowserHandler.ExecuteScriptContentBrowser(runCommand);
    }

    
}
