using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class GameSession : SASSingleton<GameSession>
{
	public GameObject entity;
	public float spawnRate = 20f;

	public float gridCellSize = 3f;
	public Transform ground;
	public Transform homeZoneRed;
	public Transform homeZoneBlue;
	public GameObject gameOverScreen;
	
	private int gridResolution;

	public int getGridResolution() {return gridResolution;}

	public override void Awake()
	{
		base.Awake();

		gridResolution = Mathf.CeilToInt(Mathf.Max(ground.localScale.x, ground.localScale.z) / gridCellSize);
		gameOverScreen.SetActive(false);
	}

	void OnEnable()
	{
		InvokeRepeating(slowUpdate, 0.25f);
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

	//---------------------------------------------------------------------------//

	void slowUpdate()
	{
		//check if an entity is in the final region of the opposing party
		foreach(var e in Entity.allEntities)
		{
			if((e.bias < 0f && isInZone(homeZoneBlue, e.transform.position))
			 || (e.bias > 0f && isInZone(homeZoneRed, e.transform.position)))
			{
				//LOST
				gameOverScreen.SetActive(true);
			}
		}
	}

	bool isInZone(Transform zone, Vector3 pos)
	{
		pos = zone.InverseTransformPoint(pos);
		return (zone.localScale.x * 0.5f > Mathf.Abs(pos.x)) && (zone.localScale.z * 0.5f > Mathf.Abs(pos.z));
	}

	//---------------------------------------------------------------------------//
}
