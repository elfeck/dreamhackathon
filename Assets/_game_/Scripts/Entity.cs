using UnityEngine;
using System.Collections;

public class Entity : MonoBehaviour
{
    public float reach;
    [HideInInspector] public float bias = 0f;

    [HideInInspector] public Movement movement;

    void Awake()
    { 
        movement = GetComponent<Movement>();

        bias = Random.Range(-1f, 1f);

        //"Random spawn"
        Vector3 _tmp = new Vector3();
        _tmp.x = Random.Range((GameObject.Find("obj_Ground").transform.localScale.x / 2) * -1, (GameObject.Find("obj_Ground").transform.localScale.x / 2));
        _tmp.z = Random.Range((GameObject.Find("obj_Ground").transform.localScale.z / 2) * -1, (GameObject.Find("obj_Ground").transform.localScale.z / 2));
        transform.position = _tmp;
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

    public void InteractWithTarget()
    {
        if (Random.Range(-10, 10) > 0)
        {
            //Do good
        }

        else 
        { 
            //Do evil
        }
    }
}
