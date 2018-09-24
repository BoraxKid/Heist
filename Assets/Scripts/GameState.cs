using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class GameState : MonoBehaviour
{
    [Header("Events")]
    [SerializeField] private UnityEvent _startLevelEvent;

    private void Awake()
    {
        GameConstants.paused = false;
        GameConstants.gameOver = false;
    }

    private void Start()
    {
        this._startLevelEvent.Invoke();
    }

    public void OnPause()
    {
        GameConstants.paused = true;
    }

    public void OnResume()
    {
        GameConstants.paused = false;
    }

    public void OnGameOver()
    {
        GameConstants.gameOver = true;
    }
}
