using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[ExecuteInEditMode]
public class Arena : MonoBehaviour
{
    [SerializeField]
    private Camera Cam;

    public static float Width { get; private set; }
    public static float Height { get; private set; }

    void Update()
    {
#if UNITY_EDITOR 
        if (!Application.isPlaying)
        {
            Calculate();
        }
#endif
    }

    public void Calculate()
    {
        if (Cam != null)
        {
            Height = CameraUtils.FrustumHeightAtDistance(Cam.farClipPlane - 1.0f, Cam.fieldOfView);
            Width = Height * Cam.aspect;
            transform.localScale = new Vector3(Width * 0.1f, 1.0f, Height * 0.1f);
        }
    }

	public static Vector3 RandomArenaBorderPosition() 
	{
		/*
			Can spawn anywhere on the 4 vectors:
				min width to max width (min height)
				min width to max width (max height)
				min height to max height (min width)
				min height to max height (max width)
		*/
		float spawnOffset = 5.0f;

		Vector3 bottomLeft = new Vector3(-Arena.Width / 2, 0.0f, -Arena.Height / 2);
		Vector3 bottomRight = new Vector3(Arena.Width / 2, 0.0f, -Arena.Height / 2);
		Vector3 topLeft = new Vector3(-Arena.Width / 2, 0.0f, Arena.Height / 2);
		Vector3 topRight = new Vector3(Arena.Width / 2, 0.0f, Arena.Height / 2);

		//A random number to select which vector
		int selection = Random.Range(0, 4);

		//A random number to select a position along the selected vector.
		Vector3 result = new Vector3();
		float randomPositionNumber = Random.Range(0.0f, 1.0f);

		switch(selection) 
		{
			case 0:
				//Top Horizontal
				result = Vector3.Lerp(topLeft, topRight, randomPositionNumber) + new Vector3(0,0,spawnOffset);
				break;
			case 1:
				//Bottom horizontal
				result = Vector3.Lerp(bottomLeft, bottomRight, randomPositionNumber) + new Vector3(0,0,-spawnOffset);
				break;
			case 2:
				//Left vertical
				result = Vector3.Lerp(bottomLeft, topLeft, randomPositionNumber) + new Vector3(-spawnOffset,0,0);
				break;
			case 3:
				//Right vertical
				result = Vector3.Lerp(bottomRight, topRight, randomPositionNumber) + new Vector3(spawnOffset,0,0);
				break;
		}

		result.y = 3.5f; //Collider offset - asteroid and player collision height.
		return result;
	}
}
