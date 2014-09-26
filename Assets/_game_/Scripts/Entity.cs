using UnityEngine;
using System.Collections;

public class Entity : MonoBehaviour
{
	public float bias = 0f;

	void Awake()
	{
		bias = Random.Range(-1f, 1f);
	}

	public void applyBias(float change)
	{
		bias += change;
		bias = Mathf.Clamp(bias, -1f, 1f);
	}

	void Update()
	{
		renderer.material.color = Color.Lerp(Color.red, Color.blue, Mathf.Clamp01((bias + 1f) / 2f));
	}
}
