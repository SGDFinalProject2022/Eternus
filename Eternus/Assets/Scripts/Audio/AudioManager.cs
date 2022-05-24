using UnityEngine.Audio;
using System;
using System.Collections;
using UnityEngine;
/// <summary>
/// Manages audio either for a whole scene or just a GameObject, created by Josiah Holcom (with help from Brackys)
/// </summary>
public class AudioManager : MonoBehaviour
{

	public static AudioManager instance;

	public AudioMixerGroup mixerGroup;

	public Sound[] sounds;

	void Awake()
	{
		foreach (Sound s in sounds)
		{
			s.source = gameObject.AddComponent<AudioSource>();
			s.source.clip = s.clip;
			s.source.loop = s.loop;
			s.source.playOnAwake = s.playOnAwake;
			s.source.spatialBlend = s.spatialBlend;

			s.source.outputAudioMixerGroup = mixerGroup;
		}
		//Play("BGM"); //uncomment when we have BGM
	}
	/// <summary>
	/// Plays the selected sound
	/// </summary>
	/// <param name="sound"></param>
	public void Play(string sound)
	{
		Sound s = Array.Find(sounds, item => item.name == sound);
		if (s == null)
		{
			Debug.LogWarning("Sound: " + sound + " not found!");
			return;
		}

		s.source.volume = s.volume * (1f + UnityEngine.Random.Range(-s.volumeVariance / 2f, s.volumeVariance / 2f));
		s.source.pitch = s.pitch * (1f + UnityEngine.Random.Range(-s.pitchVariance / 2f, s.pitchVariance / 2f));

		s.source.Play();
	}
	/// <summary>
	/// Stops playing the selected sound
	/// </summary>
	/// <param name="sound"></param>
	public void Stop(string sound)
	{
		Sound s = Array.Find(sounds, item => item.name == sound);
		if (s == null)
		{
			Debug.LogWarning("Sound: " + sound + " not found!");
			return;
		}

		s.source.Stop();
	}
	/// <summary>
	/// Changes the pitch at runtime
	/// </summary>
	/// <param name="sound"></param>
	/// <param name="pitch"></param>
	public void ChangePitch(string sound, float pitch)
    {
		Sound s = Array.Find(sounds, item => item.name == sound);
		if (s == null)
		{
			Debug.LogWarning("Sound: " + sound + " not found!");
			return;
		}

		s.source.pitch = pitch;
	}
	/// <summary>
	/// Changes the volume at runtime
	/// </summary>
	/// <param name="sound"></param>
	/// <param name="volume"></param>
	public void ChangeVolume(string sound, float volume)
    {
		Sound s = Array.Find(sounds, item => item.name == sound);
		if (s == null)
		{
			Debug.LogWarning("Sound: " + sound + " not found!");
			return;
		}

		s.source.volume = volume;
	}
	/// <summary>
	/// Changes the volume of all sounds in the certain soundtype
	/// </summary>
	/// <param name="type"></param>
	/// <param name="volume"></param>
	public void ChangeVolumeOfType(soundType type, float volume)
    {
		Sound[] s = Array.FindAll(sounds, item => item.type == type);
		if (s == null)
        {
			Debug.LogWarning("No sounds detected of type: " + type);
        }
		
		foreach (Sound sound in s)
        {
			sound.volume = volume;
			sound.source.volume = volume;
        }
    }
	/// <summary>
	/// Fades sound to destination over time
	/// </summary>
	/// <param name="sound"></param>
	/// <param name="destination"></param>
	/// <param name="time"></param>
	public void VolumeFade(string sound, float destination, float time)
    {
		Sound s = Array.Find(sounds, item => item.name == sound);
		if (s == null)
		{
			Debug.LogWarning("Sound: " + sound + " not found!");
			return;
		}
		StartCoroutine(VolumeFadeCoroutine(s, destination, time));
	}
	IEnumerator VolumeFadeCoroutine(Sound s, float destination, float time)
	{
		//float offset = time - s.source.volume;
		float increment = time / 10;
		if (destination < s.volume) //if the destination is quieter than source
        {
			increment = -increment;
        }
		for (float vol = s.source.volume; vol >= destination; vol += increment)
		{ //fades volume to destination over increment
			s.source.volume = vol;
			yield return new WaitForSeconds(time / 10);
		}		
	}
	/// <summary>
	/// Replaces the current audio clip with a new one
	/// </summary>
	/// <param name="sound"></param>
	/// <param name="newAudioClip"></param>
	public void ReplaceClip(string sound, AudioClip newAudioClip)
    {
		Sound s = Array.Find(sounds, item => item.name == sound);
		if (s == null)
		{
			Debug.LogWarning("Sound: " + sound + " not found!");
			return;
		}

		s.source.clip = newAudioClip;
	}

}
