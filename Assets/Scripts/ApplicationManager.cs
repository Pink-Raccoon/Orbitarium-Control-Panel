using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System.Runtime.InteropServices;
using System;
using System.Diagnostics;
using System.Runtime.CompilerServices;
using System.IO;

public class ApplicationManager : MonoBehaviour
{
    private LogManager log;
    private BrowserManager browserManager;
    private ProjectionManager projectionManager;
    private SettingsManager settings;

    void Start()
    {
        log = new LogManager();
        browserManager = new BrowserManager(log);
        //projectionManager = new ProjectionManager(log);
        settings = new SettingsManager(log);
        settings.LoadSettings();
    }

    void Update()
    {

    }

    public void StartApplication()
    {
        log.LogWrite("Starting Application");
        try
        {
            RunPreChecks();
            projectionManager.SetupInputView();
            browserManager.StartDisplayBrowser();
            System.Threading.Thread.Sleep(5000);
            browserManager.StartContentBrowser();
            projectionManager.StartManyCam();
            projectionManager.RenderManyCam();
            projectionManager.SetupDisplays();
            
        } catch (Exception e)
        {
            log.LogWrite(e.Message);
            log.LogWrite("Application failed to start.");
        }
    }

    public void RunPreChecks()
    {
        log.LogWrite("Running pre checks...");
        try
        {
            settings.CheckSettings();
            browserManager.CheckForDisplayDriver();
        } catch (Exception e)
        {
            throw new Exception(e.Message + "\nPre checks result: FAIL");
        }
        log.LogWrite("Pre checks result: PASS");
        
    }

    public void StartDisplayBrowser()
    {
        browserManager.StartDisplayBrowser();
    }
    
    public void StartContentBrowser()
    {
        browserManager.StartContentBrowser();
    }

    public void StartDisplayRecorder()
    {
        projectionManager.StartManyCam();
    }

    public void RenderDisplayRecorder()
    {
        projectionManager.RenderManyCam();
    }

    public void SetupInternalViews()
    {
        projectionManager.SetupInputView();
        projectionManager.SetupTransformedView();
    }

    public void SaveSettings()
    {
        settings.SaveSettings();
    }
    
    public void AdjustProjector(string action)
    {
        if (action.Equals("up"))
        {
            projectionManager.AdjustProjection(AdjustAction.UP);
        } 
        else if (action.Equals("down"))
        {
            projectionManager.AdjustProjection(AdjustAction.DOWN);
        }
        else if (action.Equals("right"))
        {
            projectionManager.AdjustProjection(AdjustAction.RIGHT);
        }
        else if (action.Equals("left"))
        {
            projectionManager.AdjustProjection(AdjustAction.LEFT);
        }
        else if (action.Equals("mirror"))
        {
            projectionManager.AdjustProjection(AdjustAction.MIRROR);
        }
    }

    public void CheckDisplays()
    {
        projectionManager.SetupDisplays();
    }
}
