using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class Entity : MonoBehaviour
{
	static private List<HashSet<Entity>> _grid;

	public float bias = 0f;
	private int _currGridIndex = -1;

	void Awake()
	{
		bias = Random.Range(-1f, 1f);
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

		_currGridIndex = getGridIndex();
		_grid[_currGridIndex].Add(this);

		while(true)
		{
			//update grid position
			var index = getGridIndex();
			if(_currGridIndex != index)
			{
				_grid[_currGridIndex].Remove(this);
				_currGridIndex = index;
				_grid[_currGridIndex].Add(this);
			}

			yield return new WaitForSeconds(Random.Range(0.5f, 1f) * 0.25f);
		}
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
