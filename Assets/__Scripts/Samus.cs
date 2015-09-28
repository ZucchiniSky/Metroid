using UnityEngine;
using System.Collections;

//Controls Samus' movement and shooting
// Move Samus left and right
// Allow Samus to jump
// Allow Samus to fire

public enum Facing
{
    R, //Right
    L, //Left
    RU, //RIght up
    LU,
    U, //up
    D //down
}

public class Samus : MonoBehaviour {
    //These are variables set in the Inspector
    public Facing _face = Facing.R;
    public Sprite spRight, spUp, spMorph;
    public GameObject bulletPrefab, missilePrefab;
    public Transform bulletOrigin, bulletOriginUp;
    public bool ____________;

    public float speedX = 4f;
    public float speedXStandingJump = 3f;
    public float speedJump = 14f;
    public float speedBullet = 10f;
    public bool runningJump = false;
    public bool hasMorph = false;
    public bool isMorph = false;
    public bool canUnmorph = true;
    public bool hasMissiles = false;
    public bool usingMissiles = false;
    public float missiles = 0f;
    public float maxMissiles = 0f;
    public bool hasLongBeam = false;
    public float bulletStopDist = 3f;
    public float health = 35f;
    public bool jump = false;
    public bool jumpCancel = false;
    public float invincibleTimer = 0f;
    public Vector3 hitVel = new Vector3(0, 0, 0);
    public bool hit = false;
    public Vector3 knockback;
    public float knockbackYSpeed = 3f;
    public Collider lastCollision;

    private bool door = false;
    private bool doorRight = false;
    private float doorX;
    
    public float respawn = 0f;
    static public Samus S;

    //These are variables set on Start()
    public Rigidbody rigid;
    public SpriteRenderer spRend;
    public bool grounded = false;
    public CapsuleCollider feet;
	public CapsuleCollider samusFull;
    public int groundPhysicsLayerMask;
    public Vector3 groundedCheckOffset;

    RigidbodyConstraints noRotZ, noRotYZ;
    Quaternion turnLeft = Quaternion.Euler(0, 180, 0);
    Quaternion turnUp = Quaternion.Euler(0, 0, 90);
    void Awake() {
		S = this;
        Physics.gravity = new Vector3(0f, -17f, 0f);
	}

    // Use this for initialization
	void Start () {
        rigid = GetComponent<Rigidbody>();

        //get SpriteREnder of Sprite
        spRend = transform.Find("Sprite").GetComponent<SpriteRenderer>();
        rigid.velocity = new Vector3(0, 0, 0);

		//get regular samus
		samusFull = GetComponent<CapsuleCollider>();

        //get the feet
        Transform feetTrans = this.transform.Find("Feet");
        feet = feetTrans.GetComponent<CapsuleCollider>();

        //set groundPhysicsLayerMask
        groundPhysicsLayerMask = LayerMask.GetMask("Ground");
        groundedCheckOffset = new Vector3(feet.height * 0.4f, 0, 0);

        noRotZ = RigidbodyConstraints.FreezeRotation | RigidbodyConstraints.FreezePositionZ;
        noRotYZ = noRotZ | RigidbodyConstraints.FreezePositionY;
    }
    void Update()
    {

        //Switch weapon
        if (Input.GetKeyDown(KeyCode.RightShift))
        {
            if (!usingMissiles && missiles > 0)
            {
                usingMissiles = true;
            }
            else if (usingMissiles)
            {
                usingMissiles = false;
            }
        }
        if (Input.GetKeyDown(KeyCode.I))
        {
            if (invincibleTimer != -2)
            {
                invincibleTimer = -2;
            }
            else
            {
                invincibleTimer = 0;
            }
        }
        //Press S to fire the gun
        if (Input.GetKeyDown(KeyCode.S) && !isMorph)
        {
            Fire();
        }
        if (Input.GetKeyDown(KeyCode.A))
        {
            jump = true;
        }
        if (Input.GetKeyUp(KeyCode.A))
        {
            jumpCancel = true;
        }
    }
    // FixedUpdate is called once per physics engine update (50fps)
    void FixedUpdate () {
        if (invincibleTimer > 0)
        {
            invincibleTimer--;
        }
        //Check to see whether we're grounded or not
        Vector3 checkLoc = feet.transform.position + Vector3.up * (feet.radius * 0.9f);
        grounded = Physics.Raycast(checkLoc, Vector3.down, feet.radius, groundPhysicsLayerMask) ||
                    Physics.Raycast(checkLoc + groundedCheckOffset, Vector3.down, feet.radius, groundPhysicsLayerMask) ||
                    Physics.Raycast(checkLoc - groundedCheckOffset, Vector3.down, feet.radius, groundPhysicsLayerMask);
        //Normalize on ground
        if (grounded)
        {
            runningJump = false;
            transform.position = new Vector3(transform.position.x, Mathf.Round(transform.position.y * 2f) / 2f);
        }
		//check if can unmorph
		if (isMorph) 
		{
			Vector3 checkUp = samusFull.transform.position + Vector3.up * (samusFull.radius * 0.9f);
			canUnmorph = !Physics.Raycast(checkUp, Vector3.up, 1.15f, groundPhysicsLayerMask);// ||
			//	Physics.Raycast(checkUp + groundedCheckOffset, Vector3.up, samusFull.radius, groundPhysicsLayerMask) ||
			//		Physics.Raycast(checkUp - groundedCheckOffset, Vector3.up, samusFull.radius, groundPhysicsLayerMask);
		}

        //If grounded, constrain Y in RigidBody
        rigid.constraints = grounded ? noRotYZ : noRotZ; 

        Vector3 vel = rigid.velocity;

        if (door)
        {
            if (CameraScrolling.S.state != CameraState.TRANSITION)
            {
                door = false;
            } else if (doorRight == doorX > transform.position.x)
            {
                vel.x = doorRight ? speedX : -speedX;
            } else
            {
                vel.x = 0f;
            }
            rigid.velocity = vel;
            return;
        }

        //Handle L and R movement
        float speed = grounded ? speedX : (runningJump ? speedX : speedXStandingJump);
        if (Input.GetKey(KeyCode.LeftArrow) && !Input.GetKey(KeyCode.RightArrow))
        {
            vel.x = -speed;
            face = Facing.L;
        }
        else if (Input.GetKey(KeyCode.RightArrow) && !Input.GetKey(KeyCode.LeftArrow))
        {
            vel.x = speed;
            face = Facing.R;
        }
        else
        {
            if (!grounded && runningJump)
            {
                vel.x *= 9/10f;
            } else
            {
                vel.x = 0;
            }
        }
        //Raises and lowers gun
        if(Input.GetKey(KeyCode.UpArrow))
        {
            face = Facing.U;
			if(isMorph && canUnmorph)
			{
			    isMorph = false;
			    samusFull.height = 1.8f;
			    samusFull.center = new Vector3(0f, 1f,0f);
			}
        }
        else
        {
            face = Facing.D;
        }
		//Morph ball
		if (Input.GetKeyDown(KeyCode.DownArrow) && grounded && hasMorph)
		{
			isMorph = true;
			samusFull.height = .85f;
			samusFull.center = new Vector3(0f,.4f,0f);
		}
        //Allow Jump
        if (jump)
        {
			if(isMorph && canUnmorph)
			{
				isMorph = false;
				samusFull.height = 1.8f;
				samusFull.center = new Vector3(0f, 1f,0f);
			}
			else if(!isMorph && grounded)
			{
				rigid.constraints = noRotZ; // unlocks Y movement in Rigidbody
            	vel.y = speedJump;
                if (vel.x != 0)
                {
                    runningJump = true;
                } else
                {
                    runningJump = false;
                }
			}
            jump = false;
        }
        //Allow Jump cancel
        if (jumpCancel)
        {
            if (vel.y > 0 && !grounded)
            {
                vel.y = 0;
            }
            jumpCancel = false;
        }
        //Enemy knockback
        if(respawn > 0)
        {
            rigid.velocity = new Vector3(0, 0, 0);
            respawn--;
        }
        if (hit)
        {
            rigid.velocity = knockback;
            hit = false;
        } else if (invincibleTimer <= 0)
        {
            rigid.velocity = vel;
            lastCollision = null;
        }


    }  

