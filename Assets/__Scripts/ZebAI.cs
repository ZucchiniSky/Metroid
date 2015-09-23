using UnityEngine;
using System.Collections;

enum state
{
    STARTING,
    UPWARD,
    WAITING,
    ATTACKING
}

public class ZebAI : MonoBehaviour {
    public float speedUp = 5f;
    public float speedAir = 10f;
    private state state = state.STARTING;
    private int delay = 10;
    private Rigidbody rigid;
    private bool right = true;
    private int hp = 2;
    private int shot = 0;

	// Use this for initialization
	void Start () {
        rigid = GetComponent<Rigidbody>();
        if (Samus.S.transform.position.x < gameObject.transform.position.x)
        {
            right = false;
            transform.rotation = Quaternion.Euler(0, 180, 0);
        } else
        {
            right = true;
        }
	}
	
	// Update is called once per frame
	void FixedUpdate () {
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

        if (shot > 0)
        {
            rigid.velocity = new Vector3(0f, 0f);
            --shot;
            return;
        }

        switch (state)
        {
            case state.STARTING:
                if (Mathf.Abs(gameObject.transform.position.x - Samus.S.transform.position.x) < 6)
                {
                    state = state.UPWARD;
                }
                break;
            case state.UPWARD:
                if (gameObject.transform.position.y > Samus.S.transform.position.y)
                {
                    rigid.velocity = new Vector3(0f, 0f);
                    state = state.WAITING;
                } else
                {
                    rigid.velocity = new Vector3(0f, speedUp);
                }
                break;
            case state.WAITING:
                --delay;
                if (delay <= 0)
                {
                    state = state.ATTACKING;
                }
                break;
            case state.ATTACKING:
                rigid.velocity = new Vector3(right ? speedAir : -speedAir, 0f);
                break;
        }
    }

    void OnTriggerEnter(Collider other)
    {
        if (other.tag == "Bullet" || other.tag == "Missile")
        {
            hp--;
            shot = 3;
            if (hp <= 0)
            {
                Destroy(gameObject);
            }
        }
    }
}
