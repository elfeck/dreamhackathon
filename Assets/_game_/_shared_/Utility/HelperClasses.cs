using UnityEngine;
using System.Collections;
using System.Collections.Generic;
#if UNITY_EDITOR
using UnityEditor;
#endif

//---------------------------------------------------------------//

static public class HelperFunctions
{
	static public Quaternion emptyQuaternion = new Quaternion(0f,0f,0f,0f);
	
	static public Quaternion multiplyQuaternionScalar(Quaternion q, float scalar)
	{
		return new Quaternion(q.x * scalar, q.y * scalar, q.z * scalar, q.w * scalar);
	}
	static public Quaternion addQuaternions(Quaternion a, Quaternion b)
	{
		return new Quaternion(a.x + b.x, a.y + b.y, a.z + b.z, a.w + b.w);
	}
	static public Quaternion conjugateQuaternion(Quaternion q)
	{
		return new Quaternion(-q.x, -q.y, -q.z, q.w);
	}
	static public Quaternion integrateQuaternion(Quaternion q, Vector3 angVel, float dt)
	{
		//q += dt * 0.5 * q * w
		Quaternion w = new Quaternion(-angVel.x, -angVel.y, -angVel.z, 0f);
		Quaternion derivative = w * q;
		derivative = multiplyQuaternionScalar(derivative, -0.5f * dt);
		w = addQuaternions(derivative, q);
		float norm = Mathf.Sqrt(w.x * w.x + w.y * w.y + w.z * w.z + w.w * w.w);
		w = multiplyQuaternionScalar(w, 1f / norm);
		return w;
	}
	
	/// <summary>
	/// Computes a rotation matrix from a certain forward direction to another forward-direction
	/// </summary>
	/// <param name="maxAngle">
	/// Limits the angle of the given rotation to this maximum value. If negative, then no limit is applied.
	/// Angle is given in DEG.
	/// </param>
	static public Quaternion fromToRotationLimited(Vector3 f, Vector3 t, float maxAngle)
	{
		float angle = Vector3.Angle(f, t); //this is always positive
		if(Mathf.Abs(angle) < 0.0001f) return Quaternion.identity;
		//limit angle
		if(maxAngle >= 0f && angle > maxAngle) angle = maxAngle;
		
		Vector3 rotationAxis = Vector3.Normalize(Vector3.Cross(f, t));
		if(rotationAxis == Vector3.zero)
		{
			//f and t are parallel!
			rotationAxis = (f == Vector3.up ? Vector3.right : Vector3.up);

			//check if the rotation should be in one or the other direction
			//this is not needed if the rotation was found by the cross product
			if(Vector3.Dot(Vector3.Cross(rotationAxis, f), t) < 0f)
				angle = -angle;
		}

		return Quaternion.AngleAxis(angle, rotationAxis);
	}
	
	static public Vector4 frac(Vector4 val)
	{
		return new Vector4(val.x - (float)((int)(val.x)), val.y - (float)((int)(val.y)),
		                   val.z - (float)((int)(val.z)), val.w - (float)((int)(val.w)));
	}
	
	//pack a float number into R8G8B8A8. val has to be in range (-1f, 1f). 1f and -1f are NOT ALLOWED
	static public Color packFloatIntoColor(float val)
	{
		Vector4 shift = new Vector4(256.0f * 256.0f * 256.0f, 256.0f * 256.0f, 256.0f, 1.0f);
		Vector4 mask = new Vector4(0.0f, 1.0f / 256.0f, 1.0f / 256.0f, 1.0f / 256.0f);
		Vector4 ret = frac(val * shift);
		ret.x -= ret.x * mask.x;
		ret.y -= ret.x * mask.y;
		ret.z -= ret.y * mask.z;
		ret.w -= ret.z * mask.w;
		return new Color(ret.w, ret.z, ret.y, ret.x);
	}
	//unpack a float number from R8G8B8A8
	static public float unpackFloatFromColor(Color val)
	{
		Vector4 shift = new Vector4(1f, 1f / 256f, 1f / (256f * 256f), 1f / (256f * 256f * 256f));
		return Vector4.Dot((Vector4)val, shift);
	}
	//pack a float number into R8G8B8A8. val has to be in range (0f, 1f). 1f is NOT ALLOWED
	static public Color EncodeFloatRGBA(float val)
	{
		Vector4 kEncodeMul = new Vector4(1.0f, 255.0f, 65025.0f, 160581375.0f);
		Vector4 enc = kEncodeMul * val;
		enc = frac(enc);
		float kEncodeBit = 1.0f / 255.0f;
		enc -= new Vector4(enc.y, enc.z, enc.w, 0f) * kEncodeBit;
		return enc;
	}
	static public float DecodeFloatRGBA(Color enc)
	{
		Vector4 kDecodeDot = new Vector4(1.0f, 1f/255.0f, 1f/65025f, 1f/160581375.0f);
		return Vector4.Dot((Vector4)(enc), kDecodeDot);
	}
	
