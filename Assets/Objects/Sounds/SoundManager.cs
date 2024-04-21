using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Audio;
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
        if (Input.GetKeyDown(KeyCode.G))
            PlayWithDelay(soundTest, 1.5f);

        if (Input.GetKeyDown(KeyCode.H))
            StopAll();
    }

    public void PlayWithDelay(SOSound sound, float delay)
    {
        StartCoroutine(PlayDelayed(sound, delay));
    }

    private IEnumerator PlayDelayed(SOSound sound, float delay, bool isRealTime = false)
    {
        if (isRealTime)
            yield return new WaitForSecondsRealtime(delay);
        else
            yield return new WaitForSeconds(delay);

        Play(sound);
    }

    public void Play(SOSound sound)
    {
        int currentSoundIndex = GetRandomSound(sound.sounds);
        if (currentSoundIndex == -1) return;
        
        var newSoundEmitter = pool.Get();
        newSoundEmitter.Init(pool.Release);

        var currentSound = sound.sounds[currentSoundIndex];
        var currentPitch = sound.isPitchRandom ? Random.Range(sound.randomPitch.x, sound.randomPitch.y) : sound.pitch;

        newSoundEmitter.AudioSource.clip = currentSound.clip;
        newSoundEmitter.AudioSource.outputAudioMixerGroup = sound.audioMixerGroup;
        newSoundEmitter.AudioSource.volume = sound.isVolumeRandom ? Random.Range(sound.randomVolume.x, sound.randomVolume.y) : sound.volume;
        newSoundEmitter.AudioSource.pitch = currentPitch;
        newSoundEmitter.AudioSource.loop = sound.loop;


        if (sound.loop) 
        {
            if(sound.numberOfLoops > 0)
                newSoundEmitter.Invoke("DestroySoundEmitter", currentSound.clip.length / currentPitch * sound.numberOfLoops);
        }
        else newSoundEmitter.Invoke("DestroySoundEmitter", currentSound.clip.length / currentPitch);

        newSoundEmitter.AudioSource.Play();

        if(!soundEmitters.Contains(newSoundEmitter)) soundEmitters.Add(newSoundEmitter);
    }

    public void StopAll()
    {
        for (int i = soundEmitters.Count - 1; i >= 0; i--)
        {
            if (!soundEmitters[i].isActiveAndEnabled) continue;

            soundEmitters[i].CancelInvoke();
            soundEmitters[i].DestroySoundEmitter();
        }
    }

    private int GetRandomSound(Sound[] sounds)
    {
        if (sounds == null || sounds.Length == 0) return -1;

        float weight;
        float total = 0;
        for (int i = 0; i < sounds.Length; i++)
        {
            weight = sounds[i].weight;

            if (float.IsPositiveInfinity(weight))
            {
                return i;
            }
            else if (weight >= 0f && !float.IsNaN(weight))
            {
                total += sounds[i].weight;
            }
        }

        float randomValue = Random.value;
        float sum = 0f;

        for (int i = 0; i < sounds.Length; i++)
        {
            weight = sounds[i].weight;
            if (float.IsNaN(weight) || weight <= 0f) continue;

            sum += weight / total;
            if (sum >= randomValue)
            {
                return i;
            }
        }

        return -1;
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