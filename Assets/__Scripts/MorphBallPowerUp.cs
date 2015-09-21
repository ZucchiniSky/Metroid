using UnityEngine;
using System.Collections;

public class MorphBallPowerUp : MonoBehaviour {

	void OnTriggerEnter(Collider other)
	{
		if (other.tag == "Player") 
		{
			Samus.S.hasMorph = true;
			Destroy (gameObject);
		}
	}
}
