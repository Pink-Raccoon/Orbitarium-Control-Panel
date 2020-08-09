using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Rotator : MonoBehaviour
{
    public float rotateSpeed = 0.0f;
    public Slider mainSlider;
    float speed;
    // Start is called before the first frame update
    void Start()
    {
        speed = rotateSpeed;
        mainSlider.onValueChanged.AddListener(delegate { OnSliderValueChanged(); });
    }

    // Update is called once per frame
    void Update()
    {
        //Debug.Log("update");
        transform.Rotate(new Vector3(0,0,1), speed * Time.deltaTime, Space.World);
    }

    public void OnSliderValueChanged()
    {
        //Debug.Log(value);
        speed = mainSlider.value;
    }
}
