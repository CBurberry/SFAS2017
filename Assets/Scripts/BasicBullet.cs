using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BasicBullet : MonoBehaviour {

	[HideInInspector]
	public GameObject ownedObject;

	private Rigidbody rb;
	private string ownerTag;
	private int ownerScoreValue;

	void OnTriggerEnter(Collider other) 
	{
		Asteroid ast;

		//Check layer 8 'Asteroid'
		if(other.CompareTag("Asteroid") && ownedObject.CompareTag("Player")) 
		{
			ast = other.GetComponent<Asteroid>();
			ownedObject.GetComponent<Player>().AddToScore(ast.scoreValue);
			ast.Destroyed();
			gameObject.SetActive(false);
		}

		else if(other.CompareTag("UFO") && ownedObject.CompareTag("Player")) 
		{
			UFOController ufo = other.GetComponent<UFOController>();
			ownedObject.GetComponent<Player>().AddToScore(ufo.ScoreValue);
			gameObject.SetActive(false);
			other.gameObject.SetActive(false);
		}

		//Aka this is an enemy bullet
		else if(ownedObject.CompareTag("UFO") && other.CompareTag("Player")) 
		{
			gameObject.SetActive(false);
			other.GetComponent<Player>().LoseLife();
		}
	}

	void OnTriggerExit(Collider other) 
	{
		if(other.CompareTag("Arena")) 
		{
			gameObject.SetActive(false);
		}
	}
}
