using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Rotator : MonoBehaviour
{
    public float rotateSpeed = 10.0f;
    float speed;
    // Start is called before the first frame update
    void Start()
    {
        speed = rotateSpeed;
    }

    // Update is called once per frame
    void Update()
    {
        //Debug.Log("update");
        transform.Rotate(new Vector3(0,0,1), speed * Time.deltaTime, Space.World);
    }

    public void OnSliderValueChanged(float value)
    {
        //Debug.Log(value);
        speed = value;
    }
}
