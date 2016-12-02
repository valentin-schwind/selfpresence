using UnityEngine;
using System.Collections;
using UnityEngine.UI;

[RequireComponent(typeof(AudioSource))]
[RequireComponent(typeof(Text))]

public class Typer : MonoBehaviour {
/*
a sample typer script to write text to a canvas one letter at a time.
not needed.
but can be useful
*/
    [SerializeField] 
    public string msg = "Replace";
    private Text textComp;
    public float startDelay = 2f;
    public float typeDelay = 0.01f;
    public AudioClip putt;

	void Start () {
        StartCoroutine("TypeIn");
	}
	
    void Awake ()
    {
        textComp = GetComponent<Text>();
    }
    
    public IEnumerator TypeIn()
    {
        yield return new WaitForSeconds(startDelay);

        for(int i = 0; i < msg.Length; i++)
        {
            textComp.text = msg.Substring(0, i);
            GetComponent<AudioSource>().PlayOneShot(putt);
            yield return new WaitForSeconds(typeDelay);
        }
    }

    public IEnumerator TypeOff()
    {
        for( int i = msg.Length; i >= 0; i--)
        {
            textComp.text = msg.Substring(0, i);
            yield return new WaitForSeconds(typeDelay);
        }
    }
}
