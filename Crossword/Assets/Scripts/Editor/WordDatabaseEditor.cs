using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;
using UnityEditor;
using System.Text;

namespace Crossword
{
    public class WordDatabaseEditor : EditorWindow
    {
		static GUIContent[] editor_contents =
		{
            /* 0 */ new GUIContent("Import Word List", "Opens a text file containing words to supply the database."),
            /* 1 */ new GUIContent("Has Hints", "Toggles whether the text file supplied contains hints under each word."),
            /* 2 */ new GUIContent("New word", "Adds a new word to the list."),
            /* 3 */ new GUIContent("Word:", "Word to add/edit."),
            /* 4 */ new GUIContent("Hint:", "Hint for the word."),
			/* 5 */ new GUIContent("Clear Database", "Clear all words from the database."),
			/* 6 */ new GUIContent("Save", "Save Changes."),
			/* 7 */ new GUIContent("<", "Go to previous page."),
			/* 8 */ new GUIContent(">", "Go to next page."),
			/* 9 */ new GUIContent("Words per page", "Sets how many words to show per page.")
        };

        enum State
        {
            BLANK,
            EDIT,
            ADD
        }

        struct WordIndex
        {
            public char group;
            public int index;

            public WordIndex(char group_ = 'a', int index_ = 0)
            {
                group = group_;
                index = index_;
            }
        }

        State state;
        WordIndex selected_word;
        string newWord, newHint;
        WordDatabase db;
        Vector2 scrollPos;
        List<bool> show_alpha;
		List<int> show_page;
        bool file_has_hint = false;
		int words_per_page;

        [MenuItem("Word Database/Show Word Database")]
        public static void Init()
        {
            WordDatabaseEditor wnd = GetWindow<WordDatabaseEditor>();
            wnd.minSize = new Vector2(500, 500);
            wnd.Show();
        }

        void OnEnable()
        {
            if(db == null)
            {
                LoadDatabase();
				BlankInterface();
                show_alpha = new List<bool>();
                for(int i = 0; i <= ('z' - 'a'); ++i)
                {
                    show_alpha.Add(false);
                }
				show_page = new List<int>();
				for(int i = 0; i <= ('z' - 'a'); ++i)
				{
					show_page.Add(0);
				}
				words_per_page = 10;
				
				file_has_hint = false;
            }
        }

        void OnGUI()
        {
            EditorGUILayout.BeginVertical(GUILayout.ExpandWidth(true));
            EditorGUILayout.BeginHorizontal(GUILayout.ExpandWidth(true));
            // show word edit and word list.
            DisplayWordList();
            DisplayWordEditArea();
            EditorGUILayout.EndHorizontal();
            EditorGUILayout.EndVertical();
        }

		void OnDestroy()
		{
			EditorUtility.SetDirty(db);
			AssetDatabase.SaveAssets();
		}

		void LoadDatabase()
        {
            db = AssetDatabase.LoadAssetAtPath<WordDatabase>("Assets/" + WordDatabase.db_path);
            if(db == null)
            {
                CreateDatabase();
            }
        }

        void CreateDatabase()
        {
            db = CreateInstance<WordDatabase>();
            AssetDatabase.CreateAsset(db, "Assets/" + WordDatabase.db_path);
            AssetDatabase.SaveAssets();
            AssetDatabase.Refresh();
        }

		void Save()
		{
			AssetDatabase.SaveAssets();
		}

		void BlankInterface()
		{
			newWord = newHint = string.Empty;
			state = State.BLANK;
		}

