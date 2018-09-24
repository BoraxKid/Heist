using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "Properties/Collider", fileName = "New collider properties")]
public class ColliderProperties : ScriptableObject
{
    public float Height;
    public Vector3 Center;
}
