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
    D, //down
}

public class Samus : MonoBehaviour {
    //These are variables set in the Inspector
    public float speedX = 4f;
    public float speedJump = 10f;
    public Facing _face = Facing.R;
    public Sprite spRight, spUp, spMorph;
    public GameObject bulletPrefab, missilePrefab;
    public Transform bulletOrigin, bulletOriginUp;
    public float speedBullet = 10f;
	public bool hasMorph = false;
	public bool isMorph = false;
	public bool canUnmorph = true;
    public bool hasMissiles = false;
    public bool usingMissiles = false;
    public float missiles = 0;
    public float maxMissiles = 0;
    public bool hasLongBeam = false;
    public float bulletStopDist = 3f;


    public bool ____________;

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
        //Press S to fire the gun
        if (Input.GetKeyDown(KeyCode.S) && !isMorph)
        {
            Fire();
        }

    }
    // FixedUpdate is called once per physics engine update (50fps)
    void FixedUpdate () {
        //Check to see whether we're grounded or not
        Vector3 checkLoc = feet.transform.position + Vector3.up * (feet.radius * 0.9f);
        grounded = Physics.Raycast(checkLoc, Vector3.down, feet.radius, groundPhysicsLayerMask) ||
                    Physics.Raycast(checkLoc + groundedCheckOffset, Vector3.down, feet.radius, groundPhysicsLayerMask) ||
                    Physics.Raycast(checkLoc - groundedCheckOffset, Vector3.down, feet.radius, groundPhysicsLayerMask);
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

        //Handle L and R movement
        if (Input.GetKey(KeyCode.LeftArrow) && !Input.GetKey(KeyCode.RightArrow))
        {
            vel.x = -speedX;
            face = Facing.L;
        }
        else if (Input.GetKey(KeyCode.RightArrow) && !Input.GetKey(KeyCode.LeftArrow))
        {
            vel.x = speedX;
            face = Facing.R;
        }
        else
        {
            vel.x = 0;
        }
        //Raises and lowers gun
        if(Input.GetKey(KeyCode.UpArrow))
        {
            face = Facing.U;
			if(isMorph && canUnmorph)
			{
			isMorph = false;
			samusFull.height = 2f;
			samusFull.center = new Vector3(0f, 1f,0f);
			}
        }
        else
        {
            face = Facing.D;
        }
		//morph ball
		if (Input.GetKeyDown(KeyCode.DownArrow) && grounded && hasMorph)
		{
			isMorph = true;
			samusFull.height = .85f;
			samusFull.center = new Vector3(0f,.4f,0f);
		}
        //Allow Jump
        if (Input.GetKeyDown(KeyCode.A))
        {
			if(isMorph && canUnmorph)
			{
				isMorph = false;
				samusFull.height = 2f;
				samusFull.center = new Vector3(0f, 1f,0f);
			}
			else if(!isMorph && grounded)
			{
				rigid.constraints = noRotZ; // unlocks Y movement in Rigidbody
            	vel.y = speedJump;
			}
        }
        //Switch weapon
        if (Input.GetKeyDown(KeyCode.RightShift))
        {
            if(!usingMissiles && missiles > 0)
            {
                usingMissiles = true;
            }
            else if (usingMissiles)
            {
                usingMissiles = false;
            }
        }

            rigid.velocity = vel;

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

        if (face == Facing.R || face == Facing.L)
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
}
