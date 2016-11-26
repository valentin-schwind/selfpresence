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
        gameManager = transform.GetComponent<StraysGameManager>();
        typerCanvas = infoCanvas.transform.Find("Dialogue").Find("Text").GetComponent<Typer>();
        pDraw = transform.GetComponent<PinchDraw>();
        titleText = infoCanvas.transform.Find("TitleBar").Find("Text").GetComponent<Text>();
        titleText.text = "Task " + gameManager.getCurrentTaskNumber() + ": DrawTask";
        infoCanvas.GetComponent<Canvas>().enabled = true;
        typerCanvas.enabled = true;
        pDraw.enabled = true;
    }

    // can be deleted now and instead, inserted in Start at the end
    //public void beginTask(int taskNumber)
    //{        
    //}
	
	// Update is called once per frame

    
	public void disableTask () {
        // Delete Draw(objects)
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