    void Fire()
    {
        GameObject go = null;
        if (usingMissiles && missiles <= 0)
        {
            usingMissiles = false;
        }
        if(usingMissiles)
        {
            go = Instantiate<GameObject>(missilePrefab);
            missiles--;
        }
        else
        {
            go = Instantiate<GameObject>(bulletPrefab);

        }

        if (face == Facing.R || face == Facing.L || runningJump)
        {
            go.transform.position = bulletOrigin.position;
            go.GetComponent<Rigidbody>().velocity = bulletOrigin.right * speedBullet;
            if(face == Facing.L)
            {
                go.transform.rotation = turnLeft;
            }
        }
        else
        {
            go.transform.position = bulletOriginUp.position;
            go.GetComponent<Rigidbody>().velocity = bulletOrigin.up * speedBullet;
            go.transform.rotation = turnUp;
        }
    }

    void OnTriggerEnter(Collider other)
    {
        if (other.tag == "Enemy" && invincibleTimer == 0 && other != lastCollision)
        {
            knockback = new Vector3(-speedX * Mathf.Sign(other.attachedRigidbody.position.x - rigid.position.x), knockbackYSpeed + Mathf.Sign(other.attachedRigidbody.position.y - rigid.position.y), 0);
            hit = true;
            health -= 8f;
            invincibleTimer = 30f;
            lastCollision = other;
            if (health <= 0)
            {
                transform.position = new Vector3(56, 260, 0);
                rigid.velocity = new Vector3(0, 0, 0);
                CameraScrolling.S.transform.position = transform.position + CameraScrolling.S.offset;
                CameraScrolling.S.state = CameraState.HORIZONTAL;
                health = 35;
                respawn = 50f;
            }
        }
    }

    public Facing face {
		get { return _face; }
		set {
			if (_face == value)
				return;
			switch (value) {
			case Facing.U:
				if (_face == Facing.R || _face == Facing.RU) {
					_face = Facing.RU;
				} else {
					_face = Facing.LU;
				}
				break;
			case Facing.D:
				if (_face == Facing.R || _face == Facing.RU) {
					_face = Facing.R;
				} else {
					_face = Facing.L;
				}
				break;
			default:
				_face = value;
				break;
			}
			//change the sprite and rotation
			if (isMorph) {
				spRend.sprite = spMorph;
			} else {
				if (_face == Facing.R || _face == Facing.L) {
					spRend.sprite = spRight;
				} else {
					spRend.sprite = spUp;
				}

				if (_face == Facing.R || _face == Facing.RU) {
					transform.rotation = Quaternion.identity;
				} else {
					transform.rotation = turnLeft;
				}
			}
		}
	}

    public void passThroughDoor(float x)
    {
        door = true;
        doorX = x;
        doorRight = doorX > transform.position.x;
    }
}
