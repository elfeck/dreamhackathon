using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public enum PlayerPowers
{
	Good = 0,
	Evil = 1
}

public class PlayerController : SASSingleton<PlayerController>
{
	public float conversionSpeed = 1f;
	public Transform negativeInfluencer;
	public Transform positiveInfluencer;
	public float influenceRadius = 3f;

	private Vector3 _camForward;
	private Vector3 _camPos;

	public int cursors = 2;

	private List<Transform> _influencer = new List<Transform>();
	private Vector3[] actionPos = new Vector3[2];
	//private bool _useEyetracking = false;

	void Start()
	{
		_influencer.Add(Instantiate(positiveInfluencer) as Transform);
		_influencer.Add(Instantiate(negativeInfluencer) as Transform);

		for(int i = 0; i < _influencer.Count; ++i) 
			_influencer[i].localScale = Vector3.one * influenceRadius;

		actionPos[0] = actionPos[1] = new Vector3(Screen.width, Screen.height, 0f) * 0.5f;

		_camForward = Camera.main.transform.forward;
		_camPos = Camera.main.transform.position;
	}

	void Update()
	{
		Camera.main.transform.forward = _camForward;
		Camera.main.transform.position = _camPos;

		actionPos[0] = Input.mousePosition;
		actionPos[1].x = Screen.width - actionPos[0].x;
		actionPos[1].y = Screen.height - actionPos[0].y;

		//========================================//

		var entities = Entity.allEntities;

		for(int i = 0; i < 2; ++i)
		{
			_influencer[i].gameObject.SetActive(i < cursors);
			if(i >= cursors) continue;

			var ray = Camera.main.ScreenPointToRay(actionPos[(int)i]);
			RaycastHit hitInfo;
			if(!Physics.Raycast(ray, out hitInfo, 100f, Layers.Start().Add("Default"))) continue;

			foreach(var e in entities)
			{
				if(Vector3.SqrMagnitude(e.transform.position - hitInfo.point) > influenceRadius * influenceRadius) continue;

				float bias = i == (int)PlayerPowers.Good ? 1f : -1f;
				e.applyBias(bias * Time.deltaTime * conversionSpeed);
			}

			_influencer[i].position = hitInfo.point;
		}

		//========================================//

		//if(Input.GetKeyDown(KeyCode.Space))
		//{
		//	//reset!
		//	GameSession.inst.reset();
		//}

		//========================================//

		if(Input.GetKeyDown(KeyCode.Escape))
			Application.LoadLevel("Menu");
	}
}
