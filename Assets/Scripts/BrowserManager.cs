using UnityEngine;
using OpenQA.Selenium;
using OpenQA.Selenium.Chrome;
using OpenQA.Selenium.Support.UI;
using System;
using System.IO;
using System.ComponentModel;

public class BrowserManager
{
    private IWebDriver displayBrowser;
    private IWebDriver contentBrowser;

    public BrowserManager()
    {
        
    }

    public void StartDisplayBrowser()
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
        displayBrowser.FindElement(By.Id("server")).SendKeys(PlayerPrefs.GetString("ip"));
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

    private void ExecuteScriptOnDriver(string script)
    {
        IJavaScriptExecutor js = (IJavaScriptExecutor)displayBrowser;
        js.ExecuteScript(script);
    }

    private string GetScript(string scriptPath)
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

    public void StartContentBrowser()
    {
        ChromeOptions options = new ChromeOptions();
        options.AddExcludedArgument("enable-automation");
        options.AddAdditionalCapability("useAutomationExtension", false);
        contentBrowser = new ChromeDriver(options);
        contentBrowser.Navigate().GoToUrl("http://localhost/orbitarium.ba/welcome");        
    }

    public void CheckForDisplayDriver()
    {
        //log.LogWrite("Search for display driver service");
        ////check if display driver is installed and running
        //ServiceController sc = null;
        //try
        //{
        //    sc = new ServiceController("spacedeskService");
        //}
        //catch (Win32Exception)
        //{
        //    var ex = new InvalidOperationException("Display driver not found. Please install spacedesk windows DRIVER");
        //    throw ex;
        //}

        //log.LogWrite("Status of display driver service is:" + sc.Status.ToString());
        //if (!sc.Status.Equals(ServiceControllerStatus.Running))
        //{
        //    var ex = new InvalidOperationException("Display driver error: The spacedesk display driver is not running.\nPlease start the service.");
        //    throw ex;
        //}
    }

}
