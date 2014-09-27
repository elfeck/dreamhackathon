using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class Entity : MonoBehaviour
{
	static private List<HashSet<Entity>> _grid;
	static public List<Entity> allEntities = new List<Entity>();

	public float bias = 0f;
	private int _currGridIndex = -1;
	public float cellInfluenceFactor = 0.2f;

	public float reach;
	[HideInInspector]
	public Movement movement;

	void Awake()
	{
		movement = GetComponent<Movement>();

		bias = Mathf.Sign(Random.Range(-1f, 1f));

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

	void die()
	{
		_grid[_currGridIndex].Remove(this);
		Destroy(gameObject);
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

	int getGridIndex()
	{
		return GameSession.inst.getGridIndex(transform.position);
	}
}