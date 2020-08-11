using Newtonsoft.Json;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Net.Sockets;
using UnityEngine;
using UnityEngine.UI;

public class AnimationsHandler : MonoBehaviour
{
    public static Dictionary<string, AnimationInformation> AnimationSummary { get; set; }
    public static AnimationInformation CurrentAnimation { get; set; }
    public static Dropdown animationsDropdown = GameObject.Find("animationsDropdown").GetComponent<Dropdown>();
    public static Text animationDescription = GameObject.Find("animationDescription").GetComponent<Text>();
    public static Button loadAnimationButton = GameObject.Find("loadAnimationButton").GetComponent<Button>();

    public static AnimationsHandler _instance;
    public static AnimationsHandler Instance
    {
        get
        {
            if (_instance == null)
            {
                _instance = GameObject.FindObjectOfType<AnimationsHandler>();

                if (_instance == null)
                {
                    GameObject container = new GameObject("AnimationsHandler");
                    _instance = container.AddComponent<AnimationsHandler>();
                }
            }

            return _instance;
        }
    }
    
    public static async void GetAnimationsSummary()
    {
        string summaryString;
        LogHandler.WriteMessage("Getting available Animations from Animation Data application");
        try
        {
            summaryString = await ApiHandler.GetAnimationsSummary();
            summaryString = JsonConvert.DeserializeObject<string>(summaryString);
        }
        catch (Exception e)
        {
            LogHandler.WriteMessage("FAIL: Error when retrieving animations. Message was:\n" + e.Message);
            LogHandler.DisplayInformation("Error when retrieving animations. See messages", Color.red);
            return;
        }
        LogHandler.WriteMessage("SUCCESS: Got answer about animations");
        LogHandler.WriteMessage("Analyzing available animations");
        
        if (summaryString.Equals("noanimations"))
        {
            LogHandler.WriteMessage("FAIL: There are no animations. Please generate them in the Animation Data Application.");
            LogHandler.DisplayInformation("No animtions found. See messages", Color.red);
            return;
        } else
        {
            var summary = JsonConvert.DeserializeObject<Dictionary<string, AnimationInformation>>(summaryString);
            Debug.Log(summary);
            AnimationSummary = summary;
        }
        LogHandler.WriteMessage("SUCCESS: Found " + AnimationSummary.Count + " animations.");
        LogHandler.DisplayInformation("Animations found. Please select one.", Color.green);
        List<string> animationDropdownList = new List<string>();
        foreach (var entry in AnimationSummary)
        {
            animationDropdownList.Add(entry.Key);
        }
        var firstElement = animationDropdownList[0];
        var description = AnimationSummary[firstElement].AnimationDescription;
        animationDescription.text = description;
        animationsDropdown.AddOptions(animationDropdownList);
        animationsDropdown.onValueChanged.AddListener(delegate {
            DropdownValueChanged(animationsDropdown);
        });
        animationsDropdown.interactable = true;
        loadAnimationButton.interactable = true;
    }

    static void DropdownValueChanged(Dropdown change)
    {
        var element = change.value;
        animationDescription.text = AnimationSummary[animationsDropdown.options[element].text].AnimationDescription;
    }

    public static async void StopAnimation() {
        string message = await ApiHandler.StopAnimation();
        if (message.Equals("stopped")) {
            LogHandler.WriteMessage("Animation stopped");
        }        
    }

    public static async void ContinueAnimation()
    {
        string message = await ApiHandler.ContinueAnimation();
        if (message.Equals("continued"))
        {
            LogHandler.WriteMessage("Animation continued");
        }
    }

    public static bool CheckAnimationDataAvailability()
    {
        LogHandler.WriteMessage("Checking availability of Animation Data Application");
        try
        {
            ApiHandler.CheckAnimationDataAvailability();
        } catch (Exception e)
        {
            LogHandler.WriteMessage("FAIL: Animation Data Application not available. Check settings and ensure the application is running");
            LogHandler.DisplayInformation("FAIL: Animation Data Application not available.\nSee messages.", Color.red);
            return false;
        }
        LogHandler.WriteMessage("SUCCESS: Animation Data Application online");
        return true;
    }



    
}
