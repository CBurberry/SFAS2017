using UnityEngine;

public class AsteroidSpawner : MonoBehaviour {

	/*
		We want the spawners to be ahead of the player starting position,
		providing them ample time to dodge/react.

		Based on input parameters? We will have X spawn points at which an
		astroid will spawn as the round starts.

		These spawn points will be within the area enclosed of all X and 
		top 1/3rd Y, union left and right 1/4ths of X and all Y. (n shape)
	*/

	public GameObject[] asteroidPrefabs = new GameObject[4];
	public float velocityBound;
	public int startNumberOfAsteroids;

	[HideInInspector]
	public int AsteroidSplitDepth;

	[HideInInspector]
	public int numberOfAsteroids;

	private ObjectPooler asteroidPool;

	void Awake() 
	{
		//Initialise the pool.
		asteroidPool = new ObjectPooler(asteroidPrefabs);
		numberOfAsteroids = startNumberOfAsteroids;
	}

	//Expected to be called by GameManager - or game state handler.
	public void roundSpawn() 
	{
		Vector3 spawnPoint;

		if(asteroidPool == null) 
		{
			//Initialise the pool.
			asteroidPool = new ObjectPooler(asteroidPrefabs);
		}

		for(int i = 0; i < numberOfAsteroids; i++) 
		{
			spawnPoint = Arena.RandomArenaBorderPosition();
			spawnPoint.y = 3.0f;
			//Get first inactive asteroid
			GameObject instance = asteroidPool.GetNewObject();
			instance.transform.localPosition = spawnPoint;
			
			//Set astroid velocity & spawner reference
			Asteroid currentObj = instance.GetComponent<Asteroid>();
			currentObj.spawnerRef = this;
			currentObj.splitDepth = AsteroidSplitDepth;
			currentObj.scoreValue = currentObj.initialScoreValue;
			float velocityX = Random.Range(-velocityBound, velocityBound);
			float velocityY = Random.Range(-velocityBound, velocityBound);
			currentObj.SetVelocity(new Vector2(velocityX, velocityY));
		}
	}

	public GameObject getAsteroidObject() 
	{
		if(asteroidPool == null) 
		{
			//Initialise the pool.
			asteroidPool = new ObjectPooler(asteroidPrefabs);
		}

		//Get first inactive asteroid
		GameObject item = asteroidPool.GetNewObject();
		float velocityX = Random.Range(-velocityBound, velocityBound);
		float velocityY = Random.Range(-velocityBound, velocityBound);
		item.GetComponent<Asteroid>().SetVelocity(new Vector2(velocityX, velocityY));
		return item;
	}

	public int getAsteroidPoolSize() 
	{
		return asteroidPool.Length();
	}

	public bool isAnyAsteroidActive() 
	{
		return asteroidPool.HasActiveElements();
	}

	public int getActiveCount() 
	{
		return asteroidPool.CountActiveElements();
	}
}
