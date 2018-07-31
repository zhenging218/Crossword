using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;
using UnityEditor;
using Crossword;

public class WordGenTest : EditorWindow {

	int num_words = 0;
	int from = 0;
	WordDatabase db = null;

	[MenuItem("Crossword Generation/Test Crossword Generation")]
	public static void Init()
	{
		WordGenTest wnd = GetWindow<WordGenTest>();
		wnd.minSize = new Vector2(400, 400);
		wnd.Show();
	}

	void OnEnable()
	{
		db = AssetDatabase.LoadAssetAtPath<WordDatabase>("Assets/" + WordDatabase.db_path);
		if(db == null)
		{
			EditorUtility.DisplayDialog("Error", "Database not created! Create one first!", "OK");
			this.Close();
		}
	}

	void OnGUI()
	{
		EditorGUILayout.BeginVertical();
		num_words = EditorGUILayout.IntSlider("Number of words", num_words, 2, 10);
		from = EditorGUILayout.IntSlider("Words starting with", from, 0, 25);
		EditorGUILayout.LabelField(((char)('a' + from)).ToString());
		if (GUILayout.Button("Generate"))
		{
			Board b = null;
			var words = db.GetRandomWordList((char)('a' + from), num_words);
			List<Alphaword> awords = new List<Alphaword>();
			for (int i = 0; i < words.Count; ++i)
			{
				awords.Add(db['a', words[i]]);
			}
			b = LevelGenerator.Generate(awords);
			if(b != null)
			{
				string board = Board.PrintBoard(b);
				string filename = @"Assets/Scripts/Test.txt";
				using (StreamWriter sw = new StreamWriter(filename))
				{
					sw.Write(board.ToCharArray());
				}
				EditorUtility.DisplayDialog("Success", "Board generated with size " + b.Width.ToString() + "x" + b.Height.ToString() + ". You may view results in \"" + filename + "\".", "OK");
				this.Close();
			} else
			{
				EditorUtility.DisplayDialog("Error", "Board generation failed. Please check if there are words in the database to use. Otherwise, board generation may fail simply because of bad fitting (which you may simply try generating again).", "OK");
			}
		}
		
		EditorGUILayout.EndVertical();
	}
}
