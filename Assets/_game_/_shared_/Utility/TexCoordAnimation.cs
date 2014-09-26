using UnityEngine;
using System.Collections;
using System.Collections.Generic;

[AddComponentMenu("stillalive-studios/Effects/Texcoord animation")]
public class TexCoordAnimation : SASMonoBehaviour
{
	/// <summary>
	/// The tex coord X component speed
	/// </summary>
	public float offsetXSpeed = 0f;
	/// <summary>
	/// If not 0 the offset speed may vary by this value.
	/// </summary>
	public float speedVariationX = 0f;
	/// <summary>
	/// The tex coord Y component speed
	/// </summary>
	public float offsetYSpeed = 0f;
	/// <summary>
	/// If not 0 the offset speed may vary by this value.
	/// </summary>
	public float speedVariationY = 0f;
	/// <summary>
	/// Has only an effect if speedVariation is not 0. Will change the speed in this time interval
	/// </summary>
	public float speedChangeInterval = -1f;
	/// <summary>
	/// The texture names for which the texcoords should be animated
	/// </summary>
	public List<string> textureNames = new List<string> {"_MainTex"};
	
	private Material _material;
	private Renderer _renderer;
	private Vector2 _lastOffsetSpeed;
	private Vector2 _nextOffsetSpeed;
	private float _lastSpeedChangeTime = 0f;
	
	void Awake()
	{
		_renderer = renderer;
		_material = renderer.material;
		_lastOffsetSpeed = _nextOffsetSpeed = new Vector2(offsetXSpeed, offsetYSpeed);
	}
	void OnEnable()
	{
		if(speedChangeInterval > 0f) InvokeRepeating(changeSpeed, Random.Range(0f, speedChangeInterval), speedChangeInterval);
	}

	void changeSpeed()
	{
		_nextOffsetSpeed.x = offsetXSpeed + Random.Range(-speedVariationX, speedVariationX);
		_nextOffsetSpeed.y = offsetYSpeed + Random.Range(-speedVariationY, speedVariationY);
		_lastSpeedChangeTime = Time.time;
	}
	
	void Update()
	{
		if(!_renderer.isVisible) return;

		//linearily interpolate between the last and the next offsetspeed
		var curr = Vector2.Lerp(_lastOffsetSpeed, _nextOffsetSpeed, Mathf.Clamp01(Time.time - _lastSpeedChangeTime / speedChangeInterval));

		foreach(string tex in textureNames)
		{
			Vector2 offset = _material.GetTextureOffset(tex);
			offset += Time.deltaTime * curr;
			_material.SetTextureOffset(tex, offset);
		}
	}
}
