using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.Events;

public class EnemyShoot : MonoBehaviour
{
    [Tooltip("Time between every shoot")]
    [SerializeField] private LayerMask _shootLayerMask;
    [SerializeField] private FloatReference _shootTime;
    [SerializeField] private FloatReference _updateTime;
    [SerializeField] private FloatReference _spreadAngle;
    [SerializeField] private FloatReference _maxBulletDistance;
    [SerializeField] private GameObject _weapon;
    [SerializeField] private UnityEvent _gunshotEvent;
    [SerializeField] private UnityEvent _playerShotEvent;

    private bool _firstShoot;
    private float _elapsedTime;

    private void Awake()
    {
        if (this._weapon == null)
            this._weapon = this.gameObject;
    }

    private void OnEnable()
    {
        this._firstShoot = true;
        this._elapsedTime = 0.0f;
        this.InvokeRepeating("UpdateShoot", 0.0f, this._updateTime.Value);
    }

    private void OnDisable()
    {
        this.CancelInvoke("UpdateShoot");
    }

    private void UpdateShoot()
    {
        if (GameConstants.paused || !GameConstants.playerVariable.isAlive)
            return;
        this._elapsedTime += Time.deltaTime;

        if (this._elapsedTime >= this._shootTime.Value)
        {
            this._elapsedTime -= this._shootTime.Value;
            this.Shoot();
        }
    }

    private void Shoot()
    {
        Vector3 targetCenter = Helper.GetCenter(GameConstants.playerVariable.gameObject);
        Vector3 origin = this._weapon.transform.position;
        Vector3 direction = targetCenter - origin;
        Vector3 tmp;
        RaycastHit hitInfo;
        bool hitSomething = false;
        if (Physics.Raycast(origin, tmp = direction + this.Spread(), out hitInfo, this._maxBulletDistance.Value, this._shootLayerMask.value))
        {
            // On the first shot, the bullet shouldn't touch the player so he has time to think
            if (this._firstShoot)
            {
                if (hitInfo.collider != GameConstants.playerVariable.collider)
                    hitSomething = true;
                else
                {
                    bool shot = false;
                    int count = 0;
                    while (++count < 10 && (shot = Physics.Raycast(origin, tmp = direction + this.Spread() * count, out hitInfo, this._maxBulletDistance.Value, this._shootLayerMask.value)))
                    {
                        if (hitInfo.collider != GameConstants.playerVariable.collider)
                            break;
                    }
                    if (count >= 10)
                        return;
                    if (shot)
                        hitSomething = true;
                }
                // The raycast hasn't touched the player, so we can continue!
                this._firstShoot = false;
            }
            else
                hitSomething = true;
        }

        this._gunshotEvent.Invoke();

        if (hitSomething && hitInfo.collider == GameConstants.playerVariable.collider)
            this._playerShotEvent.Invoke();

        // FX
        BulletTrail bulletTrail = GameConstants.bulletTrailPool.Request().GetComponent<BulletTrail>();
        if (hitSomething)
            tmp = hitInfo.point;
        else
            tmp = origin + tmp * this._maxBulletDistance.Value;
        bulletTrail.SetPoints(origin, tmp);
        bulletTrail.Display();
        Debug.DrawRay(origin, tmp, Color.red, this._shootTime.Value);
    }

    private Vector3 Spread()
    {
        return (new Vector3(Random.Range(-this._spreadAngle.Value, this._spreadAngle.Value),
            Random.Range(-this._spreadAngle.Value, this._spreadAngle.Value),
            Random.Range(-this._spreadAngle.Value, this._spreadAngle.Value)));
    }
}