        void DisplayWordList()
        {
			var center_word = new GUIStyle(GUI.skin.label) { alignment = TextAnchor.MiddleCenter };
			EditorGUILayout.BeginVertical(GUILayout.Width(250));
            EditorGUILayout.Space();
			if(GUILayout.Button(editor_contents[5], GUILayout.ExpandWidth(true))) {
				if(EditorUtility.DisplayDialog("Confirm destructive action", "Are you sure you want to clear all words from the database? It cannot be undone.", "Yes", "No"))
				{
					db.ClearAllWords();
					BlankInterface();
                    EditorUtility.SetDirty(this);
					return;
				}
			}
			scrollPos = EditorGUILayout.BeginScrollView(scrollPos, "box", GUILayout.ExpandHeight(true));
			for (char c = 'a'; c <= 'z'; ++c)
            {
                // Alphabet area (of current alphabet c)
                var curr_list = db[c];
                if (curr_list.Count > 0)
                {
                    show_alpha[c - 'a'] = EditorGUILayout.Foldout(show_alpha[c - 'a'], char.ToUpper(c).ToString() + " (" + curr_list.Count.ToString() + " word" + ((curr_list.Count == 1) ? "" : "s") + ")");
                    if (show_alpha[c - 'a'])
                    {
                        // word list area (of words starting with current alphabet c)
						EditorGUILayout.BeginHorizontal();
						if(show_page[c - 'a'] > 0)
						{
							// enable prev page button
							if(GUILayout.Button(editor_contents[7], GUILayout.Width(25)))
							{
								show_page[c - 'a'] = show_page[c - 'a'] - 1;
							}
						} else
						{
                            GUI.enabled = false;
                            GUILayout.Button(editor_contents[7], "button", GUILayout.Width(25));
                            GUI.enabled = true;
                        }
						EditorGUILayout.LabelField("Page " + (show_page[c - 'a'] + 1).ToString(), center_word, GUILayout.ExpandWidth(true));
						if((show_page[c - 'a'] + 1) * words_per_page < curr_list.Count)
						{
							// enable next page button
							if (GUILayout.Button(editor_contents[8], GUILayout.Width(25)))
							{
								show_page[c - 'a'] = show_page[c - 'a'] + 1;
							}
						} else
						{
                            GUI.enabled = false;
                            GUILayout.Button(editor_contents[8], "button", GUILayout.Width(25));
                            GUI.enabled = true;
                        }
						EditorGUILayout.EndHorizontal();
						// show only some words and let user flip through pages to prevent lag.
						for (int i = show_page[c - 'a'] * words_per_page; i < ((show_page[c - 'a'] + 1) * words_per_page) && i < curr_list.Count; ++i)
                        {
                            EditorGUILayout.BeginHorizontal();
                            if (GUILayout.Button(curr_list[i].word, GUILayout.ExpandWidth(true)))
                            {
                                selected_word = new WordIndex(c, i);
                                newWord = db[selected_word.group, selected_word.index].word;
                                newHint = db[selected_word.group, selected_word.index].hint;
                                state = State.EDIT;
                            }
                            if (GUILayout.Button("-", GUILayout.Width(25)))
                            {
								db.RemoveWord(c, i);
                                EditorUtility.SetDirty(db);
								BlankInterface();
                                return;
                            }
                            EditorGUILayout.EndHorizontal();
                        }
                        // end word list area
                    }
                } else
                {
                    show_alpha[c - 'a'] = false;
                    // EditorGUILayout.LabelField(char.ToUpper(c).ToString());
                }
                // end alphabet area
            }
            EditorGUILayout.EndScrollView();
            EditorGUILayout.BeginHorizontal(GUILayout.ExpandWidth(true));
            EditorGUILayout.LabelField("Total words: " + db.Size.ToString());
            EditorGUILayout.Space();
            if(GUILayout.Button(editor_contents[2]))
            {
                state = State.ADD;
            }
			
            EditorGUILayout.EndHorizontal();
            EditorGUILayout.Space();
            words_per_page = EditorGUILayout.IntSlider(editor_contents[9], words_per_page, 1, 20, GUILayout.ExpandWidth(true));
            EditorGUILayout.Space();
            DisplayFileLoader();
			EditorGUILayout.EndVertical();
        }

