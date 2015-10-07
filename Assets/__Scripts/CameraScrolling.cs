using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public enum CameraState
{
    HORIZONTAL,
    VERTICAL,
    TRANSITION
}

public class CameraScrolling : MonoBehaviour
{
    static public CameraScrolling S; // This is a Singleton of the CameraFollow class. - JB

    public Transform poi; // The Point Of Interest of the CameraFollow script. - JB
    private int transitionX;
    public Vector3 offset = new Vector3(0, 2, -15);
    private float camSpeed = .2f;

    private Camera cam;

    public CameraState state = CameraState.HORIZONTAL;
    private CameraState lastState = CameraState.HORIZONTAL;
    private int screenWidth = 16;
    
    private List<int> doorsXList = new List<int>();
    private int minDoor = int.MinValue;
    private int maxDoor = int.MaxValue;
    private bool scrollingRight;
    private int numDoors = 0;

    void Awake()
    {
        S = this;

        cam = GetComponent<Camera>();

        ResetCamera();
    }

    public void ResetCamera()
    {
        state = CameraState.HORIZONTAL;
        lastState = CameraState.HORIZONTAL;
        // Initially position the camera exactly over the poi - JB
        transform.position = poi.position + offset;
        doorsXList.Clear();
        minDoor = int.MinValue;
        maxDoor = int.MaxValue;
        numDoors = 0;
    }

    // Update is called once per frame - JB
    void FixedUpdate()
    {

        Vector3 position = transform.position;
        switch(state)
        {
            case CameraState.HORIZONTAL:
                position.x = poi.position.x;
                if (position.x > maxDoor - screenWidth / 2)
                {
                    position.x = maxDoor - screenWidth / 2;
                } else if (position.x < minDoor + screenWidth / 2)
                {
                    position.x = minDoor + screenWidth / 2;
                }
                break;
            case CameraState.VERTICAL:
                position.y = poi.position.y + 1;
                break;
            case CameraState.TRANSITION:
                if (transitionX > transform.position.x)
                {
                    position.x += camSpeed;
                } else
                {
                    position.x -= camSpeed;
                }
                
                if (Mathf.Abs(transitionX - transform.position.x) < camSpeed)
                {
                    state = lastState == CameraState.HORIZONTAL ? CameraState.VERTICAL : CameraState.HORIZONTAL;
                }
                break;
        }
        if (state != CameraState.TRANSITION && Mathf.Abs(poi.position.x - transform.position.x) > (screenWidth / 2 - 1))
        {
            scrollingRight = poi.position.x > transform.position.x;
            Samus.S.passThroughDoor(scrollingRight ? (float)maxDoor + 3f : (float)minDoor - 3f);
            onEnterDoor(scrollingRight ? maxDoor : minDoor);
        }
        position.x = RoundToNearestPixel(position.x, cam);
        position.y = RoundToNearestPixel(position.y, cam);

        transform.position = position;
    }

    // From https://www.reddit.com/r/Unity3D/comments/34ip2j/gaps_between_tiled_sprites_help/ - JB
    private float RoundToNearestPixel(float unityUnits, Camera viewingCamera)
    {
        float valueInPixels = (Screen.height / (viewingCamera.orthographicSize * 2)) * unityUnits;
        valueInPixels = Mathf.Round(valueInPixels);
        float adjustedUnityUnits = valueInPixels / (Screen.height / (viewingCamera.orthographicSize * 2));
        return adjustedUnityUnits;
    }

    public void onCreateDoor(int x)
    {
        if (doorsXList.Find((doorX) => doorX == x) == 0)
        {
            doorsXList.Add(x);
            if (x < transform.position.x && minDoor == int.MinValue)
            {
                minDoor = x;
            } else if (x > transform.position.x && maxDoor == int.MaxValue)
            {
                maxDoor = x;
            }
            numDoors++;
        }
    }

    public void onEnterDoor(int x)
    {
        lastState = state;
        state = CameraState.TRANSITION;
        doorsXList.Sort();
        if (scrollingRight)
        {
            minDoor = x + 1;
            maxDoor = int.MaxValue;
            int index = doorsXList.FindIndex((doorX) => doorX == minDoor);
            if (index + 1 < numDoors)
            {
                maxDoor = doorsXList[index + 1];
            }
        }
        else
        {
            maxDoor = x - 1;
            minDoor = int.MinValue;
            int index = doorsXList.FindIndex((doorX) => doorX == maxDoor);
            if (index - 1 >= 0)
            {
                minDoor = doorsXList[index - 1];
            }
        }
        if (lastState == CameraState.HORIZONTAL)
        {
            doorsXList.Clear();
            numDoors = 0;
            doorsXList.Add(minDoor);
            doorsXList.Add(minDoor - 1);
            doorsXList.Add(maxDoor);
            doorsXList.Add(maxDoor + 1);
        }
        transitionX = scrollingRight ? minDoor + screenWidth / 2 : maxDoor - screenWidth / 2;
    }
}
