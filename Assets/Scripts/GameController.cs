using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System;
using UnityEngine.EventSystems;

public class GameController : MonoBehaviour
{
    public GameObject restartButton;
    private CubePos nowCube = new CubePos(0, 1, 0);
    public float cubeChangePlaceSpeed = 0.5f;
    public Transform cubeToPlace;
    public GameObject cubeToCreate, allCubes, vfx, LoseMusic, calmMusic, verka;
    private Rigidbody allCubesRB;
    private bool isLose = false;
    private Coroutine showCubePlace;
    public Text scoreTxt;
    private float camMoveToYPosition, camMoveSpeed = 5f;
    private Transform mainCam;
    public GameObject[] canvasStartPage;

    private bool firstCube = false;
    [SerializeField]
    private List<Vector3> allCubesPositions = new List<Vector3>()
    {
        new Vector3(0, 0, 0),
        new Vector3(1, 0, 0),
        new Vector3(-1, 0, 0),
        new Vector3(0, 1, 0),
        new Vector3(0, 0, 1),
        new Vector3(0, 0, -1),
        new Vector3(1, 0, 1),
        new Vector3(-1, 0, 1),
        new Vector3(1, 0, -1),
        new Vector3(-1, 0, -1),
    };
    [SerializeField]
    private int prevCountMaxHorizontal = 0;
    [SerializeField]
    private int maxX = 0;
    [SerializeField]
    private int maxY = 0;
    [SerializeField]
    private int maxZ = 0;
    [SerializeField]
    int maxHor = 0;
    bool isPlaying;
    bool startedEver;
    bool isZero = false;
    private void Update()
    {
        if (PlayerPrefs.GetString("music") == "No" && isPlaying != false && !isLose)
        {
            verka.GetComponent<AudioSource>().Pause();
            
        }
        if(PlayerPrefs.GetString("music") !="No" && !isLose)
        {
            verka.GetComponent<AudioSource>().UnPause();
            if (isPlaying == false)
            {
                verka.GetComponent<AudioSource>().Play();
                isPlaying = true;
            }
        }

        if ((Input.GetMouseButtonDown(0) || Input.touchCount > 0) && cubeToPlace != null && allCubes != null && !EventSystem.current.IsPointerOverGameObject())
        {
#if !UNITY_EDITOR
            if(Input.GetTouch(0).phase != TouchPhase.Began)
            {
                return;
            }
#endif 
            
            if(!firstCube)
            {
                firstCube = true;
                foreach(GameObject ob in canvasStartPage)
                {
                    ob.SetActive(false);
                }
            }
            if(IsPositionEmpty(cubeToPlace.position))
            {

                GameObject newCube = Instantiate(
                    cubeToCreate,
                    cubeToPlace.position,
                    Quaternion.identity
                    ) as GameObject;
                newCube.transform.SetParent(allCubes.transform);
                nowCube.setVector(cubeToPlace.position);
                allCubesPositions.Add(nowCube.getVector());
                Instantiate(vfx, cubeToPlace.position, Quaternion.identity);
            
                allCubesRB.isKinematic = true;
                allCubesRB.isKinematic = false;
                if(PlayerPrefs.GetString("music") != "No")
                {

                    GetComponent<AudioSource>().Play();
                }
                MoveCameraCahngeBg();
            }
        }
        
        if(!isLose && allCubesRB.velocity.magnitude > 0.05f)
        {
            Destroy(cubeToPlace.gameObject);
            isLose = true;
            StopCoroutine(showCubePlace);
            restartButton.SetActive(true);
            if (PlayerPrefs.GetString("music") != "No")
            {
                verka.GetComponent<AudioSource>().Pause();
                LoseMusic.GetComponent<AudioSource>().Play();
            }
        }
        mainCam.localPosition = Vector3.MoveTowards(mainCam.localPosition, new Vector3(mainCam.localPosition.x, camMoveToYPosition, mainCam.localPosition.z), 
            camMoveSpeed*Time.deltaTime);
    }
    private bool IsPositionEmpty(Vector3 targetPos)
    {
        if (targetPos.y == 0)
        {
            return false;
        }
        foreach (Vector3 pos in allCubesPositions)
        {
            if (pos.x == targetPos.x && pos.y == targetPos.y && pos.z == targetPos.z)
            {
                return false;
            }
        }
        return true;

    }
    private void Start()
    {
        scoreTxt.text = "<size=40>Now score: " + 0 + "\n</size> <color=blue><size=40> Best score: " + PlayerPrefs.GetInt("score") + " </size></color>";
        if (PlayerPrefs.GetString("music") != "No" && startedEver == false)
        {
            verka.GetComponent<AudioSource>().Play();
            isPlaying = true;
        }
        if (PlayerPrefs.GetString("music") != "No" && startedEver == true)
        {
            verka.GetComponent<AudioSource>().UnPause();
            isPlaying = true;
        }
        startedEver = true;
        mainCam = Camera.main.transform;
        camMoveToYPosition = 5.9f + nowCube.y - 1f;
        allCubesRB = allCubes.GetComponent<Rigidbody>();
        showCubePlace = StartCoroutine(ShowCubePlace());
    }
    IEnumerator ShowCubePlace()
    {
        while (true)
        {
            SpawnPositions();
            yield return new WaitForSeconds(cubeChangePlaceSpeed);
        }
    }
    private void SpawnPositions()
    {
        
        List<Vector3> positions = new List<Vector3>();
        if (IsPositionEmpty(new Vector3(nowCube.x + 1, nowCube.y, nowCube.z)) && (nowCube.x + 1 != cubeToPlace.position.x || isZero))
        {
            positions.Add(new Vector3(nowCube.x + 1, nowCube.y, nowCube.z));
        }
        if (IsPositionEmpty(new Vector3(nowCube.x - 1, nowCube.y, nowCube.z)) && (nowCube.x - 1 != cubeToPlace.position.x || isZero))
        {
            positions.Add(new Vector3(nowCube.x - 1, nowCube.y, nowCube.z));
        }
        if (IsPositionEmpty(new Vector3(nowCube.x, nowCube.y+1, nowCube.z)) && (nowCube.y + 1 != cubeToPlace.position.y || isZero))
        {
            positions.Add(new Vector3(nowCube.x, nowCube.y+1, nowCube.z));
        }
        if (IsPositionEmpty(new Vector3(nowCube.x, nowCube.y-1, nowCube.z)) && (nowCube.y - 1 != cubeToPlace.position.y || isZero))
        {
            positions.Add(new Vector3(nowCube.x, nowCube.y-1, nowCube.z));
        }
        else if (IsPositionEmpty(new Vector3(nowCube.x, nowCube.y, nowCube.z+1)) && (nowCube.z + 1 != cubeToPlace.position.z || isZero))
        {
            positions.Add(new Vector3(nowCube.x , nowCube.y, nowCube.z+1));
        }
        if (IsPositionEmpty(new Vector3(nowCube.x, nowCube.y, nowCube.z-1)) && (nowCube.z - 1 != cubeToPlace.position.z || isZero))
        {
            positions.Add(new Vector3(nowCube.x, nowCube.y, nowCube.z-1));
        }



        if(positions.Count > 1)
        {
            cubeToPlace.position = positions[UnityEngine.Random.Range(0, positions.Count)];
            isZero = false;
        }
        else if(positions.Count == 1)
        {
            cubeToPlace.position = positions[0];
            isZero = true;
        }
        else
        {
            if (!isLose )
            {
                Destroy(cubeToPlace.gameObject);
                isLose = true;
                StopCoroutine(showCubePlace);
                restartButton.SetActive(true);
                if (PlayerPrefs.GetString("music") != "No")
                {
                    LoseMusic.GetComponent<AudioSource>().Play();
                }
            }
        }
    }

