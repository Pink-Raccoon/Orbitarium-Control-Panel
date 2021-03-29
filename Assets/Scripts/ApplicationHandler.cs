using System;
using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using UnityEngine.UI;

public class ApplicationHandler : MonoBehaviour
{
    public Text SelectedVideo;

    private RenderHandler renderHandler;
    private string videoPath;

    //called before start
    void Awake()
    {
        Debug.Log("Application started");
        LogHandler.WriteMessage("Application started");
        LogHandler.WriteMessage("Begin initialization");
        var settingsHandler = GameObject.Find("ApplicationHandlerObject").GetComponent<SettingsHandler>();
        settingsHandler.LoadSettings();
        renderHandler = GameObject.Find("RenderingObject").GetComponent<RenderHandler>();
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

    public void Load()
    {
        var animationsDropdown = GameObject.Find("animationsDropdown").GetComponent<Dropdown>();
        var selectedAnimation = animationsDropdown.value;
        var animationInformation = AnimationsHandler.AnimationSummary[animationsDropdown.options[selectedAnimation].text];
        AnimationsHandler.CurrentAnimation = animationInformation;
        var initUri = animationInformation.InitUri;
        var runCommand = animationInformation.RunCommand;
        AnimationsHandler.StopAnimation();
        BrowserHandler.StartDisplayDriver();
        BrowserHandler.StartContentBrowser(initUri);
        LogHandler.WriteMessage("Please start ManyCam and choose the Desktop displaying the Animation.");
    }
    
    public void SelectVideo()
    {
        videoPath = EditorUtility.OpenFilePanel("Select Video", "", "*");
        SelectedVideo.text = videoPath;
    }

    public void StartVideo()
    {
        if (videoPath.Length != 0)
        {
            renderHandler.RenderVideo(videoPath);
        }
    }

    public void PlayPauseVideo()
    {
        renderHandler.PlayPauseVideo();
    }

    public void Backward()
    {
        renderHandler.Backward();
    }

    public void Forward()
    {
        renderHandler.Forward();
    }

    public void Loop()
    {
        renderHandler.Loop();
    }

    public void StartAnimation()
    {
        var runCommand = AnimationsHandler.CurrentAnimation.RunCommand;
        BrowserHandler.ExecuteScriptContentBrowser(runCommand);
    }

    public void StopAnimation()
    {
        AnimationsHandler.StopAnimation();
    }

    public void ContinueAnimation()
    {
        AnimationsHandler.ContinueAnimation();
    }
}
