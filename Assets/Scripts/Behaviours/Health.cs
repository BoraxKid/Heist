using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class Health : MonoBehaviour
{
    [SerializeField] private IntReference _defaultHealth;
    [SerializeField] private IntReference _maxHealth;
    [SerializeField] private FloatUnityEvent _decreaseHealthEvent;
    [SerializeField] private FloatUnityEvent _increaseHealthEvent;
    [SerializeField] private UnityEvent _onDeathEvent;

    private bool _isAlive = true;
    private int _currentHealth;

    private void Start()
    {
        this._currentHealth = this._defaultHealth.Value;
        this.NormalizeHealth();
        this._increaseHealthEvent.Invoke(this._currentHealth);
    }

    public void Decrease(int amount)
    {
        if (!this._isAlive)
            return;
        this._currentHealth -= amount;
        this.NormalizeHealth();
        this._decreaseHealthEvent.Invoke(this._currentHealth);
    }

    public void Increase(int amount)
    {
        if (!this._isAlive)
            return;
        this._currentHealth += amount;
        this.NormalizeHealth();
        this._increaseHealthEvent.Invoke(this._currentHealth);
    }

    private void NormalizeHealth()
    {
        this._currentHealth = Mathf.Clamp(this._currentHealth, 0, this._maxHealth.Value);
        if (this._currentHealth == 0 && this._isAlive)
        {
            this._isAlive = false;
            this._onDeathEvent.Invoke();
        }
    }
}
