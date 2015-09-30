using UnityEngine;
using System.Collections;

public class scottAI : MonoBehaviour {
    public Rigidbody rigid;
    public CapsuleCollider body;
    // Use this for initialization
    void Start () {
        rigid = GetComponent<Rigidbody>();
        body = GetComponent<CapsuleCollider>();
    }
	
	void FixedUpdate () {
        int x = Mathf.RoundToInt(CameraScrolling.S.transform.position.x);
        int y = Mathf.RoundToInt(CameraScrolling.S.transform.position.y);
        int i0 = x - 18;
        int i1 = x + 18;
        int j0 = y - 18;
        int j1 = y + 18;
        Vector3 vel = rigid.velocity;
        if (transform.position.x < i0 - 9 || transform.position.x > i1 + 9
            || transform.position.y < j0 - 9 || transform.position.y > j1 + 9)
        {
            Destroy(gameObject);
        }
        else if (transform.position.x < i0 || transform.position.x > i1
            || transform.position.y < j0 || transform.position.y > j1)
        {
            vel.x = 0;
            vel.y = 0;
        }
        rigid.velocity = vel;
    }
    void OnTriggerEnter(Collider other)
    {
        if (other.tag == "chargedShot" )
        {
            rigid.constraints = RigidbodyConstraints.FreezeRotation;
            float charge = other.GetComponent<SamusBullet>().charge;
            Vector3 vel = rigid.velocity;
            float bulletVel = other.GetComponent<Rigidbody>().velocity.x;
            bool right;
            if (bulletVel != 0)
            {
                right = bulletVel > 0;
            } else
            {
                right = other.transform.position.x < transform.position.x;
            }
            if (right)
            {
                vel.x += charge * 1f;
            }
            else
            {
                vel.x -= charge * 1f;
            }
            rigid.velocity = vel;
        }


    }
    void OnCollisionEnter(Collision other)
    {
        if (other.gameObject.tag == "Player")
        {
            rigid.constraints = RigidbodyConstraints.FreezeAll;
        }
    }
    void OnCollisionExit(Collision other)
    {
        if (other.gameObject.tag == "Player")
        {
            rigid.constraints = RigidbodyConstraints.FreezeAll;
        }
    }

}
