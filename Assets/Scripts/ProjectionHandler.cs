using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ProjectionHandler : MonoBehaviour
{
    private Camera projectorCamera1;
    private Camera projectorCamera2;
    private Camera projectorCamera1preview;
    private Camera projectorCamera2preview;

    // Start is called before the first frame update
    void Start()
    {
        projectorCamera1 = GameObject.Find("projector1").GetComponent<Camera>();
        projectorCamera2 = GameObject.Find("projector2").GetComponent<Camera>();
        projectorCamera1preview = GameObject.Find("projectorpreview1").GetComponent<Camera>();
        projectorCamera2preview = GameObject.Find("projectorpreview2").GetComponent<Camera>();
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void AdjustProjection(string action)
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

        if (!action.Equals("mirror"))
        {
            var pixels = Int32.Parse(GameObject.Find("byPixels").GetComponent<InputField>().text);
            if (action.Equals("up"))
            {
                MoveProjectorUp(projector, pixels);
                MoveProjectorUp(projectorPreview, pixels);
            }
            else if (action.Equals("down"))
            {
                MoveProjectorDown(projector, pixels);
                MoveProjectorDown(projectorPreview, pixels);
            }
            else if (action.Equals("right"))
            {
                MoveProjectorRight(projector, pixels);
                MoveProjectorRight(projectorPreview, pixels);
            }
            else if (action.Equals("left"))
            {
                MoveProjectorLeft(projector, pixels);
                MoveProjectorLeft(projectorPreview, pixels);
            }
        }
        else
        {
            MirrorProjector(projector);
            MirrorProjector(projectorPreview);
        }

    }

    private void MoveProjectorUp(Camera cam, int pixels)
    {
        LogHandler.WriteMessage("moving up by " + pixels);
        var position = cam.transform.position;
        var newPosition = new Vector3(position.x, position.y + pixels, position.z);
        cam.transform.position = newPosition;
    }

    private void MoveProjectorDown(Camera cam, int pixels)
    {
        LogHandler.WriteMessage("moving down by " + pixels);
        var position = cam.transform.position;
        var newPosition = new Vector3(position.x, position.y - pixels, position.z);
        cam.transform.position = newPosition;
    }

    private void MoveProjectorRight(Camera cam, int pixels)
    {
        LogHandler.WriteMessage("moving right by " + pixels);
        var position = cam.transform.position;
        var newPosition = new Vector3(position.x + pixels, position.y, position.z);
        cam.transform.position = newPosition;
    }

    private void MoveProjectorLeft(Camera cam, int pixels)
    {
        LogHandler.WriteMessage("moving left by " + pixels);
        var position = cam.transform.position;
        var newPosition = new Vector3(position.x - pixels, position.y, position.z);
        cam.transform.position = newPosition;
    }

    private void MirrorProjector(Camera cam)
    {
        LogHandler.WriteMessage("rotate by 180");
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
}
