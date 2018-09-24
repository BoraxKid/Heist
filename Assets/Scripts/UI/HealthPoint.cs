using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class HealthPoint : MonoBehaviour
{
    [SerializeField] private Image _filling;
    [SerializeField] private Image _outline;
    [SerializeField] private Color _fillingActive;
    [SerializeField] private Color _fillingInactive;
    [SerializeField] private Color _outlineActive;
    [SerializeField] private Color _outlineInactive;

    public void Activate()
    {
        this._filling.color = this._fillingActive;
        this._outline.color = this._outlineActive;
    }

    public void Deactivate()
    {
        this._filling.color = this._fillingInactive;
        this._outline.color = this._outlineInactive;
    }
}
