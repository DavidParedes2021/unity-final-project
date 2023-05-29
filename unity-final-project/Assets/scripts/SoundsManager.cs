using System.Collections.Generic;
using UnityEngine;

public class SoundsManager : MonoBehaviour
{
    public List<AudioClip> zombieNearAudios;
    public List<AudioClip> zombieAttacking;
    public List<AudioClip> zombieInSightOfPlayer;
    public AudioClip endGame;
    public AudioClip winGame;
    public AudioClip reload;
    public AudioClip perkSound;
    public AudioClip breathingSound;

    public static AudioSource GetNewASC(GameObject gameObject)
    {
        var addComponent = gameObject.AddComponent<AudioSource>();
        return addComponent;
    }

    public static void PlayClipAndDestroy(GameObject gameObject, AudioClip sound)
    {
        var audioSource = GetNewASC(gameObject);
        var soundLength = sound.length;
        audioSource.PlayOneShot(sound);
        Destroy(audioSource,soundLength);
    }

    public static void ReleaseAS(AudioSource audioSource) {
        Destroy(audioSource);
    }
}