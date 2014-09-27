using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class Entity : MonoBehaviour
{
	static private List<HashSet<Entity>> _grid;
	static public List<Entity> allEntities = new List<Entity>();
	static public int deathCount = 0;

	static public void destroyAll()
	{
		foreach(var e in allEntities)
			Destroy(e.gameObject);

		if(_grid != null)
		{
			foreach(var g in _grid)
				g.Clear();
			_grid.Clear();
		}

		allEntities.Clear();
		_grid = null;
	}

	//---------------------------------------------------------------------------//

	public float bias = 0f;
	private int _currGridIndex = -1;
	public float cellInfluenceFactor = 0.2f;

	public GameObject deathEffect;

	private float _influencedTime = 0f;

	public float reach;
	[HideInInspector]
	public Movement movement;
	private Vector3 _initialScale;

	void Awake()
	{
		movement = GetComponent<Movement>();
		_initialScale = transform.localScale;

		allEntities.Add(this);
	}

	void OnDestroy()
	{
		allEntities.Remove(this);
	}

	IEnumerator Start()
	{
		if(_grid == null)
		{
			_grid = new List<HashSet<Entity>>(2048);
			var res = GameSession.inst.getGridResolution();
			res *= res;
			for(int i = 0; i < res; ++i)
				_grid.Add(new HashSet<Entity>());
		}

		//========================================//

		yield return new WaitForSeconds(Random.Range(0f, 0.5f));

		_currGridIndex = getGridIndex();
		_grid[_currGridIndex].Add(this);

		float interval = 0.25f;
		while(true)
		{
			float cellBias = 0f;
			foreach(var e in _grid[_currGridIndex])
				cellBias += e.bias;

			if(cellBias * Mathf.Sign(bias) < -1f)
			{
				die();
				yield break;
			}

			//drive towards average
			//var avg = cellBias / (float)(_grid[_currGridIndex].Count);
			//applyBias(interval * Mathf.Sign(avg - bias) * cellInfluenceFactor);
			applyBias(interval * cellBias * cellInfluenceFactor);

			//update grid position
			var index = getGridIndex();
			if(_currGridIndex != index)
			{
				_grid[_currGridIndex].Remove(this);
				_currGridIndex = index;
				_grid[_currGridIndex].Add(this);
			}

			yield return new WaitForSeconds(interval);
		}
	}

	public void die()
	{
		_grid[_currGridIndex].Remove(this);
		Destroy(gameObject);
		deathCount++;

		if(deathEffect) ObjectPoolController.Instantiate(deathEffect, transform.position, deathEffect.transform.rotation);
	}

	public void applyBias(float change)
	{
		if(Mathf.Abs(bias) > 0.99f && bias * change > 0f)
			_influencedTime += 2f * Time.deltaTime;

		bias += change;
		bias = Mathf.Clamp(bias, -1f, 1f);
	}

	void Update()
	{
		renderer.material.color = Color.Lerp(Color.red, Color.blue, Mathf.Clamp01((bias + 1f) / 2f));

		_influencedTime -= Time.deltaTime;
		_influencedTime = Mathf.Max(0, _influencedTime);
		//if(_influencedTime > 0.5f)
		//	die();

		//========================================//

		const float dangerZone = 0.6f;

		var size =  GameSession.inst.ground.localScale.x * 0.5f;
		float danger = Mathf.Clamp01((Mathf.Abs(transform.position.x) - size * dangerZone) / (size * (1f-dangerZone)));
		if(bias * transform.position.x > 0f) danger = 0f;
		transform.localScale = Vector3.Lerp(_initialScale, _initialScale * 2.5f, danger);
	}

	int getGridIndex()
	{
		return GameSession.inst.getGridIndex(transform.position);
	}
}