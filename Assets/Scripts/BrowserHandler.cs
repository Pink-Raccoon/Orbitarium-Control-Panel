using OpenQA.Selenium;
using OpenQA.Selenium.Chrome;
using OpenQA.Selenium.Support.UI;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Threading;
using UnityEngine;

public class BrowserHandler : MonoBehaviour
{
    public static IWebDriver DisplayBrowser { get; set; }
    public static IWebDriver ContentBrowser { get; set; }

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
        if(DisplayBrowser == null)
        {
            LogHandler.WriteMessage("Starting display driver...");
            //set profile so that driver does not have to be configured
            var pathLocalAppData = Environment.GetEnvironmentVariable("LocalAppData");
            var chromeDefaultProfilePath = Path.Combine(pathLocalAppData, @"Google\Chrome\User Data");

            //hide command line used to start browser
            var driverService = ChromeDriverService.CreateDefaultService();
            driverService.HideCommandPromptWindow = true;

            //Configure chrome options (hide indication that browser is controlled by selenium, set default user profile).
            ChromeOptions options = new ChromeOptions();
            options.AddArgument("user-data-dir=" + chromeDefaultProfilePath);
            options.AddExcludedArgument("enable-automation");
            options.AddAdditionalCapability("useAutomationExtension", false);

            //navigate to url
            DisplayBrowser = new ChromeDriver(options);
            DisplayBrowser.Navigate().GoToUrl("http://viewer.spacedesk.net/");
            //wait for element "server" to be present
            WebDriverWait wait = new WebDriverWait(DisplayBrowser, TimeSpan.FromSeconds(10));
            wait.Until(SeleniumExtras.WaitHelpers.ExpectedConditions.ElementToBeClickable(By.CssSelector("#server")));
            DisplayBrowser.FindElement(By.Id("buttonLogin")).Click();
            Thread.Sleep(1000);
            LogHandler.WriteMessage("SUCCESS: Display driver started");
        } else
        {
            LogHandler.WriteMessage("INFO: Display driver already running");
        }
        
    }

    internal static void ExecuteScriptContentBrowser(string runCommand)
    {
        IJavaScriptExecutor js = (IJavaScriptExecutor)ContentBrowser;
        js.ExecuteScript(runCommand);
    }

    public static void StartContentBrowser(string startUri)
    {
        if(ContentBrowser != null)
        {
            ContentBrowser.Close();
        }
        //hide command line used to start browser
        var driverService = ChromeDriverService.CreateDefaultService();
        driverService.HideCommandPromptWindow = true;
        ChromeOptions options = new ChromeOptions();
        options.AddExcludedArgument("enable-automation");
        options.AddAdditionalCapability("useAutomationExtension", false);
        ContentBrowser = new ChromeDriver(driverService, options);
        ContentBrowser.Navigate().GoToUrl(startUri);

        var leftBound = System.Windows.Forms.Screen.PrimaryScreen.Bounds.Left;
        var posPoint = new Point(leftBound, 0);
        ContentBrowser.Manage().Window.Position = posPoint;
        ContentBrowser.Manage().Window.FullScreen();
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
