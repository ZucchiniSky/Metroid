using UnityEngine;
using System.Collections;

public class HealthManager : MonoBehaviour {

	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {
        GUIText gt = this.GetComponent<GUIText>();
        gt.text = "" + Samus.S.health;
    }
}
