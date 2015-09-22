using UnityEngine;
using System.Collections;

public class MissileItemPowerUp : MonoBehaviour {

    void OnTriggerEnter(Collider other)
    {
        if (other.tag == "Player")
        {
            Samus.S.hasMissiles = true;
            Samus.S.missiles += 5;
            Samus.S.maxMissiles += 5;
            Destroy(gameObject);
        }
    }
}
