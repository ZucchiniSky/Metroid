using UnityEngine;
using System.Collections;

public class SkreeAI : MonoBehaviour {

    public float health = 3f;
    public float speedX = 2f;
    public float speedY = -7f;
    public float explodeDelay = 10f;
    public bool dive = false;
    public bool hitFloor = false;
    public bool _______;
    public Rigidbody rigid;
    public CapsuleCollider body;
    public int groundPhysicsLayerMask;

    // Use this for initialization
    void Start () {
        rigid = GetComponent<Rigidbody>();
        body = GetComponent<CapsuleCollider>();
        groundPhysicsLayerMask = LayerMask.GetMask("Ground");
    }
	
	void FixedUpdate () {
        float distance = Samus.S.transform.position.x - transform.position.x;
        Vector3 vel = rigid.velocity; 
        if (!dive)
        {
            if(System.Math.Abs(distance) < 5)
            {
                dive = true;
            }
        }
        else
        {
            if (!hitFloor)
            {
                if (distance > 0)
                {
                    vel.x = speedX;
                }
                else if (distance < 0)
                {
                    vel.x = -speedX;
                }
                vel.y = speedY;
                Vector3 checkLoc = body.transform.position + Vector3.down * (body.height * 0.5f);
                if (Physics.Raycast(checkLoc, Vector3.down, .5f, groundPhysicsLayerMask))
                {
                    hitFloor = true;
                    vel.x = 0;
                    vel.y = 0;
                }
                rigid.velocity = vel;

            }
            else
            {
                Destroy(gameObject);
            }
        }
	}
    void OnTriggerEnter(Collider other)
    {
        if (other.tag == "Bullet")
        {
            health--;
            if(health <= 0)
            {
                Destroy(gameObject);
            }
        }
        
        
    }
}
