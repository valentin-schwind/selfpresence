using UnityEngine;
using System.Collections;
using Leap.Unity;
using Leap.Unity.DetectionExamples;
using UnityEngine.UI;
using System.Collections.Generic;
using Leap;

public class DrawTask : MonoBehaviour {
    [SerializeField]
    private GameObject infoCanvas;
    private GameObject pinchDraw;
    PinchDraw pDraw;
    Typer typerCanvas;
    Text titleText;
    public int taskNumber;
    StraysGameManager gameManager;

    GameObject[] lineObjects;

    // Use this for initialization
    void Start () {

        /*
            When this script(on the current GameObject) is enabled,
            this script enables all the required Objects and Scripts for the task.
            First : find all the required Objects
        */
        gameManager = transform.GetComponent<StraysGameManager>();
        typerCanvas = infoCanvas.transform.Find("Dialogue").Find("Text").GetComponent<Typer>();
        pDraw = transform.GetComponent<PinchDraw>();
        titleText = infoCanvas.transform.Find("TitleBar").Find("Text").GetComponent<Text>();
        titleText.text = "Task " + gameManager.getCurrentTaskNumber() + ": DrawTask";
        
        // and then enable them
        infoCanvas.GetComponent<Canvas>().enabled = true;
        typerCanvas.enabled = true;
        pDraw.enabled = true;
    }

	public void disableTask () {
        // Delete the drawn lines and disable the task-objects
        lineObjects = GameObject.FindGameObjectsWithTag("DynamicTag");
        for (int i = 0; i < lineObjects.Length; i++)
        {
            Destroy(lineObjects[i]);
        }

        pDraw.enabled = false;
        infoCanvas.GetComponent<Canvas>().enabled = false;
        this.enabled = false;        
    }
}
