using UnityEngine;
using System.Collections;

[AddComponentMenu("stillalive-studios/Particles/Emission toggle")]
public class ParticleEmissionToggle : SASMonoBehaviour
{
	public bool initialEmissionOn = true;
	public bool onDestroyEmissionOn = false;
	public float[] toggleEmissionTimesSorted;
	
	private ParticleSystem _sys;
	private float _startTime;
	private int _currToggle = 0;
	
	//can be pooled too!
	void Start()
	{
		_sys = particleSystem;
		_sys.enableEmission = initialEmissionOn;
		_startTime = Time.time;
		_currToggle = 0;
	}
	
	void onDestroy()
	{
		_sys.enableEmission = onDestroyEmissionOn;
	}
	
	void Update()
	{
		if(_currToggle < toggleEmissionTimesSorted.Length && toggleEmissionTimesSorted[_currToggle] + _startTime < Time.time)
		{
			toggle();
			_currToggle++;
		}
	}
	
	void toggle()
	{
		_sys.enableEmission = !_sys.enableEmission;
	}
}
