using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ShootingAsteroid : MonoBehaviour {

	[SerializeField]
	private float IconDuration;

	[SerializeField]
	private Vector3 IconMinScale;

	[SerializeField]
	private Vector3 IconMaxScale;

	private Rigidbody rb;
	private float spawnTimer = 0.0f;

	void Awake() 
	{
		rb = GetComponent<Rigidbody>();
	}

	public void Activate() 
	{
		StartCoroutine(SpawnAndPlayIcon(transform.position));
	}

	//This method assumes Icon is already rotated in velocity direction.
	private IEnumerator SpawnAndPlayIcon(Vector3 spawnPosition) 
	{
		Vector3 temp = rb.velocity;
		rb.isKinematic = true;
		spawnTimer = 0.0f;
		float progression = 0.0f;
		GameObject iconObject = transform.GetChild(0).GetChild(0).gameObject;
		GameObject rotationObject = transform.GetChild(0).gameObject;
		GameObject meshObject = transform.GetChild(1).gameObject;
		meshObject.SetActive(true);
		rotationObject.SetActive(true);
		Transform iconTransform = iconObject.transform;

		while(spawnTimer < IconDuration) 
		{
			progression = spawnTimer / IconDuration;
			iconObject.transform.localScale = Vector3.Lerp(IconMinScale, IconMaxScale, progression);
			spawnTimer += Time.deltaTime;
			yield return null;
		}

		rotationObject.SetActive(false);
		meshObject.SetActive(true);
		rb.isKinematic = false;
		rb.velocity = temp;
		yield return null;
	}
}
