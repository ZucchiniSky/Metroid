using UnityEngine;
using System.Collections;

public class SkreeExplosion : MonoBehaviour {
    private float timer = 20f;
	// Use this for initialization
	void Start () {
	    
	}
	
	void FixedUpdate () {
        if (timer <= 0)
            Destroy(gameObject);
        timer--;
	}
}
