using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UFOController : MonoBehaviour {

	[SerializeField]
	private float FireRate;

	[SerializeField]
	private float BulletSpeed;

	public int ScoreValue;

	//Not sure where to place aiming offset

	private GameObject PlayerOne;
	private GameObject PlayerTwo;
	private float nextFire = 0.0f;
	private Vector3 targetPosition = new Vector3();
	private GameModeManagerBase instance;

	void Update() 
	{
		if(instance == null)
		{
			GameModeManagerBase.SetManagerInstance(ref instance);
			PlayerOne = instance.GetPlayerOne();
			PlayerTwo = instance.GetPlayerTwo();
		}

		if(instance.mState == GameModeManagerBase.State.Playing) 
		{
			AimAndFire();
		}
	}

	void OnTriggerExit(Collider other) 
	{
		if(other.CompareTag("Arena")) 
		{
			gameObject.SetActive(false);
		}
	}

	private void AimAndFire() 
	{
		if(Time.time > nextFire) {
			nextFire = Time.time + FireRate;
			GameObject clone = instance.GetBulletSpawner().Spawn(gameObject);
			clone.transform.position = transform.position;
			clone.transform.LookAt(targetPosition);
			clone.GetComponent<Rigidbody>().velocity = clone.transform.forward * BulletSpeed;
		}
	}

	public IEnumerator TargetNearestPlayer() 
	{
		float playerDistance1 = 0.0f;
		float playerDistance2 = 0.0f;
		float smallestDistance;

		while(true)
		{
			if(instance == null)
			{
				yield return null;
			}

			//Only get player one
			if(GameMaster.instance.twoPlayersActive == false || !PlayerTwo.gameObject.activeSelf)
			{
				smallestDistance = Vector3.Distance(transform.position,PlayerOne.transform.position);
				playerDistance1 = smallestDistance;
			}
			//Two player mode and p1 is dead.
			else if(!PlayerOne.gameObject.activeSelf) 
			{
				smallestDistance = Vector3.Distance(transform.position,PlayerTwo.transform.position);
				playerDistance2 = smallestDistance;
			}
			//Else get both and compare the distances
			else
			{
				playerDistance1 = Vector3.Distance(transform.position,PlayerOne.transform.position);
				playerDistance2 = Vector3.Distance(transform.position,PlayerTwo.transform.position);
				smallestDistance = Mathf.Min(playerDistance1,playerDistance2);
			}

			if(smallestDistance == playerDistance1)
			{
				targetPosition = PlayerOne.transform.position;
			}
			else
			{
				targetPosition = PlayerTwo.transform.position;
			}

			yield return new WaitForEndOfFrame();
		}
	}
}
