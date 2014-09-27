using UnityEngine;
using System.Collections;

public class Movement : MonoBehaviour {

	public float speed;
	Entity thisEntity;

	[HideInInspector] public bool pushed;
	[HideInInspector] public Vector3 pushDir;

	void Awake()
	{
		thisEntity = GetComponent<Entity>();
	}

	void Update()
	{
		//if (pushed) Debug.Log(pushDir);

		//"Walk"
		//if (pushed) transform.position += speed * Time.deltaTime * pushDir;

		//else transform.position += speed * Time.deltaTime * Vector3.left * Mathf.Sign(thisEntity.bias);
		transform.position += speed * Time.deltaTime * Vector3.left * thisEntity.bias;
	}
}
