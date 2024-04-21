using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.Audio;
using UnityEngine.Pool;

public class SoundManager : MonoBehaviour
{
    private ObjectPool<SoundEmitter> pool;

    [SerializeField] private SoundEmitter audioSourcePrefab;

    [SerializeField] private int initialAudioSource;
    [SerializeField] private int maxAudioSource;

    [Space]
    [SerializeField] private List<MixerType> mixerTypes = new List<MixerType>();

    private Dictionary<int, SoundEmitter> soundEmitters = new Dictionary<int, SoundEmitter>();
    private int id = 0;

    [Header("Test")]
    public SOSound soundTest;
    public int lastSoundPlayed = 0;

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
            lastSoundPlayed = Play(soundTest);

        if (Input.GetKeyDown(KeyCode.H))
            Stop(lastSoundPlayed);
    }

    /// <summary>
    /// Play a sound. Return the id of the emitter which can be used in Stop
    /// </summary>
    /// <param name="sound"></param>
    /// <returns></returns>
    public int Play(SOSound sound)
    {
        int currentSoundIndex = GetRandomSound(sound.sounds);
        if (currentSoundIndex == -1) return -1;
        
        var newSoundEmitter = pool.Get();
        newSoundEmitter.Init(pool.Release);

        var currentSound = sound.sounds[currentSoundIndex];
        var currentPitch = sound.isPitchRandom ? Random.Range(sound.randomPitch.x, sound.randomPitch.y) : sound.pitch;
        AudioMixerGroup currentMixer = null;

        foreach (MixerType mixer in mixerTypes)
        {
            if (sound.soundType == mixer.soundType)
            {
                currentMixer = mixer.audioMixerGroup;
                break;
            }
        }


        newSoundEmitter.AudioSource.clip = currentSound.clip;
        newSoundEmitter.AudioSource.outputAudioMixerGroup = currentMixer;
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

        if (!soundEmitters.ContainsValue(newSoundEmitter)) soundEmitters.Add(id, newSoundEmitter);
        return id++;
    }

    public void Stop(int id)
    {
        if (!soundEmitters.ContainsKey(id) || !soundEmitters[id].isActiveAndEnabled) return;
        soundEmitters[id].CancelInvoke();
        soundEmitters[id].DestroySoundEmitter();
    }

    public void StopAll()
    {
        var keys = soundEmitters.Keys.ToArray().Reverse();

        foreach (var key in keys)
        {
            if (!soundEmitters[key].isActiveAndEnabled) continue;
            soundEmitters[key].CancelInvoke();
            soundEmitters[key].DestroySoundEmitter();
        }
    }

    public void StopAll(MixerType.SoundType type)
    {
        AudioMixerGroup currentMixer = null;
        foreach (var mixer in mixerTypes)
        {
            if(mixer.soundType == type)
            {
                currentMixer = mixer.audioMixerGroup;
                break;
            }
        }
        if (currentMixer == null) return;

        var keys = soundEmitters.Keys.ToArray().Reverse();

        foreach (var key in keys)
        {
            if (!soundEmitters[key].isActiveAndEnabled ||
                soundEmitters[key].AudioSource.outputAudioMixerGroup != currentMixer) continue;

            soundEmitters[key].CancelInvoke();
            soundEmitters[key].DestroySoundEmitter();
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
        foreach (var item in soundEmitters)
        {
            if (item.Value == _soundEmitterToDestroy)
            {
                soundEmitters.Remove(item.Key);
                break;
            }
        }

        Destroy(_soundEmitterToDestroy.gameObject);
    }
    #endregion 
}


[System.Serializable]
public class MixerType
{
    public AudioMixerGroup audioMixerGroup;
    public SoundType soundType;

    public enum SoundType
    {
        Music,
        SFX
    }
}