using System.Collections;
using UnityEngine;
using UnityEngine.UI;

public abstract class GameModeManagerBase : MonoBehaviour {

	public enum State { Paused, Playing }

	[SerializeField]
    protected Arena Arena;

	[SerializeField]
	protected GameObject PlayerOne;

	[SerializeField]
	protected GameObject PlayerTwo;

	[SerializeField]
	protected Canvas PlayerUICanvas;

	[SerializeField]
	protected Canvas PauseMenu;

	[SerializeField]
	protected Canvas WaitScreen;

	[SerializeField]
	protected Canvas GameOverScreen;

	[SerializeField]
	protected Canvas TutorialScreen;

	[SerializeField]
	protected Canvas ControlScreen;

	[SerializeField]
	protected GameObject FadePanel;

	[HideInInspector]
    public State mState;

	protected bool initialKeyPressed = false;
	protected bool waitingForPress = false;
	protected bool startWaitComplete = false;
	protected bool startGame = false;
	protected BasicBulletSpawner BulletSpawner;
	protected bool isGameOver = false;

	protected virtual void Awake() 
	{
		if(GameMaster.instance.playTutorial)
		{
			ShowTutorial();
		}
		else 
		{
			startGame = true;
		}
	}
	protected virtual void Start() 
	{
		Arena.Calculate();
		if(GameMaster.instance.twoPlayersActive == false)
		{
			PlayerTwo.GetComponent<Player>().playerUI.SetActive(false);
			PlayerTwo.SetActive(false);
		}
		else 
		{
			PlayerTwo.GetComponent<Player>().playerUI.SetActive(true);
			PlayerTwo.SetActive(true);
		}
	}
	protected virtual void Update() 
	{
		if(Input.GetKeyDown(KeyCode.Escape)) 
		{
			ReturnToMenu();
		}
	}

	public static void SetManagerInstance(ref GameModeManagerBase instance) 
	{
		switch(GameMaster.instance.activeScene) 
		{
			case GameMaster.ActiveScene.ClassicGame:
				instance = ClassicManager.instance;
				break;
			case GameMaster.ActiveScene.SurvivalGame:
				instance = SurvivalManager.instance;
				break;
			case GameMaster.ActiveScene.DodgeGame:
				instance = DodgeManager.instance;
				break;
		}
	}

	public void ShowTutorial() 
	{
		SpriteRenderer render = FadePanel.GetComponent<SpriteRenderer>();
		render.color = new Color(0.0f, 0.0f, 0.0f, 1.0f);
		PlayerUICanvas.gameObject.SetActive(false);
		WaitScreen.gameObject.SetActive(false);
		ControlScreen.gameObject.SetActive(false);
		TutorialScreen.gameObject.SetActive(true);
	}

	public void ShowControls() 
	{
		TutorialScreen.gameObject.SetActive(false);
		ControlScreen.gameObject.SetActive(true);
	}

	public void StartGame() 
	{
		SpriteRenderer render = FadePanel.GetComponent<SpriteRenderer>();
		render.color = new Color(0.0f, 0.0f, 0.0f, 0.0f);
		PlayerUICanvas.gameObject.SetActive(true);
		WaitScreen.gameObject.SetActive(true);
		startGame = true;
		TutorialScreen.gameObject.SetActive(false);
	}

	protected virtual void PressAnyKeyToStart() 
	{
		//If not waiting and initial key was not pressed
		if(!waitingForPress && !initialKeyPressed)
		{
			mState = State.Paused;

			//Put up "press to start" panel
			WaitScreen.gameObject.SetActive(true);
			StartCoroutine(WaitForAnyKey());
			StartCoroutine(BlinkPressText());
			waitingForPress = true;
		}
		//If waiting for keypress and initial key was not pressed already.
		else if(waitingForPress && !initialKeyPressed)
		{
			return;
		}
		//InitialKeyPressed == true;
		else 
		{	if(waitingForPress)
			{
				//Hide panel
				WaitScreen.gameObject.SetActive(false);
				mState = State.Playing;
				waitingForPress = false;
				startWaitComplete = true;
			}
		}
	}

	protected virtual IEnumerator BlinkPressText() 
	{
		float blinkTimer = 0.0f;
		while(!initialKeyPressed)
		{
			blinkTimer += Time.deltaTime;
			if(blinkTimer > 1.0f) 
			{
				//Toggle press text.
				WaitScreen.GetComponentInChildren<Text>().enabled = 
					!WaitScreen.GetComponentInChildren<Text>().enabled;
				blinkTimer = 0.0f;
			}
			yield return null;
		}
		yield return null;
	}

