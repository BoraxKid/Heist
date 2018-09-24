using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "Variables/Interact", fileName = "New interact variable")]
public class InteractVariable : ScriptableObject
{
    public bool allowed { get; set; }
    public GameObject interactable;
    public float fillAmount;
    public string customText;
}