	//encode uint into color32
	static public Color32 encodeUintColor32(uint val)
	{
		return new Color32((byte)(val), (byte)(val/256), (byte)(val/65536), (byte)(val/16777216));
	}
	static public uint decodeUintColor32(Color32 col)
	{
		return (uint)(col.r) + (uint)(col.g)*256 + (uint)(col.b)*65536 + (uint)(col.a)*16777216;
	}
	
	static public Vector3 vector3Product(Vector3 a, Vector3 b)
	{
		return new Vector3(a.x * b.x, a.y * b.y, a.z * b.z);
	}
	
	//Cartesian coords <=> spherical coords: phi [-PI/2,PI/2], theta [0,PI]
	static public void cartesian2spherical(Vector3 dir, out float radius, out float theta, out float phi)
	{
		radius = dir.magnitude;
		dir /= radius;
		theta = Mathf.Acos(dir.y);
		phi = Mathf.Atan2(dir.x, dir.z);
	}
	static public Vector3 spherical2cartesian(float r, float theta, float phi)
	{
		return new Vector3(r * Mathf.Sin(theta) * Mathf.Sin(phi), r * Mathf.Cos(theta), r * Mathf.Sin(theta) * Mathf.Cos(phi));
	}

	//-------------------------------------------------------------------------//
	
	/// <summary>
	/// Finds the first component of the gameObject implementing the asked interface
	/// </summary>
	/// <returns>
	/// The component implementing interface.
	/// </returns>
	static public T getComponentImplementingInterface<T>(GameObject obj) where T : class
	{
		return obj.GetComponent(typeof(T)) as T;
	}
	static public Component getComponentImplementingInterface(GameObject obj, System.Type T)
	{
		return obj.GetComponent(T);
	}

	/// <summary>
	/// Finds the first component of the gameObject or any of its children implementing the asked interface.
	/// </summary>
	static public T getComponentInChildrenImplementingInterface<T>(GameObject obj) where T : class
	{
		return obj.GetComponentInChildren(typeof(T)) as T;
	}
	/// <summary>
	/// Finds the first component of the gameObject or any of its children implementing the asked interface.
	/// </summary>
	static public Component getComponentInChildrenImplementingInterface(GameObject obj, System.Type T)
	{
		return obj.GetComponentInChildren(T);
	}

	/// <summary>
	/// Gets all the components on the gameObject implementing interface T.
	/// </summary>
	static public Component[] getComponentsImplementingInterface(GameObject obj, System.Type T)
	{
		return obj.GetComponents(T); //couldn't get this working with a cast to T[]
	}

	/// <summary>
	/// Gets all the components on the gameObject and its children implementing interface T.
	/// </summary>
	static public Component[] getComponentsInChildrenImplementingInterface(GameObject obj, System.Type T)
	{
		return obj.GetComponentsInChildren(T); //couldn't get this working with a cast to T[]
	}

	//-------------------------------------------------------------------------//
	
	/// <summary>
	/// Traverses the hierarchy from this object to the root and searches in each object for the component asked.
	/// The first occurence is returned immediately. If nothing can be found null is returned.
	/// </summary>
	/// <returns>
	/// A reference to the component found in parents.
	/// </returns>
	static public T getComponentInParents<T>(GameObject obj) where T : Component
	{
		Transform curr = obj.transform;
		while(curr)
		{
			T tmp = curr.GetComponent<T>();
			if(tmp) return tmp;
			curr = curr.parent;
		}
		return default(T);
	}
	
