using UnityEngine;
using UnityEngine.UI;
using System.Collections;

namespace Leap.Unity.InputModule {
  public class ToggleToggler : MonoBehaviour {
    public Text text;
    public UnityEngine.UI.Image image;
    public Color OnColor;
    public Color OffColor;
	[SerializeField] public string Ontext;
	[SerializeField] public string Offtext;


    public void SetToggle(Toggle toggle) {
      if (toggle.isOn) {
		text.text = Ontext;
        text.color = Color.white;
        image.color = OnColor;
      } else {
        text.text = Offtext;
        text.color = new Color(0.3f, 0.3f, 0.3f);
        image.color = OffColor;
      }
    }
  }
}