using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class BrightnessMeter : MonoBehaviour
{
    [SerializeField] private List<Image> _images;

    private void LateUpdate()
    {
        if (GameConstants.paused)
            return;

        float brightness = GameConstants.playerVariable.brightness;
        foreach (Image image in this._images)
            image.fillAmount = brightness;
    }
}