	/// <summary>
	/// Performs a Physics.Raycast but with the special support to ignore ONE ARBITRARY collider. If this collider
	/// is hit, then another raycast is performed to get the collision point behind this collider.
	/// </summary>
	/// <param name='ignoreCollider'>
	/// Specifies the collider to be ignored.
	/// </param>
	static public bool Raycast(Vector3 origin, Vector3 dir, out RaycastHit hitInfo, float range, int layerMask, Collider ignoreCollider)
	{
		List<Collider> ignoreList = new List<Collider>();
		ignoreList.Add(ignoreCollider);
		return Raycast(origin, dir, out hitInfo, range, layerMask, ignoreList);
	}
	static public bool Raycast(Vector3 origin, Vector3 dir, out RaycastHit hitInfo, float range, int layerMask, List<Collider> ignoreCollider)
	{
		return SphereCast(origin, 0f, dir, out hitInfo, range, layerMask, ignoreCollider);
	}

	static public bool SphereCast(Vector3 origin, float radius, Vector3 dir, out RaycastHit hitInfo, float range, int layerMask, List<Collider> ignoreCollider)
	{
		hitInfo = new RaycastHit();
		int count = 0;
		while(true)
		{
			if(thickRaycast(origin, radius, dir, out hitInfo, range, layerMask))
			{
				if(ignoreCollider != null && ignoreCollider.Contains(hitInfo.collider))
				{
					//do another raycast with shortened range
					float offset = 0.001f + radius;
					range -= Vector3.Distance(hitInfo.point, origin) + offset;
					origin = hitInfo.point + dir * offset;
					hitInfo.point = origin + dir * range;
				}
				else return true;
			}
			else return false;
			
			if(ignoreCollider == null || ++count > ignoreCollider.Count + 1000 || range < 0f) break;
		}
		Debug.LogWarning("This point should never be reached!");
		return false;
	}

	/// <summary>
	/// Performs a generic raycast that may have a radius (spherecast) or not (raycast).
	/// </summary>
	static public bool thickRaycast(Vector3 origin, float radius, Vector3 dir, out RaycastHit hitInfo, float range, int layerMask)
	{
		if(radius <= 0f) return Physics.Raycast(origin, dir, out hitInfo, range, layerMask);
		else return Physics.SphereCast(origin, radius, dir, out hitInfo, range, layerMask);
	}

	//---------------------------------------------------------------------------//

