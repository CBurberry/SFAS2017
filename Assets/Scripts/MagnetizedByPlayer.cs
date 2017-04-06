using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Rigidbody))]
public class MagnetizedByPlayer : MonoBehaviour
{

    [SerializeField]
    private float MinimumDistance = 1.0f;

	[SerializeField]
	private float AttractionForce = 1000.0f;

    private Player mPlayer;
    private Rigidbody mBody;

    void Awake()
    {
        mPlayer = FindObjectOfType<Player>();
        mBody = GetComponent<Rigidbody>();
    }

	void Update()
    {
        if( mPlayer != null)
        {
            Vector3 difference = mPlayer.transform.position - transform.position;
            if( difference.magnitude <= MinimumDistance )
            {
                mBody.AddForce(difference * AttractionForce * Time.deltaTime);
            }
        }		
	}
}
