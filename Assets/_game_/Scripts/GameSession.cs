using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class GameSession : SASSingleton<GameSession>
{
	public GameObject entity;
	public float spawnRate = 20f;

	public float gridCellSize = 3f;
	public Transform ground;
	
	private int gridResolution;

	public int getGridResolution() {return gridResolution;}

	public override void Awake()
	{
		base.Awake();


		gridResolution = Mathf.CeilToInt(Mathf.Max(ground.localScale.x, ground.localScale.z) / gridCellSize);
	}

	IEnumerator Start()
	{
		float spawn = 0f;
		while(true)
		{
			spawn += Time.deltaTime * spawnRate;

			while(spawn > 1f)
			{
				spawnGuy();
				spawn -= 1f;
			}

			yield return null;
		}
	}

	private void spawnGuy()
	{
		var go = Instantiate(entity) as GameObject;
		var e = go.GetComponent<Entity>();

		var pos = go.transform.position;
		pos.x = Mathf.Sign(e.bias) * ground.localScale.x * 0.5f;
		pos.z = Random.Range(-1f, 1f) * ground.localScale.z * 0.5f;
		go.transform.position = pos;

		go.GetComponent<Entity>();
	}

	public int getGridIndex(Vector3 pos)
	{
		pos += ground.localScale * 0.5f;
		int x = Mathf.CeilToInt(pos.x / gridCellSize);
		int y = Mathf.CeilToInt(pos.z / gridCellSize);

		return gridResolution * y + x;
	}
}
