using UnityEngine;

public class Asteroid : MonoBehaviour {

	private Rigidbody rb;
	private GameModeManagerBase instance;

	[HideInInspector]
	public AsteroidSpawner spawnerRef;
	[HideInInspector]
	public int scoreValue;
	[HideInInspector]
	public int splitDepth = 1;                         //Specifer for children splitting (children recieve value -1)

	public int initialScoreValue;						//Base score of large asteroids.

	[SerializeField]
	private float rotationPerFrame = 20.0f;				//Amount to rotate per frame.
	[SerializeField]
	private float rotationOffset = 10.0f;               //Offset for astroids to have varied rotations.
	[SerializeField]
	private float scaleOffset = 0.5f;					//Initial scaling offset bound.
	[SerializeField]
	private float childSpawnOffset;                     //Offset bound for spawn position of possible children.
	[SerializeField]
	private float childVelocityMultiplier;				//Speedup by child asteroids.
	[SerializeField]
	private float childScoreMultiplier;                 //Multiplier of score per child level.
	[SerializeField]
	private int childSpawnCount;

	Quaternion deltaRotation;

	//Define how many times it will split & the factor of splitting.!!!!

	void Awake() 
	{
		//Initialisation
		rb = GetComponent<Rigidbody>();
		ResetRotation();
		ResetScale();
		scoreValue = initialScoreValue;
	}

	void FixedUpdate() 
	{
		if(instance == null) 
		{
			GameModeManagerBase.SetManagerInstance(ref instance);
		}

		if(instance.mState != GameModeManagerBase.State.Paused) 
		{
			rb.MoveRotation(rb.rotation * deltaRotation);
		}
	}

	//Public velocity setter for use by spawner.
	public void SetVelocity(Vector2 force) 
	{
		if(rb == null) 
		{
			rb = GetComponent<Rigidbody>();
		}
		rb.velocity = new Vector3(force.x,0,force.y);
	}

	private void ResetRotation() 
	{
		//reset rotation
		float rotDegree = Random.Range(rotationPerFrame - rotationOffset, rotationPerFrame + rotationOffset);
		Vector3 eulerAngleVelocity = new Vector3(0, 0, rotDegree);
		deltaRotation = Quaternion.Euler(eulerAngleVelocity * Time.deltaTime);
	} 

	private void ResetScale() 
	{
		//reset scale
		float offset = Random.Range(0, scaleOffset);
		gameObject.transform.localScale = (Vector3.one + new Vector3(offset, 0, offset)) * 100;
	}

	//Behaviour on destruction, child spawns etc. - return to object pool
	public void Destroyed() 
	{

		//Play sound effect
		//Play particle effect
		//Apply any scoring mechanism usage here...


		if(splitDepth > 0) 
		{
			for(int i = 0; i < childSpawnCount; i++) 
			{
				SpawnChild();
			}
		}

		//Set self active so it can be reused from the pool.
		gameObject.SetActive(false);

		//Reset its scale and rotations based on inspector params.
		ResetRotation();
		ResetScale();
	}

	//Spawn children, set sizes, rotation, velocity, position.
	void SpawnChild() 
	{
		//Get inactive asteroid instance from the ObjectPool.
		GameObject child = spawnerRef.getAsteroidObject();

		//Set their transforms to self + small offset, change object scale.
		float offset = Random.Range(-childSpawnOffset, 0);
		Vector3 currentTransform = transform.position;
		child.transform.position = new Vector3(currentTransform.x + offset, 
												currentTransform.y, 
												currentTransform.z + offset);
		child.transform.localScale = transform.localScale * 0.6f;
		Vector3 copy = child.GetComponent<Rigidbody>().velocity;
		child.GetComponent<Rigidbody>().velocity = 
					new Vector3(copy.x * childVelocityMultiplier, 0.0f, copy.z * childVelocityMultiplier);

		Asteroid ast = child.GetComponent<Asteroid>();
		ast.spawnerRef = spawnerRef;
		ast.splitDepth = splitDepth - 1;
		ast.scoreValue = (int) (scoreValue * childScoreMultiplier);
	}
}
