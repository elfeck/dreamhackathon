using UnityEngine;
using System.Collections;

public class Movement : MonoBehaviour {

	public float speed;
	bool follow;
	bool walk;
	bool walkAround = true;
	bool formationWalk = true;

	static GameObject ground;
	Entity thisEntity;
	Entity targetEntity;
	Vector3 targetPos;

	void Awake()
	{
		thisEntity = GetComponent<Entity>();
		ground = GameObject.Find("obj_Ground");
		speed = speed / 10;
	}

	void Start () 
	{
		//WalkToRandomPos();
		WalkToOtherSide();
		StartCoroutine(CheckForReach(0.3f));
	}

	void Update()
	{
		//"Walk"
		if (walk && targetPos != transform.position)
			transform.position += speed * Time.deltaTime * (targetPos - transform.position).normalized;

		//"Seamless map"
		CheckForBorder();
	}

	void OnReach()
	{
		if (walkAround) WalkToRandomPos();

		else
		{
			walk = false;
			follow = false;

			if (targetEntity)
			{
				targetEntity.movement.walk = false;
				targetEntity.InteractWithTarget();

				targetEntity = null;
			}
		}
	}

	void CheckForBorder()
	{
		if (transform.position.x > (ground.transform.localScale.x / 2) || transform.position.x < (-(ground.transform.localScale.x / 2)))
		{
			transform.position = new Vector3(transform.position.x * -1, transform.position.y, transform.position.z);


			targetPos.x -= Mathf.Sign(transform.position.x) * ground.transform.localScale.x;
		}

		if (transform.position.z > (ground.transform.localScale.z / 2) || transform.position.z < ((ground.transform.localScale.z / 2) * -1))
		{
			transform.position = new Vector3(transform.position.x, transform.position.y, transform.position.z * -1);

			targetPos.z -= Mathf.Sign(transform.position.z) * ground.transform.localScale.z;
		}
	}

	void WalkToRandomPos(float radius = 10f)
	{
		targetPos = new Vector3(transform.position.x + Random.Range(-radius, radius), transform.position.y, transform.position.z + Random.Range(-radius, radius));
		
		walk = true;
	}

	void WalkToOtherSide()
	{
		targetPos = transform.position;
		targetPos.x = -Mathf.Sign(transform.position.x) * ground.transform.localScale.x * Random.Range(0.3f, 0.5f);

		walk = true;
	}

	public void StartFollowing(Entity _entity)
	{
		targetEntity = _entity;

		follow = true;
		walk = true;
	}

	IEnumerator CheckForReach(float _updateFreq)
	{
		for (; ; )
		{
			if(follow) targetPos = targetEntity.transform.position;

			if (Vector3.Distance(transform.position, targetPos) <= thisEntity.reach)
			{
				OnReach();
			}

			yield return new WaitForSeconds(_updateFreq);
		}
	}
}
