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

    
     Serialize Fields show up in the inspector
     In the Editor : (if not done by default :) drag the specified GameObjects into the associated Inspector fields
     again: try to avoid too many serializefields ( thorougly discussed in the question-after-task-script)

     */
    [SerializeField]
    private GameObject testCubinho;
    [SerializeField]
    private GameObject infoCanvas;
    [SerializeField]
    private GameObject planeTable;
    [SerializeField]
    private GameObject nextCubeCanvas; 
    [SerializeField]
    private Material cubeInUseMaterial;         //chose the material for the currently active cube
    [SerializeField]
    private Material cubeNotInUseMaterial;      //choose the material for the non-active cubes
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
        /*
        This method should be called on the next-cube-button.
        once the button is pressed, it spawns a new cube and sets its colour to red,
        to visualise which cube is currently active.
        sets color of the old cube back to grey.
        a lot of this code are remnants of an older version : so feel free to strip it down
        */
        if (firstCube)
        {
            cubes.AddLast(Instantiate(testCubinho, cubeSpawnPosition, cubeSpawnRotation) as GameObject);  // spawn a new cube
            cube = cubes.Last.Value;
            cube.GetComponent<MeshRenderer>().material = cubeInUseMaterial; //this here sets the colour to red(in form of a material) for the first cube
            cube.GetComponent<LeapRTS>().PinchDetectorA = pinchDetectorA;   // pinch detectors so it can be interacted with via leap motion
            cube.GetComponent<LeapRTS>().PinchDetectorB = pinchDetectorB;
            pinchDetectorA.addCube(cube);
            pinchDetectorB.addCube(cube);
            firstCube = false;
        } else
        {
            cube = cubes.Last.Value;
            rts = cube.GetComponent<LeapRTS>();
            rts.enabled = false;
            cube.GetComponent<MeshRenderer>().material = cubeNotInUseMaterial;   // sets colour of old cube to non-active material
            cubes.AddLast(Instantiate(testCubinho, cubeSpawnPosition, cubeSpawnRotation) as GameObject);  //spawn a new cube
            cube = cubes.Last.Value;
            pinchDetectorA.addCube(cube);
            pinchDetectorB.addCube(cube);
            cube.GetComponent<MeshRenderer>().material = cubeNotInUseMaterial;      //set colour of the new cube to non-active-material (for now)
            rts = cube.GetComponent<LeapRTS>();
            // introduces a delay for bug fixing:
            // when next button was pressed, the spawned cube was being moved simultaneously ,since the leap detected a pinch
            StartCoroutine(WaitTilNextRTS(waitForRTS));         

        }
    }

    private IEnumerator WaitTilNextRTS(float seconds)
    {
        yield return new WaitForSeconds(seconds);
        cubeCounter++;
        Debug.LogWarning("Current RTS_Cube: " + (cubeCounter + 1));

        cube.GetComponent<MeshRenderer>().material = cubeInUseMaterial;             //set colour of newest cube to active-material

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
