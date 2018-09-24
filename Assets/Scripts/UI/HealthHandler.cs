using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HealthHandler : MonoBehaviour
{
    [SerializeField] private HealthPoint _healthPointPrefab;
    [SerializeField] private IntReference _defaultHealth;

    private List<HealthPoint> _healthPoints = new List<HealthPoint>();

    public void UpdateHealth(float fHealth)
    {
        int health = (int)fHealth;
        int i;
        List<HealthPoint> destroyed = new List<HealthPoint>();

        while (this._healthPoints.Count < health)
            this._healthPoints.Add(Instantiate(this._healthPointPrefab, this.transform));
        this._healthPoints = new List<HealthPoint>(this.GetComponentsInChildren<HealthPoint>());

        i = 0;
        foreach (HealthPoint hp in this._healthPoints)
        {
            if (i < health)
                hp.Activate();
            else
            {
                if (i >= this._defaultHealth.Value)
                {
                    destroyed.Add(hp);
                    Destroy(hp.gameObject);
                }
                else
                    hp.Deactivate();
            }
            ++i;
        }
        foreach (HealthPoint hp in destroyed)
            this._healthPoints.Remove(hp);
    }

    public void IncreaseHealth(float fHealth)
    {
        int health = (int)fHealth;

    }

    public void DecreaseHealth(float fHealth)
    {
        int health = (int)fHealth;
    }
}