	/// <summary>
	/// Intelligent thick raycast & cone cast. Performs several raycasts and spherecasts, around the passed pixel position (screen coordinates!).
	/// Uses all these results to find the object that the user most likely meant to be selected/hit/highlighted/etc. Intelligently weights
	/// the results according to the preferred layermasks. NOTE: only returns true and a hitcollider if it was able to find an object
	/// of the preferred layer.
	/// NOTE: this method requires several ray and sphere-casts thus is not very fast and should not be used excessively.
	/// </summary>
	/// <param name="pickingPixelScreenCoords">The screen coordinates around which the picking should be performed.</param>
	/// <param name="pickRange">The length of the casts</param>
	/// <param name="preferredLayerMask">The preferred layermask to determine if a certain collision should be preferred over some other.</param>
	/// <param name="hitCollider">the hit collider after the selection has been made</param>
	/// <param name="pickPos">returns the picking pos of the selected raycast</param>
	/// <param name="ignoredCollider">An optional list of colliders to be ignored</param>
	/// <param name="pickRay">the successful pickray if true is returned.</param>
	/// <param name="pickLayerMask">The layer mask used for the collision test of the ray/sphere casts</param>
	/// <param name="cam">The camera to be used to calculate the picking rays from (given the screen coords)</param>
	/// <param name="sphereCastRadii">Defines the radii of the performed sphere casts.</param>
	/// <param name="coneCastApertureCM">The aperture radius for the cone casts to be performed. The aperture is given in cm (centi-meters)
	/// on the screen. So basically from the pixel coords you move away about this value of cm and perform a ring-cast (cone cast) there.</param>
	/// <param name="coneCastSampleCount">Defines the cone-cast sample count. Basically a cone cast is performed as a sampled sphere of a given
	/// aperture (radius). 8 for instance means that the circle is sampled at every 45�.</param>
	/// <returns>True if something was hit, false otherwise.</returns>
	static public bool intelligentPickingCast(Vector3 pickingPixelScreenCoords, Camera cam, float pickRange, int preferredLayerMask, out Collider hitCollider,
		out Vector3 pickPos, out Ray pickRay, int pickLayerMask, List<Collider> ignoredCollider,
		float[] sphereCastRadii, float[] coneCastApertureCM, int coneCastSampleCount = 8)
	{
		pickPos = Vector3.zero;
		hitCollider = null;
		var ray = cam.ScreenPointToRay(pickingPixelScreenCoords);
		pickRay = ray;

		//do multiple ray/sphere cast of variing radius
		RaycastHit pickInfo = new RaycastHit();
		int pickCount = 0;
		//paramater for the picking-queries: positive sets a radius for a spherecast, negative equals to a pixel deviation
		//given in cm (and converted to pixels using DPI) and thus an apearture angle for a cone cast
		List<float> param = new List<float> {0f}; //the exact raycast with width 0 is always performed
		if(sphereCastRadii != null)
			foreach(var rr in sphereCastRadii) param.Add(rr);
		if(coneCastApertureCM != null)
			foreach(var rr in coneCastApertureCM) param.Add(-rr); //for cone casts a negative radius is expected below

		for(int i = 0; i < param.Count; ++i)
		{
			if(param[i] >= 0f)
			{
				//perform raycast of radius param[i]
				float r = param[i];
				RaycastHit hitInfo;
				if(HelperFunctions.SphereCast(ray.origin, r, ray.direction, out hitInfo, pickRange, pickLayerMask, ignoredCollider))
				{
					++pickCount;

					if(((1<<hitInfo.collider.gameObject.layer) & preferredLayerMask) != 0)
					{
						//found an object on a preferred layer, stop immediately
						pickInfo = hitInfo;
						break;
					}
					else if(pickCount <= 1)
						//found an objects (first one: if nothing else is found, return this one)
						pickInfo = hitInfo;
				}
			}
			else if(preferredLayerMask != 0) //only perform cone casts if there is a preferred layer for picking
			{
				//perform a cone cast (serveral raycasts) with a specified aperture angle
				float dpi = Screen.dpi > 0f ? Screen.dpi : /* default DPI estimation for "normal screens" */ 100f;
				float pixelOffset = (-param[i] / 2.54f) * dpi;

				Vector3 offset = new Vector3(pixelOffset, 0f, 0f);
				Quaternion deltaRot = Quaternion.AngleAxis(360f / coneCastSampleCount, -Vector3.forward);
				for(int phi = 0; phi < coneCastSampleCount; ++phi)
				{
					ray = cam.ScreenPointToRay(pickingPixelScreenCoords + offset);
					RaycastHit hitInfo;
					if(HelperFunctions.Raycast(ray.origin, ray.direction, out hitInfo, pickRange, pickLayerMask, ignoredCollider))
					{
						if(((1<<hitInfo.collider.gameObject.layer) & preferredLayerMask) != 0)
						{
							++pickCount;
							pickInfo = hitInfo;
							pickRay = ray;
							break;
						}
					}

					//if the offset is less than a single pixel, break the loop as one raycast is enough!
					if(offset.sqrMagnitude < 0.25f) break;
					else offset = deltaRot * offset;
				}
			}
		}

		//return the found object
		if(pickCount > 0)
		{
			pickPos = pickInfo.point;
			hitCollider = pickInfo.collider;
			return true;
		}
		else return false;
	}

	//---------------------------------------------------------------------------//
	
	/// <summary>
	/// Finds a child transform of a certain name. Attention: this might have an impact on performance and should therefore
	/// not be used too often. Returns the first child in the hirarchy (BFS) of matching name.
	/// </summary>
	static public Transform FindChildRecursive(Transform root, string name)
	{
		Queue<Transform> queue = new Queue<Transform>();
		queue.Enqueue(root);
		while(queue.Count > 0)
		{
			Transform node = queue.Dequeue();
			if(node.name == name) return node;
			//add all childs of this node
			foreach(Transform child in node)
				queue.Enqueue(child);
		}
		return null;
	}
	
	static public void applyRecursiveFilter(ref float curr, float target, float factor)
	{
		curr = factor * target + (1f-factor) * curr;
	}
	static public void applyRecursiveFilter(ref Vector3 curr, Vector3 target, float factor)
	{
		curr = factor * target + (1f-factor) * curr;
	}
	static public float applyRecursiveFilter(float curr, float target, float factor)
	{
		applyRecursiveFilter(ref curr, target, factor);
		return curr;
	}
	static public Vector3 applyRecursiveFilter(Vector3 curr, Vector3 target, float factor)
	{
		applyRecursiveFilter(ref curr, target, factor);
		return curr;
	}
	static public Quaternion applyRecursiveFilter(Quaternion curr, Quaternion target, float factor)
	{
		return Quaternion.Slerp(curr, target, factor);
	}
	/// <summary>
	/// Applies a recursive filter to an angle in RAD, taking the shorter rotation direction.
	/// </summary>
	static public void applyRecursiveFilterToAngleRAD(ref float angle, float targetAngle, float factor)
	{
		applyRecursiveFilter(ref angle, angle + acuteAngleDifferenceRAD(angle, targetAngle), factor);
	}
	static public void applyRecursiveFilterToAngleDEG(ref float angle, float targetAngle, float factor)
	{
		applyRecursiveFilter(ref angle, angle + acuteAngleDifferenceDEG(angle, targetAngle), factor);
	}
	