	protected virtual IEnumerator WaitForAnyKey() 
	{
		while(!Input.anyKey) 
		{
			yield return null;
		}
		initialKeyPressed = true;
		yield return null;
	}

	public virtual void PauseGame()
	{
		mState = State.Paused;
		PauseMenu.gameObject.SetActive(!PauseMenu.gameObject.activeSelf);
		gameObject.GetComponent<PauseHandler>().HandlePause(this);
	}

	public virtual void UnpauseGame() 
	{
		mState = State.Playing;
		PauseMenu.gameObject.SetActive(!PauseMenu.gameObject.activeSelf);
		gameObject.GetComponent<PauseHandler>().HandlePause(this);
	}

	public void ReturnToMenu() 
	{
		GameMaster.instance.activeScene = GameMaster.ActiveScene.MainMenu;
		GameMaster.instance.SwapScenes();
	}

	protected virtual bool CheckGameOver()
	{
		if(!PlayerOne.activeSelf && !PlayerTwo.activeSelf) 
		{
			isGameOver = true;
		}
		return isGameOver;
	}

	public virtual void GameOver() 
	{
		//Disable player UI score components
		PlayerOne.GetComponent<Player>().playerUI.SetActive(false);
		PlayerTwo.GetComponent<Player>().playerUI.SetActive(false);

		//Enable GameOver Screen & get references to its text elements.
		GameOverScreen.gameObject.SetActive(true);
		Text bestScore = GameOverScreen.transform.GetChild(1).GetChild(0).GetChild(0).gameObject.GetComponent<Text>();
		bestScore.gameObject.SetActive(true);
		Text totalScore = GameOverScreen.transform.GetChild(1).GetChild(0).GetChild(1).gameObject.GetComponent<Text>();
		totalScore.gameObject.SetActive(true);
		Text newHighScore = GameOverScreen.transform.GetChild(1).GetChild(0).GetChild(2).GetChild(0).gameObject.GetComponent<Text>();

		//Load the previous highscore
		int index = 0;
		switch(GameMaster.instance.activeScene) 
		{
			case GameMaster.ActiveScene.ClassicGame:
				index = 0;
				break;
			case GameMaster.ActiveScene.SurvivalGame:
				index = 1;
				break;
			case GameMaster.ActiveScene.DodgeGame:
				index = 2;
				break;
		}

		int prevTotal = GameMaster.instance.GameInfoArray[index].HighScore;
		bestScore.text = "Best Score:\t" + System.Convert.ToString(prevTotal);
		int newTotal = PlayerOne.GetComponent<Player>().GetScore() +  PlayerTwo.GetComponent<Player>().GetScore();
		totalScore.text = "Total Score:\t" + System.Convert.ToString(newTotal);
		if(newTotal > prevTotal) 
		{
			newHighScore.gameObject.SetActive(true);
			StartCoroutine(FlashNHSText(newHighScore));

			//Update the GameMaster with new highscore
			GameMaster.instance.UpdateHighScore(index, newTotal);
		}
	}

	protected virtual IEnumerator FlashNHSText(Text textElement)
	{
		float flashDuration = 5.0f;
		float switchDuration = 0.3f;
		float timer = 0.0f;
		float subtimer = 0.0f;
		bool bSwitch = false;
		Color c1 = new Color(0.9f, 1.0f, 0.0f, 1.0f);
		Color c2 = new Color(0.12f, 1.0f, 0.0f, 1.0f);

		while(timer < flashDuration) 
		{
			if(subtimer > switchDuration) 
			{
				subtimer = 0.0f;
				switch(bSwitch) 
				{
					case true:
						textElement.color = c1;
						bSwitch = false;
						break;
					case false:
						textElement.color = c2;
						bSwitch = true;
						break;
				}
			}
			timer += Time.deltaTime;
			subtimer += Time.deltaTime;
			yield return null;
		}
		textElement.color = c1;
		yield return null;
	}

	public GameObject GetPlayerOne() 
	{
		return PlayerOne;
	}

	public GameObject GetPlayerTwo() 
	{
		return PlayerTwo;
	}

	public BasicBulletSpawner GetBulletSpawner() 
	{
		return BulletSpawner;
	}
}
