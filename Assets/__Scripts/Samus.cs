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
    public float speedX = 4f;
    public float speedJump = 10f;
    public Facing _face = Facing.R;
    public Sprite spRight, spUp;
    public GameObject bulletPrefab;
    public Transform bulletOrigin, bulletOriginUp;
    public float speedBullet = 10f;

    public bool ____________;

    //These are variables set on Start()
    public Rigidbody rigid;
    public SpriteRenderer spRend;
    public bool grounded = false;
    public CapsuleCollider feet;
    public int groundPhysicsLayerMask;
    public Vector3 groundedCheckOffset;

    RigidbodyConstraints noRotZ, noRotYZ;
    Quaternion turnLeft = Quaternion.Euler(0, 180, 0);

    // Use this for initialization
	void Start () {
        rigid = GetComponent<Rigidbody>();

        //get SpriteREnder of Sprite
        spRend = transform.Find("Sprite").GetComponent<SpriteRenderer>();
        rigid.velocity = new Vector3(0, 0, 0);

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
        if (Input.GetKeyDown(KeyCode.S))
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
        }
        else
        {
            face = Facing.D;
        }
        
        //Allow Jump
        if (Input.GetKeyDown(KeyCode.A) && grounded)
        {
            rigid.constraints = noRotZ; // unlocks Y movement in Rigidbody
            vel.y = speedJump;
        }

        rigid.velocity = vel;

    }  

    void Fire()
    {
        GameObject go = Instantiate<GameObject>(bulletPrefab);
        if (face == Facing.R || face == Facing.L)
        {
            go.transform.position = bulletOrigin.position;
            go.GetComponent<Rigidbody>().velocity = bulletOrigin.right * speedBullet;
        }
        else
        {
            go.transform.position = bulletOriginUp.position;
            go.GetComponent<Rigidbody>().velocity = bulletOrigin.up * speedBullet;
        }
    }

    public Facing face
    {
        get { return _face; }
        set
        {
            if (_face == value) return;
            switch (value)
            {
                case Facing.U:
                    if (_face == Facing.R || _face == Facing.RU)
                    {
                        _face = Facing.RU;
                    }
                    else
                    {
                        _face = Facing.LU;
                    }
                    break;
                case Facing.D:
                    if (_face == Facing.R || _face == Facing.RU)
                    {
                        _face = Facing.R;
                    }
                    else
                    {
                        _face = Facing.L;
                    }
                    break;
                default:
                    _face = value;
                    break;
            }
            //change the sprite and rotation
            if(_face == Facing.R || _face== Facing.L)
            {
                spRend.sprite = spRight;
            }
            else
            {
                spRend.sprite = spUp;
            }
            if(_face == Facing.R || _face == Facing.RU)
            {
                transform.rotation = Quaternion.identity;
            }
            else
            {
                transform.rotation = turnLeft;
            }
        }
    }
}
