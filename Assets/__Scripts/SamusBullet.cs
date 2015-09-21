using UnityEngine;
using System.Collections;

//1. Stop the bullet after a distance of 3
//2. Stop the bullet if it hits the Ground or enemy or something else

public class SamusBullet : MonoBehaviour {
    public float bulletStopDist = 3f;
    
    Vector3 bulletOrigin; 

    void Start()
    {
        bulletOrigin = transform.position;
    }
    void FixedUpdate()
    {
        float dist = (transform.position - bulletOrigin).magnitude;
        if(dist >= bulletStopDist)
        {
            Destroy(gameObject);
        }
    }

	void OnTriggerEnter(Collider other)
    {
        Destroy(gameObject);
    }
}
