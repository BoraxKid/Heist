using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class FloatReference
{
    public bool _useConstant = true;
    public float _constantValue;
    public FloatVariable _variable;

    public float Value
    {
        get
        {
            return (this._useConstant ? this._constantValue : this._variable.Value);
        }
    }
}
