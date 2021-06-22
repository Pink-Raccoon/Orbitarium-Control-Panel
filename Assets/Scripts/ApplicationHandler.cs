using System.Linq;
using UnityEngine;
using UnityEngine.UI;

public class ApplicationHandler : MonoBehaviour
{
    public InputField WebsiteUrlText;

    public Text SelectedVideo;

    public Image PlayPauseButtonImage;
    public Sprite PlayButton;
    public Sprite PauseButton;
    private bool isPlaying;

    public Slider OverlappingSlider;
    public Slider Co2Slider;
    public Text Co2AmountText;
    public Earth Earth;

    private RenderHandler renderHandler;
    private string videoPath;
    private bool resetPressed;
    private double[] k = { 0, 0.0118, 0.110, 0.250, 0.359, 0.465, 0.596, 0.711, 0.841, 0.940, 1 };
    private double ppm = 0;

    //called before start
    void Awake()
    {
        LogHandler.WriteMessage("Application started");
        LogHandler.WriteMessage("Begin initialization");
        var settingsHandler = GameObject.Find("ApplicationHandlerObject").GetComponent<SettingsHandler>();
        settingsHandler.LoadSettings();
        renderHandler = GameObject.Find("RenderingObject").GetComponent<RenderHandler>();
        //renderHandler.SetActive(false);
        //renderHandler.SetActive(true);
    }

    // Start is called before the first frame update
    void Start()
    {
        InitializeApplication();

        float camOffset = PlayerPrefs.GetFloat("OverlappingOffset");
        OverlappingSlider.value = camOffset;

        // Setup Website Url for Windy
        WebsiteUrlText.text = "https://www.windy.com/?21.453,23.159,3";
    }

    private void InitializeApplication()
    {
        var settingsHandler = GameObject.Find("ApplicationHandlerObject").GetComponent<SettingsHandler>();
        settingsHandler.ApplySettingsToInternalObjects();
        if (AnimationsHandler.CheckAnimationDataAvailability())
        {
            AnimationsHandler.GetAnimationsSummary();
        }
        
    }

    public void Load()
    {
        var animationsDropdown = GameObject.Find("animationsDropdown").GetComponent<Dropdown>();
        var selectedAnimation = animationsDropdown.value;
        var animationInformation = AnimationsHandler.AnimationSummary[animationsDropdown.options[selectedAnimation].text];
        AnimationsHandler.CurrentAnimation = animationInformation;
        var initUri = animationInformation.InitUri;
        var runCommand = animationInformation.RunCommand;
        AnimationsHandler.StopAnimation();
        BrowserHandler.StartDisplayDriver();
        BrowserHandler.StartContentBrowser(initUri);
        LogHandler.WriteMessage("Please start ManyCam and choose the Desktop displaying the Animation.");
    }
    public void OpenWebsite()
    {
        string initUri = WebsiteUrlText.text;

        AnimationsHandler.StopAnimation();
        BrowserHandler.StartDisplayDriver();
        BrowserHandler.StartContentBrowser(initUri);
        LogHandler.WriteMessage("Please start ManyCam and choose the Desktop displaying the Animation.");
    }

    public void SelectVideo()
    {
        SimpleFileBrowser.FileBrowser.ShowLoadDialog((string[] paths) =>
        {
            videoPath = paths[0];
            SelectedVideo.text = videoPath;
        }, null, SimpleFileBrowser.FileBrowser.PickMode.Files, false, null, null, "Select Video");
    }

    public void StartVideo()
    {
        if (videoPath.Length != 0)
        {
            renderHandler.RenderVideo(videoPath);
        }
    }

    public void PlayPauseVideo()
    {
        isPlaying = renderHandler.PlayPauseVideo();

        if (isPlaying)
        {
            PlayPauseButtonImage.sprite = PauseButton;
        } 
        else
        {
            PlayPauseButtonImage.sprite = PlayButton;
        }

    }

    public void Backward()
    {
        renderHandler.Backward();
    }

    public void Forward()
    {
        renderHandler.Forward();
    }

    public void Loop()
    {
        renderHandler.Loop();
    }

    public void ToggleIsSquare()
    {
        renderHandler.ToggleIsSquare();
    }

    public void StartAnimation()
    {
        var runCommand = AnimationsHandler.CurrentAnimation.RunCommand;
        BrowserHandler.ExecuteScriptContentBrowser(runCommand);
    }

    public void StopAnimation()
    {
        AnimationsHandler.StopAnimation();
    }

    public void ContinueAnimation()
    {
        AnimationsHandler.ContinueAnimation();
    }

    public void OnOverlappingChanged()
    {
        float value = OverlappingSlider.value;
        renderHandler.AdjustOverlapping(value);
    }


    public void OnCo2ValueChanged()
    {
        if (!Input.GetJoystickNames().Contains("SideWinder Joystick"))
        {
            double value = Co2Slider.value;
            Earth.setPpm(value);
            Co2AmountText.text = value.ToString();
        }
    }

    void FixedUpdate()
    {
        if (Input.GetJoystickNames().Contains("SideWinder Joystick"))
        {
            double joystickValue = Input.GetAxis("Adjust Co2");
            joystickValue = (joystickValue + 0.5); // * 1000; //Braucht es nur wenn man die Berechnung unten nicht macht.
            
            //Anpassung für die Skala auf dem Hardware Control Panel
            for (int i = 1; i <= 10; i++)
            {
                if (joystickValue >= k[i - 1] && joystickValue <= k[i])
                {
                    ppm = (i - 1) * 100 + (joystickValue - k[i - 1]) * 100 / (k[i] - k[i - 1]);
                }
            }
            Earth.setPpm(ppm);
            Co2AmountText.text = ppm.ToString();
        }

        if (Input.GetButton("Reset Co2") && !resetPressed)
        {
            //Fire once
            Debug.Log("trigger");
            resetPressed = true;
            Earth.reset();
        }
        else if (Input.GetButton("Reset Co2") && resetPressed)
        {
            //wait after fire reset
        }
        else
        {
            resetPressed = false;
        }

    }

    public void PlayPauseCo2Animation()
    {
        Earth.PlayPause();
        renderHandler.RenderCo2(Earth.RenderTexture);
    }
}
