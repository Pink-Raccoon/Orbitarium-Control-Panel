using UnityEditor;
using UnityEngine;
using UnityEngine.UI;

public class ApplicationHandler : MonoBehaviour
{
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
        double value = Co2Slider.value;
        Earth.setPpm(value);
        Co2AmountText.text = value.ToString();
    }

    public void PlayPauseCo2Animation()
    {
        Earth.PlayPause();
        renderHandler.RenderCo2(Earth.RenderTexture);
    }
}
