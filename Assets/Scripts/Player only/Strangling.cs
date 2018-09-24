using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Strangling : MonoBehaviour
{
    [SerializeField] private FloatReference _distance;
    [SerializeField] private FloatReference _transformEnemyTime;

    private Animator _animator;

    private void Awake()
    {
        this._animator = this.GetComponent<Animator>();
    }

    public void Strangle(GameObject strangled)
    {
        this.StartCoroutine(this.TransformStrangled(strangled.transform));
        strangled.GetComponent<BeingStrangled>().enabled = true;
        this.enabled = true;
    }

    private void OnEnable()
    {
        this._animator.SetTrigger("Strangle");
    }

    private void OnDisable()
    {
        this._animator.ResetTrigger("Strangle");
    }

    private IEnumerator TransformStrangled(Transform strangled)
    {
        float elapsedTime = 0.0f;
        Vector3 originalPosition = strangled.position;
        Quaternion originalRotation = strangled.rotation;
        Vector3 targetPosition = this.transform.position + this.transform.forward * this._distance.Value;
        Quaternion targetRotation = this.transform.rotation;

        while (elapsedTime <= this._transformEnemyTime.Value)
        {
            if (GameConstants.paused)
            {
                yield return null;
                continue;
            }
            strangled.position = Vector3.Lerp(originalPosition, targetPosition, elapsedTime / this._transformEnemyTime.Value);
            strangled.rotation = Quaternion.Lerp(originalRotation, targetRotation, elapsedTime / this._transformEnemyTime.Value);
            yield return null;
            elapsedTime += Time.deltaTime;
        }
        strangled.position = targetPosition;
        strangled.rotation = targetRotation;
        yield break;
    }
}
