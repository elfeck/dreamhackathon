using UnityEngine;
using System.Collections;
using System.Collections.Generic;

[RequireComponent(typeof(Camera))]
public class CameraEffects : SASMonoBehaviour
{
	public enum Type {SHAKE, BLUR, FOV, BLEND};
	
	struct Effect
	{
		public CameraEffects.Type type;
		public Vector3 pos;
		public float maxAmplitude;
		public float range;
		public float startTime;
		public float duration;
		public Vector4 parameter; //this parameter is used SOMEHOW individually by each effect
		public Color color; //this parameter is used SOMEHOW individually by each effect
		public Camera visibleTo; //if != null, then this effect is shown to the one camera only
	}
	static private List<Effect> _effectsList = new List<Effect>();
	
	//===============================================================//

	/// <summary>
	/// Registers a camera shaker at position pos and a certain maximal amplitude. The shake is applied
	/// for duration time to all cameras that are within the sphere of radius range.
	/// </summary>
	static public void registerShaker(Vector3 position, float amplitude, float range, float duration)
	{
		CameraEffects.Effect info = new CameraEffects.Effect();
		info.pos = position;
		info.maxAmplitude = amplitude;
		info.range = range;
		info.duration = duration;
		info.startTime = Time.realtimeSinceStartup;
		info.type = Type.SHAKE;
		info.parameter = Vector4.zero;
		info.color = Color.white;
		_effectsList.Add(info);
	}
	
	//===============================================================//

	/// <summary>
	/// Registers a camera blur at position pos and a certain maximal amplitude. The blur is applied
	/// for duration time to all cameras that are within the sphere of radius range.
	/// </summary>
	static public void registerBlur(Vector3 pos, float amplitude, float range, float duration)
	{
		CameraEffects.Effect info = new CameraEffects.Effect();
		info.pos = pos;
		info.maxAmplitude = amplitude;
		info.range = range;
		info.duration = duration;
		info.startTime = Time.realtimeSinceStartup;
		info.type = Type.BLUR;
		info.visibleTo = null;
		info.parameter = Vector4.zero;
		info.color = Color.white;
		_effectsList.Add(info);
	}

	/// <summary>
	/// Registers a camera blur of amplitude for duration seconds that is applied to all cameras or just the defined one.
	/// </summary>
	static public void registerBlur(float amplitude, float duration, Camera onlyVisibleTo)
	{
		CameraEffects.Effect info = new CameraEffects.Effect();
		info.pos = Vector3.zero; //not used when range == 0f
		info.maxAmplitude = amplitude;
		info.range = 0f; //dont use any position!
		info.duration = duration;
		info.startTime = Time.realtimeSinceStartup;
		info.type = Type.BLUR;
		info.visibleTo = onlyVisibleTo;
		info.parameter = Vector4.zero;
		info.color = Color.white;
		_effectsList.Add(info);
	}
	
	//===============================================================//

	/// <summary>
	/// Registers a field of view (FOV) change for duration seconds that changes towards the new FOV in fadeInTime
	/// and fades it out again in fadeOutTime.
	/// </summary>
	static public void registerFOVChange(float targetFOV, bool relativeFOV, float duration,
		float fadeInTime, float fadeOutTime, Camera onlyVisibleTo)
	{
		CameraEffects.Effect info = new CameraEffects.Effect();
		info.pos = Vector3.zero; //not used when range == 0f
		info.maxAmplitude = targetFOV;
		info.range = 0f; //dont use any position!
		info.duration = duration + fadeInTime + fadeOutTime;
		info.startTime = Time.realtimeSinceStartup;
		info.type = Type.FOV;
		info.visibleTo = onlyVisibleTo;
		info.parameter = new Vector4(fadeInTime, fadeOutTime, relativeFOV ? 1f : 0f, 0f);
		info.color = Color.white;
		_effectsList.Add(info);
	}
	
	//===============================================================//

	/// <summary>
	/// Registers a full screen blend for duration seconds that changes towards the new FOV in fadeInTime
	/// and fades it out again in fadeOutTime.
	/// </summary>
	static public void registerFullScreenBlend(Vector3 position, float range, float targetBlendFactor, float duration,
		float fadeInTime, float fadeOutTime, Color blendColor, Camera onlyVisibleTo)
	{
		CameraEffects.Effect info = new CameraEffects.Effect();
		info.pos = position;
		info.maxAmplitude = targetBlendFactor;
		info.range = range;
		info.duration = duration + fadeInTime + fadeOutTime;
		info.startTime = Time.realtimeSinceStartup;
		info.type = Type.BLEND;
		info.visibleTo = onlyVisibleTo;
		info.parameter = new Vector4(fadeInTime, fadeOutTime, 0f, 0f);
		info.color = blendColor;
		_effectsList.Add(info);
	}

