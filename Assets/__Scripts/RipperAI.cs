using UnityEngine;
using System.Collections;

public class RipperAI : MonoBehaviour {
    
    public bool wallLeft = false;
    public bool wallRight = false;
    public float velX = 2f;
    public bool facingRight = true;


    public bool _______;
    public Rigidbody rigid;
    public CapsuleCollider body;
    public int groundPhysicsLayerMask;
    RigidbodyConstraints noRotYZ;
    Quaternion turnLeft = Quaternion.Euler(0, 180, 0);
    // Use this for initialization
    void Start () {
        rigid = GetComponent<Rigidbody>();
        body = GetComponent<CapsuleCollider>();
        groundPhysicsLayerMask = LayerMask.GetMask("Ground");
        noRotYZ = RigidbodyConstraints.FreezeRotation | RigidbodyConstraints.FreezePositionZ 
            | RigidbodyConstraints.FreezePositionY;
        rigid.constraints = noRotYZ;
    }
	
	void FixedUpdate () {
        int x = Mathf.RoundToInt(CameraFollow.S.transform.position.x);
        int y = Mathf.RoundToInt(CameraFollow.S.transform.position.y);
        int i0 = x - 10;
        int i1 = x + 10;
        int j0 = y - 9;
        int j1 = y + 9;
        if(transform.position.x < i0 || transform.position.x > i1
            || transform.position.y < j0 || transform.position.y > j1)
        {
            Destroy(gameObject);
        }
        Vector3 vel = rigid.velocity;
        Vector3 checkLoc1 = body.transform.position + Vector3.left * (body.height * 0.9f);
        wallLeft= Physics.Raycast(checkLoc1, Vector3.left, body.radius, groundPhysicsLayerMask);
        Vector3 checkLoc2 = body.transform.position + Vector3.right * (body.height * 0.9f);
        wallRight = Physics.Raycast(checkLoc2, Vector3.right, body.radius, groundPhysicsLayerMask);
        if (wallLeft && wallRight)
        {
            velX = 0;
        }
        else if (facingRight && wallRight)
        {
            facingRight = false;
            transform.rotation = turnLeft;
            Vector3 pos = transform.position;
            pos.x += 1f;
            transform.position = pos;
            velX = -2f;
        }
        else if (!facingRight && wallLeft)
        {

            facingRight = true;
            transform.rotation = Quaternion.identity;
            Vector3 pos = transform.position;
            pos.x -= 1f;
            transform.position = pos;
            velX = 2f;
        }
        else
        {
            vel.x = velX;
            rigid.velocity = vel;
        }

        
    }
}
