using UnityEngine;
using System.Collections;

public class scottAI : MonoBehaviour {
    public Rigidbody rigid;
    public CapsuleCollider body;
    // Use this for initialization
    void Start ()
    {
        rigid = GetComponent<Rigidbody>();
        body = GetComponent<CapsuleCollider>();
    }
	
	void FixedUpdate () {
        int x = Mathf.RoundToInt(CameraScrolling.S.transform.position.x);
        int y = Mathf.RoundToInt(CameraScrolling.S.transform.position.y);
        int i0 = x - 27;
        int i1 = x + 27;
        int j0 = y - 27;
        int j1 = y + 27;
        if (transform.position.x < i0 || transform.position.x > i1
            || transform.position.y < j0 || transform.position.y > j1)
        {
            Destroy(gameObject);
        } else if (transform.position.x < i0 + 11 || transform.position.x > i1 - 11
            || transform.position.y < j0 + 11 || transform.position.y > j1 - 11)
        {
            rigid.constraints = RigidbodyConstraints.FreezeAll;
        } else
        {
            rigid.constraints = RigidbodyConstraints.FreezeRotation | RigidbodyConstraints.FreezePositionZ;
        }
        if (transform.position.x == Mathf.Round(transform.position.x))
        {
            transform.position = transform.position + new Vector3(-.05f, 0f, 0f);
        }
        if (Physics.Raycast(transform.position, Vector3.down, 1.5f, LayerMask.GetMask("Ground")) ||
            Physics.Raycast(transform.position + new Vector3(-1.5f, 0f, 0f), Vector3.down, 1.5f, LayerMask.GetMask("Ground")) ||
            Physics.Raycast(transform.position + new Vector3(1.5f, 0f, 0f), Vector3.down, 1.5f, LayerMask.GetMask("Ground")))
        {
            transform.position = new Vector3(transform.position.x, Mathf.Round(transform.position.y * 2f) / 2f);
        }
    }

    void OnTriggerEnter(Collider other)
    {
        if (other.tag == "chargedShot" )
        {
            float charge = other.GetComponent<SamusBullet>().charge;
            Vector3 vel = rigid.velocity;
            float bulletVelX = other.GetComponent<Rigidbody>().velocity.x;
            float bulletVelY = other.GetComponent<Rigidbody>().velocity.y;
            if (bulletVelX != 0)
            {
                vel.x += (bulletVelX > 0 ? 1 : -1) * charge * 1f;
            }
            if (bulletVelY != 0)
            {
                vel.y += (bulletVelY > 0 ? 1 : -1) * charge * 1f;
            }
            rigid.velocity = vel;
        }
    }
}
