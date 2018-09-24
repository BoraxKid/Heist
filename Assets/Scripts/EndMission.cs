using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EndMission : MonoBehaviour
{
    [SerializeField] private GameEvent _pauseEvent;
    [SerializeField] private AnimationCurve _pauseSlowdownCurve = AnimationCurve.EaseInOut(0.0f, 1.0f, 0.2f, 0.0f);

    public void End()
    {
        this._pauseEvent.Raise();
    }
}
