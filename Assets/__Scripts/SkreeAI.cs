using UnityEngine;
using System.Collections;

public class SkreeAI : MonoBehaviour {

    public float hp = 3f;
    public float speedX = 4f;
    public float speedYAccel = -1f;
    public float maxYSpeed = -10f;
    public float explodeDelay = 70f;
    public bool dive = false;
    public bool hitFloor = false;
    
    public bool _______;
    
    public float shot = 0;
    public Rigidbody rigid;
    public CapsuleCollider body;
    public int groundPhysicsLayerMask;
    public GameObject energyPrefab, missilePrefab, skreeExplosionPrefab;

    // Use this for initialization
    void Start () {
        rigid = GetComponent<Rigidbody>();
        body = GetComponent<CapsuleCollider>();
        groundPhysicsLayerMask = LayerMask.GetMask("Ground");
    }
	
	void FixedUpdate () {
        float distance = Samus.S.transform.position.x - transform.position.x;
        GetComponent<SpriteRenderer>().color = Color.white;
        Vector3 vel = rigid.velocity; 
        if (!dive)
        {
            if(System.Math.Abs(distance) < 3)
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
                
                vel.y += speedYAccel;
                if (vel.y < maxYSpeed) { vel.y = maxYSpeed; }
                if (shot > 0)
                {
                    GetComponent<SpriteRenderer>().color = Color.red;
                    shot--;
                    vel.x = 0;
                    vel.y = 0;
                }
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

                explodeDelay--;
                if (explodeDelay <= 0)
                {

                    GameObject go1 = Instantiate<GameObject>(skreeExplosionPrefab);
                    GameObject go2 = Instantiate<GameObject>(skreeExplosionPrefab);
                    GameObject go3 = Instantiate<GameObject>(skreeExplosionPrefab);
                    GameObject go4 = Instantiate<GameObject>(skreeExplosionPrefab);
                    go1.transform.position = transform.position + new Vector3(1, 0, 0);
                    go2.transform.position = transform.position + new Vector3(-1, 0, 0);
                    go3.transform.position = transform.position + new Vector3(.5f, .5f, 0);
                    go4.transform.position = transform.position + new Vector3(-.5f, .5f, 0);
                    go1.GetComponent<Rigidbody>().velocity = new Vector3(8, 0, 0);
                    go2.GetComponent<Rigidbody>().velocity = new Vector3(-8, 0, 0);
                    go3.GetComponent<Rigidbody>().velocity = new Vector3(4, 6, 0);
                    go4.GetComponent<Rigidbody>().velocity = new Vector3(-4, 6, 0);

                    Destroy(gameObject);
                }
            }
        }
	}
    void OnTriggerEnter(Collider other)
    {
        if (other.tag == "Bullet" || other.tag == "Missile")
        {
            if (other.tag == "Bullet")
                hp--;
            else
                hp = 0;
            shot = 3f;
            if(hp <= 0)
            {
                int initDir = (int)Mathf.Round(Random.Range(0, 3));
                if (initDir == 0)
                {
                    GameObject go = Instantiate(energyPrefab);
                    go.transform.position = transform.position;
                }
                else if (initDir == 1 && Samus.S.hasMissiles)
                {
                    GameObject go = Instantiate(missilePrefab);
                    go.transform.position = transform.position;
                }
                Destroy(gameObject);
            }
        }
        
        
    }
}
