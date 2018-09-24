using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

[RequireComponent(typeof(Image))]
public class FillImage : MonoBehaviour
{
    [SerializeField] private InteractVariable _variables;

    private Image _image;

    private void Awake()
    {
        this._image = this.GetComponent<Image>();
        if (this._image.type != Image.Type.Filled)
        {
            Debug.LogWarning("The attached image is not of type Filled");
        }
    }

    public void Fill()
    {
        this._image.fillAmount = this._variables.fillAmount;
    }

    public void Fill(float amount)
    {
        this._image.fillAmount = amount;
    }

    public void FinishFill()
    {
        this._image.fillAmount = 1.0f;
    }

    public void CancelFill()
    {
        this._image.fillAmount = 0.0f;
    }
}
