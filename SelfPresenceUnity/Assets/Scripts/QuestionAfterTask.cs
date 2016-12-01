using UnityEngine;
using System.Collections.Generic;
using System.Collections;
using UnityEngine.UI;
using System;
using System.IO;
using System.Text;
using System.Timers;

public class QuestionAfterTask : MonoBehaviour {
	/*
		Handles the script, which after each task asks the user how they liked it.
		enables the questionaire_panel after a task is completed,
		disables it once the user confirmed his answer & logs the answer

		This Script is propably not the most elegant way of solvig this!!(some might call it terrible) it has many flaws.

		TODO: find a better and more elegant solution. i would HIGHLY recommend implementing your own version : this one is poorly made...very very poory
		Probably by interacting directly with the game manager.

		another note: Serialize fields are also not the best solution.
		They are a quick and easy solution to finding game objects but cause many problems.
		(every serializefield has to be handassigned if the script is applied to a new gameobject)
		TODO : find a better solution for this as well. hint: find by tag ?

		code by : patrick f.
	*/
	[SerializeField] Button forwardbutton_2;
	[SerializeField] GameObject UIElementsPanel_Question_after_Task;
	[SerializeField] GameObject ThanksPanel;
	[SerializeField] GameObject UIElementsPanel_Buttons;
	[SerializeField] GameObject Whole_Questionnaire;
	private double currentTime;
	private bool gogogo;
	public bool run_once = true;
    private string builder;

	// The gameobject called StraysGameManager
	[SerializeField] StraysGameManager gameManager;
	//the actual script, which is a component of the gameobject
	StraysGameManager stray;
	//set the filepath here! make sure the name matches the one in StraysGameManager!!!
	private string filepath = @"StudyLog.csv";
	


	void Start () {
		Whole_Questionnaire.SetActive(false);
		stray = gameManager.GetComponent<StraysGameManager>();
	}

	// Update is called once per frame
	void Update () {
		// if a task has been completed run "delaytime" which makes sure that the panel does not keep popping up all the time
		if (stray.taskscomplete > 0 && stray.taskisrunning  == false) {
			StartCoroutine (delayTime());
		}
		// if a task has been started : disable all Questionaire Elements
		if (stray.taskisrunning == true) {
			Whole_Questionnaire.SetActive(false);
			UIElementsPanel_Question_after_Task.SetActive (false);
			UIElementsPanel_Buttons.SetActive (false);
		}
	}

	void Awake () {
		// adds a on-click-listener to the confirm-button
		forwardbutton_2.onClick.AddListener (questionaftertask);
	}

	void questionaftertask() {
		/*
			triggers if the user presses the confirm-button.
			creates a ToggleGroup for the buttons.
			ToggleGroups ensure that at any time only one of the buttons can be toggled.
		*/
		ToggleGroup question_after = UIElementsPanel_Question_after_Task.GetComponentInChildren<ToggleGroup>() as ToggleGroup;
		if (question_after.AnyTogglesOn ()) {

			IEnumerator<Toggle> toggleEnum = question_after.ActiveToggles().GetEnumerator();
			toggleEnum.MoveNext();
			Toggle toggle = toggleEnum.Current;
			string string_toggle = collectToggles (toggle); // "collect" toggles -> read which answer the user has given
            question_after.SetAllTogglesOff();				
			builder = string_toggle + ";"; //Zahl mit angehaengtem Semikolon
			//write the answer into the file
			StartCoroutine ("csvWrite");
			ThanksPanel.SetActive (true);
			UIElementsPanel_Buttons.SetActive (false);	
			gogogo = true;	
			currentTime = 0;		// reset the timer
			StartCoroutine (countPanelTime());
			StopCoroutine (delayTime());
		}
	}

	string collectToggles(Toggle toggle)
	{
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
		/*
			This is makes sure that the panel only is visible once, by 
			introducing a delay until all elements are set to false
		*/
		while(gogogo){
			currentTime += Time.deltaTime;   // count the time
			if(currentTime > 3){						//after 3 seconds set the questionaire to false
				Whole_Questionnaire.SetActive (false);
				ThanksPanel.SetActive(false);
				gogogo = false;
				stray.decisionmaking();
			}
			yield return null;				// probably redundant
		}
		yield return null;
	}

	public IEnumerator delayTime(){
		// enable all elements and then sleep for 10 seconds
		run_once = false;
		Whole_Questionnaire.SetActive(true);
		UIElementsPanel_Question_after_Task.SetActive (true);
		UIElementsPanel_Buttons.SetActive (true);
		if (run_once == false) {
			yield return new WaitForSeconds (10);
		}
	}
}
