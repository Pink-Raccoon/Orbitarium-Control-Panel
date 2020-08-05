using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class LogHandler : MonoBehaviour
{
    public static Text logText = GameObject.Find("LogText").GetComponent<Text>();
    public static Text infoText = GameObject.Find("infoText").GetComponent<Text>();

    public static LogHandler _instance;
    public static LogHandler Instance
    {
        get
        {
            if (_instance == null)
            {
                _instance = GameObject.FindObjectOfType<LogHandler>();

                if (_instance == null)
                {
                    GameObject container = new GameObject("LogHandler");
                    _instance = container.AddComponent<LogHandler>();
                }
            }

            return _instance;
        }
    }

    public static void WriteMessage(string message)
    {
        logText.text += message + "\n";
    }

    public static void DisplayInformation(string information, Color color)
    {
        infoText.color = color;
        infoText.text = information;
    }
}
