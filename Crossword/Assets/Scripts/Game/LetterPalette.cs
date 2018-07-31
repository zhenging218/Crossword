using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class LetterPalette : MonoBehaviour {
    // Use this for initialization

    WordCell curr = null;
    static LetterPalette instance = null;
    public static LetterPalette Instance
    {
        get { return instance; }
    }
    void Awake()
    {
        if(instance && instance != this)
        {
            Destroy(this.gameObject);
        } else
        {
            instance = this;
        }
    }
    void OnDestroy()
    {
        if(instance == this)
        {
            instance = null;
        }
    }

    void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
		
	}

    public void ReceiveInput(WordCell cell)
    {
        curr = cell;
    }

    public void ClearInput()
    {
        curr = null;
    }
}
