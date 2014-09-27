using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class GameSession : SASSingleton<GameSession>
{
	public GameObject entity;
	public float spawnRate = 20f;

	public float gridCellSize = 3f;
	[HideInInspector] public Transform ground;

	public Vector2 minMaxSpeed = Vector2.zero;
	
	private int gridResolution;

	public bool enableSpawnBalanceCurve = true;
	public AnimationCurve spawnBalance;
	public float spawnBalancePeriod = 20f;
	private float _currBalance = 0.5f;
	private float _startTime = 0f;
	private int _initialLifes;

	private float _currTimeScale = 1f;

	public int redLifes = 10;
	public int blueLifes = 10;
	public bool allowGameOver = true;

	public int getGridResolution() {return gridResolution;}
	public void setSpawnBalance(float bal) { _currBalance = bal; }
	public bool gameOver() { return allowGameOver && (redLifes <= 0 || blueLifes <= 0);}

	public override void Awake()
	{
		base.Awake();

		_initialLifes = redLifes;
		ground = GameObject.FindGameObjectWithTag("Ground").transform;
		gridResolution = Mathf.CeilToInt(Mathf.Max(ground.localScale.x, ground.localScale.z) / gridCellSize);

		reset();
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

	void Update()
	{
		if(gameOver())
			_currTimeScale = 0.001f;
		else
		{
			if(allowGameOver) _currTimeScale = 1f;
			else
			{
				if(_currTimeScale < 0.98f)
					_currTimeScale = Mathf.Lerp(_currTimeScale, 1f, Time.deltaTime * 0.2f);
				else _currTimeScale = 1f;
			}
		}

		Time.timeScale = _currTimeScale;
	}

	void slowUpdate()
	{
		Screen.showCursor = false;

		//check if an entity is in the final region of the opposing party
		foreach(var e in Entity.allEntities)
		{
			if((e.bias < 0f && isInZone(true, e.transform.position))
				|| (e.bias > 0f && isInZone(false, e.transform.position)))
			{
				e.die();
				removeLife(e.bias < 0f, e.transform.position);
			}
		}


		if(enableSpawnBalanceCurve)
		{
			float duration = spawnBalance.keys[spawnBalance.length-1].time;
			float factor = Mathf.Repeat((Time.time - _startTime) / spawnBalancePeriod, 1f);
			_currBalance = spawnBalance.Evaluate(factor * duration);
		}
		
	}

	bool isInZone(bool blue, Vector3 pos)
	{
		var limit = GameSession.inst.ground.localScale.x * 0.49f;
		return (blue && pos.x > limit) || (!blue && pos.x < -limit);
	}


	public GameObject redMinusEffect;
	public GameObject blueMinusEffect;

	void removeLife(bool blue, Vector3 pos)
	{
		if(!allowGameOver) return;

		if(blue) --blueLifes;
		else --redLifes;

		var prefab = blue ? blueMinusEffect : redMinusEffect;
		if(prefab) ObjectPoolController.Instantiate(prefab, pos, prefab.transform.rotation);

		//slow down a little
		_currTimeScale = 0.01f;
	}

	//---------------------------------------------------------------------------//

	public void reset()
	{
		redLifes = blueLifes = _initialLifes;

		_startTime = Time.time;
		Entity.deathCount = 0;

		Entity.destroyAll();
	}
}
