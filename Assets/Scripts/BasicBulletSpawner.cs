using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BasicBulletSpawner : MonoBehaviour {

	[SerializeField]
	private GameObject BulletPrefab;

	private ObjectPooler bulletPool;

	public GameObject Spawn(GameObject owner) 
	{
		if(bulletPool == null) 
		{
			GameObject[] goArray = new GameObject[1];
			goArray[0] = BulletPrefab;
			bulletPool = new ObjectPooler(goArray);
		}

		GameObject instance = bulletPool.GetNewObject();
		BasicBullet bulletRef = instance.GetComponent<BasicBullet>();
		bulletRef.ownedObject = owner;
		return instance;
	}

	public GameObject Spawn(Vector3 position, Quaternion rotation, GameObject owner) 
	{
		if(bulletPool == null) 
		{
			GameObject[] goArray = new GameObject[1];
			goArray[0] = BulletPrefab;
			bulletPool = new ObjectPooler(goArray);
		}

		GameObject instance = bulletPool.GetNewObject();
		instance.transform.position = position;
		instance.transform.rotation = rotation;
		BasicBullet bulletRef = instance.GetComponent<BasicBullet>();
		bulletRef.ownedObject = owner;
		return instance;
	}
}