        void DisplayWordEditArea()
        {
            EditorGUILayout.BeginVertical(GUILayout.Width(250), GUILayout.ExpandWidth(true));
            EditorGUILayout.Space();

            switch(state)
            {
                case State.ADD:
                    EditorGUILayout.BeginHorizontal(GUILayout.Width(250));
                    newWord = EditorGUILayout.TextField(editor_contents[3], newWord);
                    EditorGUILayout.LabelField(newWord.Length.ToString());
                    EditorGUILayout.EndHorizontal();
                    EditorGUILayout.LabelField(editor_contents[4]);
                    newHint = EditorGUILayout.TextArea(newHint);
                    EditorGUILayout.Space();
                    if (GUILayout.Button("Done", GUILayout.Width(100))) {
                        bool ok = db.AddWord(newWord, newHint);
                        if(!ok)
                        {
                            EditorUtility.DisplayDialog("Error!", "The word " + newWord + " already exists!", "OK");
                        }
                        else
                        {
                            EditorUtility.SetDirty(db);
                            newWord = newHint = string.Empty;
                        }
                    }
                    break;
                case State.EDIT:
                    EditorGUILayout.BeginHorizontal(GUILayout.Width(250));
                    newWord = EditorGUILayout.TextField(editor_contents[3], newWord);
                    EditorGUILayout.LabelField(newWord.Length.ToString());
                    EditorGUILayout.EndHorizontal();
                    EditorGUILayout.LabelField(editor_contents[4]);
                    newHint = EditorGUILayout.TextArea(newHint);
                    EditorGUILayout.Space();
                    if (GUILayout.Button("Done", GUILayout.Width(100)))
                    {
                        string old_word = db[selected_word.group, selected_word.index].word;
                        if (newWord != old_word)
                        {
                            if (db[selected_word.group].Exists(newWord))
                            {
                                EditorUtility.DisplayDialog("Error!", "The word " + newWord + " already exists!", "OK");
                                newWord = old_word;
                            }
                            else
                            {
                                db[selected_word.group, selected_word.index].word = newWord;
                                db[selected_word.group, selected_word.index].hint = newHint;
                                EditorUtility.SetDirty(db);
                            }
                        } else
                        {
                            db[selected_word.group, selected_word.index].hint = newHint;
                            EditorUtility.SetDirty(db);
                        }
                    }
                    break;
                default:
                    break;
            }

            EditorGUILayout.Space();
            EditorGUILayout.EndVertical();
        }

        void DisplayFileLoader()
        {
            EditorGUILayout.BeginHorizontal(GUILayout.ExpandWidth(true));
            if(GUILayout.Button(editor_contents[0]))
            {
                string filename = EditorUtility.OpenFilePanel("Select Word List", "", "txt");
				EditorUtility.SetDirty(this);
				if (filename.Length > 0)
                {
					FileInfo src = new FileInfo(filename);
					StreamReader ifs = src.OpenText();
					long total_bytes = src.Length;
					long curr_bytes = 0;
					int old_size = db.Size;
					while (!ifs.EndOfStream)
					{
						string curr = ifs.ReadLine();
						if(curr.Length < 2)
						{
							continue;
						}
						string hint = string.Empty;
						if (file_has_hint)
						{
							hint = ifs.ReadLine();
						}
						// do not accept non alphabetical items
						db.AddWord(curr, hint);
						var ascii_bytes = Encoding.ASCII.GetByteCount(curr);
						var unicode_bytes = Encoding.Unicode.GetByteCount(curr);
						curr_bytes += (ascii_bytes == unicode_bytes) ? ascii_bytes : unicode_bytes;
						if(EditorUtility.DisplayCancelableProgressBar("Importing word list", "Please wait... Importing word list... (" + curr + ")", (float)curr_bytes / total_bytes))
						{
							EditorUtility.DisplayDialog("Message", "Import progress interrupted.", "OK");
							break;
						}
					}
					EditorUtility.ClearProgressBar();
					ifs.Close();
					ifs = null;
				}
            }
            file_has_hint = EditorGUILayout.Toggle(editor_contents[1], file_has_hint);
            EditorGUILayout.EndHorizontal();
			EditorGUILayout.Space();
        }
    }
}
