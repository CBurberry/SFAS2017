using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ClampPositionToArena : MonoBehaviour {
	
	void FixedUpdate() 
	{
		Rigidbody rb = gameObject.GetComponent<Rigidbody>();
		float cameraLowerHorizontalBound = -Arena.Width / 2 + Arena.Width / 15;
		float cameraUpperHorizontalBound = Arena.Width / 2 - Arena.Width / 15;
		float cameraLowerVerticalBound = -Arena.Height / 2 + Arena.Height / 15;
		float cameraUpperVerticalBound = Arena.Height / 2 - Arena.Height / 15;

		rb.position = new Vector3(
							Mathf.Clamp(rb.position.x, cameraLowerHorizontalBound, cameraUpperHorizontalBound),
							rb.position.y,
							Mathf.Clamp(rb.position.z, cameraLowerVerticalBound, cameraUpperVerticalBound));
	}
}
