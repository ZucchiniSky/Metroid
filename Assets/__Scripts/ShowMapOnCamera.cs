﻿using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class ShowMapOnCamera : MonoBehaviour {


    static public ShowMapOnCamera   S;
	static public int[,]		MAP;
	static public List<Tile>	TILE_POOL;
    static public Tile[,]       MAP_TILES; // This will largely remain empty
    static public Vector2       TEX_SCALE;
    static public int           SPRITE_SHEET_W;
    static public float         SSF;

    public int              ss = 16;
    public Vector2          screenSize = new Vector2(32,30);
    public bool[] Items = new bool[3];
    public int              screenSizeOverage = 2;
    public int              tileClearOverage = 2;
    public Texture2D        mapSprites;

    public TextAsset        mapData;
    public TextAsset        collisionData;
    public TextAsset        destructibleData;
    public GameObject       tilePrefab;
    public Vector2[]        stopPoints;

	public GameObject morphBallPowerupPrefab, MissileItemPrefab, LongBeamPrefab;
    public GameObject zoomerPrefab, zoomerRedPrefab, ripperPrefab, skreePrefab, zebPrefab, reoPrefab
        , scottPrefab, platformPrefab;

    public bool             ________________;
    public int              w, h;
    public string           collisionS, destructibleS;
    public int              screenW, screenH, screenW2, screenH2;
    public Transform        mapAnchor;
    public int              spriteSheetW;

    private HashSet<int> missilePackX = new HashSet<int>();


    void Awake() {
        S = this;

        SSF = (float) ss;
        float texScale = 1.0f / SSF;
        TEX_SCALE = Vector2.one * texScale;

        SPRITE_SHEET_W = mapSprites.width / ss;
    }


	void Start () {
		// Remove the line endings from the text of the colision and destructible data
        collisionS = RemoveLineEndings( collisionData.text );
        destructibleS = RemoveLineEndings( destructibleData.text );

		// Read in the map data
		string[] lines = mapData.text.Split('\n');
		h = lines.Length;
		string[] tileNums = lines[0].Split(' ');
		w = tileNums.Length;

		// Place the map data into a 2D Array to make it faster to access
		MAP = new int[w,h];
		for (int j=0; j<h; j++) {
			tileNums = lines[j].Split(' '); // Yes, this is slightly inefficient because it repeats a prev line for j=0. Does that actually matter? - JB
			for (int i=0; i<w; i++) {
				MAP[i,j] = int.Parse( tileNums[i] );
			}
		}


		// Generate the mapAnchor to which all of the Tiles will be parented
		GameObject go;
		go = new GameObject("MapAnchor");
		mapAnchor = go.transform;

		// Generate quad pool
		screenW = (int)screenSize.x + 2*screenSizeOverage;
		screenH = (int)screenSize.y + 2*screenSizeOverage;
        screenW2 = screenW/2;
        screenH2 = screenH/2; // Because screenH is 15, this will be a little short, but screenSizeOverage takes care of that - JB
		TILE_POOL = new List<Tile>();

        MAP_TILES = new Tile[w,h]; // Should fill with nulls - JB

		RedrawScreen(true);
	}


    void FixedUpdate() {
        RedrawScreen();
    }


    public void RedrawScreen(bool clearAll=false) {
        if (clearAll) {
            // Clear every Tile that was on screen
            for (int i=0; i<w; i++) {
                for (int j=0; j<h; j++) {
                    if (MAP_TILES[i,j] != null) {
                        PushTile( MAP_TILES[i,j] ); // Move this tile back on to the stack
                    }
                }
            }
        }

        int x = Mathf.RoundToInt(CameraScrolling.S.transform.position.x);
        int y = Mathf.RoundToInt(CameraScrolling.S.transform.position.y);
        
        int i0 = x - screenW2;
        int i1 = x + screenW2;
        int j0 = y - screenH2;
        int j1 = y + screenH2;

        Tile t;
        int tileNum;
        for (int i=i0 - tileClearOverage; i<i1 + tileClearOverage; i++) {
            for (int j=j0 - tileClearOverage; j<j1 + tileClearOverage; j++) {
                if (i<0 || j<0 || i>=ShowMapOnCamera.S.w || j>=ShowMapOnCamera.S.h ) { // Don't go out of bounds
                    continue;
                }
                if (i<i0 || i>i1 || j<j0 || j>j1) {     // Offscreen Tile
                    if (MAP_TILES[i,j] != null) {
                        PushTile( MAP_TILES[i,j] );
                    }
                    continue;
                } else {                                // On-Screen Tile
                    tileNum = MAP[i,j];
                    
                    if (tileNum == 0) { // Empty space
                        if (MAP_TILES[i,j] != null) {
                            PushTile( MAP_TILES[i,j] );
                        }
                    } else {
                        if (MAP_TILES[i,j] == null) {
                            t = GetTile();
							t.SetTile(i,j);
                            if (tileNum == 111)
                            {
                                CameraScrolling.S.onCreateDoor(i);
                            }
							if (tileNum > 900)
							{
                                GameObject go = null;
                                string name = i.ToString("D3") + "x" + j.ToString("D3") + "spawnable";
                                if (tileNum == 901 && GameObject.Find("/" + mapAnchor.name + "/" + name) == null)
                                {
                                    go = Instantiate(zoomerPrefab);
                                    ZoomerAI zoomerAi = go.GetComponent<ZoomerAI>();
                                    zoomerAi.originX = i;
                                    zoomerAi.originY = j;
                                    zoomerAi.currentX = i;
                                    zoomerAi.currentY = j;
                                }
                                if (tileNum == 902 && GameObject.Find("/" + mapAnchor.name + "/" + name) == null)
                                {
                                    go = Instantiate(zoomerRedPrefab);
                                    ZoomerAI zoomerAi = go.GetComponent<ZoomerAI>();
                                    zoomerAi.originX = i;
                                    zoomerAi.originY = j;
                                    zoomerAi.currentX = i;
                                    zoomerAi.currentY = j;
                                }
                                else if (tileNum == 903 && GameObject.Find("/" + mapAnchor.name + "/" + name) == null)
                                {
                                    go = Instantiate(skreePrefab);
                                }
                                else if (tileNum == 904 && GameObject.Find("/" + mapAnchor.name + "/" + name) == null)
                                {
                                    go = Instantiate(ripperPrefab);
                                }
                                else if (tileNum == 905 && GameObject.Find("/" + mapAnchor.name + "/" + name) == null)
                                {
                                    go = Instantiate(reoPrefab);
                                }
                                else if (tileNum == 906 && GameObject.Find("/" + mapAnchor.name + "/" + name) == null)
                                {
                                    go = Instantiate(zebPrefab);
                                    ZebAI zebAi = go.GetComponent<ZebAI>();
                                    zebAi.originX = i;
                                    zebAi.originY = j;
                                }
                                else if (tileNum == 907 && GameObject.Find("/" + mapAnchor.name + "/" + name) == null)
                                {
                                    go = Instantiate(scottPrefab);
                                }
                                else if (tileNum == 908 && GameObject.Find("/" + mapAnchor.name + "/" + name) == null)
                                {
                                    go = Instantiate(platformPrefab);
                                }
                                else if (tileNum == 950 && Items[0] == false)
								{
									go = Instantiate(morphBallPowerupPrefab);
                                    Items[0] = true;
								}
                                else if(tileNum == 951 && !missilePackX.Contains(i))
                                {
                                    go = Instantiate(MissileItemPrefab);
                                    missilePackX.Add(i);
                                }
                                else if(tileNum == 952 && Items[2] == false)
                                {
                                    go = Instantiate(LongBeamPrefab);
                                    Items[2] = true;
                                }
                                if (go != null)
                                {
                                    go.transform.SetParent(mapAnchor, true);
                                    go.transform.localPosition = new Vector3(i, j, 0);
                                    go.name = name;
                                }
                            }
						}
					}
				}
			}
		}
	}
	


	static public Tile GetTile() {
        int n = TILE_POOL.Count-1;

        // If the pool is empty, create a new Tile
        if (n < 0) {
            GameObject go = Instantiate<GameObject>( S.tilePrefab );
            go.transform.SetParent(S.mapAnchor, true);
            go.SetActive(false);
            return( go.GetComponent<Tile>() );
        }

        Tile t = TILE_POOL[ n ];
        TILE_POOL.RemoveAt( n );
        return t;
	}

	static public void PushTile(Tile t) {
        // Remove the Tile from MAP_TILES if necessary
        if (t.x>=0 && t.x<S.w && t.y>=0 && t.y<S.h) {
            if (MAP_TILES[t.x, t.y] == t) {
                MAP_TILES[t.x, t.y] = null;
            }
        }

		t.gameObject.SetActive(false);
		TILE_POOL.Add(t);
	}

	
	static public string ReorderLinesOfDataFiles(string sIn) {
		string sOut;
		sIn = sIn.Trim();
		string[] lines = sIn.Split('\n');
		sOut = "";
		for (int i=lines.Length-1; i>=0; i--) {
			sOut += lines[i];
        }
        return sOut;
    }

    public static string RemoveLineEndings(string sIn) {
        if(System.String.IsNullOrEmpty(sIn)) {
            return sIn;
        }
        string lineSeparator = ((char) 0x2028).ToString();
        string paragraphSeparator = ((char)0x2029).ToString();
        
        return sIn.Replace("\r\n", string.Empty).Replace("\n", string.Empty).Replace("\r", string.Empty).Replace("\f", string.Empty).Replace(lineSeparator, string.Empty).Replace(paragraphSeparator, string.Empty);
    }



}

