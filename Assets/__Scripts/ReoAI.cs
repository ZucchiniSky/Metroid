using UnityEngine;
using System.Collections;

enum reoState
{
    PERCHED,
    DIVING,
    HOVERING,
    ASCENDING
}

public class ReoAI : MonoBehaviour {

    public int speedX = 7;
    public int speedY = 8;
    private reoState state = reoState.PERCHED;
    private Rigidbody rigid;
    private bool right = false;
    private static int initDelay = 50;
    private int delay = initDelay;
    private CapsuleCollider body;
    public int groundPhysicsLayerMask;

    // Use this for initialization
    void Start () {
        rigid = GetComponent<Rigidbody>();
        body = GetComponent<CapsuleCollider>();
        groundPhysicsLayerMask = LayerMask.GetMask("Ground");
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        int x = Mathf.RoundToInt(CameraFollow.S.transform.position.x);
        int y = Mathf.RoundToInt(CameraFollow.S.transform.position.y);
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
            vel.x = 0;
            vel.y = 0;
        }
        else
        {
            if (Physics.Raycast(transform.position, right ? Vector3.right : Vector3.left, body.radius, groundPhysicsLayerMask))
            {
                right = !right;
            }

            switch (state)
            {
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
                    if (Physics.Raycast(transform.position, Vector3.down, 2f, groundPhysicsLayerMask))
                    {
                        if (Samus.S.grounded)
                        {
                            delay = initDelay;
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
                    --delay;
                    if (!Samus.S.grounded || delay <= 0)
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
        }
    }
}
