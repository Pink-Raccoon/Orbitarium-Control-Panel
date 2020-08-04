using OpenQA.Selenium.Internal;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.XR.WSA;

public enum AdjustAction
{
    UP,
    DOWN,
    RIGHT,
    LEFT,
    MIRROR
}

public class ProjectionManager : MonoBehaviour
{
    private LogManager log;
    private Process manyCamProc;
    private RawImage inputImage;
    private RawImage transformedImage;
    private Camera inputCamera;
    private Camera projectorCamera1;
    private Camera projectorCamera2;
    private Camera projectorCamera1preview;
    private Camera projectorCamera2preview;

    void Start() {
    }

    public ProjectionManager(LogManager log)
    {
        this.log = log;
        inputImage = GameObject.Find("inputImage").GetComponent<RawImage>();
        transformedImage = GameObject.Find("transformedImage").GetComponent<RawImage>();
        inputCamera = GameObject.Find("inputCamera").GetComponent<Camera>();
        projectorCamera1 = GameObject.Find("projector1").GetComponent<Camera>();
        projectorCamera2 = GameObject.Find("projector2").GetComponent<Camera>();
        projectorCamera1preview = GameObject.Find("projectorpreview1").GetComponent<Camera>();
        projectorCamera2preview = GameObject.Find("projectorpreview2").GetComponent<Camera>();

    }

    public void SetupInputView()
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
    }

    public void SetupTransformedView()
    {
        //setting position and size of projection image 
        var resX = PlayerPrefs.GetInt("resX");
        var resY = PlayerPrefs.GetInt("resY");
        var transformedImagePosition = new Vector3(4000, 4000, 0);
        var transformedImageSize = new Vector2(resX, resX);
        transformedImage.rectTransform.position = transformedImagePosition;
        transformedImage.rectTransform.sizeDelta = transformedImageSize;

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

    public void AdjustProjection(AdjustAction action)
    {
        var projectorString = GameObject.Find("forProjector").GetComponent<InputField>().text;
        Camera projector;
        Camera projectorPreview;
        if (projectorString.Equals("1"))
        {
            projector = projectorCamera1;
            projectorPreview = projectorCamera1preview;
        } 
        else
        {
            projector = projectorCamera2;
            projectorPreview = projectorCamera2preview;
        }

        if(action != AdjustAction.MIRROR)
        {
            var pixels = Int32.Parse(GameObject.Find("byPixels").GetComponent<InputField>().text);
            if(action == AdjustAction.UP)
            {
                MoveProjectorUp(projector, pixels);
                MoveProjectorUp(projectorPreview, pixels);
            } else if (action == AdjustAction.DOWN)
            {
                MoveProjectorDown(projector, pixels);
                MoveProjectorDown(projectorPreview, pixels);
            }
            else if (action == AdjustAction.RIGHT)
            {
                MoveProjectorRight(projector, pixels);
                MoveProjectorRight(projectorPreview, pixels);
            }
            else if (action == AdjustAction.LEFT)
            {
                MoveProjectorLeft(projector, pixels);
                MoveProjectorLeft(projectorPreview, pixels);
            }
        } else
        {
            MirrorProjector(projector);
            MirrorProjector(projectorPreview);
        }

    }

    private void MoveProjectorUp(Camera cam, int pixels)
    {
        log.LogWrite("moving up by " + pixels);
        var position = cam.transform.position;
        var newPosition = new Vector3(position.x, position.y + pixels, position.z);
        cam.transform.position = newPosition;
    }

    private void MoveProjectorDown(Camera cam, int pixels)
    {
        log.LogWrite("moving down by " + pixels);
        var position = cam.transform.position;
        var newPosition = new Vector3(position.x, position.y - pixels, position.z);
        cam.transform.position = newPosition;
    }

    private void MoveProjectorRight(Camera cam, int pixels)
    {
        log.LogWrite("moving right by " + pixels);
        var position = cam.transform.position;
        var newPosition = new Vector3(position.x + pixels, position.y, position.z);
        cam.transform.position = newPosition;
    }

    private void MoveProjectorLeft(Camera cam, int pixels)
    {
        log.LogWrite("moving left by " + pixels);
        var position = cam.transform.position;
        var newPosition = new Vector3(position.x - pixels, position.y, position.z);
        cam.transform.position = newPosition;
    }

    private void MirrorProjector(Camera cam)
    {
        log.LogWrite("rotate by 180");
        var rotation = cam.transform.rotation;
        float newZ;
        if (rotation.z == 180.0f)
        {
            newZ = 0.0f;
        }
        else
        {
            newZ = 180.0f;
        }
        var newRotation = new Quaternion(rotation.x, rotation.y, newZ, rotation.w);
        cam.transform.rotation = newRotation;
    }

    public void SetupDisplays()
    {
        log.LogWrite("Recognized Displays:");
        var displays = Display.displays;
        var resX = PlayerPrefs.GetInt("resX");
        var resY = PlayerPrefs.GetInt("resY");
        displays[1].Activate(resX, resY,60);
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
        var displays = Display.displays;
        foreach (var disp in displays)
        {
            log.LogWrite(disp.ToString());
        }
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
        log.LogWrite("ManyCam found!");
        //getting cam feed of manycam
        WebCamTexture tex = new WebCamTexture(devices[manyCamId].name);
        
        inputImage.texture = tex;
        transformedImage.texture = tex;
        
        tex.Play();
        UnityEngine.Debug.Log(tex.GetPixels().Length);
    }

    public void Update()
    {
        //UnityEngine.Debug.Log("update!");
    }

    
}
