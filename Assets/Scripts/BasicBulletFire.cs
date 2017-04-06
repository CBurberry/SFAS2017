using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BasicBulletFire : MonoBehaviour {

	public float fireRate = 0.5f;
	public float nextFire = 0.0f;

	[SerializeField]
	private float bulletSpeed;

	[SerializeField]
	private Transform shotSpawn;

	private GameObject player;
	private Rigidbody rb;
	private GameModeManagerBase instance;

	void Awake() 
	{
		GameModeManagerBase.SetManagerInstance(ref instance);
		player = transform.root.gameObject;
	}
	
	void Update () {
		if(instance == null) 
		{
			GameModeManagerBase.SetManagerInstance(ref instance);
		}

		if(instance.mState == GameModeManagerBase.State.Paused) 
		{
			return;
		}

		string fireButton = "";
		if(player.name == "Player 1") 
		{
			fireButton = "Fire1";
		}

		if(player.name == "Player 2") 
		{
			fireButton = "Fire2";
		}

		if(Input.GetButton(fireButton) && Time.time > nextFire) 
		{
			nextFire = Time.time + fireRate;
			GameObject clone = instance.GetBulletSpawner()
						.Spawn(shotSpawn.position, shotSpawn.rotation, player);
			rb = clone.GetComponent<Rigidbody>();
			rb.velocity = clone.transform.forward * bulletSpeed;
		}
	}
}
