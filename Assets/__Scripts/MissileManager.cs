using UnityEngine;
using System.Collections;

public class MissileManager : MonoBehaviour {

	// Use this for initialization
	void Start () {
	
	}
    
    void FixedUpdate()
    {
        GUIText gt = this.GetComponent<GUIText>();
        if (Samus.S.hasMissiles)
        {
            
            gt.text = "M " + ("000" + Samus.S.missiles).Substring((""+Samus.S.missiles).Length);
            if (Samus.S.usingMissiles)
            {
                gt.color = Color.yellow;
            }
            else
            {
                gt.color = Color.white;
            }
        }
        else
        {
            gt.text = "";
        }
        
    }
}
