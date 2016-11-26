using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using Leap.Unity;
using UnityEngine.UI;
using System;

public class PyramidTask : MonoBehaviour
{
    [SerializeField]
    private GameObject testCubinho;
    [SerializeField]
    private GameObject infoCanvas;
    [SerializeField]
    private GameObject planeTable;
    [SerializeField]
    private GameObject nextCubeCanvas;
    [SerializeField]
    private Material cubeInUseMaterial;
    [SerializeField]
    private Material cubeNotInUseMaterial;
    [SerializeField]
    private PinchDetector pinchDetectorA;
    [SerializeField]
    private PinchDetector pinchDetectorB;
    public Quaternion cubeSpwanRotation = Quaternion.Euler(55, 0, 55);

    private bool firstCube = true;
    Vector3 cubeSpawnPosition = new Vector3(-1.6f, 0.05f, -1.5f);
    public int taskNumber;
    private GameObject cube;
    LeapRTS rts;
    Typer typerCanvas;
    Text titleText;
    int cubeCounter = 0;

    LinkedList<GameObject> cubes = new LinkedList<GameObject>();

    StraysGameManager gameManager;
    [Tooltip("Set time in seconds til RTS will be activated after cube-spawn")]
    public float waitForRTS = 0.5f;

    void Start ()
    {
        planeTable.SetActive(true);
        nextCubeCanvas.GetComponent<Canvas>().enabled = true;
        gameManager = transform.GetComponent<StraysGameManager>();
        typerCanvas = infoCanvas.transform.Find("Dialogue").Find("Text").GetComponent<Typer>();
        titleText = infoCanvas.transform.Find("TitleBar").Find("Text").GetComponent<Text>();
        titleText.text = "Task " + gameManager.getCurrentTaskNumber() + ": Pyramid Task";
        infoCanvas.GetComponent<Canvas>().enabled = true;
        typerCanvas.enabled = true;
        Debug.LogWarning("Starting with RTS_Cube: " + (cubeCounter + 1));
    }

    
	void Update ()
    {
        if (Input.GetKeyUp(KeyCode.N))
        {
            Debug.Log("n gedrückt");
            nextCube();
        }

    }

    public void nextCube()
    {
        if (firstCube)
        {
            //cubes.AddLast(Instantiate(testCubinho) as GameObject);
            cubes.AddLast(Instantiate(testCubinho, cubeSpawnPosition, cubeSpwanRotation) as GameObject);
            cube = cubes.Last.Value;
            cube.GetComponent<MeshRenderer>().material = cubeInUseMaterial;
            cube.GetComponent<LeapRTS>().PinchDetectorA = pinchDetectorA;
            cube.GetComponent<LeapRTS>().PinchDetectorB = pinchDetectorB;
            pinchDetectorA.addCube(cube);
            pinchDetectorB.addCube(cube);
            firstCube = false;
        } else
        {
            cube = cubes.Last.Value;
            rts = cube.GetComponent<LeapRTS>();
            rts.enabled = false;
            cube.GetComponent<MeshRenderer>().material = cubeNotInUseMaterial;
            //cubes.AddLast(Instantiate(testCubinho) as GameObject);
            cubes.AddLast(Instantiate(testCubinho, cubeSpawnPosition, cubeSpwanRotation) as GameObject);
            cube = cubes.Last.Value;
            pinchDetectorA.addCube(cube);
            pinchDetectorB.addCube(cube);
            cube.GetComponent<MeshRenderer>().material = cubeNotInUseMaterial;
            rts = cube.GetComponent<LeapRTS>();
            Debug.Log("GetComponent rts und Coroutine wird aufgerufen");
            StartCoroutine(WaitTilNextRTS(waitForRTS));

        }
    }

    private IEnumerator WaitTilNextRTS(float seconds)
    {
        Debug.Log("Coroutine wurde aufgerufen");
        yield return new WaitForSeconds(seconds);
        Debug.Log("in Coroutine nach WaitForSeconds: " + seconds);
        cubeCounter++;
        Debug.LogWarning("Current RTS_Cube: " + (cubeCounter + 1));
        cube.GetComponent<MeshRenderer>().material = cubeInUseMaterial;

        rts.enabled = true;
        cubes.Last.Value.GetComponent<LeapRTS>().PinchDetectorA = pinchDetectorA;
        cubes.Last.Value.GetComponent<LeapRTS>().PinchDetectorB = pinchDetectorB;
    }

    public void disableTask()
    {
        foreach (GameObject cube in cubes)
        {
                Destroy(cube);
        }
        pinchDetectorA.setRigidOff();
        pinchDetectorB.setRigidOff();
        infoCanvas.GetComponent<Canvas>().enabled = false;
            this.enabled = false;
            Debug.LogWarning("Canvas sollte disabled sein");
            planeTable.SetActive(false);
            nextCubeCanvas.GetComponent<Canvas>().enabled = false;
    }
}
