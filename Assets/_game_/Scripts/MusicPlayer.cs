using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class MusicPlayer : SASSingleton<MusicPlayer>
{
	public List<AudioClip> tracks = new List<AudioClip>();

	// Use this for initialization
	IEnumerator Start ()
	{
		var src = GetComponent<AudioSource>();
		src.loop = false;
		DontDestroyOnLoad(gameObject);

		int index = 0;
	
		while(true)
		{
			if(!src.isPlaying)
			{
				src.clip = tracks[index];
				index = (++index) % tracks.Count;

				src.Play();
			}

			yield return new WaitForSeconds(0.5f);
		}
	}
}
