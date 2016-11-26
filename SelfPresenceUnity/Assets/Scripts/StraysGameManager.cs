using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.SceneManagement;
using System.IO;
using System;

/*______________________________________________________________________________
GAME MANAGER
------------------------------------------------------------------------------
calls a coroutine of "CheckForKeyInput",
which checks if f Key is pressed every 0.5 seconds.

Sets the order of the tasks.
Put the tasks in the correct order into the SerializeFields.
Only use following strings:
keyboardtask
boxestask
drawtask

Code done by stray
______________________________________________________________________________
*/

public class StraysGameManager : MonoBehaviour{
    public enum WhichTask : int
    {
        keyboardtask,
        drawtask,
        cubetask
    }
    // Alternativ Shuffle
    [Header("Task Order (descending priority!)")]
    [Tooltip("Highest priority! Ignores the  manuelly and random settings.")]
    public bool shuffleTask = true;
    [Tooltip("Ignores the setting of first, second and third task and instead, choose it randomly.")]
    public bool randomTask = false;
    int taskToStart;
    [Tooltip("Task which will be start first.")]
    public WhichTask firstTask = WhichTask.cubetask;
    [Tooltip("Task which will be start first.")]
    public WhichTask secondTask = WhichTask.drawtask;
    [Tooltip("Task which will be start first.")]
    public WhichTask thirdTask = WhichTask.keyboardtask;
    private WhichTask[] tasks = new WhichTask[3];
    public LinkedList<WhichTask> taskslist = new LinkedList<WhichTask>();
    public LinkedListNode<WhichTask> currenttask;

    public int taskscomplete = 0;
    public bool taskisrunning = false;
    private double tasktime;
    private string pathforfile = @"StudyLog.csv";
    [SerializeField]
    private bool isfirstscene = false;
    [SerializeField]
    private GameObject infoCanvas;
    private bool infoActive = true;
    //private string participantname = "Stanley";
    private int parnumber = 18;
    PyramidTask pyramidTask;
    DrawTask drawTask;
    KeyboardTask keyboardTask;
    StraysLoader strayloader;
    OVRScreenFade screenfader;


  public void Start()
  {
        pyramidTask = transform.GetComponent<PyramidTask>();
        drawTask = transform.GetComponent<DrawTask>();
        keyboardTask = transform.GetComponent<KeyboardTask>();
        strayloader = transform.GetComponent<StraysLoader>();
        screenfader = GameObject.Find("LMHeadMountedRig").transform.FindChild("CenterEyeAnchor").GetComponent<OVRScreenFade>();

        // Nur bei initialisierung gebraucht: PlayerPrefs.SetInt("First Task", 0);

        // Funktioniert... Debug.LogWarning("Playerpref ist " + PlayerPrefs.GetInt("First Task"));

        // When tasks are manually chosen and both randomTask and shuffletask set to false
        tasks[0] = firstTask;
        tasks[1] = secondTask;
        tasks[2] = thirdTask;

        // When randomTask is set to true and shuffleTask is false in Inspector
        if (!shuffleTask && randomTask)
        {
            ShuffleArray(tasks);
        }
        // When shuffleTask is set to true in Inspector (highest priority)
        if (shuffleTask)
        {
            taskToStart = PlayerPrefs.GetInt("First Task");
            tasks[taskToStart] = firstTask;
            tasks[(taskToStart + 1) % 3] = secondTask;
            tasks[(taskToStart + 2) % 3] = thirdTask;
            PlayerPrefs.SetInt("First Task", (taskToStart + 1) % 3);
        }
        for (int i =0; i < 3; i++)
            {
              print("task" + i + tasks[i]);
            }
        for(int j =0; j < tasks.Length; j++){
          taskslist.AddLast(tasks[j]);
        }
        // if this is a new participant : create a new line with the participant nummer

        string firstscenestring =  " participant" + parnumber +";";
        File.AppendAllText(pathforfile, firstscenestring);

        // for each scene write which scene is currently active
        File.AppendAllText(pathforfile, SceneManager.GetActiveScene().name + ";");
        currenttask = taskslist.First;
        print("GameManager started");
        // commented out for Button implementation.
        StartCoroutine(checkinput());
        //

  }

  public IEnumerator checkinput(){
        /*
        checks every .001 seconds if the f key is pressed.
        */
    print("checkinput is called");
    while(true){
      if(Input.GetKeyDown(KeyCode.F)){
        if(infoActive == true)
                {
                    infoActive = false;
                    infoCanvas.SetActive(false);

                }
        print("Key press recognized");

                    decisionmaking();

      }
      yield return new WaitForSeconds(.001f);
    }
  }

