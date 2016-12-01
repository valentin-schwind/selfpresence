using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using Leap.Unity;
using UnityEngine.UI;
using System;

public class PyramidTask : MonoBehaviour
{
    /*
        Once this script is enabled : enable all required objects.
        After task is done : disable all the objects

    *//
    // Serialize Fields show up in the inspector
    // In the Editor : (if not done by default :) drag the specified GameObjects into the associated Inspector fields
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
    private bool firstCube = true;

    public Quaternion cubeSpawnRotation = Quaternion.Euler(55, 0, 55);
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
        //Find the required GameObjects
        gameManager = transform.GetComponent<StraysGameManager>();
        typerCanvas = infoCanvas.transform.Find("Dialogue").Find("Text").GetComponent<Typer>();
        titleText = infoCanvas.transform.Find("TitleBar").Find("Text").GetComponent<Text>();
        titleText.text = "Task " + gameManager.getCurrentTaskNumber() + ": Pyramid Task";
        //enable them
        planeTable.SetActive(true);
        nextCubeCanvas.GetComponent<Canvas>().enabled = true;
        infoCanvas.GetComponent<Canvas>().enabled = true;
        typerCanvas.enabled = true;
    }

    public void nextCube()
    {
        if (firstCube)
        {
            cubes.AddLast(Instantiate(testCubinho, cubeSpawnPosition, cubeSpawnRotation) as GameObject);
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
            cubes.AddLast(Instantiate(testCubinho, cubeSpawnPosition, cubeSpawnRotation) as GameObject);
            cube = cubes.Last.Value;
            pinchDetectorA.addCube(cube);
            pinchDetectorB.addCube(cube);
            cube.GetComponent<MeshRenderer>().material = cubeNotInUseMaterial;
            rts = cube.GetComponent<LeapRTS>();

            StartCoroutine(WaitTilNextRTS(waitForRTS));

        }
    }

    private IEnumerator WaitTilNextRTS(float seconds)
    {
        yield return new WaitForSeconds(seconds);
        cubeCounter++;
        Debug.LogWarning("Current RTS_Cube: " + (cubeCounter + 1));
        cube.GetComponent<MeshRenderer>().material = cubeInUseMaterial;

        rts.enabled = true;
        cubes.Last.Value.GetComponent<LeapRTS>().PinchDetectorA = pinchDetectorA;
        cubes.Last.Value.GetComponent<LeapRTS>().PinchDetectorB = pinchDetectorB;
    }

    public void disableTask()
    {   //disables the task
        // destroys all created cubes
        // disables all gameObjects associated with pyramid task
        foreach (GameObject cube in cubes)
        {
                Destroy(cube);
        }
        pinchDetectorA.setRigidOff();
        pinchDetectorB.setRigidOff();
        infoCanvas.GetComponent<Canvas>().enabled = false;
            this.enabled = false;
            planeTable.SetActive(false);
            nextCubeCanvas.GetComponent<Canvas>().enabled = false;
    }
}
