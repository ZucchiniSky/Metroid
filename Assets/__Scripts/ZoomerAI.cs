using UnityEngine;
using System.Collections;

public enum direction
{
    UP,
    LEFT,
    RIGHT,
    DOWN
}

public class ZoomerAI : MonoBehaviour {
    public int hp;

    public bool _______;
    public float speed = 2f;
    public direction dir = direction.RIGHT;
    public Rigidbody rigid;
    public CapsuleCollider body;
    public int groundPhysicsLayerMask;
    Quaternion turnLeft = Quaternion.Euler(0, 180, 0);
    Quaternion turnUp = Quaternion.Euler(0, 90, 0);
    Quaternion turnDown = Quaternion.Euler(0, 270, 0);

    // Use this for initialization
    void Start () {
        int initDir = (int) Mathf.Round(Random.Range(0, 1));
        if (initDir == 0)
        {
            dir = direction.LEFT;
        } else
        {
            dir = direction.RIGHT;
        }
        rigid = GetComponent<Rigidbody>();
        body = GetComponent<CapsuleCollider>();
        groundPhysicsLayerMask = LayerMask.GetMask("Ground");
    }
	
	// Update is called once per frame
	void Update () {
        int x = Mathf.RoundToInt(CameraFollow.S.transform.position.x);
        int y = Mathf.RoundToInt(CameraFollow.S.transform.position.y);
        int i0 = x - 9;
        int i1 = x + 9;
        int j0 = y - 9;
        int j1 = y + 9;
        Vector3 vel = rigid.velocity;

        if (transform.position.x < i0 - 20 || transform.position.x > i1 + 20
            || transform.position.y < j0 - 20 || transform.position.y > j1 + 20)
        {
            Destroy(gameObject);
        }
        else if (transform.position.x < i0 || transform.position.x > i1
            || transform.position.y < j0 || transform.position.y > j1)
        {
            speed = 0;
        }
        else
        {
            Vector2 checkLoc = body.transform.position;
            switch (dir)
            {
                case direction.LEFT:
                    break;
                case direction.RIGHT:
                    break;
                case direction.UP:
                    break;
                case direction.DOWN:
                    break;
            }
            /*Vector3 checkLoc1 = body.transform.position + Vector3.left * (body.height * 0.45f);
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
            }*/
        }
        switch (dir)
        {
            case direction.LEFT:
                vel.x = -speed;
                transform.rotation = turnLeft;
                break;
            case direction.RIGHT:
                vel.x = speed;
                transform.rotation = Quaternion.identity;
                break;
            case direction.UP:
                vel.y = speed;
                transform.rotation = turnUp;
                break;
            case direction.DOWN:
                vel.y = -speed;
                transform.rotation = turnDown;
                break;
        }
        rigid.velocity = vel;
    }
}
