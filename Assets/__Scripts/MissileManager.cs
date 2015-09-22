using UnityEngine;
using System.Collections;

public class MissileManager : MonoBehaviour {

	// Use this for initialization
	void Start () {
	
	}

    // Update is called once per frame
    void Update()
    {
        GUIText gt = this.GetComponent<GUIText>();
        if (Samus.S.hasMissiles)
        {
            
            gt.text = "M " + ("000" + Samus.S.missiles).Substring((""+Samus.S.missiles).Length);
        }
        else
        {
            gt.text = "";
        }
    }
}
