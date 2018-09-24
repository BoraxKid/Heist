using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class PauseInput : MonoBehaviour
{
    [SerializeField] private UnityEvent _onPause;
    [SerializeField] private UnityEvent _onResume;

    private void Update()
    {
        if (!GameConstants.gameOver && Input.GetButtonDown(GameConstants.INPUT_PAUSE))
        {
            if (GameConstants.paused)
                this._onResume.Invoke();
            else
                this._onPause.Invoke();
        }
    }
}