	/// <summary>
	/// Resets the local transformation. position to zero. rotation to identity. scale to one.
	/// </summary>
	static public void resetLocalTransform(Transform t)
	{
		t.localPosition = Vector3.zero;
		t.localRotation = Quaternion.identity;
		t.localScale = Vector3.one;
	}
	
	/// <summary>
	/// Compares the angles a and b if they are equal within a certain tolerance. Takes the equality of 360 and 0
	/// into consideration
	/// </summary>
	static public bool equalAnglesDEG(float a, float b, float tolerance)
	{
		if(a > 180f) a -= 360f; if(a < -180f) a += 360f;
		if(b > 180f) b -= 360f; if(b < -180f) b += 360f;
		float diff = Mathf.Abs(a-b);
		return diff < tolerance || diff > 360f-tolerance;

//		return Mathf.Abs(acuteAngleDifferenceDEG(a, b)) < tolerance;
	}

	static public bool equalAnglesRAD(float a, float b, float tolerance)
	{
		if(a > Mathf.PI) a -= 2f*Mathf.PI; if(a < -Mathf.PI) a += 2f*Mathf.PI;
		if(b > Mathf.PI) b -= 2f*Mathf.PI; if(b < -Mathf.PI) b += 2f*Mathf.PI;
		float diff = Mathf.Abs(a-b);
		return diff < tolerance || diff > 2*Mathf.PI-tolerance;
	}

	/// <summary>
	/// Returns the acute angle (i.e. the shortest) difference between two angles.
	/// The sign of the returned value tells the sense of rotation.
	/// </summary>
	/// <returns>
	/// The difference in [-180, +180] DEG.
	/// </returns>
	static public float acuteAngleDifferenceDEG(float fromAngle, float toAngle)
	{
		while(toAngle - fromAngle > 180f)
			fromAngle += 360f;

		while(toAngle - fromAngle < -180f)
			fromAngle -= 360f;

		return toAngle - fromAngle;
	}
	static public float acuteAngleDifferenceRAD(float fromAngle, float toAngle)
	{
		while(toAngle - fromAngle > Mathf.PI)
			fromAngle += 2f*Mathf.PI;
		
		while(toAngle - fromAngle < -Mathf.PI)
			fromAngle -= 2f*Mathf.PI;
		
		return toAngle - fromAngle;
	}
	
	/// <summary>
	/// Limits the inclination angle of a direction vector. If the vertical angle is smaller or bigger than the limits, it is
	/// projected back into the confined region. 0 = no inclination, otherwise the angle is within [-PI/2, PI/2]
	/// </summary>
	/// <param name='v'>
	/// Input direction vector: This one needs to be normalized!
	/// </param>
	/// <param name='minAngle'>
	/// Minimum angle.
	/// </param>
	/// <param name='maxAngle'>
	/// Maximum angle.
	/// </param>
	static public Vector3 limitInclinationAngle(Vector3 v, float minAngle, float maxAngle)
	{
		float angle = Mathf.Asin(v.y);
		if(angle < minAngle || angle > maxAngle)
		{
			angle = Mathf.Clamp(angle, minAngle, maxAngle);
			v.y = Mathf.Sqrt(v.x*v.x + v.z*v.z) * Mathf.Tan(angle);
			v.Normalize();
		}
		return v;
	}
	
	/// <summary>
	/// Returns a random vector3 inside the unit circle in the xz plane.
	/// </summary>
	static public Vector3 RandomVector3InsideUnitCircle()
	{
		var dir = Random.insideUnitCircle;
		var v = new Vector3(dir.x, 0f, dir.y);
		return v;
	}
	/// <summary>
	/// Returns a random vector3 on the unit circle in the xz plane.
	/// </summary>
	static public Vector3 RandomVector3OnUnitCircle()
	{
		var v = RandomVector3InsideUnitCircle();
		return v.normalized;
	}

