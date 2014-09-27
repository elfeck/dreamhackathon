using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class RandomEvents : MonoBehaviour {

    [SerializeField] Vector2 eventDistribution;
    [SerializeField] bool speedUpEnabled;
    [SerializeField] Vector2 speedUpMultiplier;
    [SerializeField] Vector2 speedUpDuration;
    [SerializeField] bool spawnRateEnabled;
    [SerializeField] Vector2 spawnRateMultiplier;
    [SerializeField] Vector2 spawnRateDuration;
    [SerializeField] bool colorBombEnabled;
    [SerializeField] GameObject colorBombEffect;
    [SerializeField] Vector2 colorBombRadius;

    float timer;
    //bool eventOngoing;
    int[] sides = new int[] { -1, 1, 2 };

    List<IEnumerator> allEvents;

    void Awake()
    {
        allEvents = new List<IEnumerator>() 
        {
            SpeedUp(sides[Random.Range(0, sides.Length)]),
            SpawnRateUp(),
            ColorBomb(),
            ColorBomb(),
            ColorBomb()
        };
    }

	// Use this for initialization
	void Start () 
    {
        StartCoroutine(PickEvent(5f));
	}
	
	// Update is called once per frame
	void Update () {
	    if(Input.GetKeyDown(KeyCode.Alpha7))
            StartCoroutine(SpeedUp(sides[Random.Range(0, sides.Length)]));

        if (Input.GetKeyDown(KeyCode.Alpha8))
            StartCoroutine(SpawnRateUp());

        if (Input.GetKeyDown(KeyCode.Alpha9))
            StartCoroutine(ColorBomb());
	}

    IEnumerator PickEvent(float _updateFreq)
    {
        float _timer = 0;

        for (; ; )
        {
            _timer += _updateFreq;

            if (_timer > Random.Range(eventDistribution.x, eventDistribution.y))
            {
                StartCoroutine(allEvents[Random.Range(0, allEvents.Count)]);
                _timer = 0;
            }

            yield return new WaitForSeconds(_updateFreq);
        }
    }


    /// <param name="_sideToAffect">Has to be -1, 1 or 2. 2 means "both sides"</param>
    IEnumerator SpeedUp(int _sideToAffect)
    {
        if(speedUpEnabled)
        {
            timer = 0;
            float _duration = Random.Range(speedUpDuration.x, speedUpDuration.y);
            float _speed = Random.Range(speedUpMultiplier.x, speedUpMultiplier.y);

            List<Entity> _affected = new List<Entity>();

            if (_sideToAffect == -1 || _sideToAffect == 1)
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
        }
    }
    IEnumerator SpawnRateUp()
    {
        if(spawnRateEnabled)
        {
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
        } 
    }
    /// <param name="_sideToBecome">Has to be -1 or 1.</param>
    IEnumerator ColorBomb()
    {
        if(colorBombEnabled)
        {
            float _radius = Random.Range(colorBombRadius.x, colorBombRadius.y);

            GameObject _ground = GameObject.Find("obj_Ground");
            Vector3 _pos = new Vector3
            (
                Random.Range(_ground.transform.position.x - _ground.transform.localScale.x / 4f, _ground.transform.position.x + _ground.transform.localScale.x / 4f),
                _ground.transform.position.y,
                Random.Range(_ground.transform.position.z - _ground.transform.localScale.z / 4f, _ground.transform.position.z + _ground.transform.localScale.z / 4f)
            );

            foreach (Entity _ent in Entity.allEntities)
            {
                if (Vector3.Distance(_ent.transform.position, _pos) < Mathf.Sqrt(_radius))
                {
                    _ent.applyBias(Mathf.Sign(_ent.bias) == 1 ? -2 : 2);
                }
            }

            //Play particle system
            if (colorBombEffect) ObjectPoolController.Instantiate(colorBombEffect, new Vector3(_pos.x, _pos.y + 1, _pos.z), colorBombEffect.transform.rotation);

            yield return null;
        }
    }
}
