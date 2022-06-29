using UnityEngine.Audio;
using UnityEngine;

public enum soundType { SFX, music }
/// <summary>
/// Sound settings for the AudioManager, use (sound).source.[...] for changing the sound on runtime
/// </summary>
[System.Serializable]
public class Sound {

	public string name;

	public soundType type;

	public AudioClip clip;

    public AudioSource customAudioSource;

	[Range(0f, 1f)]
	public float volume = .75f;
	[Range(0f, 1f)]
	public float volumeVariance = .1f;
	[HideInInspector] public float originalVolume;

	[Range(.1f, 3f)]
	public float pitch = 1f;
	[Range(0f, 1f)]
	public float pitchVariance = .1f;

	[Range(0f, 1f)]
	public float spatialBlend = 1f;

	public bool loop = false;
	public bool playOnAwake = false;
    public bool bypassReverbZones = false;

	public AudioMixerGroup mixerGroup;

	[HideInInspector]
    public AudioSource source;
	[HideInInspector]
    public bool isPlaying;
}
