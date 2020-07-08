using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;
using UnityEngine;
using UnityEditor;

public class LogManager
{
    private Text logText;
    private Text aspectInfo;
    private Text infoText;
    
    public LogManager()
    {
        logText = GameObject.Find("LogText").GetComponent<Text>();
        aspectInfo = GameObject.Find("aspectInfo").GetComponent<Text>();
        infoText = GameObject.Find("infoText").GetComponent<Text>();
    }

    public void LogWrite(string entry)
    {
        logText.text += entry + "\n";
    }

    public void ShowAspectRatio(int aspectx, int aspecty, bool valid)
    {
        if (valid)
        {
            aspectInfo.color = Color.green;
        } 
        else
        {
            aspectInfo.color = Color.red;
        }
        aspectInfo.text = aspectx + ":" + aspecty;
    }

    public void ShowSaveSettingsInformation(string text, bool fail)
    {
        if (fail)
        {
            infoText.color = Color.red;
        } else
        {
            infoText.color = Color.green;
        }
        infoText.text = text;
    }
}
