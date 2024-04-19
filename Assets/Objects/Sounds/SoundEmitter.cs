using System;
using UnityEngine;

public class SoundEmitter : MonoBehaviour
{
    private AudioSource audioSource;
    public AudioSource AudioSource { get => audioSource;}

    private Action<SoundEmitter> killAction;

    private void Awake()
    {
        audioSource = GetComponent<AudioSource>();
    }

    public void Init(Action<SoundEmitter> _action)
    {
        killAction = _action;
    }

    public void DestroySoundEmitter()
    {
        audioSource.Stop();
        killAction(this);
    }

}
