using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Pool;

public class SoundManager : MonoBehaviour
{
    private ObjectPool<SoundEmitter> pool;

    [SerializeField] private SoundEmitter audioSourcePrefab;

    [SerializeField] private int initialAudioSource;
    [SerializeField] private int maxAudioSource;

    private List<SoundEmitter> soundEmitters = new List<SoundEmitter>();

    [Header("Test")]
    public SOSound soundTest;


    private static SoundManager _instance = null;

    public static SoundManager Instance
    {
        get => _instance;
    }

    private void Awake()
    {
        if (_instance == null) _instance = this;
        pool = new ObjectPool<SoundEmitter>(CreateFunction, OnGetFunction, OnReleaseFunction, OnDestroyFunction, false, initialAudioSource, maxAudioSource);
    }

    private void Update()
    {
        if(Input.GetKeyDown(KeyCode.G))
            Play(soundTest);
    }

    public void Play(SOSound sound)
    {
        var newSoundEmitter = pool.Get();
        newSoundEmitter.Init(pool.Release);

        var currentClip = sound.clip[Random.Range(0, sound.clip.Length)];
        var currentPitch = sound.isPitchRandom ? Random.Range(sound.randomPitch.x, sound.randomPitch.y) : sound.pitch;

        newSoundEmitter.AudioSource.clip = currentClip;
        newSoundEmitter.AudioSource.outputAudioMixerGroup = sound.audioMixerGroup;
        newSoundEmitter.AudioSource.volume = sound.isVolumeRandom ? Random.Range(sound.randomVolume.x, sound.randomVolume.y) : sound.volume;
        newSoundEmitter.AudioSource.pitch = currentPitch;
        newSoundEmitter.AudioSource.loop = sound.loop;


        if (sound.loop) 
        {
            if(sound.numberOfLoops > 0)
                newSoundEmitter.Invoke("DestroySoundEmitter", currentClip.length / currentPitch * sound.numberOfLoops);
        }
        else newSoundEmitter.Invoke("DestroySoundEmitter", currentClip.length / currentPitch);

        newSoundEmitter.AudioSource.Play();

        if(!soundEmitters.Contains(newSoundEmitter)) soundEmitters.Add(newSoundEmitter);
    }



    #region Pool Functions

    private SoundEmitter CreateFunction()
    {
        return Instantiate(audioSourcePrefab);
    }

    private void OnGetFunction(SoundEmitter _soundEmitterGet)
    {
        _soundEmitterGet.gameObject.SetActive(true);
    }

    private void OnReleaseFunction(SoundEmitter _soundEmitterToRelease)
    {
        _soundEmitterToRelease.gameObject.SetActive(false);
    }

    private void OnDestroyFunction(SoundEmitter _soundEmitterToDestroy)
    {
        soundEmitters.Remove(_soundEmitterToDestroy);
        Destroy(_soundEmitterToDestroy.gameObject);
    }
    #endregion 
}