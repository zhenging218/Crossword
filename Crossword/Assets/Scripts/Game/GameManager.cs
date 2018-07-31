using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Crossword;

public class GameManager : MonoBehaviour {

    static GameManager instance = null;
    public static GameManager Instance
    {
        get { return instance; }
    }

    Board board;

    public int width, height;


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

    public void CheckAnswer(Coordinates coords, char c)
    {

    }

    // Use this for initialization
    void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
		
	}
}
