using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class rollingsounds : MonoBehaviour
{
    AudioSource audioSource;
    public AudioClip[] sounds;
    private void Start()
    {
        audioSource = GetComponent<AudioSource>();
    }
    public void StartRoll()
    {
        audioSource.clip = sounds[0];
        audioSource.Play();
    }
    public void endRoll()
    {
        audioSource.clip = sounds[1];
        audioSource.Play();
    }
}
