using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//Jumps the object when outside of arena transform bounds
public class WrapPosition : MonoBehaviour
{
	
	void Update ()
    {
        Vector3 position = transform.position;

        if (position.x < Arena.Width * -0.5f)
        {
            position.x += Arena.Width;
        }
        else if (position.x > Arena.Width * 0.5f)
        {
            position.x -= Arena.Width;
        }

        if (position.z < Arena.Height * -0.5f)
        {
            position.z += Arena.Height;
        }
        else if (position.z > Arena.Height * 0.5f)
        {
            position.z -= Arena.Height;
        }

        transform.position = position;
    }
}
