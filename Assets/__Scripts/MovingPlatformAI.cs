using UnityEngine;
using System.Collections;

public class MovingPlatformAI : MonoBehaviour
{
    public Rigidbody rigid;
    public CapsuleCollider body;
    public Sprite poweredOn;

    private bool charged = false;
    private bool dir = true;
    private RigidbodyConstraints moving = RigidbodyConstraints.FreezeRotation | RigidbodyConstraints.FreezePositionY | RigidbodyConstraints.FreezePositionZ;

    // Use this for initialization
    void Start()
    {
        rigid = GetComponent<Rigidbody>();
        rigid.constraints = RigidbodyConstraints.FreezeAll;
        body = GetComponent<CapsuleCollider>();
    }

    void FixedUpdate()
    {
        int x = Mathf.RoundToInt(CameraScrolling.S.transform.position.x);
        int y = Mathf.RoundToInt(CameraScrolling.S.transform.position.y);
        int i0 = x - 18;
        int i1 = x + 18;
        int j0 = y - 18;
        int j1 = y + 18;
        if (transform.position.x < i0 || transform.position.x > i1
            || transform.position.y < j0 || transform.position.y > j1)
        {
            Destroy(gameObject);
        }
        if (charged)
        {
            rigid.constraints = moving;
            if (Physics.Raycast(transform.position, dir ? Vector3.right : Vector3.left, 1.5f, LayerMask.GetMask("Ground")))
            {
                dir = !dir;
            }
            rigid.velocity = new Vector3(dir ? 3f : -3f, 0f, 0f);
        } else
        {
            rigid.velocity = new Vector3(0f, 0f, 0f);
            rigid.constraints = RigidbodyConstraints.FreezeAll;
        }
    }

    void OnTriggerEnter(Collider other)
    {
        if (other.tag == "chargedShot")
        {
            charged = other.GetComponent<SamusBullet>().charge == 10;
            if (charged)
            {
                GetComponent<SpriteRenderer>().sprite = poweredOn;
            }
        }
    }
}
