using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DetectionDisplay : MonoBehaviour
{
    private struct Element
    {
        public RectTransform rectTransform;
        public DetectionElement detectionElement;
    }

    [SerializeField] private DetectionElement _detectionPrefab;
    [SerializeField] private Camera _camera;

    private Dictionary<Sight, Element> _detectionElements = new Dictionary<Sight, Element>();
    private Vector2 _playerViewportPosition;

    private void LateUpdate()
    {
        if (GameConstants.paused)
            return;

        this._playerViewportPosition = this._camera.WorldToViewportPoint(Helper.GetCenter(GameConstants.playerVariable.gameObject));

        foreach (KeyValuePair<Sight, Element> pair in this._detectionElements)
            this.TransformElement(pair.Key, pair.Value);
    }

    private void TransformElement(Sight sight, Element element)
    {
        Vector2 enemyViewportPosition = this._camera.WorldToViewportPoint(Helper.GetCenter(sight.gameObject));
        element.rectTransform.rotation = Quaternion.Euler(0, 0, Mathf.Atan2((enemyViewportPosition.y - this._playerViewportPosition.y), (enemyViewportPosition.x - this._playerViewportPosition.x)) * Mathf.Rad2Deg);
        enemyViewportPosition.x = Mathf.Clamp(enemyViewportPosition.x, -1.0f, 1.0f);
        enemyViewportPosition.y = Mathf.Clamp(enemyViewportPosition.y, -1.0f, 1.0f);
        element.rectTransform.anchorMin = enemyViewportPosition;
        element.rectTransform.anchorMax = enemyViewportPosition;
        element.detectionElement.SetValue(sight.DetectProgress);
    }

    public void OnRegisterEnemy(GameObject gameObject)
    {
        Sight sight = gameObject.GetComponent<Sight>();
        if (sight == null)
        {
            Debug.LogWarning("GameObject #" + gameObject.name + " doesn't have a Sight component.");
            return;
        }
        if (this._detectionElements.ContainsKey(sight))
        {
            Debug.LogWarning("GameObject #" + gameObject.name + " already present.");
            return;
        }

        Element element = new Element();

        element.detectionElement = Instantiate(this._detectionPrefab, this.transform);
        element.rectTransform = element.detectionElement.GetComponent<RectTransform>();

        this._playerViewportPosition = this._camera.WorldToViewportPoint(Helper.GetCenter(GameConstants.playerVariable.gameObject));

        this.TransformElement(sight, element);

        this._detectionElements.Add(sight, element);
        if (this._detectionElements.Count > 0)
            this.enabled = true;
    }

    public void OnUnregisterEnemy(GameObject gameObject)
    {
        Sight sight = gameObject.GetComponent<Sight>();
        if (sight == null)
        {
            Debug.LogWarning("GameObject #" + gameObject.name + " doesn't have a Sight component.");
            return;
        }
        if (this._detectionElements.ContainsKey(sight))
        {
            this._detectionElements[sight].detectionElement.Delete();
            this._detectionElements.Remove(sight);
        }
        if (this._detectionElements.Count == 0)
            this.enabled = false;
    }

    public void OnDetectionComplete(GameObject gameObject)
    {
        Sight sight = gameObject.GetComponent<Sight>();
        if (sight == null)
        {
            Debug.LogWarning("GameObject #" + gameObject.name + " doesn't have a Sight component.");
            return;
        }
        if (this._detectionElements.ContainsKey(sight))
        {
            this._detectionElements[sight].detectionElement.Complete();
            this._detectionElements.Remove(sight);
        }
        if (this._detectionElements.Count == 0)
            this.enabled = false;
    }
}
