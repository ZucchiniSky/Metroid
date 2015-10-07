using UnityEngine;
using System.Collections;

public class Tile : MonoBehaviour {
    public GameObject zoomerYellowPrefab;

    public bool ______________________;

    static Sprite[] spriteArray;

    public Texture2D spriteTexture;
    public int x, y;
    public int tileNum;
    public int missileDoorHp = 5;
    private BoxCollider bc;
    private Material mat;

    private SpriteRenderer sprend;

    private char destructibility;

    void Awake() {
        if (spriteArray == null) {
            spriteArray = Resources.LoadAll<Sprite>(spriteTexture.name);
        }

        bc = GetComponent<BoxCollider>();

        sprend = GetComponent<SpriteRenderer>();
    }

    public void SetTile(int eX, int eY, int eTileNum = -1) {
        if (x == eX && y == eY) return; // Don't move this if you don't have to. - JB

        x = eX;
        y = eY;
        transform.localPosition = new Vector3(x, y, 0);
        gameObject.name = x.ToString("D3") + "x" + y.ToString("D3");

        tileNum = eTileNum;
        if (tileNum == -1 && ShowMapOnCamera.S != null) {
            tileNum = ShowMapOnCamera.MAP[x, y];
            if (tileNum == 0) {
                ShowMapOnCamera.PushTile(this);
            }
        }

        if (tileNum >= 0 && tileNum <= spriteArray.Length)
        {
            sprend.sprite = spriteArray[tileNum];
            sprend.enabled = true;

			bc.enabled = true;
			
			if (ShowMapOnCamera.S != null)
			{
				SetCollider();
				destructibility = ShowMapOnCamera.S.destructibleS[tileNum];

                if ((destructibility >= '3' && destructibility <= '8') || (destructibility >= 'A' && destructibility <= 'F'))
                {
                    Tile[] doorTiles = getDoorTiles();
                    foreach (Tile i in doorTiles)
                    {
                        if (i == null || i.gameObject == null) continue;
                        i.gameObject.GetComponent<BoxCollider>().enabled = true;
                        i.gameObject.GetComponent<SpriteRenderer>().enabled = true;
                    }
                }
            }
		} else
		{
			// this tile is used to spawn an enemy
            sprend.enabled = false;

            bc.enabled = false;
        }

        gameObject.SetActive(true);
        if (ShowMapOnCamera.S != null) {
            if (ShowMapOnCamera.MAP_TILES[x, y] != null) {
                if (ShowMapOnCamera.MAP_TILES[x, y] != this) {
                    ShowMapOnCamera.PushTile(ShowMapOnCamera.MAP_TILES[x, y]);
                }
            } else {
                ShowMapOnCamera.MAP_TILES[x, y] = this;
            }
        }
    }

    void OnTriggerEnter(Collider other)
    {
        if (ShowMapOnCamera.S == null) return;

        if (other.gameObject.tag == "Bullet" || other.gameObject.tag == "Missile" || other.gameObject.tag == "chargedShot")
        {
            if ((destructibility >= '3' && destructibility <= '8') || (other.gameObject.tag == "Missile" && destructibility >= 'A' && destructibility <= 'F'))
            {
                // this tile represents a door;
                Tile[] doorTiles = getDoorTiles();
                foreach (Tile i in doorTiles)
                {
                    if (i == null || i.gameObject == null) continue;
                    bool enable = true;
                    if (other.gameObject.tag == "Missile" && destructibility >= 'A' && destructibility <= 'F')
                    {
                        --i.missileDoorHp;
                        if (i.missileDoorHp == 0)
                        {
                            enable = false;
                        }
                    } else
                    {
                        enable = false;
                    }
                    if (!enable)
                    {
                        i.gameObject.GetComponent<BoxCollider>().enabled = false;
                        i.gameObject.GetComponent<SpriteRenderer>().enabled = false;
                    }
                }
            }
        }
    }

    void OnCollisionEnter(Collision collision)
    {
        if (collision.collider.tag == "Player" && tileNum == 151)
        {
            Samus.S.onLavaCollision();
        }
    }