	/// <summary>
	/// Registers a full screen blend for duration seconds that changes towards the new blend in fadeInTime
	/// and fades it out again in fadeOutTime.
	/// </summary>
	static public void registerFullScreenBlend(float targetBlendFactor, float duration,
		float fadeInTime, float fadeOutTime, Color blendColor, Camera onlyVisibleTo)
	{
		CameraEffects.Effect info = new CameraEffects.Effect();
		info.pos = Vector3.zero;
		info.maxAmplitude = targetBlendFactor;
		info.range = 0f; //dont use any position!
		info.duration = duration + fadeInTime + fadeOutTime;
		info.startTime = Time.realtimeSinceStartup;
		info.type = Type.BLEND;
		info.visibleTo = onlyVisibleTo;
		info.parameter = new Vector4(fadeInTime, fadeOutTime, 0f, 0f);
		info.color = blendColor;
		_effectsList.Add(info);
	}
	
	//~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~//
	
	public float maxCameraShakeAmplitude = 0.03f;
	public float maxBlurAmplitude = 5f;
	
	private Camera _camera;
	private BlurEffect _blurEffect;
	private Transform _trans;
	private float _initialFOV;
	private TextureBlender _textureBlender;
	
	void Awake()
	{
		_trans = transform;
		_camera = GetComponent<Camera>();
		_blurEffect = _camera.GetComponent<BlurEffect>();
		_initialFOV = _camera.fieldOfView;

		//add texture blender to the camera for the full screen blend effects
		_textureBlender = gameObject.AddComponent<TextureBlender>();
		_textureBlender.blendFactor = 0f;
		_textureBlender.blendFactorSmoothing = 0f;
	}
	
	void OnPreCull()
	{
		float blurAmpl = 0f;
		float shakingAmpl = 0f;
		Color blendColor = Color.black;
		float blendFactor = 0f;
		
		//consider all camera effects and apply them
		for(int i = CameraEffects._effectsList.Count - 1; i >= 0; --i)
		{
			Effect effect = CameraEffects._effectsList[i];
			if(effect.visibleTo != null && effect.visibleTo != _camera) continue;
			float dt = Time.realtimeSinceStartup - effect.startTime;
			
			if(effect.type == Type.SHAKE)
			{
				float dist = Vector3.Distance(effect.pos, _trans.position);
				float timeDamping = effect.duration > 0f ? (effect.duration - dt) / effect.duration : 1f;
				float shake = effect.maxAmplitude *	Mathf.Clamp01((effect.range - dist) / effect.range * timeDamping);
				shakingAmpl += shake;
			}
			else if(effect.type == Type.BLUR)
			{
				float amplitude = 0f;
				if(effect.range <= 0f) amplitude = effect.maxAmplitude;
				else amplitude = effect.maxAmplitude * (1f - Mathf.Clamp01(Vector3.Distance(effect.pos, _trans.position) / effect.range));

				//blur with the given amplitude
				blurAmpl += amplitude * Mathf.Clamp01((effect.duration - dt) / effect.duration);
			}
			else if(effect.type == Type.FOV)
			{
				float targetFOV = effect.parameter[2] > 0.5f ? _initialFOV + effect.maxAmplitude : effect.maxAmplitude;
				if(targetFOV > 90f)
				{
					Debug.LogWarning("CameraEffect exceeded maximum FOV of 90deg! Clamping.");
					targetFOV = 90f;
				}
				//the FOV change is set directly here, because only one should be active anyways
				if(dt < effect.parameter[0])
					_camera.fieldOfView = Mathf.Lerp(_initialFOV, targetFOV, Mathf.Clamp01(dt / effect.parameter[0]));
				else if(dt > effect.duration - effect.parameter[1])
					_camera.fieldOfView = Mathf.Lerp(_initialFOV, targetFOV, Mathf.Clamp01((effect.duration - dt) / effect.parameter[1]));
			}
			else if(effect.type == Type.BLEND)
			{
				float factor = effect.maxAmplitude;
				if(effect.range > 0f) factor *= (1f - Mathf.Clamp01(Vector3.Distance(effect.pos, _trans.position) / effect.range));

				if(dt < effect.parameter[0]) factor *= Mathf.Clamp01(dt / effect.parameter[0]);
				else if(dt > effect.duration - effect.parameter[1]) factor *= Mathf.Clamp01((effect.duration - dt) / effect.parameter[1]);
				blendFactor += factor;
				blendColor += factor * effect.color;
			}
			
			//delete this effect, if it's lifetime is consumed (it may go on forever too!)
			if(effect.duration > 0f && effect.duration < Time.realtimeSinceStartup - effect.startTime)
				CameraEffects._effectsList.RemoveAt(i);
		}
		
		shakingAmpl = Mathf.Clamp(shakingAmpl, 0f, maxCameraShakeAmplitude);
		blurAmpl = Mathf.Clamp(blurAmpl, 0f, maxBlurAmplitude);
		blendFactor = Mathf.Clamp01(blendFactor);
		
		//-------------------------------------//
		
		//apply camera shake
		_trans.position += Random.onUnitSphere * shakingAmpl;
		
		//apply blur
		if(_blurEffect != null)
		{
			if(blurAmpl > 0f)
			{
				_blurEffect.enabled = true;
				_blurEffect.blurSpread = blurAmpl;
			}
			else _blurEffect.enabled = false;
		}

		//apply fullscreen blend
		if(_textureBlender != null)
		{
			_textureBlender.blendFactor = blendFactor;
			_textureBlender.blendingColor = blendColor;
		}
	}
}
