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
    Quaternion turnLeft = Quaternion.Euler(0, 180, 0);
    // Use this for initialization
    void Start () {
        rigid = GetComponent<Rigidbody>();
        body = GetComponent<CapsuleCollider>();
        groundPhysicsLayerMask = LayerMask.GetMask("Ground");
    }

    void FixedUpdate() {
        int x = Mathf.RoundToInt(CameraFollow.S.transform.position.x);
        int y = Mathf.RoundToInt(CameraFollow.S.transform.position.y);
        int i0 = x - 18;
        int i1 = x + 18;
        int j0 = y - 18;
        int j1 = y + 18;
        Vector3 vel = rigid.velocity;
        
        if(transform.position.x < i0-20 || transform.position.x > i1+20
            || transform.position.y < j0-20 || transform.position.y > j1 + 20)
        {
            Destroy(gameObject);
        }
        else if (transform.position.x < i0 || transform.position.x > i1
            || transform.position.y < j0 || transform.position.y > j1)
        {
            velX = 0;
        }
        else
        {
            
            Vector3 checkLoc1 = body.transform.position + Vector3.left * (body.height * 0.45f);
            wallLeft = Physics.Raycast(checkLoc1, Vector3.left, body.radius, groundPhysicsLayerMask);
            Vector3 checkLoc2 = body.transform.position + Vector3.right * (body.height * 0.45f);
            wallRight = Physics.Raycast(checkLoc2, Vector3.right, body.radius, groundPhysicsLayerMask);
            if (wallLeft && wallRight)
            {
                velX = 0;
            }
            else if (facingRight && wallRight)
            {
                facingRight = false;
                transform.rotation = turnLeft;
            }
            else if (!facingRight && wallLeft)
            {
                facingRight = true;
                transform.rotation = Quaternion.identity;
            }
            if (facingRight)
            {
                velX = 2f;
            }
            else if (!facingRight)
            {
                velX = -2f;
            }
        }
        vel.x = velX;
        rigid.velocity = vel;
    }
}
