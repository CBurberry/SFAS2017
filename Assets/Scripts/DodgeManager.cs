using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DodgeManager : GameModeManagerBase {

	public static DodgeManager instance;

	[SerializeField]
	private int StartSpawnPerTime;

	[SerializeField]
	private float TimePerSpawn;

	[SerializeField]
	private float SecondsToSpawnIncrease;

	[SerializeField]
	private int SpawnIncreaseAmount;


	private ShootingAsteroidSpawner spawner;
	private int currentSpawnAmount;


	protected override void Awake() 
	{
		instance = this;
		base.Awake();
		spawner = GetComponent<ShootingAsteroidSpawner>();
		currentSpawnAmount = StartSpawnPerTime;
	}

	protected override void PressAnyKeyToStart()
	{
		base.PressAnyKeyToStart();
		if(startWaitComplete) 
		{
			StartCoroutine(RoundLogic());
		}
	}

	protected override void Update()
	{
		base.Update();

		if(!startGame) 
		{
			return;
		}

		if(!startWaitComplete)
		{
			PressAnyKeyToStart();
		}
		else if(!isGameOver && CheckGameOver())
		{
			GameOver();
		}
		else
		{
			/*
			//Check for pause key - DISABLED DUE TO LACK OF FORESIGHT INVOLVING COROUTINES.
			if(Input.GetKeyDown(KeyCode.Escape)) 
			{
				if(mState == State.Playing)
				{
					PauseGame();
				}
				else
				{
					UnpauseGame();
				}
			}
			*/
		}
	}

	private IEnumerator RoundLogic() 
	{
		float timeSinceLastLoop = 0.0f;
		float timeElapsed = 0.0f;
		while(true) 
		{
			if(mState == State.Paused)
			{
				timeSinceLastLoop = Time.time;
				yield return new WaitForEndOfFrame();
			}
			else 
			{
				timeElapsed += Time.time - timeSinceLastLoop;
				timeSinceLastLoop = Time.time;
				ScoreUpdate(10);
				if(timeElapsed > SecondsToSpawnIncrease) 
				{
					currentSpawnAmount += SpawnIncreaseAmount;
					timeElapsed = 0.0f;
				}

				for(int i = 0; i < currentSpawnAmount; i++) 
				{
					spawner.Spawn();
				}
				yield return new WaitForSeconds(TimePerSpawn);
			}
		}
	}

	private void ScoreUpdate(int addedValue) 
	{
		if(PlayerOne.activeSelf) 
		{
			PlayerOne.GetComponent<Player>().AddToScore(addedValue);
		}

		if(PlayerTwo.activeSelf) 
		{
			PlayerTwo.GetComponent<Player>().AddToScore(addedValue);
		}
	}
}

