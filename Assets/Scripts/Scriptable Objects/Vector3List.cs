using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "List/Vector3", fileName = "New Vector3 list")]
public class Vector3List : ScriptableObject
{
    public List<Vector3> Value;
}
