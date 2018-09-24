using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "Properties/Loading", fileName = "New loading properties")]
public class LoadingProperties : ScriptableObject
{
    public int loadingScreenSceneIndex;
    public int sceneIndex;

    public void SetSceneIndex(int index)
    {
        this.sceneIndex = index;
    }
}
