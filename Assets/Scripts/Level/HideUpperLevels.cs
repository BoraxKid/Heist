using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HideUpperLevels : MonoBehaviour
{
    [SerializeField] private FloatReference _changeStateTime;
    [SerializeField] private List<GameObject> _parents;

    private List<List<HideDisplayObject>> _objectsByLevel = new List<List<HideDisplayObject>>();
    private List<List<Light>> _lightsByLevel = new List<List<Light>>();
    private List<List<Patrol>> _guardsByLevel = new List<List<Patrol>>();
    private int _currentLevel = 1;

    private void Awake()
    {
        foreach (GameObject parentLevel in this._parents)
        {
            this._objectsByLevel.Add(new List<HideDisplayObject>(parentLevel.GetComponentsInChildren<HideDisplayObject>()));
            this._lightsByLevel.Add(new List<Light>(parentLevel.GetComponentsInChildren<Light>()));
            this._guardsByLevel.Add(new List<Patrol>(parentLevel.GetComponentsInChildren<Patrol>()));
        }
    }

    private void OnEnable()
    {
        this.ChangeLevel(this._currentLevel);
    }

    private void OnDisable()
    {
        int i = 0;
        while (i <= this._parents.Count)
        {
            foreach (HideDisplayObject child in this._objectsByLevel[i])
                child.ChangeState(true, this._changeStateTime.Value);
            foreach (Light light in this._lightsByLevel[i])
                light.enabled = true;
            ++i;
        }
    }

    public void ChangeLevel(int level)
    {
        this._currentLevel = level;

        if (!this.enabled)
            return;

        int i = 0;

        while (i <= level)
        {
            foreach (HideDisplayObject child in this._objectsByLevel[i])
                child.ChangeState(true, this._changeStateTime.Value);
            foreach (Light light in this._lightsByLevel[i])
                light.enabled = true;
            foreach (Patrol guard in this._guardsByLevel[i])
                guard.gameObject.SetActive(true);
            ++i;
        }

        while (i < this._parents.Count)
        {
            foreach (HideDisplayObject child in this._objectsByLevel[i])
                child.ChangeState(false, this._changeStateTime.Value);
            foreach (Light light in this._lightsByLevel[i])
                light.enabled = false;
            foreach (Patrol guard in this._guardsByLevel[i])
                guard.gameObject.SetActive(false);
            ++i;
        }
    }
}
