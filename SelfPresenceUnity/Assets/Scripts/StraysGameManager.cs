using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.SceneManagement;
using System.IO;
using System;

/*______________________________________________________________________________
GAME MANAGER
------------------------------------------------------------------------------
Skript which handles the correct procedure.
Sets the order for the tasks and shuffles them.
The core of this script is the decisionmaking()-algorithm.
This method is called everytime the user pressed the load/finish task button.
It decides if and which task should be loaded or stopped and logs the corresponding task time.
loads next text after each succesful task.
loads next scene if all tasks are complete

written by : Stray
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

    [Tooltip("Highest priority! Ignores the  manual and random settings.")]
    public bool shuffleTask = true;

    [Tooltip("Ignores the setting of first, second and third task and instead, chooses it randomly.")]
    public bool randomTask = false;

    int taskToStart;

    [Tooltip("Task which will be started first.")]
    public WhichTask firstTask = WhichTask.cubetask;

    [Tooltip("Task which will be started first.")]
    public WhichTask secondTask = WhichTask.drawtask;

    [Tooltip("Task which will be started first.")]
    public WhichTask thirdTask = WhichTask.keyboardtask;

    private WhichTask[] tasks = new WhichTask[3];

    public LinkedList<WhichTask> taskslist = new LinkedList<WhichTask>();
    public LinkedListNode<WhichTask> currenttask;

    // number of tasks completed in current scene.
    public int taskscomplete = 0;
    public bool taskisrunning = false;
    private double tasktime;
    //path in which the logfile will be created
    private string pathforfile = @"StudyLog.csv";

    [SerializeField]
    private bool isfirstscene = false;

    [SerializeField]
    private GameObject infoCanvas;

    private bool infoActive = true;
    // Number of the current participant. Needed for the logs. Increase manualy
    private int parnumber = 0;

    PyramidTask pyramidTask;
    DrawTask drawTask;
    KeyboardTask keyboardTask;
    StraysLoader strayloader;
    OVRScreenFade screenfader;


  public void Start()
  {
        // load the scripts from the current gameobject to enable/disable them.
        pyramidTask = transform.GetComponent<PyramidTask>();
        drawTask = transform.GetComponent<DrawTask>();
        keyboardTask = transform.GetComponent<KeyboardTask>();
        strayloader = transform.GetComponent<StraysLoader>();
        screenfader = GameObject.Find("LMHeadMountedRig").transform.FindChild("CenterEyeAnchor").GetComponent<OVRScreenFade>();

        /*To set the order of the tasks, you have 3 options:
            (1) Set the order manually
            (2) Shuffle the order by randomly swapping tasks (not perfect randomness)
            (3) randomize further
        */
        //When tasks are manually chosen and both randomTask and shuffletask set to false
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

        // print the order in which the tasks will be loaded -> just for visibility
        for (int i =0; i < 3; i++)
            {
              print("task" + i + tasks[i]);
            }
        //load the tasks into a list
        for(int j =0; j < tasks.Length; j++){
          taskslist.AddLast(tasks[j]);
        }

        // write the current participants number into a string
        string firstscenestring =  " participant" + parnumber +";";
        //Append the string into the logfile
        File.AppendAllText(pathforfile, firstscenestring);

        // At the start each scene write which scene is currently active into logfile
        File.AppendAllText(pathforfile, SceneManager.GetActiveScene().name + ";");

        currenttask = taskslist.First;
        print("GameManager started");

        // Start the checkinput coroutine
        StartCoroutine(checkinput());


  }

  public IEnumerator checkinput(){
        /*
        checks every .001 seconds if the f key is pressed.
        if so : manually loads the decisionmaking() method.
        ONLY USE IN THE DRAW TASK,
        since the user cant end the scene by himself.
        TODO : Find an automated procedure for this.
        */
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
    Is called after user presses load/ finish button,
    or f key is pressed ( for draw task only)
    Checks which Task should be the task to run
    Starts that task if no task is already running.
    stops that taks if there is already one.
    */
    WhichTask tasktoload;
    tasktoload = currenttask.Value;

    // if all tasks are already done : load next scene
    if (taskscomplete == 3) {
            // Start changing to the next scene
            screenfader.StartFade();
            changeScene();
    } else{
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
                    }
                    break;
        default:
          // TODO: check if default really equals all tasks complete
          // If so, put the code tho load next scene here.
          print("default case : all tasks complete?");
          break;
      }
    }
  }

  public static void ShuffleArray<T>(T[] arr){
    /*
    Randomly swaps array elements with eachother.
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
        print("time needed for current task: " + tasktime);
        //write the current task and the tasktime into the file via logtasktime() method
        logTaskTime(calledtask);
        string tasktimestring = tasktime.ToString("F3");
        logTaskTime(tasktimestring);

         yield return null;
    }

  public void logTaskTime(string texttoadd){
      /*
        simply writes its parameter into the logfile
        does not need to be the task time.
        e.g. : current participant-nr,taskname, annotations?
      */
    // if there is no logfile yet : creates one!
    if (!File.Exists(pathforfile))
    {
            //Create file to write to:
            string createText = "Selfpresence in Virtual Reality Study" + Environment.NewLine ;
            File.WriteAllText(pathforfile,createText);
    }
    // seperates all entrys by ";" for visibility
    string texttowrite = texttoadd + ";";
    File.AppendAllText(pathforfile,texttowrite);

    }

    public int getCurrentTaskNumber()
    {
        return taskscomplete+1;
    }

    public void changeScene()
    {
        //enables the gameobject that handles the scene changing
        strayloader.enabled = true;
    }
}
