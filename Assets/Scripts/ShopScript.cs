using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ShopScript : MonoBehaviour {

	public GameObject ShopCanvas;

	[SerializeField]
	private GameObject ScrollViewContent;

	[SerializeField]
	private Text PlayerIDText;

	[SerializeField]
	private Text PointsText;

	[SerializeField]
	private Text ItemNameText;

	[SerializeField]
	private Text ItemDescText;

	[SerializeField]
	private Button BuyButton;

	[SerializeField]
	private Text NoMoneyText;

	private PlayerUpgradeTracker playerOneData = new PlayerUpgradeTracker();
	private PlayerUpgradeTracker playerTwoData = new PlayerUpgradeTracker();

	private ItemDetails spdUpgrade = new ItemDetails();
	private ItemDetails firerateUpgrade = new ItemDetails();
	private ItemDetails turnSpeedUpgrade = new ItemDetails();
	private ItemDetails fullHeal = new ItemDetails();
	private ItemDetails allyRevive = new ItemDetails();

	private int currentlyAccessingPlayer = 0;
	private int currentlySelectedItem = 0;
	private ItemDetails[] ItemDetails = new ItemDetails[5];
	private PlayerUpgradeTracker[] PlayerData = new PlayerUpgradeTracker[2];
	private Player[] Players = new Player[2];

	void Awake() 
	{
		//Initialise all description texts and store them.
		spdUpgrade.name = "Thrust Upgrade";
		spdUpgrade.cost = "1000";
		spdUpgrade.type = "Upgrade Level ";
		spdUpgrade.effect = "Increase Thrust power by 10%.";

		firerateUpgrade.name = "Fire Rate Upgrade";
		firerateUpgrade.cost = "1000";
		firerateUpgrade.type = "Upgrade Level ";
		firerateUpgrade.effect = "Increase Fire Rate by 10%.";

		turnSpeedUpgrade.name = "Turn Speed Upgrade";
		turnSpeedUpgrade.cost = "500";
		turnSpeedUpgrade.type = "Upgrade Level ";
		turnSpeedUpgrade.effect = "Increase Turn Speed by 10%.";

		fullHeal.name = "Ship Repair";
		fullHeal.cost = "2000";
		fullHeal.type = "Instant Use";
		fullHeal.effect = "Repair ship back to maximum lives.";

		allyRevive.name = "Ally Revival";
		allyRevive.cost = "2500";
		allyRevive.type = "Instant Use";
		allyRevive.effect = "Revive a dead ally back to full health.";

		ItemDetails[0] = spdUpgrade;
		ItemDetails[1] = firerateUpgrade;
		ItemDetails[2] = turnSpeedUpgrade;
		ItemDetails[3] = fullHeal;
		ItemDetails[4] = allyRevive;

		ShopCanvas.SetActive(false);
		ShopCanvas.transform.GetChild(0).gameObject.SetActive(false);
		ShopCanvas.transform.GetChild(1).gameObject.SetActive(false);

		Players[0] = gameObject.GetComponent<SurvivalManager>().GetPlayerOne().GetComponent<Player>();
		Players[1] = gameObject.GetComponent<SurvivalManager>().GetPlayerTwo().GetComponent<Player>();

		PlayerData[0] = playerOneData;
		PlayerData[1] = playerTwoData;
	}

	public void StartShopScript() 
	{
		currentlySelectedItem = 0;
		if(Players[0].gameObject.activeSelf)
		{
			currentlyAccessingPlayer = 0;
		}
		else 
		{
			currentlyAccessingPlayer = 1;
		}
		ShowModalWindow();
	}

	public void OpenShop() 
	{
		Players[0].GetComponent<Player>().playerUI.gameObject.SetActive(false);
		Players[1].GetComponent<Player>().playerUI.gameObject.SetActive(false);
		if(!GameMaster.instance.twoPlayersActive || Players[0].gameObject.activeSelf && Players[1].gameObject.activeSelf) 
		{
			ScrollViewContent.transform.GetChild(4).gameObject.SetActive(false);
		}

		HideModalWindow();
		currentlySelectedItem = 0;
		PointsText.text = "Points: " + Players[currentlyAccessingPlayer].GetScore().ToString();
		LoadBuyPanel(currentlySelectedItem);
		ShopCanvas.transform.GetChild(1).gameObject.SetActive(true);
	}

	private void ShowModalWindow() 
	{
		ShopCanvas.SetActive(true);
		if(GameMaster.instance.twoPlayersActive)
		{
			PlayerIDText.gameObject.SetActive(true);
			PlayerIDText.text = "(Player " + (currentlyAccessingPlayer+1) + ")";
		}
		else 
		{
			PlayerIDText.gameObject.SetActive(false);
		}
		Players[currentlyAccessingPlayer].GetComponent<Player>().playerUI.gameObject.SetActive(true);
		ShopCanvas.transform.GetChild(1).gameObject.SetActive(false);
		ShopCanvas.transform.GetChild(0).gameObject.SetActive(true);
	}

	private void HideModalWindow() 
	{
		ShopCanvas.transform.GetChild(0).gameObject.SetActive(false);
	}
		

	public void LoadBuyPanel(int index) 
	{
		PointsText.text = "Points: " + Players[currentlyAccessingPlayer].GetComponent<Player>().GetScore().ToString();
		NoMoneyText.gameObject.SetActive(false);
		currentlySelectedItem = index;
		ItemDetails currentItem = ItemDetails[currentlySelectedItem];
		ItemNameText.text = currentItem.name;
		BuyButton.interactable = true;

		switch(currentlySelectedItem) 
		{
			case 0:
			case 1:
			case 2:
				ItemDescText.text = "Cost: " + currentItem.cost + "\n\nType: " 
					+ currentItem.type;

				if(PlayerData[currentlyAccessingPlayer].upgradeLevels[currentlySelectedItem] == 6)
				{
					ItemDescText.text += (PlayerData[currentlyAccessingPlayer].upgradeLevels[currentlySelectedItem] - 1)
					+ "\n\nYOU HAVE REACHED THE MAXIMUM UPGRADE FOR THIS ITEM!";
					BuyButton.interactable = false;
				}
				else 
				{
					ItemDescText.text += PlayerData[currentlyAccessingPlayer].upgradeLevels[currentlySelectedItem] + "\n\nEffect:\n"
					+ currentItem.effect;
				}
				break;
			case 3:
				ItemDescText.text = "Cost: " + currentItem.cost + "\n\nType: " + currentItem.type + "\n\nEffect:\n" + currentItem.effect;
				if(Players[currentlyAccessingPlayer].lifebar.Count() == Players[currentlyAccessingPlayer].MaxLives) 
				{
					BuyButton.interactable = false;
				}
				break;
			case 4:
				ItemDescText.text = "Cost: " + currentItem.cost + "\n\nType: " + currentItem.type + "\n\nEffect:\n" + currentItem.effect;
				if(!GameMaster.instance.twoPlayersActive || Players[0].lifebar.Count() != 0 && Players[1].lifebar.Count() != 0 ) 
				{
					BuyButton.interactable = false;
				}
				break;
		}

		if(Players[currentlyAccessingPlayer].GetScore() < System.Convert.ToInt32(currentItem.cost)) 
		{
			BuyButton.interactable = false;
			NoMoneyText.gameObject.SetActive(true);
		}
	}

	private void UpdatePoints() 
	{
		int cost = System.Convert.ToInt32(ItemDetails[currentlySelectedItem].cost);
		int newPoints = Players[currentlyAccessingPlayer].GetScore() - cost;
		PointsText.text = "Points: " + newPoints.ToString();
		Players[currentlyAccessingPlayer].AddToScore(-cost);
	}

	public void BuyItem() 
	{
		UpdatePoints();
		ApplyItem();
		LoadBuyPanel(currentlySelectedItem);
	}

	private void ApplyItem() 
	{
		if(currentlySelectedItem >= 0 && currentlySelectedItem < 3) 
		{
			//Upgrade the update number for load buy panel.
			PlayerData[currentlyAccessingPlayer].upgradeLevels[currentlySelectedItem]++;
		}
		Player currentPlayer = Players[currentlyAccessingPlayer];

		switch(currentlySelectedItem) 
		{
			case 0:
				currentPlayer.UpgradeSpeed();
				break;
			case 1:
				currentPlayer.UpgradeFireRate();
				break;
			case 2:
				currentPlayer.UpgradeTurnSpeed();
				break;
			case 3:
				currentPlayer.lifebar.SetLife(currentPlayer.MaxLives);
				currentPlayer.currentLives = currentPlayer.MaxLives;
				break;
			case 4:
				if(currentlyAccessingPlayer == 0)
				{
					Players[1].lifebar.SetLife(Players[1].MaxLives);
					Players[1].currentLives = Players[1].MaxLives;
					Players[1].IsDead = false;
					Players[1].gameObject.SetActive(true);
				}
				else 
				{
					Players[0].lifebar.SetLife(Players[0].MaxLives);
					Players[0].currentLives = Players[0].MaxLives;
					Players[0].IsDead = false;
					Players[0].gameObject.SetActive(true);
				}
				break;
		}
	}

	public void ExitShop() 
	{
		//increase the currently accesing player value - wrap around.
		if(currentlyAccessingPlayer == 1)
		{
			currentlyAccessingPlayer = 0;
			ExitShopScript();
		}
		else 
		{
			currentlyAccessingPlayer++;
			if(GameMaster.instance.twoPlayersActive)
			{
				ShowModalWindow();
			}
			else 
			{
				ExitShopScript();
			}
		}
	}

	private void ExitShopScript() 
	{
		ShopCanvas.SetActive(false);
		ShopCanvas.transform.GetChild(0).gameObject.SetActive(false);
		ShopCanvas.transform.GetChild(1).gameObject.SetActive(false);

		Players[0].GetComponent<Player>().playerUI.gameObject.SetActive(true);
		if(GameMaster.instance.twoPlayersActive) 
		{
			Players[1].GetComponent<Player>().playerUI.gameObject.SetActive(true);
		}

		Players[0].transform.position = new Vector3(-5, 3, -5);
		Players[0].transform.rotation = Quaternion.Euler(0.0f, 0.0f, 0.0f);
		Players[0].GetComponent<Rigidbody>().velocity = new Vector3(0.0f, 0.0f, 0.0f);
		Players[1].transform.position = new Vector3(5, 3, -5);
		Players[1].transform.rotation = Quaternion.Euler(0.0f, 0.0f, 0.0f);
		Players[1].GetComponent<Rigidbody>().velocity = new Vector3(0.0f, 0.0f, 0.0f);


		//Resume round logic
		StartCoroutine(GetComponent<SurvivalManager>().ShopOutroSequence());
	}
}

//Simple data class for player upgrade panel to show upgrade level
public class PlayerUpgradeTracker
{
	public int[] upgradeLevels = {0, 0, 0};
}

//Simple data class for describing the item in the shop.
public class ItemDetails 
{
	public string name = "Item Name";
	public string cost = "-1";
	public string type = "POOF";
	public string effect = "POOF";
}