	/// <summary>
	/// Checks the collision matrix in the physics inspector and sets up the 
	/// collision-layermask that is defined for the passed layer l.
	/// </summary>
	static public int extractCollisionLayerMask(int l)
	{
		int mask = 0;
		for(int i = 0; i < 32; ++i)
		{
			if(!Physics.GetIgnoreLayerCollision(l, i))
				mask |= (1<<i);
		}
		return mask;
	}

	static private List<string> _tmpLevelList = new List<string>();
	/// <summary>
	/// Returns a list of all scenes in the "Scenes/" folder.
	/// </summary>
	static public List<string> getLevelList()
	{
		_tmpLevelList.Clear();
		_tmpLevelList.Add("");
		string[] files = System.IO.Directory.GetFiles(Application.dataPath + "/Scenes");
		foreach(string fileName in files)
		{
			if(fileName.EndsWith(".unity"))
			{
				int index = fileName.LastIndexOf('\\');
				string levelName = fileName.Substring(index+1, fileName.LastIndexOf(".unity") - (index+1));
				_tmpLevelList.Add(levelName);
			}
		}
		return _tmpLevelList;
	}

	/// <summary>
	/// Finds the index of the first occurence of an item in a enumaration.
	/// Returns -1 if not found.
	/// </summary>
	public static int FindIndexOf<T>(this IEnumerable<T> items, T item)
	{
		int index = 0;
		foreach(var it in items)
		{
			if(EqualityComparer<T>.Default.Equals(item, it))
				return index;
			++index;
		}
		return -1;
	}

	public static V FindItem<K, V>(this Dictionary<K, V> dic, System.Predicate<KeyValuePair<K,V>> match)
	{
		foreach(var it in dic)
			if(match(it))
				return it.Value;
		return default(V);
	}

	//---------------------------------------------------------------------------//

	/// <summary>
	/// Computes the impact impulse on a collision between two rigid bodies.
	/// </summary>
	static public float computeImpactImpulse(Rigidbody thisRB, Collision coll)
	{
		//although this is not entire correct to calc the impact impulse (just summing the two relative impulses of both objects),
		//it will work just fine and nobody will notice the difference. (otherwise I'd have to assume elastic/inelastic collision,
		//perform a full collision response calculation and determine the impulse change of both objects to get the impact impulse).
		var otherRB = coll.collider.attachedRigidbody;
		var speed = coll.relativeVelocity.magnitude;
		float impulse = thisRB.mass * speed;
		if(otherRB != null) impulse += otherRB.mass * speed;
		return impulse;
	}

	//---------------------------------------------------------------------------//

	/// <summary>
	/// Returns the (shared) physic material of a collider, and also properly returns the ones of terrain collider, that are
	/// NOT stored in collider.sharedMaterial
	/// </summary>
	static public PhysicMaterial getPhysicsMaterial(Collider coll)
	{
		var terr = coll as TerrainCollider;
		if(terr == null && coll != null) return coll.sharedMaterial;
		else if(terr != null) return terr.terrainData.physicMaterial;
		else return null;
	}

	//---------------------------------------------------------------------------//

	/// <summary>
	/// Generates a unique object name (supposely unique!) out of the name, the hierarchy level,
	/// the position of an object.
	/// </summary>
	static public string generateUniqueObjectName(GameObject go, bool useName = true, bool usePos = true, bool useHierarchy = true)
	{
		//position hash with 
		Vector3 pos = go.transform.position * 100f;
		string posHash = Mathf.RoundToInt(pos.x).ToString()+Mathf.RoundToInt(pos.y).ToString()+Mathf.RoundToInt(pos.z).ToString();
		
		//count the position in the hierarchy (on which level is this networkview?)
		int hierarchyHash = 0;
		Transform curr = go.transform;
		while(curr.parent != null) {curr = curr.parent; hierarchyHash++;}
		
		//compose final unique name
		return (useHierarchy ? hierarchyHash.ToString() : "")
				+ (useName ? go.name : "")
				+ (usePos ? posHash : "");
	}

	//---------------------------------------------------------------------------//

	/// <summary>
	/// Returns the name of an object including the entire hierarchy path!
	/// </summary>
	static public string getNameWithHierarchy(GameObject go)
	{
		string n = go.name;
		var t = go.transform.parent;
		while(t != null)
		{
			n = t.name + "/" + n;
			t = t.parent;
		}
		return n;
	}

