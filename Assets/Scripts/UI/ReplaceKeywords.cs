using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

[RequireComponent(typeof(TextMeshProUGUI))]
public class ReplaceKeywords : MonoBehaviour
{
    [SerializeField] private InteractVariable _variables;

    private TextMeshProUGUI _textMesh;
    private string _originalText;

    private void Awake()
    {
        this._textMesh = this.GetComponent<TextMeshProUGUI>();
        this._originalText = this._textMesh.text;
    }

    public void ReplaceName(string name)
    {
        name = "<b>" + name + "</b>";
        if (this._variables.customText != string.Empty)
            this.Replace(this._variables.customText, GameConstants.KEYWORD_NAME, name);
        else
            this.Replace(this._originalText, GameConstants.KEYWORD_NAME, name);
    }

    public void ChangeName()
    {
        this.ReplaceName(this._variables.interactable.name);
    }

    private void Replace(string originalText, string keyword, string replacement)
    {
        this._textMesh.SetText(originalText.Replace(keyword, replacement));
    }
}
