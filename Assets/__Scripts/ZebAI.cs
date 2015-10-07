using UnityEngine;
using System.Collections;

enum zebState
{
    STARTING,
    UPWARD,
    WAITING,
    ATTACKING
}

public class ZebAI : MonoBehaviour {
    public float speedUp = 5f;
    public float speedAir = 10f;
    public int originX;
    public int originY;
    public GameObject zebPrefab;
    private zebState state = zebState.STARTING;
    private int delay = 10;
    private Rigidbody rigid;
    private bool right = true;
    private int hp = 2;
    private int shot = 0;
    public GameObject energyPrefab, missilePrefab;

    // Use this for initialization
    void Start () {
        rigid = GetComponent<Rigidbody>();
        checkDir();
	}

    void checkDir()
    {
        if (Samus.S.transform.position.x < gameObject.transform.position.x)
        {
            right = false;
            transform.rotation = Quaternion.Euler(0, 180, 0);
        }
        else
        {
            right = true;
            transform.rotation = Quaternion.Euler(0, 0, 0);
        }
    }
	
	// Update is called once per frame
	void FixedUpdate () {
        int x = Mathf.RoundToInt(CameraScrolling.S.transform.position.x);
        int y = Mathf.RoundToInt(CameraScrolling.S.transform.position.y);
        int i0 = x - 18;
        int i1 = x + 18;
        int j0 = y - 18;
        int j1 = y + 18;

        if (transform.position.x < i0 - 9 || transform.position.x > i1 + 9
            || transform.position.y < j0 - 9 || transform.position.y > j1 + 9)
        {
            spawnNext();
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
            case zebState.STARTING:
                checkDir();
                if (Mathf.Abs(gameObject.transform.position.x - Samus.S.transform.position.x) < 3)
                {
                    state = zebState.UPWARD;
                }
                break;
            case zebState.UPWARD:
                if (gameObject.transform.position.y > Samus.S.transform.position.y + 1)
                {
                    rigid.velocity = new Vector3(0f, 0f);
                    state = zebState.WAITING;
                } else
                {
                    rigid.velocity = new Vector3(0f, speedUp);
                }
                break;
            case zebState.WAITING:
                --delay;
                if (delay <= 0)
                {
                    state = zebState.ATTACKING;
                }
                break;
            case zebState.ATTACKING:
                rigid.velocity = new Vector3(right ? speedAir : -speedAir, 0f);
                break;
        }
    }

    void OnTriggerEnter(Collider other)
    {
        if (other.tag == "Bullet" || other.tag == "Missile" || other.tag == "chargedShot")
        {
            if (other.tag == "Bullet")
                hp--;
            else
                hp = 0;
            shot = 3;
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
                spawnNext();
                Destroy(gameObject);
            }
        }
    }

    void spawnNext()
    {
        GameObject go = Instantiate(zebPrefab);
        go.transform.SetParent(ShowMapOnCamera.S.mapAnchor, true);
        go.transform.localPosition = new Vector3(originX, originY, 0);
        go.name = name;
    }
}
