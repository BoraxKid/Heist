using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

[RequireComponent(typeof(Image))]
public class ChangeImageColor : MonoBehaviour
{
    [SerializeField] private Color _normalColor;
    [SerializeField] private Color _alternateColor;

    private Image _image;

    private void Awake()
    {
        this._image = this.GetComponent<Image>();
        this.SetNormalColor();
    }

    public void SetNormalColor()
    {
        this._image.color = this._normalColor;
    }

    public void SetAlternateColor()
    {
        this._image.color = this._alternateColor;
    }
}
