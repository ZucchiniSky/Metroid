using UnityEngine;
using System.Collections;

public class LongBeamPowerUp : MonoBehaviour {

    void OnTriggerEnter(Collider other)
    {
        if (other.tag == "Player")
        {
            Samus.S.hasLongBeam = true;
            Samus.S.bulletStopDist = 20f;
            Destroy(gameObject);
        }
    }
}
