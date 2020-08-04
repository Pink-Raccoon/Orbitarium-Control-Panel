using System;
using System.CodeDom;
using System.Collections;
using System.Collections.Generic;
using System.Net;
using UnityEngine;
using UnityEngine.UI;

public class SettingsManager
{
    private readonly LogManager log;
    private readonly IDictionary<string, string> requiredSettings = new Dictionary<string, string>()
    { 
        { "resX", "resolution X" },
        { "resY", "resolution Y" },
        { "inputResX", "input resolution x" },
        { "inputResY", "input resolution y" }
    };

    private readonly int MAX_RES = 8000;
    private readonly int MIN_RES = 100;

    public SettingsManager(LogManager log)
    {
        this.log = log;
    }

    public void SaveSettings()
    {
        log.LogWrite("Saving settings...");
        try
        {
            int resX = GetResolution(GameObject.Find("resX").GetComponent<InputField>().text);
            int resY = GetResolution(GameObject.Find("resY").GetComponent<InputField>().text);
            int inputResX = GetResolution(GameObject.Find("inputResX").GetComponent<InputField>().text);
            int inputResY = GetResolution(GameObject.Find("inputResY").GetComponent<InputField>().text);
            var aspects = CalculateAspectRatio(resX, resY);
            var aspectX = aspects.Item1;
            var aspectY = aspects.Item2;
            PlayerPrefs.SetInt("resX", resX);
            PlayerPrefs.SetInt("resY", resY);
            PlayerPrefs.SetInt("aspectX", aspectX);
            PlayerPrefs.SetInt("aspectY", aspectY);
            PlayerPrefs.SetInt("inputResX", inputResX);
            PlayerPrefs.SetInt("inputResY", inputResY);
            PlayerPrefs.Save();
        } catch (Exception e)
        {
            log.LogWrite(e.Message);
            log.ShowSaveSettingsInformation(e.Message, true);
            log.LogWrite("Error. Settings not saved.");
            return;
        }
        log.ShowSaveSettingsInformation("Settings successfully saved!", false);
        log.LogWrite("Settings successfully saved!");
    }

    private string ValidateIP(string address)
    {
        bool validIP = IPAddress.TryParse(address, out IPAddress ip);
        if (validIP)
        {
            return address;
        } else
        {
            throw new FormatException("Entered IP Address is invalid. Please specify a valid IP-Address in settings.");
        }
    }

    public void LoadSettings()
    {
        int resx = 0;
        int resy = 0;
        int inputresx = 0;
        int inputresy = 0;
        if (PlayerPrefs.HasKey("resX"))
        {
            resx = PlayerPrefs.GetInt("resX");
            GameObject.Find("resX").GetComponent<InputField>().text = resx.ToString();
        }
        if (PlayerPrefs.HasKey("resY"))
        {
            resy = PlayerPrefs.GetInt("resY");
            GameObject.Find("resY").GetComponent<InputField>().text = resy.ToString();
        }
        if (PlayerPrefs.HasKey("inputResX"))
        {
            inputresx = PlayerPrefs.GetInt("inputResX");
            GameObject.Find("inputResX").GetComponent<InputField>().text = inputresx.ToString();
        }
        if (PlayerPrefs.HasKey("inputResY"))
        {
            inputresy = PlayerPrefs.GetInt("inputResY");
            GameObject.Find("inputResY").GetComponent<InputField>().text = inputresy.ToString();
        }

        CalculateAspectRatio(resx, resy);
    }

    public void CheckSettings()
    {
        foreach (KeyValuePair<string, string> entry in requiredSettings)
        {
            if (!PlayerPrefs.HasKey(entry.Key))
            {
                throw new ArgumentNullException("Required setting '" + entry.Value + "' missing.\nPlease enter it in 'Settings'.");
            }
        }
    }

    private int GetResolution(string resString)
    {
        bool parseSuccessful = Int32.TryParse(resString, out int res);
        if (!parseSuccessful)
        {
            var ex = new FormatException("Could not save the resolution settings.\nPlease specify numbers in the section 'Projector resolution'");
            throw ex;
        }
        if (IsValidResolution(res))
        {
            return res;
        } else
        {
            return -1;
        }
    }

    private bool IsValidResolution(int res)
    {

        if(res > MAX_RES || res < MIN_RES)
        {
            var ex = new ArgumentOutOfRangeException("Specified resolution '" + res + "' is out of range.\nPlease specify a resolution between 100 and 8000 pixels.");
            throw ex;
        }
        return true;
    }

    private (int,int) CalculateAspectRatio(int resx, int resy)
    {
        var ggt = CalcGgt(resx, resy);
        var aspectx = resx / ggt;
        var aspecty = resy / ggt;
        if (aspectx < 2 * aspecty)
        {
            log.ShowAspectRatio(aspectx, aspecty, true);
        }
        else
        {
            log.ShowAspectRatio(aspectx, aspecty, false);
            log.LogWrite("Warning: Detected anomal aspect ratio " + aspectx + ":" + aspecty);
        }
        return (aspectx, aspecty);
    }

    private int CalcGgt(int res1, int res2)
    {
        int number1 = res1;
        int number2 = res2;
        int temp = 0;
        int ggt = 0;
        while (number1 % number2 != 0)
        {
            temp = number1 % number2;
            number1 = number2;
            number2 = temp;
        }
        ggt = number2;
        return ggt;
    }
}
