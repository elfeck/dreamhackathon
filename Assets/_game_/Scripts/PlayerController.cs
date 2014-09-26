using UnityEngine;
using System.Collections;

public enum PlayerPowers
{
	Good = 0,
	Evil = 1
}

public class PlayerController : MonoBehaviour
{
	private Vector3[] actionPos = new Vector3[2];

	void Update()
	{
		//mouse position is first input
		actionPos[0] = Input.mousePosition;

		//get gaze point form datastreem (only if info valid and eyeX there!)
		actionPos[1] = Vector3.zero;
		if(EyeXController.inst.isDataAvailable())
		{
			var tmp = actionPos[1] = EyeXController.inst.getGazePointScreenCoords();
			actionPos[1] = new Vector3(tmp.x, tmp.y, 0f);
		}
		else
		{
			//DEBUG: alternative control scheme
			//TODO
		}

		//========================================//

		const float radius = 3f;
		Color[] colors = { Color.blue, Color.red };
		for(int val = 0; val < 2; ++val)
		{
			var ray = Camera.main.ScreenPointToRay(actionPos[(int)val]);
			var hits = Physics.SphereCastAll(ray, radius, 50f, Layers.Start().Add("Interactibles"));

			foreach(var info in hits)
			{
				var entity = info.collider.GetComponent<Entity>();
				if(entity == null) continue;

				float bias = val == (int)PlayerPowers.Good ? 1f : -1f;
				entity.applyBias(bias * Time.deltaTime);
			}

			RaycastHit hitInfo;
			if(Physics.Raycast(ray, out hitInfo, 100f, Layers.Start().Add("Default")))
			{
				SASDebug.inst.DrawSphere(hitInfo.point, radius, colors[val]);
			}
		}
	}
}
