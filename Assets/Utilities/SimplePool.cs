using UnityEngine;
using System.Collections.Generic;

public static class SimplePool {

	private const int DEFAULT_POOL_SIZE = 50;
	
	/// <summary>
	/// The Pool class represents the pool for a particular prefab.
	/// </summary>
	private class Pool {
		
		private int nextId=1;
		private Stack<GameObject> inactive;
		private GameObject prefab;
		
		public Pool(GameObject prefab, int initialQty) {
			this.prefab = prefab;
			inactive = new Stack<GameObject>(initialQty);
		}
		
		public GameObject Spawn(Vector3 pos, Quaternion rot) {
			GameObject obj;
			if(inactive.Count==0) {
				obj = GameObject.Instantiate(prefab, pos, rot);
				obj.name = prefab.name + " ("+(nextId++)+")";
				obj.AddComponent<PoolMember>().myPool = this;
			}
			else {
				obj = inactive.Pop();
				
				if(obj == null) {
					return Spawn(pos, rot);
				}
			}
			
			obj.transform.position = pos;
			obj.transform.rotation = rot;
			obj.SetActive(true);
			return obj;
			
		}
		
		// Return an object to the inactive pool.
		public void Despawn(GameObject obj) {
			obj.SetActive(false);
			inactive.Push(obj);
		}
	}
	
	
	/// <summary>
	/// Added to freshly instantiated objects, so we can link back
	/// to the correct pool on despawn.
	/// </summary>
	private class PoolMember : MonoBehaviour {
		public Pool myPool;
	}
	
	// All of our pools
	private static Dictionary< GameObject, Pool > pools;
	
	/// <summary>
	/// Init our dictionary.
	/// </summary>
	private static void Init (GameObject prefab=null, int qty = DEFAULT_POOL_SIZE)
	{
		pools ??= new Dictionary<GameObject, Pool>();
		if(prefab!=null && pools.ContainsKey(prefab) == false) {
			pools[prefab] = new Pool(prefab, qty);
		}
	}
	
	/// <summary>
	/// If you want to preload a few copies of an object at the start
	/// of a scene, you can use this. Really not needed unless you're
	/// going to go from zero instances to 10+ very quickly.
	/// Could technically be optimized more, but in practice the
	/// Spawn/Despawn sequence is going to be pretty darn quick and
	/// this avoids code duplication.
	/// </summary>
	public static void Preload(GameObject prefab, int qty = 1) {
		Init(prefab, qty);
		
		// Make an array to grab the objects we're about to pre-spawn.
		var obs = new GameObject[qty];
		for (var i = 0; i < qty; i++) {
			obs[i] = Spawn (prefab, Vector3.zero, Quaternion.identity);
		}
		
		// Now despawn them all.
		for (var i = 0; i < qty; i++) {
			Despawn( obs[i] );
		}
	}
	
	/// <summary>
	/// Spawns a copy of the specified prefab (instantiating one if required).
	/// NOTE: Remember that Awake() or Start() will only run on the very first
	/// spawn and that member variables won't get reset.  OnEnable will run
	/// after spawning -- but remember that toggling IsActive will also
	/// call that function.
	/// </summary>
	public static GameObject Spawn(GameObject prefab, Vector3 pos, Quaternion rot) {
		Init(prefab);
		
		return pools[prefab].Spawn(pos, rot);
	}
	
	/// <summary>
	/// Despawn the specified gameobject back into its pool.
	/// </summary>
	public static void Despawn(GameObject obj) {
		var pm = obj.GetComponent<PoolMember>();
		if(pm == null) {
			Debug.Log ("Object '"+obj.name+"' wasn't spawned from a pool. Destroying it instead.");
			GameObject.Destroy(obj);
		}
		else {
			pm.myPool.Despawn(obj);
		}
	}

	public static void Despawn(List<GameObject> objects)
	{
		foreach (var obj in objects)
		{
			Despawn(obj);
		}
	}
	
}