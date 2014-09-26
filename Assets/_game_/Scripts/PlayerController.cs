using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public enum PlayerPowers
{
	Good = 0,
	Evil = 1
}

public class PlayerController : MonoBehaviour
{
	public float conversionSpeed = 1f;
	public Transform negativeInfluencer;
	public Transform positiveInfluencer;
	public float influenceRadius = 3f;

	private List<Transform> _influencer = new List<Transform>();
	private Vector3[] actionPos = new Vector3[2];

	void Start()
	{
		_influencer.Add(Instantiate(positiveInfluencer) as Transform);
		_influencer.Add(Instantiate(negativeInfluencer) as Transform);

		for(int i = 0; i < _influencer.Count; ++i) 
			_influencer[i].localScale = Vector3.one * influenceRadius;
	}

	void Update()
	{
		//get gaze point form datastreem (only if info valid and eyeX there!)
		if(EyeXController.inst.isDataAvailable())
		{
			//mouse position is first input
			actionPos[0] = Input.mousePosition;

			var tmp = actionPos[1] = EyeXController.inst.getGazePointScreenCoords();
			actionPos[1] = new Vector3(tmp.x, tmp.y, 0f);
		}
		else
		{
			//alternative control scheme
			if(Input.GetKey(KeyCode.LeftControl))
				actionPos[0] = Input.mousePosition;
			else if(Input.GetKey(KeyCode.Space))
				actionPos[1] = Input.mousePosition;
		}

		//========================================//

		var entities = GameSession.inst.getEntities();

		for(int i = 0; i < 2; ++i)
		{
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
	}
}
