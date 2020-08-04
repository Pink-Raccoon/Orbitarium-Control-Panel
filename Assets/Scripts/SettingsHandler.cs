using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class SettingsHandler : MonoBehaviour
{
    private IDictionary<string, string> requiredSettings;

    void Start()
    {
        requiredSettings = new Dictionary<string, string>()
            {
                { "resX", "resolution X" },
                { "resY", "resolution Y" },
                { "inputResX", "input resolution x" },
                { "inputResY", "input resolution y" }
            };
    }

    // Update is called once per frame
    void Update()
    {

    }

    public void SaveSettings()
    {
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
            var infoText = GameObject.Find("infoText").GetComponent<Text>();
            infoText.color = Color.green;
            infoText.text = "Settings successfully saved";
        }
        catch (Exception e)
        {
            //log.LogWrite(e.Message);
            //log.ShowSaveSettingsInformation(e.Message, true);
            //log.LogWrite("Error. Settings not saved.");
            return;
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
        return res;
    }

    private (int, int) CalculateAspectRatio(int resx, int resy)
    {
        var ggt = CalcGgt(resx, resy);
        var aspectx = resx / ggt;
        var aspecty = resy / ggt;
        DisplayAspectInfo(aspectx, aspecty);
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

    private void DisplayAspectInfo(int aspectx, int aspecty)
    {
        var aspectInfo = GameObject.Find("aspectInfo").GetComponent<Text>();
        if (aspectx < 2 * aspecty)
        {
            aspectInfo.color = Color.green;
        }
        else
        {
            aspectInfo.color = Color.green;
            //log.LogWrite("Warning: Detected anomal aspect ratio " + aspectx + ":" + aspecty);
        }
        aspectInfo.text = aspectx + " : " + aspecty;
    }


}
