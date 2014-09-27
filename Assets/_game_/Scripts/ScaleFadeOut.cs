using UnityEngine;
using System.Collections;

public class ScaleFadeOut : MonoBehaviour {

	public float duration = 1f;
	public float scaleFactor = 5f;

	// Use this for initialization
	IEnumerator Start()
	{
		float startTime = Time.time;
		Vector3 scale = transform.localScale;

		while(startTime + duration > Time.time)
		{
			var factor = Mathf.Clamp01((Time.time - startTime) / duration);
			transform.localScale = Vector3.Lerp(scale, scaleFactor * Vector3.one, factor);

			var col = renderer.material.color;
			col.a = Mathf.Lerp(1f, 0f, factor);
			renderer.material.color = col;

			yield return null;
		}
	}
	
}
