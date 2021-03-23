using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class RenderHandler : MonoBehaviour
{
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
    private Texture2D texture;
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
        texture = new Texture2D(PlayerPrefs.GetInt("inputResY"), PlayerPrefs.GetInt("inputResY"));
        pixelsToCut = (int)(PlayerPrefs.GetInt("inputResX") - PlayerPrefs.GetInt("inputResY")) / 2;
        inputResY = PlayerPrefs.GetInt("inputResY");

    }

    // Update is called once per frame
    void Update()
    {
        // THIS CAUSES PERFORMANCE ISSUES

        //// Todo: 1.Überflüssige schwarze Balken wegschneiden(PlayerPrefs.GetInt("inputResX", inputResX);)
        //if (rendering)
        //{
        //    color = inputFeedTexture.GetPixels(pixelsToCut, 0, inputResY, inputResY);
        //    texture.SetPixels(color);
        //    texture.Apply();
        //    //inputImage.texture = texture;
        //    transformedImage.texture = texture;
        //}
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
        var inputImageSize = new Vector2(inputResY, inputResY);
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
        var cam1y = transformedImage.rectTransform.position.y + transformedImage.rectTransform.sizeDelta.y / 2 - resY / 2;
        var cam2y = transformedImage.rectTransform.position.y - transformedImage.rectTransform.sizeDelta.y / 2 + resY / 2;
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

    public void RenderManyCam()
    {
        WebCamDevice[] devices = WebCamTexture.devices;
        int manyCamId = -1;
        //search for manycam
        for (int i = 0; i < devices.Length; i++)
        {
            if (devices[i].name.ToLower().Contains("manycam"))
            {
                manyCamId = i;
            }
        }
        if (manyCamId == -1)
        {
            throw new Exception("ManyCam not found! Please install ManyCam");
        }
        LogHandler.WriteMessage("ManyCam found!");
        //getting cam feed of manycam
        inputFeedTexture = new WebCamTexture(devices[manyCamId].name, PlayerPrefs.GetInt("inputResX"), PlayerPrefs.GetInt("inputResY"));
        inputFeedTexture.Play();


        //inputImage.texture = tex;
        //transformedImage.texture = tex;

        // PREFERRED WAY OF DOING WBCAM
        transformedImage.texture = inputFeedTexture;
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
