using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class PauseHandler : MonoBehaviour
{

	private List<GameObject> rootObjects;
	private List<GameObject> rootObjectsWithRB;
	private List<Quaternion> rotations;
	private List<Vector3> velocities;
	private Scene scene;

	void Awake()
	{
		rotations = new List<Quaternion>();
		velocities = new List<Vector3>();
		rootObjects = new List<GameObject>();
		rootObjectsWithRB = new List<GameObject>();
	}

	public void HandlePause(GameModeManagerBase instance)
	{
		//clear the preexisting data for overwrite if saving and not loading.
		if(instance.mState == GameModeManagerBase.State.Paused)
		{
			rotations.Clear();
			velocities.Clear();
			rootObjects.Clear();
			rootObjectsWithRB.Clear();

			//Get root objects in scene
			scene = SceneManager.GetActiveScene();
			scene.GetRootGameObjects(rootObjects);

			//Iterate root objects, save their rotations & velocities, then freeze them
			for(int i = 0;i < rootObjects.Count;i++)
			{
				GameObject gameObject = rootObjects[i];
				Rigidbody objectRb = gameObject.GetComponent<Rigidbody>();

				if(objectRb != null)
				{
					rootObjectsWithRB.Add(rootObjects[i]);
					velocities.Add(objectRb.velocity);
					rotations.Add(objectRb.rotation);
					objectRb.isKinematic = true;
				}
			}
		}

		//Read the preexisting data for resetting rigidbodies when loading.
		if(instance.mState == GameModeManagerBase.State.Playing)
		{
			for(int i = 0; i < rootObjectsWithRB.Count; i++) 
			{
				rootObjectsWithRB[i].GetComponent<Rigidbody>().isKinematic = false;
				rootObjectsWithRB[i].GetComponent<Rigidbody>().velocity = velocities[i];
				rootObjectsWithRB[i].GetComponent<Rigidbody>().rotation = rotations[i];
			}
		}
	}
}