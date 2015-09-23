using UnityEngine;
using System.Collections;

public enum direction
{
    DOWNRIGHT, //The feet are pointed down and moving to the right
    DOWNLEFT, 
    UPRIGHT, // feet pointed up at the ceiling
    UPLEFT,
    RIGHTDOWN, //feet pointed right and going down
    RIGHTUP,
    LEFTDOWN, //feet pointed left and going down
    LEFTUP,
    

}

public class ZoomerAI : MonoBehaviour {
    public int hp = 3;
    public bool wallLeft, wallRight, wallUp, wallDown = false;
    public float delay = 0f;
    public bool _______;
    public float speed = 2f;
    public direction dir = direction.DOWNRIGHT;
    public Rigidbody rigid;
    public CapsuleCollider body;
    public int groundPhysicsLayerMask;
    Quaternion feetRight = Quaternion.Euler(0, 0, 90);
    Quaternion feetUp = Quaternion.Euler(0, 0, 180);
    Quaternion feetLeft = Quaternion.Euler(0, 0, 270);
    

    // Use this for initialization
    void Start () {
        int initDir = (int) Mathf.Round(Random.Range(0, 1));
        if (initDir == 0)
        {
            dir = direction.DOWNLEFT;
        } else
        {
            dir = direction.DOWNRIGHT;
        }
        rigid = GetComponent<Rigidbody>();
        body = GetComponent<CapsuleCollider>();
        groundPhysicsLayerMask = LayerMask.GetMask("Ground");
    }
	
