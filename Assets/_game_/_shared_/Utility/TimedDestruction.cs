using UnityEngine;
using System.Collections;

[AddComponentMenu("stillalive-studios/Other/Timed self destruction")]
public class TimedDestruction : SASMonoBehaviour
{
	public float destroyAfterTime = 5f;
	public AnimationClip destroyAfterAnimation;
	public float delayAnimationStart = 0f;
	
	void Start()
	{
		init();
	}
	
	void init()
	{
		float t = 0f;
		if(destroyAfterAnimation)
		{
			t = destroyAfterAnimation.length + 0.2f + delayAnimationStart;
			Invoke(startAnimation, delayAnimationStart);
		}
		
		Invoke("destroyNow", Mathf.Max(t, destroyAfterTime));
	}
	
	public void reset()
	{
		CancelInvoke(destroyNow);
		init();
	}
	
	void destroyNow()
	{
		ObjectPoolController.Destroy(gameObject);
	}
	
	void startAnimation()
	{
		if(animation)
		{
			animation.Play(destroyAfterAnimation.name);
		}
	}
}
