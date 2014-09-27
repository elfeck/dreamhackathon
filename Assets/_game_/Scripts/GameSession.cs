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

	public AnimationCurve spawnPositionProfile;
	public Vector2 spawnProfileOffsetChange = Vector2.zero;
	private Vector2 _spawnOffset = Vector2.zero;

	private float _currTimeScale = 1f;
	private float _timeScaleTarget = 1f;

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
		var go = ObjectPoolController.Instantiate(entity, entity.transform.position, entity.transform.rotation) as GameObject;
		var e = go.GetComponent<Entity>();

		//define bias
		e.bias = Random.Range(0.3f, 1f);
		if(Random.value < _currBalance) e.bias *= -1f;

		var pos = go.transform.position;
		pos.x = Mathf.Sign(e.bias) * ground.localScale.x * 0.5f;
		var zPos = Mathf.Clamp01(spawnPositionProfile.Evaluate(_spawnOffset[e.bias > 0f ? 0 : 1] + Random.Range(0f, 1f))) * 2f - 1f;
		pos.z = zPos * ground.localScale.z * 0.5f;
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

	public bool allowTimescaleChange = true;

	void Update()
	{

		if(allowTimescaleChange)
		{
			//change target timescale over time
			float delta = (Time.time - _startTime);
			_timeScaleTarget = 1f + delta * delta / 120f / 120f;


			if(gameOver())
				_currTimeScale = 0.001f;
			else
			{
				if(allowGameOver)
				{
					if(_currTimeScale < _timeScaleTarget * 0.99f)
						_currTimeScale = Mathf.Lerp(_currTimeScale, _timeScaleTarget, 0.1f);
					else _currTimeScale = _timeScaleTarget;
				}
				else _currTimeScale = _timeScaleTarget;
			}

			Time.timeScale = _currTimeScale;			
		}
	}

	static List<Entity> _tmp_toDelete = new List<Entity>();
	void slowUpdate()
	{
		Screen.showCursor = false;

		//check if an entity is in the final region of the opposing party
		foreach(var e in Entity.allEntities)
		{
			if((e.bias < 0f && isInZone(true, e.transform.position))
				|| (e.bias > 0f && isInZone(false, e.transform.position)))
			{
				_tmp_toDelete.Add(e);
				removeLife(e.bias < 0f, e.transform.position);
			}
		}
		for(int i = 0; i < _tmp_toDelete.Count; ++i)
			_tmp_toDelete[i].die();
		_tmp_toDelete.Clear();


		if(enableSpawnBalanceCurve)
		{
			float duration = spawnBalance.keys[spawnBalance.length-1].time;
			float factor = Mathf.Repeat((Time.time - _startTime) / spawnBalancePeriod, 1f);
			_currBalance = spawnBalance.Evaluate(factor * duration);
		}

		_spawnOffset += Time.deltaTime * spawnProfileOffsetChange;
		var profileLength = spawnPositionProfile.keys[spawnPositionProfile.length-1].time;
		for(int i = 0; i < 2; ++i )
			if(_spawnOffset[i] > profileLength - 1f) _spawnOffset[i] = 0f;
		
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

		pos.x += blue ? -5f : 5f;

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
