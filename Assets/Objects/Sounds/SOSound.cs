using UnityEngine;
using UnityEngine.Audio;
using NaughtyAttributes;

[CreateAssetMenu(menuName = "Scriptable Objects/Sound Design/New Sound")]
public class SOSound : ScriptableObject
{
    public AudioClip clip = null;
    public AudioMixerGroup audioMixerGroup = null;

    public bool isVolumeRandom = false;
    [Range(0f, 1f)]
    public float volume = 0.1f;
    [MinMaxSlider(0f, 1f)]
    public Vector2 randomVolume;

    public bool isPitchRandom = false;
    [Range(-3f, 3f)]
    public float pitch = 1.0f;
    [MinMaxSlider(-3f, 3f)]
    public Vector2 randomPitch;

    public bool playOnAwake = false;
    public bool loop = false;
    /// <summary>
    /// 0 if infinite;
    /// </summary>
    [Min(0)]
    public int numberOfLoops;
}
