using UnityEngine;
using System.Collections.Generic;
using System.Collections;
using UnityEngine.UI;
using System;
using System.IO;
using System.Text;
using System.Timers;

public class QuestionAfterTask : MonoBehaviour {
	[SerializeField] Button forwardbutton_2;
	[SerializeField] GameObject UIElementsPanel_Question_after_Task;
	[SerializeField] GameObject ThanksPanel;
	[SerializeField] GameObject UIElementsPanel_Buttons;
	[SerializeField] GameObject Whole_Questionnaire;
	private double currentTime;
	private bool gogogo;
	public bool run_once = true;
    private string builder;
	[SerializeField]
	StraysGameManager gameManager;
	StraysGameManager stray;
	private string filepath = @"StudyLog.csv";
	// Use this for initialization
	void Start () {
		Whole_Questionnaire.SetActive(false);
		stray = gameManager.GetComponent<StraysGameManager>();
	}

	// Update is called once per frame
	void Update () {
		if (stray.taskscomplete > 0 && stray.taskisrunning  == false) {
			StartCoroutine (delayTime());
		}

		if (stray.taskisrunning == true) {
			Whole_Questionnaire.SetActive(false);
			UIElementsPanel_Question_after_Task.SetActive (false);
			UIElementsPanel_Buttons.SetActive (false);
		}

	}

	void Awake () {
		forwardbutton_2.onClick.AddListener (questionaftertask);
	}

	void questionaftertask() {
		Debug.LogWarning ("klick");

		ToggleGroup question_after = UIElementsPanel_Question_after_Task.GetComponentInChildren<ToggleGroup>() as ToggleGroup;
		if (question_after.AnyTogglesOn ()) {

			IEnumerator<Toggle> toggleEnum = question_after.ActiveToggles().GetEnumerator();
			toggleEnum.MoveNext();
			Toggle toggle = toggleEnum.Current;
			string string_toggle = collectToggles (toggle);
            question_after.SetAllTogglesOff();
			builder = string_toggle + ";"; //Zahl mit angehaengtem Semikolon
			StartCoroutine ("csvWrite");
			ThanksPanel.SetActive (true);
			UIElementsPanel_Buttons.SetActive (false);	
			gogogo = true;
			currentTime = 0;
			StartCoroutine (countPanelTime());
			StopCoroutine (delayTime());
		}
	}

	string collectToggles(Toggle toggle)
	{
		Debug.Log(toggle.name);
		switch(toggle.name)
		{
		case "Toggle_1":
			return "1";
		case "Toggle_2":
			return "2";
		case "Toggle_3":
			return "3";
		case "Toggle_4":
			return "4";
		case "Toggle_5":
			return "5";
		case "Toggle_6":
			return "6";
		case "Toggle_7":
			return "7";
		default:
			return "f";
		}

	}


	void csvWrite() {
		Debug.LogWarning (builder.ToString ()); //hier output auf der konsole
		File.AppendAllText (filepath, builder.ToString());
	}

	public IEnumerator countPanelTime(){
		while(gogogo){
			currentTime += Time.deltaTime;
			if(currentTime > 3){			
				Whole_Questionnaire.SetActive (false);
				ThanksPanel.SetActive(false);
				gogogo = false;
				stray.decisionmaking();
			}
			yield return null;
		}
		yield return null;

	}

	public IEnumerator delayTime(){
		run_once = false;
		Whole_Questionnaire.SetActive(true);
		UIElementsPanel_Question_after_Task.SetActive (true);
		UIElementsPanel_Buttons.SetActive (true);
		if (run_once == false) {
			yield return new WaitForSeconds (10);
		}
	}



}
