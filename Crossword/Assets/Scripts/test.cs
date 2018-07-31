using System.Collections;
using System.IO;
using System.Collections.Generic;
using UnityEngine;
using Crossword;

public class test : MonoBehaviour {

	WordDatabase db;

	public int num_words = 2;

	// Use this for initialization
	void Start () {
        db = WordDatabase.Load();
		if (db != null)
		{
			var words = db.GetRandomWordList('a', num_words);
			for (int i = 0; i < words.Count; ++i)
			{
				Debug.Log(words[i] + " " + db['a', words[i]].word);
			}
			List<Alphaword> awords = new List<Alphaword>();
			for (int i = 0; i < words.Count; ++i)
			{
				awords.Add(db['a', words[i]]);
			}

			// gen board
			Board board = LevelGenerator.Generate(awords);
			if (board != null)
			{
				Debug.Log("board dim: " + board.Width.ToString() + ", " + board.Height.ToString());
				string b = Board.PrintBoard(board);
				string filename = @"Assets/Scripts/Test.txt";
				using (StreamWriter sw = new StreamWriter(filename))
				{
					sw.Write(b.ToCharArray());
				}
			} else
			{
				Debug.Log("board gen failed");
			}
		}
	}
	
	
}