	//---------------------------------------------------------------------------//

#if UNITY_EDITOR
	/// <summary>
	/// Finds all scripts of type T and all its derivates. You can access the types by using MonoScript.GetClass().
	/// </summary>
	static public List<MonoScript> getScriptAssetsOfType<T>()
	{
		List<MonoScript> result = new List<MonoScript>();
		string[] assetPaths = AssetDatabase.GetAllAssetPaths();
		foreach(string assetPath in assetPaths)
		{
			if(assetPath.Contains(".cs"))
			{
				var script = AssetDatabase.LoadAssetAtPath(assetPath, typeof(MonoScript)) as MonoScript;
				if(script == null) continue;

				if(script.GetClass() != null && script.GetClass().IsSubclassOf(typeof(T)))
					result.Add(script);
			}
		}
		return result;
	}
#endif

	//---------------------------------------------------------------------------//

	/// <summary>
	/// Copies a component into a gameobject. depending on the "overwrite" parameter it adds a new one in any case
	/// (false), or it tries to search for an existing one to overwrite it (true).
	/// </summary>
	static public Component copyComponent(Component original, GameObject destination, bool overwrite = false)
	{
		System.Type type = original.GetType();

		//create a new component of this type or take the existing one and overwrite it
		Component copy = null;
		if(overwrite) copy = destination.GetComponent(type);
#if UNITY_EDITOR
		if(copy == null) copy = Undo.AddComponent(destination, type);
#else
		if(copy == null) copy = destination.AddComponent(type);
#endif

		System.Reflection.FieldInfo[] fields = type.GetFields();
		foreach(System.Reflection.FieldInfo field in fields)
		{
			field.SetValue(copy, field.GetValue(original));
		}
		return copy;
	}

	//---------------------------------------------------------------------------//

	static public Bounds calcCombinedColliderBounds(GameObject root, out int colliderCount, int layerMask = -1, bool includeTrigger = false)
	{
		colliderCount = 0;
		Bounds bounds = new Bounds(root.transform.position, Vector3.zero); //gets overwritten by the first collider-bound below!

		//calculate the combined bounds of all colliders in this rigidbody
		var colliders = root.GetComponentsInChildren<Collider>();
		if(colliders == null || colliders.Length <= 0) return bounds;

		foreach(var c in colliders)
		{
			//only consider active colliders on the dynamic layer. For all telepickable objects there will be at LEAST on dynamic layer collider
			if(!c.enabled || !c.gameObject.activeInHierarchy) continue;
			if((layerMask & (1<<c.gameObject.layer)) == 0) continue;
			if(!includeTrigger && c.isTrigger) continue;

			//merge bounds
			if(colliderCount == 0)
				bounds = c.bounds;
			else
				bounds.Merge(c.bounds);
			++colliderCount;
		}
		return bounds;
	}

	//---------------------------------------------------------------------------//
}

//---------------------------------------------------------------//

static public class BoundsExtension
{
	static public void Merge(this Bounds obj, Bounds other)
	{
		obj.SetMinMax(Vector3.Min(obj.min, other.min), Vector3.Max(obj.max, other.max));
	}
}

//---------------------------------------------------------------------------//

public static class RectExtension
{
	static public Rect Merge(this Rect rect, Rect with)
	{
		rect.xMin = Mathf.Min(rect.xMin, with.xMin);
		rect.yMin = Mathf.Min(rect.yMin, with.yMin);
		rect.xMax = Mathf.Max(rect.xMax, with.xMax);
		rect.yMax = Mathf.Max(rect.yMax, with.yMax);
		return rect;
	}
}

//---------------------------------------------------------------//

/// <summary>
/// This class is a fluent interface for the LayerMask setup in Code. It provides the basic functionality
/// and should be extended to include more specific method useful for a certain project.
/// NOTE: use "c# extensions" to add functionality!
/// </summary>
public class Layers
{
	static private Layers _stackInstance = null;
	private int _layer = 0;
	
	static public implicit operator int(Layers l) {return l._layer;}
	
	/// <summary>
	/// Returns a single instance of the Layers class without allocation.
	/// NOTE: This instance can just be used until the next time Start() is called.
	/// </summary>
	static public Layers Start()
	{
		if(null == _stackInstance) _stackInstance = new Layers();
		_stackInstance.reset();
		return _stackInstance;
	}
	
	public Layers Add(string layer)
	{
		_layer |= (1<<LayerMask.NameToLayer(layer));
		return this;
	}
	public Layers reset()
	{
		_layer = 0;
		return this;
	}
	
	public bool contains(string layer)
	{
		return contains (LayerMask.NameToLayer(layer));	
	}
	
