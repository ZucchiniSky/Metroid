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
    public int hp;
    public bool _______;
    public float speed = 2f;
    public direction dir = direction.DOWNRIGHT;
    public Rigidbody rigid;
    public CapsuleCollider body;
    public int originX;
    public int originY;
    public int currentX;
    public int currentY;
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
    }
	
	void FixedUpdate () {
        int x = Mathf.RoundToInt(CameraFollow.S.transform.position.x);
        int y = Mathf.RoundToInt(CameraFollow.S.transform.position.y);
        int i0 = x - 9;
        int i1 = x + 9;
        int j0 = y - 9;
        int j1 = y + 9;
        Vector3 vel = rigid.velocity;

        if (transform.position.x < i0 - 9 || transform.position.x > i1 + 9
            || transform.position.y < j0 - 9 || transform.position.y > j1 + 9)
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
            int lastX = currentX;
            int lastY = currentY;
            if (dir == direction.DOWNLEFT || dir == direction.UPLEFT)
            {
                currentX = (int)Mathf.Ceil(transform.localPosition.x);
            } else if (dir == direction.DOWNRIGHT || dir == direction.UPRIGHT)
            {
                currentX = (int)Mathf.Floor(transform.localPosition.x);
            }
            if (dir == direction.LEFTDOWN || dir == direction.RIGHTDOWN)
            {
                currentY = (int)Mathf.Ceil(transform.localPosition.y);
            } else if (dir == direction.LEFTUP || dir == direction.RIGHTUP)
            {
                currentY = (int)Mathf.Floor(transform.localPosition.y);
            }
            

            if (lastX != currentX || lastY != currentY)
            {
                switch (dir)
                {
                    case direction.DOWNLEFT:
                        if (tileIsEmpty(currentX, currentY - 1))
                        {
                            dir = direction.RIGHTDOWN;
                        }
                        else if (!tileIsEmpty(currentX - 1, currentY))
                        {
                            dir = direction.LEFTUP;
                        }
                        break;
                    case direction.DOWNRIGHT:
                        if (tileIsEmpty(currentX, currentY - 1))
                        {
                            dir = direction.LEFTDOWN;
                        }
                        else if (!tileIsEmpty(currentX + 1, currentY))
                        {
                            dir = direction.RIGHTUP;
                        }
                        break;
                    case direction.UPLEFT:
                        if (tileIsEmpty(currentX, currentY + 1))
                        {
                            dir = direction.RIGHTUP;
                        }
                        else if (!tileIsEmpty(currentX - 1, currentY))
                        {
                            dir = direction.LEFTDOWN;
                        }
                        break;
                    case direction.UPRIGHT:
                        if (tileIsEmpty(currentX, currentY + 1))
                        {
                            dir = direction.LEFTUP;
                        }
                        else if (!tileIsEmpty(currentX + 1, currentY))
                        {
                            dir = direction.RIGHTDOWN;
                        }
                        break;
                    case direction.RIGHTDOWN:
                        if (tileIsEmpty(currentX + 1, currentY))
                        {
                            dir = direction.UPRIGHT;
                        }
                        else if (!tileIsEmpty(currentX, currentY - 1))
                        {
                            dir = direction.DOWNLEFT;
                        }
                        break;
                    case direction.RIGHTUP:
                        if (tileIsEmpty(currentX + 1, currentY))
                        {
                            dir = direction.DOWNRIGHT;
                        }
                        else if (!tileIsEmpty(currentX, currentY + 1))
                        {
                            dir = direction.UPLEFT;
                        }
                        break;
                    case direction.LEFTDOWN:
                        if (tileIsEmpty(currentX - 1, currentY))
                        {
                            dir = direction.UPLEFT;
                        }
                        else if (!tileIsEmpty(currentX, currentY - 1))
                        {
                            dir = direction.DOWNRIGHT;
                        }
                        break;
                    case direction.LEFTUP:
                        if (tileIsEmpty(currentX - 1, currentY))
                        {
                            dir = direction.DOWNLEFT;
                        }
                        else if (!tileIsEmpty(currentX, currentY + 1))
                        {
                            dir = direction.UPRIGHT;
                        }
                        break;
                }
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

    bool tileIsEmpty(int x, int y)
    {
        int tile = ShowMapOnCamera.MAP[x, y];
        return tile == 0 || tile >= 900;
    }

    void OnTriggerEnter(Collider other)
    {
        if (other.tag == "Bullet" || other.tag == "Missile")
        {
            hp--;
            if (hp <= 0)
            {
                Destroy(gameObject);
            }
        }
    }
}
