using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "Sound Pack", fileName = "New sound pack")]
public class SoundPack : ScriptableObject
{
    public List<AudioClip> clips;

    public AudioClip PickRandomClip()
    {
        if (this.clips.Count <= 0)
            Debug.LogWarning("SoundPack " + this.name + " doesn't have any clips");
        return (this.clips[Random.Range(0, this.clips.Count)]);
    }

    public void PlayRandomClip(AudioSource source)
    {
        source.PlayOneShot(this.PickRandomClip());
    }
}
