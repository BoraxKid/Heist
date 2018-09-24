using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Cinemachine;

[CreateAssetMenu(menuName = "Properties/Screen Shake", fileName = "New screen shake properties")]
public class ScreenShakeProperties : ScriptableObject
{
    public float time;
    public NoiseSettings noiseProfile;
    public float amplitudeGain;
    public float frequencyGain;
}