    private void MoveCameraCahngeBg()
    {
        
        
        foreach(Vector3 pos in allCubesPositions)
        {
            if(Mathf.Abs(Convert.ToInt32(pos.x)) > maxX)
            {
                maxX = Mathf.Abs(Convert.ToInt32(pos.x));
            }
            if (Mathf.Abs(Convert.ToInt32(pos.y)) > maxY)
            {
                maxY = Convert.ToInt32(pos.y);
            }
            if (Mathf.Abs(Convert.ToInt32(pos.z)) > maxZ)
            {
                maxZ = Mathf.Abs(Convert.ToInt32(pos.z));
            }
            maxY--;
            if(PlayerPrefs.GetInt("score") < maxY)
            {
                PlayerPrefs.SetInt("score", maxY);
            }
            scoreTxt.text = "<size=40>Now score: " + maxY + "\n</size> <color=blue><size=40> Best score: " + PlayerPrefs.GetInt("score")+" </size></color>";
            camMoveToYPosition = 5.9f + nowCube.y - 1f;
            maxHor = maxX > maxZ ? maxX : maxZ;
            if(maxHor%2 == 0 && prevCountMaxHorizontal != maxHor)
            {
                mainCam.localPosition -= new Vector3(0, 0, 4f);
                prevCountMaxHorizontal = maxHor;
            }

        }
    }
}



struct CubePos
{
    public int x, y, z;
    public CubePos(int x, int y, int z)
    {
        this.x = x;
        this.y = y;
        this.z = z;
    }

    public Vector3 getVector()
    {
        return new Vector3(x, y, z);
    }

    public void setVector(Vector3 pos)
    {
        x = Convert.ToInt32(pos.x);
        y = Convert.ToInt32(pos.y);
        z = Convert.ToInt32(pos.z);
    }
}