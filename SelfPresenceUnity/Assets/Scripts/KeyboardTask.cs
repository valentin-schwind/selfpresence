using UnityEngine;
using System.Collections;
using Leap.Unity;
using Leap.Unity.DetectionExamples;
using UnityEngine.UI;
using System.Collections.Generic;
using Leap.Unity.InputModule;

public class KeyboardTask : MonoBehaviour {

    [SerializeField]
    private GameObject infoCanvas;
    [SerializeField]
    private GameObject keybey;
    [SerializeField]
    private GameObject _LeapEventSystem;
    LeapInputModule leapInputModule;
    Typer typerCanvas;
    Text titleText;
    public int taskNumber;
    StraysGameManager gameManager;

    GameObject[] lineObjects;

    // Use this for initialization
    void Start () {
        leapInputModule = _LeapEventSystem.GetComponent<LeapInputModule>();
        leapInputModule.InteractionMode = LeapInputModule.InteractionCapability.Hybrid;
        gameManager = transform.GetComponent<StraysGameManager>();
        typerCanvas = infoCanvas.transform.Find("Dialogue").Find("Text").GetComponent<Typer>();
        titleText = infoCanvas.transform.Find("TitleBar").Find("Text").GetComponent<Text>();
        titleText.text = "Task " + gameManager.getCurrentTaskNumber() + ": KeyboardTask";
        infoCanvas.GetComponent<Canvas>().enabled = true;
        typerCanvas.enabled = true;
        keybey.SetActive(true);
    }



	public void disableTask () {

        leapInputModule.InteractionMode = LeapInputModule.InteractionCapability.Projective;
        keybey.SetActive(false);
            infoCanvas.GetComponent<Canvas>().enabled = false;
            this.enabled = false;        
    }
}