	public bool contains(int layer)
	{
		return (_layer |(1<<layer)) == _layer;
	}
}

//---------------------------------------------------------------//

public struct AABB
{
	public Vector3 min;
	public Vector3 max;
	
	public void extendToPoint(Vector3 p, float toleranceSphereRadius)
	{
		Vector3 diag = new Vector3(toleranceSphereRadius, toleranceSphereRadius, toleranceSphereRadius);
		max = Vector3.Max(max, p + diag);
		min = Vector3.Min(min, p - diag);
	}
	public void extendToPoint(Vector3 p)
	{
		extendToPoint(p, 0f);
	}
	
	public bool isUnset() {return min.x > max.x || min.y > max.y || min.z > max.z;}
	
	public void reset()
	{
		min = new Vector3(float.MaxValue, float.MaxValue, float.MaxValue);
		max = new Vector3(float.MinValue, float.MinValue, float.MinValue);
	}
	
	public void combineWith(AABB a)
	{
		max = Vector3.Max(max, a.max);
		min = Vector3.Min(min, a.min);
	}
	
	static public bool intersect(AABB a, AABB b)
	{
		if(((a.min.x >= b.min.x && a.min.x <= b.max.x) || (b.min.x >= a.min.x && b.min.x <= a.max.x) ||
		   (a.max.x >= b.min.x && a.max.x <= b.max.x) || (b.max.x >= a.min.x && b.max.x <= a.max.x)) &&
		   ((a.min.y >= b.min.y && a.min.y <= b.max.y) || (b.min.y >= a.min.y && b.min.y <= a.max.y) ||
		   (a.max.y >= b.min.y && a.max.y <= b.max.y) || (b.max.y >= a.min.y && b.max.y <= a.max.y)) &&
		   ((a.min.z >= b.min.z && a.min.z <= b.max.z) || (b.min.z >= a.min.z && b.min.z <= a.max.z) ||
		   (a.max.z >= b.min.z && a.max.z <= b.max.z) || (b.max.z >= a.min.z && b.max.z <= a.max.z)))
		{
			//AABBs intersect each other
			return true;
		}
		else return false;
	}
}

//---------------------------------------------------------------//
//some tuple classes (generics)

[System.Serializable]
public class Pair<T1, T2>
{
	public T1 first;
	public T2 second;
	public Pair() {}
	public Pair(T1 a, T2 b) {first = a; second = b;}
}
[System.Serializable]
public class Triple<T1, T2, T3>
{
	public T1 first;
	public T2 second;
	public T3 third;
	public Triple(T1 a, T2 b, T3 c) {first = a; second = b; third = c;}
}

//---------------------------------------------------------------//

static public class TransformExtension
{
	static public void setLocalX(this Transform v, float x)
	{
		v.localPosition = new Vector3(x, v.localPosition.y, v.localPosition.z);
	}
	static public void setLocalY(this Transform v, float y)
	{
		v.localPosition = new Vector3(v.localPosition.x, y, v.localPosition.z);
	}
	static public void setLocalZ(this Transform v, float z)
	{
		v.localPosition = new Vector3(v.localPosition.x, v.localPosition.y, z);
	}
	
	static public void setX(this Transform v, float x)
	{
		v.position = new Vector3(x, v.position.y, v.position.z);
	}
	static public void setY(this Transform v, float y)
	{
		v.position = new Vector3(v.position.x, y, v.position.z);
	}
	static public void setZ(this Transform v, float z)
	{
		v.position = new Vector3(v.position.x, v.position.y, z);
	}

	/// <summary>
	/// Destroys all children.
	/// </summary>
	static public void destroyChildren(this Transform trans)
	{
		foreach(Transform t in trans)
		{
			Object.Destroy(t.gameObject);
		}
	}

	/// <summary>
	/// Gets components in children, INCLUDING DISABLED ONES.
	/// </summary>
	public static T[] GetComponentsInChildrenIncludingDisabled<T>(this Transform trans) where T : Component
	{
		var list = new List<T>();

		var p = trans.GetComponent<T>();
		if(p) list.Add(p);
		foreach(Transform child in trans)
		{
			var candidate = child.GetComponent<T>();
			if(candidate)
			{
				list.Add(candidate);
			}
			list.AddRange(GetComponentsInChildrenIncludingDisabled<T>(child));
		}
		return list.ToArray();
	}
}

//---------------------------------------------------------------//

//---------------------------------------------------------------//
