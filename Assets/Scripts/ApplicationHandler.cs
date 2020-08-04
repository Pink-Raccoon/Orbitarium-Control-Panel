using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ApplicationHandler : MonoBehaviour
{
    private GameObject renderHandler;

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
        Debug.Log("started application!");
        LogHandler.Log.logText.text = "lol";   //.WriteMessage("started application!");
        //renderHandler = GameObject.Find("RenderingObject");
        //renderHandler.SetActive(false);
        var settingsHandler = GameObject.Find("ApplicationHandlerObject").GetComponent<SettingsHandler>();
        settingsHandler.LoadSettings();
        //renderHandler.SetActive(true);
    }
}