	void FixedUpdate () {
        int x = Mathf.RoundToInt(CameraFollow.S.transform.position.x);
        int y = Mathf.RoundToInt(CameraFollow.S.transform.position.y);
        int i0 = x - 18;
        int i1 = x + 18;
        int j0 = y - 18;
        int j1 = y + 18;
        Vector3 vel = rigid.velocity;

        if (transform.position.x < i0 - 20 || transform.position.x > i1 + 20
            || transform.position.y < j0 - 20 || transform.position.y > j1 + 20)
        {
            Destroy(gameObject);
        }
        else if (transform.position.x < i0 || transform.position.x > i1
            || transform.position.y < j0 || transform.position.y > j1)
        {
            vel.x = 0;
            vel.y = 0;
        }
        else
        {
            Vector3 checkLoc = body.transform.position;
            Vector3 checkLocLeft = body.transform.position + Vector3.left * body.radius;
            Vector3 checkLocRight = body.transform.position + Vector3.right * body.radius;
            Vector3 checkLocUp = body.transform.position + Vector3.up * body.radius;
            Vector3 checkLocDown = body.transform.position + Vector3.down * body.radius;
            wallLeft = Physics.Raycast(checkLoc, Vector3.left, body.height, groundPhysicsLayerMask);
            wallRight = Physics.Raycast(checkLoc, Vector3.right, body.height, groundPhysicsLayerMask);
            wallUp = Physics.Raycast(checkLoc, Vector3.up, body.height, groundPhysicsLayerMask);
            wallDown = Physics.Raycast(checkLoc, Vector3.down, body.height, groundPhysicsLayerMask);
            
                switch (dir)
                {
                    case direction.DOWNLEFT:
                        if (!Physics.Raycast(checkLocLeft, Vector3.down, body.height, groundPhysicsLayerMask)
                            && !Physics.Raycast(checkLocRight, Vector3.down, body.height, groundPhysicsLayerMask))
                        {
                            dir = direction.RIGHTDOWN;
                        }
                        else if (wallLeft)
                        {
                            dir = direction.LEFTUP;
                        }
                        break;
                    case direction.DOWNRIGHT:

                        if (!Physics.Raycast(checkLocRight, Vector3.down, body.height, groundPhysicsLayerMask)
                            && !Physics.Raycast(checkLocLeft, Vector3.down, body.height, groundPhysicsLayerMask))
                        {
                            dir = direction.LEFTDOWN;
                        }
                        else if (wallRight)
                        {
                            dir = direction.RIGHTUP;
                        }
                        break;
                    case direction.UPLEFT:
                        if (!wallLeft)
                        {
                            if (!Physics.Raycast(checkLocLeft, Vector3.up, body.height, groundPhysicsLayerMask)
                            && !Physics.Raycast(checkLocRight, Vector3.up, body.height, groundPhysicsLayerMask))
                                dir = direction.RIGHTUP;
                        }
                        else
                        {
                            dir = direction.LEFTDOWN;
                        }
                        break;
                    case direction.UPRIGHT:

                        if (!Physics.Raycast(checkLocRight, Vector3.up, body.height, groundPhysicsLayerMask)
                            && !Physics.Raycast(checkLocLeft, Vector3.up, body.height, groundPhysicsLayerMask))
                        {
                            dir = direction.LEFTUP;
                        }
                        else if (wallRight)
                        {
                            dir = direction.RIGHTDOWN;
                        }
                        break;
                    case direction.RIGHTDOWN:
                        if (!Physics.Raycast(checkLocDown, Vector3.right, body.height, groundPhysicsLayerMask)
                            && !Physics.Raycast(checkLocUp, Vector3.right, body.height, groundPhysicsLayerMask))
                        {
                            dir = direction.UPRIGHT;
                        }
                        else if (wallDown)
                        {
                            dir = direction.DOWNLEFT;
                        }
                        break;
                    case direction.RIGHTUP:
                        if (!Physics.Raycast(checkLocUp, Vector3.right, body.height, groundPhysicsLayerMask)
                            && !Physics.Raycast(checkLocDown, Vector3.right, body.height, groundPhysicsLayerMask))
                        {
                            dir = direction.DOWNRIGHT;
                        }
                        else if (wallUp)
                        {
                            dir = direction.UPLEFT;
                        }
                        break;
                    case direction.LEFTDOWN:

                        if (!Physics.Raycast(checkLocDown, Vector3.left, body.height, groundPhysicsLayerMask)
                            && !Physics.Raycast(checkLocUp, Vector3.left, body.height, groundPhysicsLayerMask))
                        {
                            dir = direction.UPLEFT;
                        }
                        else if (wallDown)
                        {
                            dir = direction.DOWNRIGHT;
                        }
                        break;
                    case direction.LEFTUP:
                        if (!Physics.Raycast(checkLocUp, Vector3.left, body.height, groundPhysicsLayerMask)
                            && !Physics.Raycast(checkLocDown, Vector3.left, body.height, groundPhysicsLayerMask))
                        {
                            dir = direction.DOWNLEFT;
                        }
                        else if (wallUp)
                        {
                            dir = direction.UPRIGHT;
                        }
                        break;
                }
            switch (dir)
            {
                case direction.DOWNLEFT:
                    vel.x = -speed;
                    vel.y = 0f;
                    transform.rotation = Quaternion.identity;
                    break;
                case direction.DOWNRIGHT:
                    vel.x = speed;
                    vel.y = 0f;
                    transform.rotation = Quaternion.identity;
                    break;
                case direction.UPLEFT:
                    vel.x = -speed;
                    vel.y = 0f;
                    transform.rotation = feetUp;
                    break;
                case direction.UPRIGHT:
                    vel.x = speed;
                    vel.y = 0f;
                    transform.rotation = feetUp;
                    break;
                case direction.RIGHTDOWN:
                    vel.x = 0f;
                    vel.y = -speed;
                    transform.rotation = feetRight;
                    break;
                case direction.RIGHTUP:
                    vel.x = 0f;
                    vel.y = speed;
                    transform.rotation = feetRight;
                    break;
                case direction.LEFTDOWN:
                    vel.x = 0f;
                    vel.y = -speed;
                    transform.rotation = feetLeft;
                    break;
                case direction.LEFTUP:
                    vel.x = 0f;
                    vel.y = speed;
                    transform.rotation = feetLeft;
                    break;
            }
        }
        
        rigid.velocity = vel;
    }
}
