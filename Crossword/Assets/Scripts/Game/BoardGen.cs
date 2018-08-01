using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Crossword;

public class BoardGen : MonoBehaviour {

	static BoardGen instance = null;
	Board board;
	public static BoardGen Instance
	{
		get { return instance; }
	}

	[Range(2,10)]
	public int TotalWords = 2;

	void Awake()
	{
		if(instance && instance != this)
		{
			Destroy(this.gameObject);
		} else
		{
			instance = this;
			board = null;
			DontDestroyOnLoad(this);
		}
	}

	void OnDestroy()
	{
		if(instance == this)
		{
			instance = null;
		}
	}

	public bool OK
	{
		get { return !(board == null); }
	}

	public Board Board
	{
		get
		{
			// once we get the board this object is useless, so delete.
			Destroy(this.gameObject);
			return board;
		}
	}

	public bool GenBoard()
	{
		WordDatabase db = WordDatabase.Load();
		if(db != null)
		{
			char start_with = (char)Random.Range('a', 'z' + 1);
			var indices = db.GetRandomWordList(start_with, TotalWords);
			if (indices == null)
			{
				Debug.LogError("NO WORDS STARTING WITH " + start_with + " IN DATABASE!");
				return true;
			}
			List<Alphaword> awords = new List<Alphaword>();
			for (int i = 0; i < indices.Count; ++i)
			{
				awords.Add(db[start_with, indices[i]]);
			}
			board = LevelGenerator.Generate(awords);
			return true;
		}
		
		return true;
	}

	// do genboard as coroutine so it wont stop loading screen from playing nice animation.
	IEnumerator DoGenBoard()
	{
		// wait for board gen to complete.
		yield return new WaitUntil(GenBoard);
	}

	public void DoDoGenBoard()
	{
		StartCoroutine(DoGenBoard());
	}

	// Use this for initialization
	void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
		
	}
}
