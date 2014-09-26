using UnityEngine;
using System.Collections;

public class Entity : MonoBehaviour
{
	public float bias = 0f;

	public void applyBias(float change)
	{
		bias += change;
		bias = Mathf.Clamp(bias, -10f, 10f);
	}

	void Update()
	{
		renderer.material.color = Color.Lerp(Color.red, Color.blue, Mathf.Clamp01((bias + 10f) / 20f));
	}
}
