using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SurvivalManager : GameModeManagerBase {

	public static SurvivalManager instance;

	[SerializeField]
	private int MaxAsteroids;

	[SerializeField]
	private int DefaultAsteroidSplitCount;

	//Shooting asteroids
	[SerializeField]
	private int SAStartingWaveSize;

	//Shooting asteroids wait time between waves.
	[SerializeField]
	private float TimeBetweenSAWaves;

	//Amount by which SA spawns increases
	[SerializeField]
	private float SASpawnIncreaseAmount;

	//UFO Initial spawn amount.
	[SerializeField]
	private int InitialUFOSpawns;

	//Number of rounds until shop reached (repeating)
	[SerializeField]
	private int RoundsUntilShop;

    private float checkEmptyTimer = 2.0f;
	private float checkEmptyTimerLimit = 3.0f;
	private AsteroidSpawner asteroidSpawner;
	private UFOSpawner ufoSpawner;
	private int asteroidSplitCount;
	private float ufoCount = 0.0f;
	private int currentUFOCount;

	private ShootingAsteroidSpawner shootingAsteroidSpawner;
	//Shooting asteroids
	private int currentSpawnAmount;
	private float currentSpawnAmountFloat;

	private bool shopActive = false;
	private int roundNumber = 0;

	protected override void Awake()
	{
		instance = this;
		base.Awake();

		//Get Spawner instances.
		asteroidSpawner = GetComponent<AsteroidSpawner>();
		ufoSpawner = GetComponent<UFOSpawner>();
		shootingAsteroidSpawner = GetComponent<ShootingAsteroidSpawner>();
		BulletSpawner = GetComponent<BasicBulletSpawner>();

		//Initialise spawn details & counters.
		currentSpawnAmountFloat = SAStartingWaveSize;
		asteroidSpawner.AsteroidSplitDepth = DefaultAsteroidSplitCount;
		asteroidSplitCount = DefaultAsteroidSplitCount;
		ufoCount = InitialUFOSpawns;
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
			//If user has not keypressed yet, blink WaitScreen
			PressAnyKeyToStart();
		}
		else if(!isGameOver && CheckGameOver() && !shopActive)
		{
			//If game over conditions are true, show game over screen.
			GameOver();
		}
		else
		{
			//Check timer has elapsed before checking if any asteroids are active.
			checkEmptyTimer += Time.deltaTime;

			//Check if not paused and the check time has elapsed.
			if(mState == State.Playing && !shopActive && checkEmptyTimer >= checkEmptyTimerLimit) 
			{
				checkEmptyTimer = 0.0f;
				RoundCheck();
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

	private void RoundIncrementLogic() 
	{
		asteroidSpawner.AsteroidSplitDepth = asteroidSplitCount;

		//Add 0 or 1 asteroids per round.
		asteroidSpawner.numberOfAsteroids += Random.Range(0, 2);

		//Clamp asteroid count to max asteroids
		asteroidSpawner.numberOfAsteroids = Mathf.Clamp(asteroidSpawner.numberOfAsteroids, 3, MaxAsteroids);

		//Incease the current number of ufo's to spawn within the round.
		ufoCount += 0.5f;
		currentUFOCount = (int)ufoCount;

		//SA spawn increase
		currentSpawnAmountFloat += SASpawnIncreaseAmount;
		currentSpawnAmount = (int) currentSpawnAmountFloat;
	}

	private void SpawnAll() 
	{
		//Call the UFOSpawnRoutine to handle UFO spawning within round.
		StartCoroutine(AsteroidSpawnRoutine());
		StartCoroutine(UFOSpawnRoutine());
		StartCoroutine(DodgeSpawnRoutine());
	}

	private IEnumerator AsteroidSpawnRoutine() 
	{
		//Call Asteroid Spawner to spawn a round's worth of asteroids
		asteroidSpawner.roundSpawn();
		yield return null;
	}

	private IEnumerator UFOSpawnRoutine() 
	{
		//While the Classic Round Logic allocated ufo count is not 0
		while(currentUFOCount > 0) 
		{
			//Get a random integer between 0 and count to indicate if this frame a UFO will spawn
			int randomUFOSpawn = Random.Range(0, currentUFOCount);

			//If the random integer is GT 0 and the allocated ufo count is GT 0
			if(randomUFOSpawn > 0 && currentUFOCount > 0) 
			{
				//Spawn a ufo and decrement the allocated ufo count.
				ufoSpawner.Spawn();
				currentUFOCount--;
			}

			//If there is LT 3 asteroids, spawn all remaining UFOs
			if(asteroidSpawner.getActiveCount() < 3 && currentUFOCount > 0) 
			{
				for(int i = 0; i < currentUFOCount; i++) 
				{
					ufoSpawner.Spawn();
				}
				currentUFOCount = 0;
			}
			//Wait for 3 seconds before trying to spawn again.
			yield return new WaitForSeconds(3.0f);
		}

		//Just return if the allocated ufo count is 0.
		yield return null;
	}

	private IEnumerator DodgeSpawnRoutine() 
	{
		//Continuous loop - no break statements.
		while(true) 
		{
			//If the game is paused, wait.
			if(mState == State.Paused)
			{
				yield return new WaitForEndOfFrame();
			}
			//If the game is not paused...
			else 
			{
				//Spawn as many shooting asteroids as the current spawn amount dictates.
				for(int i = 0; i < currentSpawnAmount; i++) 
				{
					shootingAsteroidSpawner.Spawn();
				}

				//Once done, wait for a duration until trying to spawn the next wave.
				yield return new WaitForSeconds(TimeBetweenSAWaves);
			}
		}
	}

	private void RoundCheck() 
	{
		StopAllCoroutines();

		//Check if criteria for round ending is true
		if(!asteroidSpawner.isAnyAsteroidActive() && !ufoSpawner.isAnyUFOActive()) 
		{
			//	If this was the last round of the set, stop any additional spawns,
			if(roundNumber == RoundsUntilShop)
			{
				roundNumber = 0;

				//fade out game and show shop
				StartCoroutine(ShopIntroSequence());
			}
			else 
			{
				RoundIncrementLogic();
				roundNumber++;
				SpawnAll();
			}
		} 
	}

	private IEnumerator ShopIntroSequence() 
	{
		float alpha = 0.0f;
		SpriteRenderer render = FadePanel.GetComponent<SpriteRenderer>();
		while(alpha < 1.0f) 
		{
			alpha += 0.1f;
			render.color = new Color(0.0f, 0.0f, 0.0f, alpha);

			yield return new WaitForSeconds(0.2f);
		}
		StartShopping();
		yield return null;
	}

	public IEnumerator ShopOutroSequence() 
	{
		float alpha = 0.99f;
		SpriteRenderer render = FadePanel.GetComponent<SpriteRenderer>();
		while(alpha > 0.0f) 
		{
			alpha -= 0.1f;
			render.color = new Color(0.0f, 0.0f, 0.0f, alpha);

			yield return new WaitForSeconds(0.2f);
		}
		render.color = new Color(0.0f, 0.0f, 0.0f, 0.0f);
		EndShopping();
		yield return null;
	}

	//Only call once the screen is blacked out.
	private void StartShopping() 
	{
		shopActive = true;

		//Call the ShopScript
		GetComponent<ShopScript>().StartShopScript();

	}

	//Only call after screen is restored.
	private void EndShopping() 
	{

		//Enable the players based on mode and if alive.
		if(!PlayerOne.GetComponent<Player>().IsDead) 
		{
			PlayerOne.SetActive(true);
		}

		if(GameMaster.instance.twoPlayersActive && !PlayerTwo.GetComponent<Player>().IsDead) 
		{
			PlayerTwo.SetActive(true);
		}

		shopActive = false;
		initialKeyPressed = false;
		waitingForPress = false;
		startWaitComplete = false;
	}

	public override void PauseGame()
	{
		if(shopActive)
		{
			mState = State.Paused;
			GetComponent<ShopScript>().ShopCanvas.SetActive(false);
			PauseMenu.gameObject.SetActive(!PauseMenu.gameObject.activeSelf);
		}
		else 
		{
			base.PauseGame();
		}
	}

	public override void UnpauseGame()
	{
		if(shopActive)
		{
			mState = State.Playing;
			GetComponent<ShopScript>().ShopCanvas.SetActive(true);
			PauseMenu.gameObject.SetActive(!PauseMenu.gameObject.activeSelf);
		}
		else 
		{
			base.UnpauseGame();
		}
	}

	public int GetPlayerOnePoints() 
	{
		return PlayerOne.GetComponent<Player>().GetScore();
	}
	public int GetPlayerTwoPoints() 
	{
		return PlayerTwo.GetComponent<Player>().GetScore();
	}
}
