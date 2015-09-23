using UnityEngine;
using System.Collections;

public class Energy : MonoBehaviour {

	// Use this for initialization
	void Start () {
	
	}
	
	void FixedUpdate () {
        int x = Mathf.RoundToInt(CameraFollow.S.transform.position.x);
        int y = Mathf.RoundToInt(CameraFollow.S.transform.position.y);
        int i0 = x - 18;
        int i1 = x + 18;
        int j0 = y - 18;
        int j1 = y + 18;
        if (transform.position.x < i0 || transform.position.x > i1 
            || transform.position.y < j0  || transform.position.y > j1)
        {
            Destroy(gameObject);
        }
    }
    void OnTriggerEnter(Collider other)
    {
        if (other.tag == "Player")
        {
            Samus.S.health += 5;
            Destroy(gameObject);
        }


    }
}
