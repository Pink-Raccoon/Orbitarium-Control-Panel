using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
public class AnimationInformation
{
    public string AnimationKey { get; set; }
    public string AnimationName { get; set; }
    public string AnimationDescription { get; set; }
    public string InitUri { get; set; }
    public string RunCommand { get; set; }
}
