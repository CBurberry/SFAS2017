using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UFOSpawner : MonoBehaviour {

	[SerializeField]
	private float MoveSpeed;

	[SerializeField]
	private GameObject UFOPrefab;

	private ObjectPooler ufoPool;

	public void Spawn() 
	{
		if(ufoPool == null) 
		{
			GameObject[] goArray = new GameObject[1];
			goArray[0] = UFOPrefab;
			ufoPool = new ObjectPooler(goArray);
		}

		//Get spawn position
		Vector3 spawnPosition = Arena.RandomArenaBorderPosition();

		//Get a target point offset and generate a target to move towards.
		float offsetBound = Arena.Height * 1/3;
		float targetOffsetX = Random.Range(-offsetBound, offsetBound);
		float targetOffsetZ = Random.Range(-offsetBound, offsetBound);
		Vector3 targetPosition = new Vector3(-targetOffsetX, 3.5f, targetOffsetZ);

		GameObject instance = ufoPool.GetNewObject();
		Rigidbody rb = instance.GetComponent<Rigidbody>();
		StartCoroutine(instance.GetComponent<UFOController>().TargetNearestPlayer());

		//Set transform and rigidbody parameters.
		instance.transform.position = spawnPosition;

		//Aim the forward axis towards the target, apply velocity, reset rotation.
		instance.transform.LookAt(targetPosition);
		rb.velocity = instance.transform.forward * MoveSpeed;
		instance.transform.rotation = Quaternion.Euler(0.0f, 0.0f, 0.0f);
	}

	public bool isAnyUFOActive() 
	{
		if(ufoPool == null) 
		{
			GameObject[] goArray = new GameObject[1];
			goArray[0] = UFOPrefab;
			ufoPool = new ObjectPooler(goArray);
		}

		return ufoPool.HasActiveElements();
	}

	public int GetActiveCount() 
	{
		if(ufoPool == null) 
		{
			GameObject[] goArray = new GameObject[1];
			goArray[0] = UFOPrefab;
			ufoPool = new ObjectPooler(goArray);
		}

		return ufoPool.CountActiveElements();
	}
}
