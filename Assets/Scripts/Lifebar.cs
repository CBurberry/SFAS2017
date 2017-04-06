using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Lifebar : MonoBehaviour {

	private GameObject player;
	private int count = 0;

	[SerializeField]
	private GameObject icon;

	

	public void GainLife() 
	{
		GameObject newIcon = Instantiate(icon);
		newIcon.transform.SetParent(gameObject.transform);
		count++;
	}

	public void LoseLife() 
	{
		Destroy(transform.GetChild(0).gameObject);
		count--;
	}

	public void Clear() 
	{
		foreach (Transform child in gameObject.transform) 
		{
			Destroy(child.gameObject);
		}
		count = 0;
	}

	public int Count() 
	{
		return count;
	}

	public void SetLife(int amount) 
	{
		Clear();
		for(int i = 0; i < amount; i++) 
		{
			GainLife();
		}
	}
}
