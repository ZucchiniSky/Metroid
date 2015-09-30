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
    public Sprite spRight, spUp, spMorph,
        spRunjumpR, spRunjumpL, spRunjumpU, spRunjumpD;
    public GameObject bulletPrefab, missilePrefab;
    public Transform bulletOrigin, bulletOriginUp;
    public bool ____________;

    public float speedX = 6f;
    public float speedXStandingJump = 4f;
    public float speedJump = 14f;
    public float speedBullet = 10f;
    public bool runningJump = false;
    public bool runJumpPeak = false;
    public bool runJumpAnim = false;
    public bool runJumpDir = false;
    public bool hasMorph = false;
    public bool isMorph = false;
    public bool canUnmorph = true;
    public bool hasMissiles = false;
    public bool usingMissiles = false;
    public float missiles = 0f;
    public float maxMissiles = 0f;
    public bool hasLongBeam = false;
    public float bulletStopDist = 3f;
    public float health = 30f;
    public bool jump = false;
    public bool jumpCancel = false;
    public float invincibleTimer = 0f;
    public Vector3 hitVel = new Vector3(0, 0, 0);
    public bool hit = false;
    public Vector3 knockback;
    public float knockbackYSpeed = 3f;
    public Collider lastCollision;
    public int runJumpCount = 0;
    private bool door = false;
    private bool doorRight = false;
    private float doorX;
    private bool onLava = false;
    private int lavaCounter = 0;
    private int lavaAnim = 0;
    
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
        //Press 2 to get stuff
        if (Input.GetKeyDown(KeyCode.Alpha2))
        {
            hasMissiles = true;
            maxMissiles = 100;
            missiles = 100;
            hasLongBeam = true;
            hasMorph = true;
        }
        //press 1 to take away
        if (Input.GetKeyDown(KeyCode.Alpha1))
        {
            hasMissiles = false;
            maxMissiles = 0;
            missiles = 0;
            hasLongBeam = false;
            hasMorph = false;
        }
    }
    // FixedUpdate is called once per physics engine update (50fps)
    void FixedUpdate () {
        if (invincibleTimer == -2 || (onLava && lavaCounter <= 0 && !grounded))
        {
            onLava = false;
        }
        speedX = onLava ? 3f : 6f;
        if (onLava)
        {
            if (lavaAnim % 15 == 0)
            {
                health--;
                checkHealth();
            }
            lavaAnim++;
            if (grounded)
            {
                lavaCounter = 10;
            }
            if (lavaAnim % 2 == 0)
                spRend.color = Color.white;
            else
                spRend.color = Color.clear;
            lavaCounter--;
        } else
        {
                spRend.color = Color.white;
        }
        if (invincibleTimer > 0)
        {
            invincibleTimer--;
            if(invincibleTimer%2 == 0)
                spRend.color = Color.white;
            else if (!door)
                spRend.color = Color.clear;
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
        else if (grounded || runJumpAnim)
        {
            vel.x *= 8/10f;
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
            if (runningJump && !runJumpPeak)
            {
                runJumpAnim = false;
            }
        }
        else
        {
            face = Facing.D;
            if (runningJump && !runJumpPeak)
            {
                runJumpAnim = true;
            }
        }
        if (runningJump && !runJumpPeak && vel.y < 0)
        {
            runJumpPeak = true;
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
                bool shouldRunningJump = vel.x != 0;
                runningJump = shouldRunningJump;
                runJumpPeak = false;
                runJumpAnim = shouldRunningJump;
                runJumpDir = vel.x > 0;
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
        if (Mathf.Abs(vel.x) < .5f)
        {
            vel.x = 0;
        }
        //Enemy knockback
        if (respawn > 0)
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
        if (hasLongBeam)
        {
            bulletStopDist = 40f;
        }
        else
        {
            bulletStopDist = 3f;
        }
        if (runningJump && runJumpPeak)
        {
            runJumpAnim = false;
        }
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
            if(face == Facing.L|| face == Facing.LU)
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
            checkHealth();
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
            }
            else if (runningJump){
                if (runJumpAnim)
                {
                    switch (runJumpCount)
                    {
                        case 0:
                            spRend.sprite = spRunjumpR;
                            break;
                        case 1:
                            spRend.sprite = spRunjumpU;
                            break;
                        case 2:
                            spRend.sprite = spRunjumpL;
                            break;
                        case 3:
                            spRend.sprite = spRunjumpD;
                            break;
                        default:
                            spRend.sprite = spRunjumpR;
                            break;
                    }
                    runJumpCount++;
                    if(runJumpCount >= 4)
                    {
                        runJumpCount = 0;
                    }
                } else
                {
                    spRend.sprite = spRight;
                }
            }
            else {
                if (_face == Facing.R || _face == Facing.L) {
                    spRend.sprite = spRight;
                } else {
                    spRend.sprite = spUp;
                }

                
            }
            if (_face == Facing.R || _face == Facing.RU)
            {
                transform.rotation = Quaternion.identity;
            }
            else
            {
                transform.rotation = turnLeft;
            }
        }
	}

    public void passThroughDoor(float x)
    {
        door = true;
        doorX = x;
        doorRight = doorX > transform.position.x;
        if (invincibleTimer != -2)
            invincibleTimer = 60f;
    }

    public void onLavaCollision()
    {
        if (!onLava)
        {
            onLava = true;
            lavaCounter = 10;
            lavaAnim = 0;
        }
    }

    void checkHealth()
    {
        if (health <= 0)
        {
            transform.position = new Vector3(56, 260, 0);
            rigid.velocity = new Vector3(0, 0, 0);
            CameraScrolling.S.ResetCamera();
            ShowMapOnCamera.S.RedrawScreen();
            health = 30;
            onLava = false;
            respawn = 50f;
        }
    }
}
