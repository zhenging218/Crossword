using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Crossword;

public class WordCell : MonoBehaviour {

    Coordinates coords = null;
    Text text = null;
	// Use this for initialization
	void Start () {
        text = GetComponentInChildren<Text>();
	}

    public void OnPress()
    {
        // get the palette
        LetterPalette.Instance.ReceiveInput(this);
    }

    public void GetInput(char c)
    {
        // get ask game manager if answer is correct.
    }

    public void SetCell(Coordinates c)
    {
        coords = c;
    }
}
