using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class RandomEvents : MonoBehaviour {

    public float eventDistribution;
    public Vector2 speedUpMultiplier;
    public Vector2 speedUpDuration;
    public Vector2 spawnRateMultiplier;
    public Vector2 spawnRateDuration;
    public Vector2 colorBombRadius;

    float timer;
    bool eventOngoing;
    int[] sides = new int[] { -1, 1, 2 };

    List<IEnumerator> allEvents;

    void Awake()
    {
        allEvents = new List<IEnumerator>() 
        {
            SpeedUp(sides[Random.Range(0, sides.Length)]),
        };
    }

	// Use this for initialization
	void Start () 
    {
        StartCoroutine(PickEvent(5f));
	}
	
	// Update is called once per frame
	void Update () {
	    if(Input.GetKeyDown(KeyCode.Space))
            StartCoroutine(SpeedUp(sides[Random.Range(0, sides.Length)]));

        if (Input.GetKeyDown(KeyCode.U))
            StartCoroutine(SpawnRateUp());
	}

    IEnumerator PickEvent(float _updateFreq)
    {
        for (; ; )
        {
            if (eventOngoing == false)
            {
                int _yesOrNo = Random.Range(-1, 1);
                if (Mathf.Sign(_yesOrNo) == 1)
                {
                    StartCoroutine(allEvents[Random.Range(0, allEvents.Count)]);
                }
            }

            yield return new WaitForSeconds(_updateFreq);
        }
    }


    /// <param name="_sideToAffect">Has to be -1, 1 or 2. 2 means "both sides"</param>
    IEnumerator SpeedUp(int _sideToAffect)
    {
        eventOngoing = true;

        timer = 0;
        float _duration = Random.Range(speedUpDuration.x, speedUpDuration.y);
        float _speed = Random.Range(speedUpMultiplier.x, speedUpMultiplier.y);

        List<Entity> _affected = new List<Entity>();

        if(_sideToAffect == -1 || _sideToAffect == 1)
        {
            foreach (Entity _ent in Entity.allEntities)
            {
                if (Mathf.Sign(_ent.bias) == _sideToAffect)
                {
                    _ent.movement.speed *= _speed;
                    _affected.Add(_ent);
                }
            }
        }

        else //Both sides
        {
            foreach (Entity _ent in Entity.allEntities)
            {
                _ent.movement.speed *= _speed;
                _affected.Add(_ent); //Always set so as not to affect later spawned entities
            }
        }

        while (timer < _duration)
        {
            timer += Time.deltaTime;
            yield return null;
        }

        foreach (Entity _ent in _affected)
        {
            _ent.movement.speed /= _speed;
        }

        eventOngoing = false;
    }
    IEnumerator SpawnRateUp()
    {
        eventOngoing = true;

        timer = 0;
        float _duration = Random.Range(spawnRateDuration.x, spawnRateDuration.y);
        float _multiplier = Random.Range(spawnRateMultiplier.x, spawnRateMultiplier.y);

        GameSession.inst.spawnRate *= _multiplier;

        while (timer < _duration)
        {
            timer += Time.deltaTime;
            yield return null;
        }

        GameSession.inst.spawnRate /= _multiplier;

        eventOngoing = false;
    }
    /// <param name="_sideToBecome">Has to be -1 or 1.</param>
    //IEnumerator ColorBomb(int _sideToBecome)
    //{
    //    float _radius = Random.Range(colorBombRadius.x, colorBombRadius.y);
    //    float _curSize = 0;

    //    while(_curSize < _radius)
    //    {
            
    //    }

    //    GameObject _ground = GameObject.Find("obj_Ground");
    //    Vector3 _pos = new Vector3
    //    (
    //        _ground.transform.position.x + Random.Range(0, _ground.transform.localScale.x), 
    //        _ground.transform.position.y, 
    //        _ground.transform.position.z + Random.Range(0, _ground.transform.localScale.z)
    //    );

    //    foreach(Entity _ent in Entity.allEntities)
    //    {
    //        if (Vector3.SqrMagnitude(_ent.transform.position - _pos) < Mathf.Sqrt(_radius))
    //        {
    //            _ent.applyBias(_sideToBecome);
    //        }
    //    }

    //    //Play particle system
    //}
}
