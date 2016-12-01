using UnityEngine;
using System.Collections;
 using UnityEngine.UI; // <-- you need this to access UI (button in this case) functionalities

public class StraysButtonSkript : MonoBehaviour {
    Button myButton;
    KeyboardInputHandler strayshandler;

    void Awake()
    {
        myButton = GetComponent<Button>(); // <-- you get access to the button component here
        //find the keyboard input handler
        strayshandler = GameObject.FindGameObjectWithTag("StraysTag").GetComponent<KeyboardInputHandler>();
        string mytext = myButton.GetComponentInChildren<Text>().text;
        myButton.onClick.AddListener(() => { myFunctionForOnClickEvent(mytext); });  // <-- you assign a method to the button OnClick event here
    }

    void myFunctionForOnClickEvent(string argument1)
    {
        print(argument1);
        strayshandler.WriteInputOnText(argument1);
    }


}
