using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Events;

public class DetectionElement : MonoBehaviour
{
    [SerializeField] private List<Image> _images = new List<Image>();
    [SerializeField] private float _factor = 1.0f;
    [SerializeField] private FloatReference _disableTime;
    [SerializeField] private UnityEvent _onComplete;

    private CanvasGroup _canvasGroup;

    private void Awake()
    {
        this._canvasGroup = this.GetComponent<CanvasGroup>();
    }

    public void Complete()
    {
        this._onComplete.Invoke();
    }

    public void Delete()
    {
        if (this._canvasGroup != null)
            this.StartCoroutine(this.Disable());
    }

    public void SetValue(float value)
    {
        foreach (Image image in this._images)
            image.fillAmount = value * this._factor;
    }

    private IEnumerator Disable()
    {
        float elapsedTime = 0.0f;
        float orignalAlpha = this._canvasGroup.alpha;
        while (elapsedTime <= this._disableTime.Value)
        {
            if (GameConstants.paused)
            {
                yield return null;
                continue;
            }
            this._canvasGroup.alpha = orignalAlpha - elapsedTime / this._disableTime.Value;
            yield return null;
            elapsedTime += Time.deltaTime;
        }
        this._canvasGroup.alpha = 0.0f;
        this.gameObject.SetActive(false);
        Destroy(this.gameObject);
        yield break;
    }
}
