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
	private List<Entity> _entities = new List<Entity>();

	public int getGridResolution() {return gridResolution;}

	public override void Awake()
	{
		base.Awake();

		for(int i = 0; i < totalCount; ++i)
		{
			var go = Instantiate(entity) as GameObject;
			go.transform.position = HelperFunctions.RandomVector3InsideUnitCircle() * 15f;

			var e = go.GetComponent<Entity>();
			_entities.Add(e);
		}

		gridResolution = Mathf.CeilToInt(Mathf.Max(ground.localScale.x, ground.localScale.z) / gridCellSize);
		print(gridResolution);
	}

	public List<Entity> getEntities() { return _entities; }

	public int getGridIndex(Vector3 pos)
	{
		pos += ground.localScale * 0.5f;
		int x = Mathf.CeilToInt(pos.x / gridCellSize);
		int y = Mathf.CeilToInt(pos.z / gridCellSize);

		return gridResolution * y + x;
	}
}
