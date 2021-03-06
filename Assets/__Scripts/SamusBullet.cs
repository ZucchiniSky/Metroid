﻿using UnityEngine;
using System.Collections;

//1. Stop the bullet after a distance of 3
//2. Stop the bullet if it hits the Ground or enemy or something else

public class SamusBullet : MonoBehaviour {
    public float charge = 0f;
    Vector3 bulletOrigin; 

    void Start()
    {
        bulletOrigin = transform.position;
    }
    void FixedUpdate()
    {
        
        float dist = (transform.position - bulletOrigin).magnitude;
        if (gameObject.transform.name == "SamusMissile(Clone)")
        {
            if (dist >= 40f)
            {
                Destroy(gameObject);
            }
        }
        else if (dist >= Samus.S.bulletStopDist)
        {
            Destroy(gameObject);
        }
        
    }
	void OnTriggerEnter(Collider other)
    {
        Destroy(gameObject);
    }
}
