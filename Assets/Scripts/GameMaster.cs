using System;
using UnityEngine;
using System.Runtime.Serialization.Formatters.Binary;
using System.IO;

public class GameMaster : MonoBehaviour {

	public static GameMaster instance;
	public enum ActiveScene { ClassicGame, SurvivalGame, DodgeGame, MainMenu };

	[HideInInspector]
	public ActiveScene activeScene;

	[HideInInspector]
	public bool twoPlayersActive;

	[HideInInspector]
	public bool playTutorial;

	[HideInInspector]
	public bool MainMenuSelectionReturn = false;

	[HideInInspector]
	public GameModeInfo[] GameInfoArray = new GameModeInfo[3];

	[SerializeField]
	private SceneLoader Loader;

	private void InstantiateGameInfo() 
	{
		GameInfoArray[0] = new GameModeInfo("Classic", 
							"Based on the original arcade game 'Asteroids'.", 
							"Aim to destroy as many aliens and asteroids as you can, lives are limited and as rounds progress the difficulty increases. " + 
							"A new life is gained every 2000 points (Max 6).", 
							0);
		GameInfoArray[1] = new GameModeInfo("Survival",
							"Fly between space stations and stay alive for as long as possible. Spend earned points at the space stations to get repairs and upgrades. " +
							"Similar to classic but with new powerups.",
							"Aim to destroy as many asteroids as you can, aliens are worth more points and be super careful of the shooting asteroids.",
							0);
		GameInfoArray[2] = new GameModeInfo("Dodgestroids", 
							"Dodgeball but with asteroids... only for the crazy pilots. ", 
							"Dodge the shooting asteroids as they come. There are no weapons provided. Good Luck.", 
							0);
	}

	void Awake() 
	{
		InstantiateGameInfo();
		activeScene = ActiveScene.MainMenu;
		instance = this;
	}

	public void SwapScenes() 
	{
		Loader.StartLoadNewCoroutine();
	}

	public void UpdateHighScore(int index, int newScore) 
	{
		//Update the GameMaster with new highscore
		GameInfoArray[index].HighScore = newScore;
		SaveData();
	}

	public void SaveData() 
	{
		BinaryFormatter bf = new BinaryFormatter();
		FileStream file = File.Create(Application.persistentDataPath + "/playerHighscores.dat");
		PlayerData data = new PlayerData();
		data.ClassicHighscore = GameInfoArray[0].HighScore;
		data.SurvivalHighscore = GameInfoArray[1].HighScore;
		data.DodgeHighscore = GameInfoArray[2].HighScore;

		bf.Serialize(file, data);
		file.Close();
	}

	public void LoadData() 
	{
		if(File.Exists(Application.persistentDataPath + "/playerHighscores.dat")) 
		{
			BinaryFormatter bf = new BinaryFormatter();
			FileStream file = File.Open(Application.persistentDataPath + "/playerHighscores.dat", FileMode.Open);
			PlayerData data = (PlayerData) bf.Deserialize(file);
			file.Close();

			GameInfoArray[0].HighScore = data.ClassicHighscore;
			GameInfoArray[1].HighScore = data.SurvivalHighscore;
			GameInfoArray[2].HighScore = data.DodgeHighscore;
		}
	}
}

//Simple data holding class for menu population and highscore tracking.
public class GameModeInfo {

	public string Name = "";
	public string Desc = "";
	public string Objective = "";
	public int HighScore = 0;

	public GameModeInfo(string n, string d, string o, int hs) 
	{
		Name = n;
		Desc = d;
		Objective = o;
		HighScore = hs;
	}
}

//Savedata format class.
[Serializable]
class PlayerData 
{
	public int ClassicHighscore;
	public int SurvivalHighscore;
	public int DodgeHighscore;
}
