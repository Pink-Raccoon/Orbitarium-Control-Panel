using OpenQA.Selenium;
using OpenQA.Selenium.Chrome;
using OpenQA.Selenium.Support.UI;
using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;

public class BrowserHandler : MonoBehaviour
{
    public static IWebDriver displayBrowser { get; set; }
    public static IWebDriver contentBrowser { get; set; }

    public static BrowserHandler _instance;
    public static BrowserHandler Instance
    {
        get
        {
            if (_instance == null)
            {
                _instance = GameObject.FindObjectOfType<BrowserHandler>();

                if (_instance == null)
                {
                    GameObject container = new GameObject("BrowserHandler");
                    _instance = container.AddComponent<BrowserHandler>();

                }
            }
            return _instance;
        }
    }

    public static void StartDisplayDriver()
    {
        LogHandler.WriteMessage("Starting display driver...");
        //Configure chrome options (hide indication that browser is controlled by selenium).
        ChromeOptions options = new ChromeOptions();
        options.AddExcludedArgument("enable-automation");
        options.AddAdditionalCapability("useAutomationExtension", false);
        //navigate to url
        displayBrowser = new ChromeDriver(options);
        displayBrowser.Navigate().GoToUrl("http://viewer.spacedesk.net/");
        //wait for element "server" to be present
        WebDriverWait wait = new WebDriverWait(displayBrowser, TimeSpan.FromSeconds(10));
        wait.Until(SeleniumExtras.WaitHelpers.ExpectedConditions.ElementToBeClickable(By.CssSelector("#server")));
        //clear ip field
        displayBrowser.FindElement(By.Id("server")).Clear();
        //write ip address address of network interface
        displayBrowser.FindElement(By.Id("server")).SendKeys("127.0.0.1");
        //connect
        displayBrowser.FindElement(By.Id("buttonLogin")).Click();
        LogHandler.WriteMessage("Display driver started successfully.");
        displayBrowser.Manage().Timeouts().ImplicitWait = TimeSpan.FromSeconds(10);
        wait.Until(SeleniumExtras.WaitHelpers.ExpectedConditions.ElementToBeClickable(By.CssSelector("#toggleMenu")));
        //getting js for configuration of display driver
        var basePath = Directory.GetCurrentDirectory();
        var scriptPath = basePath + @"\Assets\Scripts\js\displaydriver_config.js";
        var script = GetScript(scriptPath);
        script += "$( document ).ready(function() {setUpDisplay(" + PlayerPrefs.GetInt("resx") + "," + PlayerPrefs.GetInt("resy") + ");})";
        ExecuteScriptOnDriver(script);
        LogHandler.WriteMessage("Display driver configured successfully.");
    }

    private static string GetScript(string scriptPath)
    {
        string line;
        string script = "";
        StreamReader file = new System.IO.StreamReader(scriptPath);
        while ((line = file.ReadLine()) != null)
        {
            script += line;
        }
        file.Close();
        return script;
    }

    private static void ExecuteScriptOnDriver(string script)
    {
        IJavaScriptExecutor js = (IJavaScriptExecutor)displayBrowser;
        js.ExecuteScript(script);
    }


    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
