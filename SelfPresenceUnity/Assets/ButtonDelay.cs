using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public class ButtonDelay : MonoBehaviour {
    [SerializeField]
    private GameObject buttonObject;
    [SerializeField]
    private float secondsToWait = 10;
    Button button;

    void Awake()
    {
        button = buttonObject.GetComponent<Button>();
    }

    void Update()
    {
        if (transform.GetComponent<Canvas>().enabled)
        {
            StartCoroutine(buttonWait(secondsToWait));
        }
    }
	
	private IEnumerator buttonWait(float secondsToWait)
    {
        yield return new WaitForSeconds(secondsToWait);
        button.enabled = true;
    }
}
