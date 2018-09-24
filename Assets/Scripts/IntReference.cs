using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class IntReference
{
    public bool _useConstant = true;
    public int _constantValue;
    public IntVariable _variable;

    public int Value
    {
        get
        {
            return (this._useConstant ? this._constantValue : this._variable.Value);
        }
    }
}
