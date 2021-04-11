using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Video;

public class VideoScrub : MonoBehaviour
{
    public Slider slider;
    [HideInInspector]
    public VideoPlayer VideoPlayer; // will be set in RenderHandler.RenderVideo()

    public void OnSliderValueChanged()
    {
        if (VideoPlayer != null && VideoPlayer.frameCount > 0 && VideoPlayer.isPaused)
        {
            float value = slider.value;
            if (VideoPlayer != null && VideoPlayer.frameCount > 0)
            {
                VideoPlayer.frame = (long)(VideoPlayer.frameCount * value);
            }
        }
    }

    private void Update()
    {
        if (VideoPlayer != null && VideoPlayer.frameCount > 0 && !VideoPlayer.isPaused)
        {
            slider.value = VideoPlayer.frame / (float)VideoPlayer.frameCount;
        }
    }
}
