using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Video;

public class RenderHandler : MonoBehaviour
{
    public VideoScrub slider;
    public RenderTexture VideoRenderTexture;

    bool rendering = false;
    private RawImage inputImage;
    private RawImage transformedImage;
    private Camera inputCamera;
    private Camera projectorCamera1;
    private Camera projectorCamera2;
    private Camera projectorCamera1preview;
    private Camera projectorCamera2preview;
    private SettingsHandler settingsHandler;

    private WebCamTexture inputFeedTexture;

    private VideoPlayer videoPlayer;
    private bool isSquare = false;

    private float cam1y;
    private float cam2y;

    int pixelsToCut;
    int inputResY;
    Color[] color;

    // Start is called before the first frame update
    void Start()
    {
        
        inputImage = GameObject.Find("inputImage").GetComponent<RawImage>();
        transformedImage = GameObject.Find("transformedImage").GetComponent<RawImage>();
        inputCamera = GameObject.Find("inputCamera").GetComponent<Camera>();
        projectorCamera1 = GameObject.Find("projector1").GetComponent<Camera>();
        projectorCamera2 = GameObject.Find("projector2").GetComponent<Camera>();
        projectorCamera1preview = GameObject.Find("projectorpreview1").GetComponent<Camera>();
        projectorCamera2preview = GameObject.Find("projectorpreview2").GetComponent<Camera>();
        // texture = new Texture2D(PlayerPrefs.GetInt("inputResY"), PlayerPrefs.GetInt("inputResY"));
        pixelsToCut = (int)(PlayerPrefs.GetInt("inputResX") - PlayerPrefs.GetInt("inputResY")) / 2;
        inputResY = PlayerPrefs.GetInt("inputResY");

        float camOffset = PlayerPrefs.GetFloat("OverlappingOffset");
        AdjustOverlapping(camOffset);
    }

    public void EnableRendering()
    {
        Debug.Log("rendering enabled");
        rendering = true;
    }

    public void DisableRendering()
    {
        rendering = false;
        Debug.Log("rendering disabled");
    }

    public void SetupWebcamTexture()
    {
        var inputResX = PlayerPrefs.GetInt("inputResX");
        var inputResY = PlayerPrefs.GetInt("inputResY");
        //setting aspect ratio
        inputCamera.aspect = Convert.ToSingle(inputResX) / Convert.ToSingle(inputResY);
        var inputImagePosition = new Vector3(4000, 0, -1000);
        var inputImageSize = new Vector2(inputResX, inputResY);
        //setting image position
        inputImage.rectTransform.position = inputImagePosition;
        //setting image size
        inputImage.rectTransform.sizeDelta = inputImageSize;
        //setting camera position
        var cameraPosition = inputImagePosition;
        cameraPosition.z -= 1000;
        inputCamera.transform.position = cameraPosition;
        //calculate input camera distance
        var camPosz = inputCamera.transform.position.z;
        var imgPosz = inputImage.transform.position.z;
        //calculate and set view angle of camera
        var b = Math.Abs(camPosz - imgPosz);
        var a = inputResY / 2;
        var c = Pythagoras(a, b);
        var angle = Convert.ToSingle(CalculateAngle(a, c) * 2);
        inputCamera.fieldOfView = angle;
        LogHandler.WriteMessage("SUCCESS: Input feed texture initialized.");
    }

    public void SetupTransformedTexture()
    {
        //setting position and size of projection image 
        var resX = PlayerPrefs.GetInt("resX");
        var resY = PlayerPrefs.GetInt("resY");
        var transformedImagePosition = new Vector3(4000, 4000, 0);
        var transformedImageSize = new Vector2(resX, resX);
        transformedImage.rectTransform.position = transformedImagePosition;
        transformedImage.rectTransform.sizeDelta = transformedImageSize;

        //setting shader
        transformedImage.material.SetFloat("_InputX", PlayerPrefs.GetInt("inputResX"));
        transformedImage.material.SetFloat("_InputY", PlayerPrefs.GetInt("inputResY"));
        SetIsSquare();

        //set aspect for projection cameras
        var aspectX = Convert.ToSingle(PlayerPrefs.GetInt("aspectX"));
        var aspectY = Convert.ToSingle(PlayerPrefs.GetInt("aspectY"));
        var aspect = aspectX / aspectY;
        projectorCamera1.aspect = aspect;
        projectorCamera2.aspect = aspect;
        projectorCamera1preview.aspect = aspect;
        projectorCamera2preview.aspect = aspect;

        //setting position of projection cameras
        var camDistance = 1000;
        var camerasX = transformedImage.rectTransform.position.x;
        var camerasZ = transformedImage.rectTransform.position.z - camDistance;
        cam1y = transformedImage.rectTransform.position.y + transformedImage.rectTransform.sizeDelta.y / 2 - resY / 2;
        cam2y = transformedImage.rectTransform.position.y - transformedImage.rectTransform.sizeDelta.y / 2 + resY / 2;
        var cam1position = new Vector3(camerasX, cam1y, camerasZ);
        var cam2position = new Vector3(camerasX, cam2y, camerasZ);
        projectorCamera1.transform.position = cam1position;
        projectorCamera2.transform.position = cam2position;
        projectorCamera1preview.transform.position = cam1position;
        projectorCamera2preview.transform.position = cam2position;

        //calculate and set required field of view for camera
        var b = camDistance;
        var a = resY / 2;
        var c = Pythagoras(a, b);
        var angle = Convert.ToSingle(CalculateAngle(a, c) * 2);
        projectorCamera1.fieldOfView = angle;
        projectorCamera2.fieldOfView = angle;
        projectorCamera1preview.fieldOfView = angle;
        projectorCamera2preview.fieldOfView = angle;
        LogHandler.WriteMessage("SUCCESS: Transformed texture initialized.");
    }

