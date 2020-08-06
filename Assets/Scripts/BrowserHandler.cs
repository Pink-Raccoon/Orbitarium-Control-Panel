using OpenQA.Selenium;
using OpenQA.Selenium.Chrome;
using OpenQA.Selenium.Support.UI;
using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Threading;
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
        //set profile so that driver does not have to be configured
        var pathLocalAppData = Environment.GetEnvironmentVariable("LocalAppData");
        var chromeDefaultProfilePath = Path.Combine(pathLocalAppData, @"Google\Chrome\User Data");

        //Configure chrome options (hide indication that browser is controlled by selenium, set default user profile).
        ChromeOptions options = new ChromeOptions();
        options.AddArgument("user-data-dir=" + chromeDefaultProfilePath);
        options.AddExcludedArgument("enable-automation");
        options.AddAdditionalCapability("useAutomationExtension", false);
        
        //navigate to url
        displayBrowser = new ChromeDriver(options);
        displayBrowser.Navigate().GoToUrl("http://viewer.spacedesk.net/");
        //wait for element "server" to be present
        WebDriverWait wait = new WebDriverWait(displayBrowser, TimeSpan.FromSeconds(10));
        wait.Until(SeleniumExtras.WaitHelpers.ExpectedConditions.ElementToBeClickable(By.CssSelector("#server")));
        displayBrowser.FindElement(By.Id("buttonLogin")).Click();
        Thread.Sleep(1000);
        LogHandler.WriteMessage("SUCCESS: Display driver started");
    }

    public void StartContentBrowser()
    {
        ChromeOptions options = new ChromeOptions();
        options.AddExcludedArgument("enable-automation");
        options.AddAdditionalCapability("useAutomationExtension", false);
        contentBrowser = new ChromeDriver(options);
        contentBrowser.Navigate().GoToUrl("http://localhost/orbitarium.ba/welcome");
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
