using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class BeingStrangled : MonoBehaviour
{
    [SerializeField] private FloatReference _stranglingTime;
    [SerializeField] private UnityEvent _startStrangle;
    [SerializeField] private UnityEvent _endStrangle;

    private Animator _animator;

    private void Awake()
    {
        this._animator = this.GetComponent<Animator>();
    }

    private void OnEnable()
    {
        this._startStrangle.Invoke();
        this._animator.SetBool("BeingStrangled", true);
        this.StartCoroutine(this.EndStrangling());
    }

    private void OnDisable()
    {
        this._animator.SetBool("BeingStrangled", false);
    }

    private IEnumerator EndStrangling()
    {
        float elapsedTime = 0.0f;

        while (elapsedTime <= this._stranglingTime.Value)
        {
            if (GameConstants.paused)
            {
                yield return null;
                continue;
            }
            yield return null;
            elapsedTime += Time.deltaTime;
        }
        this.enabled = false;
        this._endStrangle.Invoke();
        yield break;
    }
}
