//http://answers.unity3d.com/questions/765574/can-someone-just-post-a-generic-object-pool-script.html
//Above post used as reference.

using System.Collections.Generic;
using UnityEngine;

public class ObjectPooler{
	private List<GameObject> list;
	private GameObject[] elementPrefabs;

	//Dynamic size (increases on demand, starts empty)
	//Elements is the possible selection of objects, on GetObject the element will be random.
	public ObjectPooler(GameObject[] elements) 
	{
		list = new List<GameObject>();
		elementPrefabs = elements;
	}

	//Get the first GameObject that has the same state as isActive.
	public GameObject GetNewObject() 
	{
		foreach(GameObject gObject in list) 
		{
			if(gObject.activeSelf == false) 
			{
				gObject.SetActive(true);
				return gObject;
			}
		}
		
		int index = Random.Range(0, elementPrefabs.Length);
		GameObject obj = Object.Instantiate(elementPrefabs[index]);
		obj.SetActive(true);
		list.Add(obj);
		return obj;
	}

	//Empty the pool and destroy the objects contained.
	public void ClearPool() 
	{
		for(int i = 0; i < list.Count; i++) 
		{
			Object.Destroy(list[i]);
		}
		list = null;
	}

	public int Length() 
	{
		return list.Count;
	}

	//Check if any element is active.
	public bool HasActiveElements() 
	{
		foreach(GameObject gObject in list) 
		{
			if(gObject.activeSelf) 
			{
				return true;
			}
		}
		return false;
	}

	public int CountActiveElements() 
	{
		int count = 0;
		foreach(GameObject gObject in list) 
		{
			if(gObject.activeSelf) 
			{
				count++;
			}
		}
		return count;
	}
	
}
