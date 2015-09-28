using UnityEngine;
using System.Collections;

enum reoState
{
    NEW,
    PERCHED,
    DIVING,
    HOVERING,
    ASCENDING
}

public class ReoAI : MonoBehaviour {
    public float hp = 4f;
    public float shot = 0f;
    public int speedX = 7;
    public int speedY = 15;
    private reoState state = reoState.NEW;
    private Rigidbody rigid;
    private bool right = false;
    private static int initDelay = 50;
    private int delay = initDelay;
    private CapsuleCollider body;
    public int groundPhysicsLayerMask;
    public GameObject energyPrefab, missilePrefab;

    // Use this for initialization
    void Start () {
        rigid = GetComponent<Rigidbody>();
        body = GetComponent<CapsuleCollider>();
        groundPhysicsLayerMask = LayerMask.GetMask("Ground");
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        int x = Mathf.RoundToInt(CameraScrolling.S.transform.position.x);
        int y = Mathf.RoundToInt(CameraScrolling.S.transform.position.y);
        int i0 = x - 18;
        int i1 = x + 18;
        int j0 = y - 18;
        int j1 = y + 18;
        Vector3 vel = rigid.velocity;

        if (transform.position.x < i0 - 9 || transform.position.x > i1 + 9
            || transform.position.y < j0 - 9 || transform.position.y > j1 + 9)
        {
            Destroy(gameObject);
        }
        else if (transform.position.x < i0 || transform.position.x > i1
            || transform.position.y < j0 || transform.position.y > j1)
        {
            rigid.velocity = new Vector3(0f, 0f);
        }
        else
        {
            if (Physics.Raycast(transform.position, right ? Vector3.right : Vector3.left, body.radius, groundPhysicsLayerMask))
            {
                right = !right;
            }

            switch (state)
            {
                case reoState.NEW:
                    if (Samus.S.transform.position.x - transform.position.x < 5f)
                    {
                        state = reoState.PERCHED;
                    }
                    break;
                case reoState.PERCHED:
                    right = Samus.S.transform.position.x > transform.position.x;

                    --delay;
                    if (delay <= 0 || (delay < 40 && !Samus.S.grounded))
                    {
                        state = reoState.DIVING;
                    } else
                    {
                        rigid.velocity = new Vector3(0f, 0f, 0f);
                    }
                    break;
                case reoState.DIVING:
                    if (Physics.Raycast(transform.position, Vector3.down, 1f, groundPhysicsLayerMask))
                    {
                        if (Samus.S.grounded)
                        {
                            state = reoState.HOVERING;
                        } else
                        {
                            state = reoState.ASCENDING;
                        }
                    } else
                    {
                        rigid.velocity = new Vector3(right ? speedX : -speedX, -speedY);
                    }
                    break;
                case reoState.HOVERING:
                    if (!Samus.S.grounded)
                    {
                        state = reoState.ASCENDING;
                    } else
                    {
                        rigid.velocity = new Vector3(right ? speedX : -speedX, 0f);
                    }
                    break;
                case reoState.ASCENDING:
                    if (Physics.Raycast(transform.position, Vector3.up, body.radius, groundPhysicsLayerMask))
                    {
                        delay = initDelay;
                        state = reoState.PERCHED;
                    } else
                    {
                        rigid.velocity = new Vector3(right ? speedX : -speedX, speedY);
                    }
                    break;
            }
            if (shot >= 0)
            {
                shot--;
                rigid.velocity = new Vector3(0, 0, 0);
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
            if (hp <= 0)
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
