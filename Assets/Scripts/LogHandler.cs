using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class LogHandler : MonoBehaviour
{
    public static LogHandler Log;

    public Text logText;
    public Text infoText;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    void Awake()
    {
        if (Log != null)
            GameObject.Destroy(Log);
        else
            Log = this;

        DontDestroyOnLoad(this);

        infoText = GameObject.Find("infoText").GetComponent<Text>();
        logText = GameObject.Find("LogText").GetComponent<Text>();
    }

    public void WriteMessage(string message)
    {
        logText.text += message + "\n";
    }

    public void DisplayInformation(string information, Color color)
    {
        infoText.color = color;
        infoText.text = information;
    }
}
