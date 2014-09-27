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

	public Vector2 minMaxSpeed = Vector2.zero;
	
	private int gridResolution;

	public AnimationCurve spawnBalance;
	public float spawnBalancePeriod = 20f;
	private float _currBalance = 0.5f;
	private float _startTime = 0f;

	public int getGridResolution() {return gridResolution;}

	public override void Awake()
	{
		base.Awake();

		gridResolution = Mathf.CeilToInt(Mathf.Max(ground.localScale.x, ground.localScale.z) / gridCellSize);
		gameOverScreen.SetActive(false);

		_startTime = Time.time;
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

		//define bias
		e.bias = Random.Range(0.3f, 1f);
		if(Random.value < _currBalance) e.bias *= -1f;

		var pos = go.transform.position;
		pos.x = Mathf.Sign(e.bias) * ground.localScale.x * 0.5f;
		pos.z = Random.Range(-1f, 1f) * ground.localScale.z * 0.5f;
		go.transform.position = pos;

		e.movement.speed = Random.Range(minMaxSpeed.x, minMaxSpeed.y);
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

		float duration = spawnBalance.keys[spawnBalance.length-1].time;
		float factor = Mathf.Repeat((Time.time - _startTime) / spawnBalancePeriod, 1f);
		_currBalance = spawnBalance.Evaluate(factor * duration);
	}

	bool isInZone(Transform zone, Vector3 pos)
	{
		pos = zone.InverseTransformPoint(pos);
		return (zone.localScale.x * 0.5f > Mathf.Abs(pos.x)) && (zone.localScale.z * 0.5f > Mathf.Abs(pos.z));
	}

	//---------------------------------------------------------------------------//
}
