using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Cinemachine;

public class ScreenShake : MonoBehaviour
{
    [SerializeField] private CinemachineVirtualCamera _mainCamera;
    [SerializeField] private ScreenShakeProperties _defaultProperties;

    private CinemachineBasicMultiChannelPerlin _noiseModule;

    private void Awake()
    {
        this._noiseModule = this._mainCamera.GetCinemachineComponent<CinemachineBasicMultiChannelPerlin>();
        if (this._noiseModule == null)
            this._noiseModule = this._mainCamera.AddCinemachineComponent<CinemachineBasicMultiChannelPerlin>();
        this.ResetNoise();
    }

    private void ResetNoise()
    {
        this._noiseModule.m_AmplitudeGain = 0;
        this._noiseModule.m_FrequencyGain = 0;
    }

    public void Shake()
    {
        this.StartCoroutine(this.Animate(this._defaultProperties));
    }

    public void Shake(ScreenShakeProperties properties)
    {
        this._noiseModule.m_NoiseProfile = properties.noiseProfile;
        this._noiseModule.m_AmplitudeGain = properties.amplitudeGain;
        this._noiseModule.m_FrequencyGain = properties.frequencyGain;
        //this.StartCoroutine(this.Animate(properties));
        this.Invoke("ResetNoise", properties.time);
    }

    private IEnumerator Animate(ScreenShakeProperties properties)
    {
        Debug.Log("Screen shaking with amplitude gain: " + properties.amplitudeGain + " & frequency gain: " + properties.frequencyGain + " for " + properties.time + " seconds.");
        float elapsedTime = 0.0f;

        //this._mainCamera.GetCinemachineComponent<CinemachineBasicMultiChannelPerlin>().m_NoiseProfile = properties.noiseProfile;
        this._noiseModule.m_AmplitudeGain = properties.amplitudeGain;
        this._noiseModule.m_FrequencyGain = properties.frequencyGain;

        while (elapsedTime <= properties.time)
        {
            if (GameConstants.paused)
            {
                yield return null;
                continue;
            }
            elapsedTime += Time.deltaTime;
        }
        this.ResetNoise();
        yield break;
    }
}
