using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class GameSession : SASSingleton<GameSession>
{
	public GameObject entity;
	public int totalCount = 100;

	public float gridCellSize = 3f;
	public Transform ground;
	
	private int gridResolution;

	public int getGridResolution() {return gridResolution;}

	public override void Awake()
	{
		base.Awake();

		for(int i = 0; i < totalCount; ++i)
		{
			var go = Instantiate(entity) as GameObject;
			var e = go.GetComponent<Entity>();

			var pos = go.transform.position;
			pos.x = Mathf.Sign(e.bias) * ground.localScale.x * Random.Range(0.25f, 0.5f);
			pos.z = Random.Range(-1f, 1f) * ground.localScale.z * 0.5f;
			go.transform.position = pos;

			go.GetComponent<Entity>();
		}

		gridResolution = Mathf.CeilToInt(Mathf.Max(ground.localScale.x, ground.localScale.z) / gridCellSize);
	}

	public int getGridIndex(Vector3 pos)
	{
		pos += ground.localScale * 0.5f;
		int x = Mathf.CeilToInt(pos.x / gridCellSize);
		int y = Mathf.CeilToInt(pos.z / gridCellSize);

		return gridResolution * y + x;
	}
}
