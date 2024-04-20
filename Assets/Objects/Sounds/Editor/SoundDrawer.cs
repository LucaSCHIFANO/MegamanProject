using UnityEditor;
using UnityEngine;

[CustomEditor(typeof(SOSound)), CanEditMultipleObjects]
public class SoundDrawer : Editor
{
    SerializedProperty m_clip;
    SerializedProperty m_audioMixer;

    SerializedProperty m_isVolumeRandom;
    SerializedProperty m_volume;
    SerializedProperty m_randomVolume;

    SerializedProperty m_isPitchRandom;
    SerializedProperty m_pitch;
    SerializedProperty m_randomPitch;

    SerializedProperty m_playOnAwake;
    SerializedProperty m_loop;
    SerializedProperty m_numberOfLoops;

    private void OnEnable()
    {
        m_clip = serializedObject.FindProperty("clip");
        m_audioMixer = serializedObject.FindProperty("audioMixerGroup");

        m_isVolumeRandom = serializedObject.FindProperty("isVolumeRandom");
        m_volume = serializedObject.FindProperty("volume");
        m_randomVolume = serializedObject.FindProperty("randomVolume");

        m_isPitchRandom = serializedObject.FindProperty("isPitchRandom");
        m_pitch = serializedObject.FindProperty("pitch");
        m_randomPitch = serializedObject.FindProperty("randomPitch");

        m_playOnAwake = serializedObject.FindProperty("playOnAwake");
        m_loop = serializedObject.FindProperty("loop");
        m_numberOfLoops = serializedObject.FindProperty("numberOfLoops");

    }
    public override void OnInspectorGUI()
    {
        serializedObject.Update();

        EditorGUILayout.LabelField("Sound", EditorStyles.boldLabel, GUILayout.Height(20));

        EditorGUILayout.PropertyField(m_clip, new GUIContent("Clip"), true);
        EditorGUILayout.Space(5);
        EditorGUILayout.PropertyField(m_audioMixer, new GUIContent("Audio Mixer Group"), GUILayout.Height(20));

        EditorGUILayout.Space(20);
        EditorGUILayout.LabelField("Volume", EditorStyles.boldLabel, GUILayout.Height(20));

        EditorGUILayout.PropertyField(m_isVolumeRandom, new GUIContent("Is Volume Random"), GUILayout.Height(20));
        if (m_isVolumeRandom.boolValue)
            EditorGUILayout.PropertyField(m_randomVolume, new GUIContent("Random Volume"), GUILayout.Height(20));
        
        else
            EditorGUILayout.PropertyField(m_volume, new GUIContent("Volume"), GUILayout.Height(20));


        EditorGUILayout.Space(20);
        EditorGUILayout.LabelField("Pitch", EditorStyles.boldLabel, GUILayout.Height(20));

        EditorGUILayout.PropertyField(m_isPitchRandom, new GUIContent("Is Pitch Random"), GUILayout.Height(20));
        if (m_isPitchRandom.boolValue)
            EditorGUILayout.PropertyField(m_randomPitch, new GUIContent("Random Pitch"), GUILayout.Height(20));

        else
            EditorGUILayout.PropertyField(m_pitch, new GUIContent("Pitch"), GUILayout.Height(20));


        EditorGUILayout.Space(20);
        EditorGUILayout.LabelField("Settings", EditorStyles.boldLabel, GUILayout.Height(20));

        EditorGUILayout.PropertyField(m_playOnAwake, new GUIContent("Play On Awake"), GUILayout.Height(20));
        EditorGUILayout.PropertyField(m_loop, new GUIContent("Loop"), GUILayout.Height(20));
        if (m_loop.boolValue)
            EditorGUILayout.PropertyField(m_numberOfLoops, new GUIContent("Number Of Loops"), GUILayout.Height(20));

        serializedObject.ApplyModifiedProperties();
    }

}
