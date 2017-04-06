using UnityEngine.UI;
using UnityEngine;

public class MenuInfoLoader : MonoBehaviour {

	[SerializeField]
	private Text NameText;

	[SerializeField]
	private Text DescriptionText;

	[SerializeField]
	private Text ObjectiveText;

	[SerializeField]
	private Text HighScoreValueText;

	[SerializeField]
	private Toggle TwoPlayerToggle;

	[SerializeField]
	private Toggle PlayTutorialToggle;

	[SerializeField]
	private GameObject MainPanel;

	[SerializeField]
	private GameObject ModePanel;

	private GameModeInfo currentMode;
	private int ModeListLength;
	private int CurrentModeIndex = 0;

	void Start () {
		if(GameMaster.instance.MainMenuSelectionReturn)
		{
			MainPanel.SetActive(false);
			ModePanel.SetActive(true);
		}
		else 
		{
			GameMaster.instance.LoadData();
			GameMaster.instance.MainMenuSelectionReturn = true;
		}
		currentMode = GameMaster.instance.GameInfoArray[0];
		ModeListLength = GameMaster.instance.GameInfoArray.Length;
		LoadModeInfo();
	}

	public void NextMode() 
	{
		if(CurrentModeIndex + 1 == ModeListLength)
		{
			CurrentModeIndex = 0;
		}
		else 
		{
			CurrentModeIndex++;
		}
		LoadModeInfo();
	}

	public void PrevMode() 
	{
		if(CurrentModeIndex - 1 < 0)
		{
			CurrentModeIndex = ModeListLength - 1;
		}
		else 
		{
			CurrentModeIndex--;
		}
		LoadModeInfo();
	}

	private void LoadModeInfo() 
	{
		TwoPlayerToggle.isOn = GameMaster.instance.twoPlayersActive;
		PlayTutorialToggle.isOn = GameMaster.instance.playTutorial;

		currentMode = GameMaster.instance.GameInfoArray[CurrentModeIndex];
		NameText.text = currentMode.Name;
		DescriptionText.text = currentMode.Desc;
		ObjectiveText.text = currentMode.Objective;
		HighScoreValueText.text = System.Convert.ToString(currentMode.HighScore);
	}

	public void ToggleTwoPlayer() 
	{
		GameMaster.instance.twoPlayersActive = TwoPlayerToggle.isOn;
	}

	public void ToggleTutorial() 
	{
		GameMaster.instance.playTutorial = PlayTutorialToggle.isOn;
	}

	//On 'PLAY' button
	public void PlayGameMode() 
	{
		switch(CurrentModeIndex) 
		{
			case 0:
				GameMaster.instance.activeScene = GameMaster.ActiveScene.ClassicGame;
				break;
			case 1:
				GameMaster.instance.activeScene = GameMaster.ActiveScene.SurvivalGame;
				break;
			case 2:
				GameMaster.instance.activeScene = GameMaster.ActiveScene.DodgeGame;
				break;
		}
		GameMaster.instance.SwapScenes();
		GameMaster.instance.twoPlayersActive = TwoPlayerToggle.isOn;
		GameMaster.instance.playTutorial = PlayTutorialToggle.isOn;
	}
}
