using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class Interactable : MonoBehaviour
{
    private enum InteractState
    {
        STANDY, // Player is in range
        INTERACTING, // Player is pushing the button
        RECOVERY // Interaction is done, wait to avoid spam
    }
    [SerializeField] private InteractVariable _variables;
    [SerializeField] private string _customText;

    [Header("Uses")]
    [SerializeField] private bool _singleUse;

    [Header("Times")]
    [SerializeField] private FloatReference _interactionTime;
    [SerializeField] private FloatReference _recoveryTime;

    [Header("Events")]
    [SerializeField] private UnityEvent _showInteractUI;
    [SerializeField] private UnityEvent _hideInteractUI;
    [SerializeField] private UnityEvent _startInteractUI;
    [SerializeField] private UnityEvent _updateInteractUI;
    [SerializeField] private UnityEvent _endInteractUI;
    [SerializeField] private UnityEvent _onInteraction;

    private InteractState _state = InteractState.STANDY;
    private float _elapsedTime;
    public bool Used
    {
        get;
        private set;
    }

    private void OnEnable()
    {
        this._elapsedTime = 0.0f;
        this._variables.interactable = this.gameObject;
        if (this._customText != string.Empty)
            this._variables.customText = this._customText;
        else
            this._variables.customText = string.Empty;
        this._showInteractUI.Invoke();
    }

    private void OnDisable()
    {
        this.StopInteracting();
        this._variables.interactable = null;
        this._variables.customText = string.Empty;
        this._hideInteractUI.Invoke();
    }

    private void FixedUpdate()
    {
        if (GameConstants.paused || !this._variables.allowed)
            return;

        this.HandleInputs();
    }

    private void Update()
    {
        if (GameConstants.paused || !this._variables.allowed)
            return;

        if (this._state == InteractState.INTERACTING)
        {
            this._elapsedTime += Time.deltaTime;
            this._variables.fillAmount = this._elapsedTime / this._interactionTime.Value;
            this._updateInteractUI.Invoke();

            if (this._elapsedTime >= this._interactionTime.Value)
            {
                this._onInteraction.Invoke();
                this._endInteractUI.Invoke();
                this.StartCoroutine(this.Recover());
            }
        }
    }

    private void HandleInputs()
    {
        if (Input.GetButton(GameConstants.INPUT_INTERACT))
        {
            if (this._state == InteractState.STANDY)
                this.StartInteracting();
        }
        else
        {
            if (this._state == InteractState.INTERACTING)
                this.StopInteracting();
        }
    }

    private void StopInteracting()
    {
        this._elapsedTime = 0.0f;
        this._variables.fillAmount = 0.0f;
        this._state = InteractState.STANDY;
        this._endInteractUI.Invoke();
    }

    private void StartInteracting()
    {
        this._state = InteractState.INTERACTING;
        this._elapsedTime = 0.0f;
        this._variables.fillAmount = 0.0f;
        this._startInteractUI.Invoke();
    }

    private IEnumerator Recover()
    {
        this._state = InteractState.RECOVERY;
        if (this._singleUse)
        {
            this.Used = true;
            this._hideInteractUI.Invoke();
            this.enabled = false;
            yield break;
        }
        float elapsedTime = 0.0f;

        while (elapsedTime <= this._recoveryTime.Value)
        {
            if (GameConstants.paused)
            {
                yield return null;
                continue;
            }
            yield return null;
            elapsedTime += Time.deltaTime;
        }
        this._state = InteractState.STANDY;
    }

    public void SetCustomText(string text)
    {
        this._customText = text;
        if (this.enabled)
        {
            this._variables.customText = this._customText;
            this._showInteractUI.Invoke();
        }
    }
}
