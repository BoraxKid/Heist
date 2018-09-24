using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AudioListenerFollowPlayer : MonoBehaviour
{
    private void LateUpdate()
    {
        this.transform.position = Helper.GetTop(GameConstants.playerVariable.gameObject);
    }
}
