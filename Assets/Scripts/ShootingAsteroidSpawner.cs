using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ShootingAsteroidSpawner : MonoBehaviour {

	[SerializeField]
	private float speed;

	[SerializeField]
	private GameObject SAPrefab;

	private ObjectPooler shootingAsteroidPool;

	public void Spawn() 
	{
		if(shootingAsteroidPool == null) 
		{
			GameObject[] goArray = new GameObject[1];
			goArray[0] = SAPrefab;
			shootingAsteroidPool = new ObjectPooler(goArray);
		}

		//Get the spawn position
		Vector3 spawnPosition = Arena.RandomArenaBorderPosition();

		//Get a target point offset
		float offsetBound = Arena.Height * 1/3;
		float targetOffsetX = Random.Range(-offsetBound, offsetBound);
		float targetOffsetZ = Random.Range(-offsetBound, offsetBound);
		Vector3 targetPosition = new Vector3(-targetOffsetX,3.5f,targetOffsetZ);

		GameObject instance = shootingAsteroidPool.GetNewObject();
		Rigidbody rb = instance.GetComponent<Rigidbody>();

		//Set transform and rigidbody parameters.
		instance.transform.position = spawnPosition;

		//Set the objects' blue axis to velocity to rotate. 
		instance.transform.LookAt(targetPosition);
		instance.transform.GetChild(1).rotation = Quaternion.Euler(90.0f, 0.0f, 0.0f);
		rb.velocity = instance.transform.forward * speed;

		instance.GetComponent<ShootingAsteroid>().Activate();
	}
}
