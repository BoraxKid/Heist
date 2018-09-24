using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ShowElement : MonoBehaviour
{
    [SerializeField] private FadeInOut _element;

    private void Awake()
    {
        if (this._element == null)
            this._element = this.GetComponent<FadeInOut>();
        if (this._element == null)
            Debug.LogWarning("_element is not set and no FadeInOut component was found");
    }

    public void Show()
    {
        this._element.Show();
    }

    public void Hide()
    {
        this._element.Hide();
    }
}