  public void decisionmaking(){
    /*
    Is called after an input is recognized.
    Checks which Task should be the task to run
    Starts that task if no task is already running.
    stops that taks if there is already one.
    */
    WhichTask tasktoload;
    tasktoload = currenttask.Value;

    // if all tasks are already done : load next scene
    if (taskscomplete == 3) {
        print("all tasks complete : loading next task");
            // Start changing to the next scene
            screenfader.StartFade(); 
            changeScene();

    }
    else{
      //check which task should be started or stopped
      switch (tasktoload){
        case WhichTask.keyboardtask:
                    if (taskisrunning == false)
                    {
                        print("keyboard task started");
                        // resets timer and starts it via taskisrunning
                        tasktime = 0.0;
                        taskisrunning = !taskisrunning;
                        // log the current task and start counting task time
                        StartCoroutine(countTaskTime("keyboardTask"));
                        //enable the keyboard task here
                        keyboardTask.enabled = true;
                    }
                    else
                    {
                        //Disable the current task
                        keyboardTask.disableTask();
                        taskisrunning = false;
                        taskscomplete = taskscomplete + 1;
                        print("keyboard task stopped");
                        print("taskscomplete= " + taskscomplete);
                        if (currenttask.Next != null)
                        {
                            currenttask = currenttask.Next;
                        }
                        else
                        {
                            print("current task has no next");
                        }

                    }
                    break;
        case WhichTask.drawtask:
                    if (taskisrunning == false)
                    {
                        print("draw task started");
                        // resets timer and starts it via taskisrunning
                        tasktime = 0.0;
                        taskisrunning = !taskisrunning;
                        // log the current task and start counting task time
                        StartCoroutine(countTaskTime("drawTask"));
                        //enable the keyboard task here
                        drawTask.taskNumber = taskscomplete;
                        drawTask.enabled = true;
                    }
                    else
                    {
                        //Disable the current task
                        drawTask.disableTask();
                        taskisrunning = false;
                        taskscomplete = taskscomplete + 1;
                        print("draw task stopped");
                        print("taskscomplete= " + taskscomplete);
                        if (currenttask.Next != null)
                        {
                            currenttask = currenttask.Next;
                        }
                        else
                        {
                            print("current task has no next");
                        }

                    }
                    break;
        case WhichTask.cubetask:
                    if (taskisrunning == false)
                    {
                        print("boxes task started");
                        // resets timer and starts it via taskisrunning
                        tasktime = 0.0;
                        taskisrunning = !taskisrunning;
                        // log the current task and start counting task time
                        StartCoroutine(countTaskTime("CubeTask"));
                        //enable the keyboard task here
                        pyramidTask.taskNumber = taskscomplete;
                        pyramidTask.enabled = true;
                    }
                    else
                    {
                        //Disable the current task
                        pyramidTask.disableTask();
                        taskisrunning = false;
                        taskscomplete = taskscomplete + 1;
                        print("boxes task stopped");

                        print("taskscomplete= " + taskscomplete);
                        if (currenttask.Next != null)
                        {
                            currenttask = currenttask.Next;
                        }
                        else
                        {
                            print("current task has no next");
                        }

                    }
                    break;
        default:
          print("default case : all tasks complete?");
          break;
      }
    }
  }

  public static void ShuffleArray<T>(T[] arr){
    /*
    Randomly swaps elements array with eachother.
    */
   for (int i = arr.Length - 1; i >= 0; i--) {
     int r = UnityEngine.Random.Range(0, i);
     T tmp = arr[i];
     arr[i] = arr[r];
     arr[r] = tmp;
   }
 }

  public IEnumerator countTaskTime(string calledtask){
        /*
        once a task is started, counts the time it takes to complete the task.
        If task is stopped, writes time with current task into file
        */
        while (taskisrunning)
        {
            tasktime += Time.deltaTime;
            yield return null;

        }
        print("timeforcurrenttaskis: " + tasktime);
        /*
        commmented out because it logged the wrong task name
        switch(currenttask.Value)
        {
            case WhichTask.cubetask:
                logTaskTime("boxestask");
                break;
            case WhichTask.keyboardtask:
                logTaskTime("keyboardtask");
                break;
            case WhichTask.drawtask:
                logTaskTime("drawtask");
                break;
        }
        */
        logTaskTime(calledtask);
        string tasktimestring = tasktime.ToString("F3");
        logTaskTime(tasktimestring);
         
         yield return null;
        
    }

  public void logTaskTime(string texttoadd){
      if (!File.Exists(pathforfile))
    {
            //Create file to write to:
            string createText = "Selfpresence in Virtual Reality Study" + Environment.NewLine ;
            File.WriteAllText(pathforfile,createText);
    }
   
    string texttowrite = texttoadd + ";";
    File.AppendAllText(pathforfile,texttowrite);

    }
    public int getCurrentTaskNumber()
    {
        return taskscomplete+1;
    }

    public void changeScene()
    {
        strayloader.enabled = true;
    }
}

