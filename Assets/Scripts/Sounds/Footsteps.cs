using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Footsteps : MonoBehaviour
{
    [SerializeField] private FloatReference _crouchVolume;
    [SerializeField] private FloatReference _walkVolume;
    [SerializeField] private FloatReference _runVolume;
    [SerializeField] private SoundPack _soundPack;
    [SerializeField] private AudioSource _leftFootAudioSource;
    [SerializeField] private AudioSource _rightFootAudioSource;
    [SerializeField] private bool _active;

    public void FootstepLeft(int type)
    {
        if (this._active)
        {
            // Debug.Log("Animation event #FootstepLeft " + type);
            this._leftFootAudioSource.volume = (type == 0) ? this._crouchVolume.Value : ((type == 1) ? this._walkVolume.Value : this._runVolume.Value);
            this._leftFootAudioSource.PlayOneShot(this._soundPack.PickRandomClip());
        }
    }

    public void FootstepRight(int type)
    {
        if (this._active)
        {
            // Debug.Log("Animation event #FootstepRight " + type);
            this._rightFootAudioSource.volume = (type == 0) ? this._crouchVolume.Value : ((type == 1) ? this._walkVolume.Value : this._runVolume.Value);
            this._rightFootAudioSource.PlayOneShot(this._soundPack.PickRandomClip());
        }
    }
}
