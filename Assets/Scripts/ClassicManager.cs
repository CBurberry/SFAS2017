using System.Collections;
using UnityEngine;

public class ClassicManager : GameModeManagerBase
{
	public static ClassicManager instance;

	[SerializeField]
	private int MaxAsteroids;

	[SerializeField]
	private int AsteroidPointValue;

	[SerializeField]
	private int DefaultAsteroidSplitCount;

    private float checkEmptyTimer = 2.0f;
	private float checkEmptyTimerLimit = 3.0f;
	private AsteroidSpawner spawner;
	private UFOSpawner ufoSpawner;
	private int asteroidSplitCount;
	private int scoreNeededForExtraLife_p1 = 2000;
	private int scoreNeededForExtraLife_p2 = 2000;
	private int scoreNeededIncrease = 2000;
	private float ufoCount = 0.0f;
	private int currentUFOCount;

    protected override void Awake()
    {
		instance = this;
		base.Awake();
		spawner = GetComponent<AsteroidSpawner>();
		ufoSpawner = GetComponent<UFOSpawner>();
		BulletSpawner = GetComponent<BasicBulletSpawner>();
		spawner.AsteroidSplitDepth = DefaultAsteroidSplitCount;
		asteroidSplitCount = DefaultAsteroidSplitCount;
    }

    protected override void Start()
    {
		base.Start();
		currentUFOCount = (int) ufoCount;
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
			//Check if either player earned a bonus life.
			BonusLifeGainCheck();

			//check timer has elapsed before checking asteroids active.
			checkEmptyTimer += Time.deltaTime;
			

			if(mState == State.Playing && checkEmptyTimer >= checkEmptyTimerLimit)
			{
				checkEmptyTimer = 0.0f;
				//If there are no active asteroids.
				if(!spawner.isAnyAsteroidActive())
				{
					//Set modifier for round increase upping
					RoundLogic();
				}
			}

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

	private void RoundLogic() 
	{
		spawner.AsteroidSplitDepth = asteroidSplitCount;
		//Add 0 or 1 asteroids per round.
		spawner.numberOfAsteroids += Random.Range(0, 2);
		//Clamp asteroid count to max asteroids.
		spawner.numberOfAsteroids = Mathf.Clamp(spawner.numberOfAsteroids, 3, MaxAsteroids);
		spawner.roundSpawn();
		ufoCount += 0.5f;
		currentUFOCount = (int) ufoCount;
		StartCoroutine(UFOSpawnRoutine());
	}

	private IEnumerator UFOSpawnRoutine() 
	{
		while(currentUFOCount > 0) 
		{
			int randomUFOSpawn = Random.Range(0, currentUFOCount+1);

			if(randomUFOSpawn > 0 && currentUFOCount > 0) 
			{
				ufoSpawner.Spawn();
				currentUFOCount--;
			}
			yield return new WaitForSeconds(3.0f);
		}
		yield return null;
	}

	private void BonusLifeGainCheck() 
	{
		Player p1 = PlayerOne.GetComponent<Player>();
		if(p1.GetScore() > scoreNeededForExtraLife_p1) 
		{
			scoreNeededForExtraLife_p1 += scoreNeededIncrease;
			if(p1.GetCurrentLives() < 6) 
			{
				p1.GainLife();
			}
		}

		if(PlayerTwo.activeSelf) 
		{
			Player p2 = PlayerTwo.GetComponent<Player>();
			if(p2.GetScore() > scoreNeededForExtraLife_p2) 
			{
				scoreNeededForExtraLife_p2 += scoreNeededIncrease;
				if(p2.GetCurrentLives() < 6) 
				{
					p2.GainLife();
				}
			}
		}
	}
}
