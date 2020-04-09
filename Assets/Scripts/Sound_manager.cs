using UnityEngine;
using System.Collections;

public class Sound_manager : MonoBehaviour {
	public Sound [] sounds;
	// Use this for initialization
	void Start () {
		foreach (Sound s in sounds)
		{
			s.source = gameObject.AddComponent<AudioSource>();
			s.source.clip = s.clip;
			s.source.loop = s.loop;
			s.source.volume = s.volume;
			s.source.loop = s.loop;
			//s.source.outputAudioMixerGroup = mixerGroup;
		}
	}
	
	// Update is called once per frame
	void Update () {
	
	}

	public void PlaySound(int i) {
		sounds[i].source.Play();
	}
}