	// Arrange the collider for this tile
	void SetCollider() {
        
        // Collider info from collisionData
        bc.enabled = true;
        char c = ShowMapOnCamera.S.collisionS[tileNum];
        switch (c) {
        case 'S': // Whole
            bc.center = Vector3.zero;
            bc.size = Vector3.one;
            break;
        case 'Q': // Top, Left
            bc.center = new Vector3( -0.25f, 0.25f, 0 );
            bc.size =   new Vector3( 0.5f, 0.5f, 1 );
            break;
        case 'W': // Top
            bc.center = new Vector3( 0, 0.25f, 0 );
            bc.size =   new Vector3( 1, 0.5f, 1 );
            break;
        case 'E': // Top, Right
            bc.center = new Vector3( 0.25f, 0.25f, 0 );
            bc.size =   new Vector3( 0.5f, 0.5f, 1 );
            break;
        case 'A': // Left
            bc.center = new Vector3( -0.25f, 0, 0 );
            bc.size =   new Vector3( 0.5f, 1, 1 );
            break;
        case 'D': // Right
            bc.center = new Vector3( 0.25f, 0, 0 );
            bc.size =   new Vector3( 0.5f, 1, 1 );
            break;
        case 'Z': // Bottom, left
            bc.center = new Vector3( -0.25f, -0.25f, 0 );
            bc.size =   new Vector3( 0.5f, 0.5f, 1 );
            break;
        case 'X': // Bottom
            bc.center = new Vector3( 0, -0.25f, 0 );
            bc.size =   new Vector3( 1, 0.5f, 1 );
            break;
        case 'C': // Bottom, Right
            bc.center = new Vector3( 0.25f, -0.25f, 0 );
            bc.size =   new Vector3( 0.5f, 0.5f, 1 );
            break;
            
        default: // Anything else: _, |, etc.
            bc.enabled = false;
            break;
        }

    }

    Tile[] getDoorTiles()
    {
        Tile[] doorTiles = new Tile[6];
        if (destructibility == '3' || destructibility == 'A')
        {
            doorTiles[0] = this;
            doorTiles[1] = ShowMapOnCamera.MAP_TILES[x, y + 1];
            doorTiles[2] = ShowMapOnCamera.MAP_TILES[x, y + 2];
            doorTiles[3] = ShowMapOnCamera.MAP_TILES[x + 3, y];
            doorTiles[4] = ShowMapOnCamera.MAP_TILES[x + 3, y + 1];
            doorTiles[5] = ShowMapOnCamera.MAP_TILES[x + 3, y + 2];
        }
        else if (destructibility == '4' || destructibility == 'B')
        {
            doorTiles[0] = ShowMapOnCamera.MAP_TILES[x, y - 1];
            doorTiles[1] = this;
            doorTiles[2] = ShowMapOnCamera.MAP_TILES[x, y + 1];
            doorTiles[3] = ShowMapOnCamera.MAP_TILES[x + 3, y - 1];
            doorTiles[4] = ShowMapOnCamera.MAP_TILES[x + 3, y];
            doorTiles[5] = ShowMapOnCamera.MAP_TILES[x + 3, y + 1];
        }
        else if (destructibility == '5' || destructibility == 'C')
        {
            doorTiles[0] = ShowMapOnCamera.MAP_TILES[x, y - 2];
            doorTiles[1] = ShowMapOnCamera.MAP_TILES[x, y - 1];
            doorTiles[2] = this;
            doorTiles[3] = ShowMapOnCamera.MAP_TILES[x + 3, y - 2];
            doorTiles[4] = ShowMapOnCamera.MAP_TILES[x + 3, y - 1];
            doorTiles[5] = ShowMapOnCamera.MAP_TILES[x + 3, y];
        }
        else if (destructibility == '6' || destructibility == 'D')
        {
            doorTiles[0] = this;
            doorTiles[1] = ShowMapOnCamera.MAP_TILES[x, y + 1];
            doorTiles[2] = ShowMapOnCamera.MAP_TILES[x, y + 2];
            doorTiles[3] = ShowMapOnCamera.MAP_TILES[x - 3, y];
            doorTiles[4] = ShowMapOnCamera.MAP_TILES[x - 3, y + 1];
            doorTiles[5] = ShowMapOnCamera.MAP_TILES[x - 3, y + 2];
        }
        else if (destructibility == '7' || destructibility == 'E')
        {
            doorTiles[0] = ShowMapOnCamera.MAP_TILES[x, y - 1];
            doorTiles[1] = this;
            doorTiles[2] = ShowMapOnCamera.MAP_TILES[x, y + 1];
            doorTiles[3] = ShowMapOnCamera.MAP_TILES[x - 3, y - 1];
            doorTiles[4] = ShowMapOnCamera.MAP_TILES[x - 3, y];
            doorTiles[5] = ShowMapOnCamera.MAP_TILES[x - 3, y + 1];
        }
        else if (destructibility == '8' || destructibility == 'F')
        {
            doorTiles[0] = ShowMapOnCamera.MAP_TILES[x, y - 2];
            doorTiles[1] = ShowMapOnCamera.MAP_TILES[x, y - 1];
            doorTiles[2] = this;
            doorTiles[3] = ShowMapOnCamera.MAP_TILES[x - 3, y - 2];
            doorTiles[4] = ShowMapOnCamera.MAP_TILES[x - 3, y - 1];
            doorTiles[5] = ShowMapOnCamera.MAP_TILES[x - 3, y];
        }
        return doorTiles;
    }

}


