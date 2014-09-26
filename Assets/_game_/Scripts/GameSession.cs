using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class GameSession : MonoBehaviour
{
	public GameObject entity;
	public int totalCount = 100;

	private List<Entity> _entities = new List<Entity>();

	void Awake()
	{
		for(int i = 0; i < totalCount; ++i)
		{
			var go = Instantiate(entity) as GameObject;
			go.transform.position = HelperFunctions.RandomVector3InsideUnitCircle() * 15f;

			var e = go.GetComponent<Entity>();
			_entities.Add(e);
		}
	}
}