    public void ActivateDisplays()
    {
        LogHandler.WriteMessage("Recognized Displays:");
        var displays = Display.displays;
        var resX = PlayerPrefs.GetInt("resX");
        var resY = PlayerPrefs.GetInt("resY");
        displays[1].Activate(resX, resY, 60);
        displays[2].Activate(resX, resY, 60);
        //Activate Display 2 and 3 for projector 1 and 2
        //for (int i = 0; i< displays.Length; i++)
        //{
        //    log.LogWrite(displays[i].ToString() + ", Status: " + displays[i].active);
        //    if(i == 1 || i == 2)
        //    {
        //        displays[i].Activate(resX, resY, 60);
        //    }
        //}
    }

    public void StartManyCam()
    {
        //log.LogWrite("Starting ManyCam...");
        //manyCamProc = new Process();
        //var path = @"C:\Program Files (x86)\ManyCam\ManyCam.exe";
        //manyCamProc.StartInfo.UseShellExecute = false;
        //manyCamProc.StartInfo.FileName = path;
        //manyCamProc.StartInfo.CreateNoWindow = false;
        //manyCamProc.Start();
        //manyCamProc.WaitForInputIdle();
        //log.LogWrite("ManyCam successfully started. Please configure it.");
    }

    public void RenderCo2(RenderTexture renderTexture)
    {
        transformedImage.texture = renderTexture;
        // assign the preview
        inputImage.texture = renderTexture;
    }

    public void RenderVideo(string path)
    {
        if (videoPlayer == null)
        {
            videoPlayer = (VideoPlayer)transformedImage.gameObject.AddComponent(typeof(VideoPlayer));
        }

        videoPlayer.url = path;
        videoPlayer.targetTexture = VideoRenderTexture;
        transformedImage.texture = VideoRenderTexture;
        // assign the preview
        inputImage.texture = VideoRenderTexture;

        slider.VideoPlayer = videoPlayer;
        videoPlayer.Pause();
    }

    public bool PlayPauseVideo()
    {
        if (videoPlayer.isPaused)
        {
            videoPlayer.Play();
            return true;
        } 

        videoPlayer.Pause();
        return false;
    }

    public void Backward()
    {
        videoPlayer.frame -= 50;
    }

    public void Forward()
    {
        videoPlayer.frame += 50;
    }

    public void Loop()
    {
        videoPlayer.isLooping = !videoPlayer.isLooping;
    }

    public void ToggleIsSquare()
    {
        isSquare = !isSquare;
        SetIsSquare();
    }

    public void SetIsSquare()
    {
        transformedImage.material.SetInt("_IsSquare", isSquare ? 1 : 0);
    }

    public void AdjustOverlapping(float value)
    {
        var cam1position = new Vector3(projectorCamera1.transform.position.x, cam1y, projectorCamera1.transform.position.z);
        var cam2position = new Vector3(projectorCamera2.transform.position.x, cam2y, projectorCamera2.transform.position.z);

        float camOffset = value;

        cam1position.y += camOffset;
        cam2position.y -= camOffset;

        projectorCamera1.transform.position = cam1position;
        projectorCamera2.transform.position = cam2position;
        projectorCamera1preview.transform.position = cam1position;
        projectorCamera2preview.transform.position = cam2position;

        PlayerPrefs.SetFloat("OverlappingOffset", camOffset);
    }

    public void RenderManyCam()
    {
        WebCamDevice[] devices = WebCamTexture.devices;
        int virtualCamId = -1;
        //search for virtualcam
        for (int i = 0; i < devices.Length; i++)
        {
            if (devices[i].name.ToLower().Contains("obs-"))
            {
                virtualCamId = i;
            }
        }
        if (virtualCamId == -1)
        {
            throw new Exception("Virtualcam not found! Please install OBS-Studio and its virtualcam-addon");
        }
        LogHandler.WriteMessage("Virtualcam found!");
        //getting cam feed of virtualcam
        inputFeedTexture = new WebCamTexture(devices[virtualCamId].name, PlayerPrefs.GetInt("inputResX"), PlayerPrefs.GetInt("inputResY"));
        inputFeedTexture.Play();


        //inputImage.texture = tex;
        //transformedImage.texture = tex;

        // Assign the webCam texture
        transformedImage.texture = inputFeedTexture;
        // assign the preview
        inputImage.texture = inputFeedTexture;
        rendering = true;

        //Debug.Log(inputFeedTexture.GetPixels().Length);
    }

    private double Pythagoras(double a, double b)
    {
        return Math.Sqrt(Math.Pow(a, 2) + Math.Pow(b, 2));
    }

    private double CalculateAngle(int a, double c)
    {
        var angle = Math.Asin(a / c) * (180 / Math.PI);
        return angle;
    }


}