/*
for (int j=0; j<h; j++) {
	string[] tiles = lines[j].Split(' ');
	w = tiles.Length;
	for (int i=0; i<w; i++) {
		foreach (Vector2 stopPoint in stopPoints) {
			if (i == stopPoint.x && j == stopPoint.y) {
				print ("Hit a stopPoint: "+i+"x"+j);
			}
		}
		
		// Find out which tile we're using - JB
		tileNum = int.Parse(tiles[i]);
		if (tileNum == 0) continue; // Skip tiles that we don't need. - JB
		
		go = Instantiate<GameObject>(quadPrefab);
		go.name = i.ToString("D3")+"x"+j.ToString("D3");
		rend = go.GetComponent<Renderer>();
		rend.material.mainTexture = mapSprites;
		rend.material.mainTextureScale = texScaleV2;
		
		x = tileNum % spriteSheetW;
		y = tileNum / spriteSheetW;
		
		rend.material.mainTextureOffset = new Vector2( x/ssF, y/ssF );
		
		go.transform.position = new Vector3(i, j, 0);
		
		// Collider info from collisionData
		BoxCollider bc = go.GetComponent<BoxCollider>();
		bc.enabled = true;
		char c = collisionS[tileNum];
		switch (c) {
		case 'S': // Whole
			bc.center = Vector3.zero;
			bc.size = Vector3.one;
			break;
		case 'Q': // Top, Left
			bc.center = new Vector3( -0.25f, 0.25f, 0 );
			bc.size =   new Vector3( 0.5f, 0.5f, 1 );
			break;
		case 'W': // Top
			bc.center = new Vector3( 0, 0.25f, 0 );
			bc.size =   new Vector3( 1, 0.5f, 1 );
			break;
		case 'E': // Top, Right
			bc.center = new Vector3( 0.25f, 0.25f, 0 );
			bc.size =   new Vector3( 0.5f, 0.5f, 1 );
			break;
		case 'A': // Left
			bc.center = new Vector3( -0.25f, 0, 0 );
			bc.size =   new Vector3( 0.5f, 1, 1 );
			break;
		case 'D': // Right
			bc.center = new Vector3( 0.25f, 0, 0 );
			bc.size =   new Vector3( 0.5f, 1, 1 );
			break;
		case 'Z': // Bottom, left
			bc.center = new Vector3( -0.25f, -0.25f, 0 );
			bc.size =   new Vector3( 0.5f, 0.5f, 1 );
			break;
		case 'X': // Bottom
			bc.center = new Vector3( 0, -0.25f, 0 );
			bc.size =   new Vector3( 1, 0.5f, 1 );
			break;
		case 'C': // Bottom, Right
			bc.center = new Vector3( 0.25f, -0.25f, 0 );
			bc.size =   new Vector3( 0.5f, 0.5f, 1 );
			break;
			
		default: // Anything else: _, |, etc.
			bc.enabled = false;
			break;
		}
	}
 */   