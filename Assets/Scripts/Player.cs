using UnityEngine;
using UnityEngine.UI;

[RequireComponent(typeof(Rigidbody))]
public class Player : MonoBehaviour
{
	[SerializeField]
	private float ContactRepelForce;

	public float Speed;
	public float TurnSpeed;
	public int MaxLives;
	public GameObject playerUI;

	[HideInInspector]
	public int currentLives;

	[HideInInspector]
	public Lifebar lifebar;

	[HideInInspector]
	public bool IsDead;

    private Rigidbody mBody;
	private Pickup pickupSlot;
	private float maxVelocity = 35.0f;
	private int score = 0;
	private Text scoreText;
	private GameObject thrustIcon;
	private float iconVisibleLimit = 0.08f;
	private float iconVisibleTime = 0.0f;
	private float flickerDuration = -0.08f;
	private BasicBulletFire bulletFireRef;
	private GameModeManagerBase instance;

    void Awake()
    {
		scoreText = playerUI.GetComponentInChildren<Text>();
		lifebar = playerUI.GetComponentInChildren<Lifebar>();

		Vector2[] meshShape = {
			new Vector2(0, 3),
			new Vector2(2, -2),
			new Vector2(0, -1),
			new Vector2(-2, -2)
		};

		// Use the triangulator to get indices for creating mesh collider
        Triangulator tr = new Triangulator(meshShape);
        int[] indices = tr.Triangulate();
 
        // Create the Vector3 vertices
        Vector3[] vertices = new Vector3[meshShape.Length];
        for (int i=0; i<vertices.Length; i++) {
            vertices[i] = new Vector3(meshShape[i].x, 0, meshShape[i].y);
        }

		// Create the mesh
        Mesh msh = new Mesh();
        msh.vertices = vertices;
        msh.triangles = indices;
        msh.RecalculateNormals();
        msh.RecalculateBounds();

		//Initialise collider and renderer.
		MeshFilter filter = GetComponent<MeshFilter>();
		filter.mesh = msh;
		MeshCollider collider = GetComponent<MeshCollider>();
		collider.sharedMesh = msh;
        mBody = GetComponent<Rigidbody>();

		//Set lives count
		currentLives = MaxLives;
		lifebar.SetLife(currentLives);
		thrustIcon = transform.GetChild(1).gameObject;
		IsDead = false;
    }

    void Update()
    {
		if(instance == null) 
		{
			GameModeManagerBase.SetManagerInstance(ref instance);
		}

		//Needs to be changed to the base class
		if(instance.mState == GameModeManagerBase.State.Paused) 
		{
			return;
		}
		ApplyThrustEffect();
    }

	private void ApplyThrustEffect() 
	{
		float forwardVelocity = 0.0f;
		if(name == "Player 1") 
		{
			forwardVelocity = Input.GetAxisRaw("Vertical_p1");
		}
		if(name == "Player 2") 
		{
			forwardVelocity = Input.GetAxisRaw("Vertical_p2");
		}
		bool oldState = thrustIcon.activeSelf;
		bool state = (forwardVelocity == 1) ? true : false;
		if(state && iconVisibleTime >= 0.0f && iconVisibleTime < iconVisibleLimit)
		{
			iconVisibleTime += Time.deltaTime;
			thrustIcon.SetActive(state);
		}
		else if(state && iconVisibleTime < 0.0f)
		{
			iconVisibleTime += Time.deltaTime;
			thrustIcon.SetActive(false);
		}
		else 
		{
			iconVisibleTime = flickerDuration;
			thrustIcon.SetActive(false);
		}
	}

	void FixedUpdate() 
	{
		float horizontalInput = 0.0f;
		float forwardVelocity = 0.0f;

		if(instance == null) 
		{
			GameModeManagerBase.SetManagerInstance(ref instance);
		}

		if(instance.mState == GameModeManagerBase.State.Paused) 
		{
			return;
		}

		if(name == "Player 1") 
		{
			horizontalInput = Input.GetAxis("Horizontal_p1");
			forwardVelocity = Input.GetAxisRaw("Vertical_p1");	
		}

		if(name == "Player 2") 
		{
			horizontalInput = Input.GetAxis("Horizontal_p2");
			forwardVelocity = Input.GetAxisRaw("Vertical_p2");	
		}

		transform.RotateAround(transform.position, transform.up, horizontalInput * TurnSpeed);
		mBody.velocity += (transform.forward * Mathf.Clamp(forwardVelocity, 0.0f, 1.0f) * Speed);
		mBody.velocity = new Vector3(
							Mathf.Clamp(mBody.velocity.x, -maxVelocity, maxVelocity),
							0.0f,
							Mathf.Clamp(mBody.velocity.z, -maxVelocity, maxVelocity));
	}

	void OnTriggerEnter(Collider other) 
	{
		//Check if layer is asteroid.
		if(other.gameObject.layer == 8) 
		{
			Vector3 forceVec = -mBody.velocity.normalized * ContactRepelForce;
			mBody.AddForce(forceVec, ForceMode.Acceleration);
			LoseLife();
		}

		if(other.CompareTag("Shooting Asteroid")) 
		{
			gameObject.SetActive(false);
			lifebar.SetLife(0);
			IsDead = true;
			//Play particle effects/sound/screenshake etc.
		}

		if(other.CompareTag("UFO")) 
		{
			//destroy ufo, add score, lose life
			other.gameObject.SetActive(false);
			LoseLife();
		}
	}

	public void LoseLife() 
	{
		//Need to cahnge to base class
		if(currentLives == 0)
		{
			gameObject.SetActive(false);
			IsDead = true;
			//Pay particle effect or sound for player destruction here.
		}
		else 
		{
			currentLives--;
			lifebar.LoseLife();
		}
	}

	public void GainLife() 
	{
		currentLives++;
		lifebar.GainLife();
	}

	//For bullet use, updates UI.
	public void AddToScore(int value) 
	{
		score += value;
		scoreText.text = "Score: " + score;
	}

	public void ResetScore() 
	{
		score = 0;
		scoreText.text = "Score: 0";
	}

	public int GetScore() 
	{
		return score;
	}

	public int GetCurrentLives() 
	{
		return currentLives;
	}

	public void UpgradeFireRate() 
	{
		bulletFireRef = GetComponent<BasicBulletFire>();
		bulletFireRef.fireRate -= 0.05f;
	}

	public void UpgradeSpeed() 
	{
		Speed += 0.1f;
	}

	public void UpgradeTurnSpeed() 
	{
		TurnSpeed += 1.0f;
	}
}
