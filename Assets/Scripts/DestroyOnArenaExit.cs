using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DestroyOnArenaExit : MonoBehaviour {

	void OnTriggerExit(Collider other) 
	{
		if(other.CompareTag("Arena")) 
		{
			transform.root.gameObject.SetActive(false);
		}
	}
}
