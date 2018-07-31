using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO;
using System.Linq;

namespace Crossword
{
    public class WordDatabase : ScriptableObject
    {
        [SerializeField]
        List<AlphaWordGroup> words;
		[SerializeField]
		int count;
		
		public static string db_path
		{
			get
			{
				return @"Resources/Database/wordDB.asset";
			}
		}

        public static WordDatabase Load()
        {
            return Resources.Load<WordDatabase>(@"Database/wordDB");
        }

        public static bool is_alpha(string word)
        {
            for (int i = 0; i < word.Length; ++i)
            {
                char c = word[i];
                bool is_lower = (c >= 'a' && c <= 'z');
                bool is_upper = (c >= 'A' && c <= 'Z');
                if (!(is_lower || is_upper)) return false;
            }
            return true;
        }

        public static bool is_alpha(char c)
        {
            bool is_lower = (c >= 'a' && c <= 'z');
            bool is_upper = (c >= 'A' && c <= 'Z');
            return is_lower || is_upper;
        }

        void OnEnable()
        {
            if(words == null)
            {
                words = new List<AlphaWordGroup>();
                for (char c = 'a'; c <= 'z'; ++c) {
                    words.Add(new AlphaWordGroup());
                }
				count = 0;
            }
        }

        public bool AddWord(string word, string hint)
        {
            string low_word = word.ToLower();
			if(!is_alpha(low_word))
			{
				return false;
			}
            char first = low_word[0];
            if (words[first - 'a'].Exists(word))
            {
                return false;
            }
            words[first - 'a'].Add(word, hint);
			++count;
            return true;
        }

		public void RemoveWord(char c, int i)
		{
			bool is_lower = (c >= 'a' && c <= 'z');
			bool is_upper = (c >= 'A' && c <= 'Z');
			bool is_alpha = is_lower || is_upper;
			if (!is_alpha)
			{
				throw (new System.Exception("cannot take non alphabetic word!"));
			}
			if (is_upper)
			{
				c = char.ToLower(c);
			}
			words[c - 'a'].Remove(i);
			--count;
		}

        public Alphaword this[char c, int i]
        {
            get {
                bool is_lower = (c >= 'a' && c <= 'z');
                bool is_upper = (c >= 'A' && c <= 'Z');
                bool is_alpha = is_lower || is_upper;
                if(!is_alpha)
                {
                    throw (new System.Exception("cannot take non alphabetic word!"));
                }
                if(is_upper)
                {
                    c = char.ToLower(c);
                }
                return words[c - 'a'][i];
            }
        }

        public AlphaWordGroup this[char c]
        {
            get
            {
                bool is_lower = (c >= 'a' && c <= 'z');
                bool is_upper = (c >= 'A' && c <= 'Z');
                bool is_alpha = is_lower || is_upper;
                if (!is_alpha)
                {
                    throw (new System.Exception("cannot take non alphabetic word!"));
                }
                if (is_upper)
                {
                    c = char.ToLower(c);
                }
                return words[c - 'a'];
            }
        }

		public int Size
		{
			get
			{
				return count;
			}
		}

        public List<int> GetRandomWordList(char start_with, int size)
        {
            if(!is_alpha(start_with))
            {
                throw (new System.Exception("cannot take non alphabetic word!"));
            }
            start_with = char.ToLower(start_with);

            var list = words[start_with - 'a'];
			size = (size > list.Count) ? list.Count : size;
			HashSet<int> indices = new HashSet<int>();
			while(indices.Count < size)
			{
				indices.Add(Random.Range(0, list.Count));
			}
			return indices.ToList();
        }

		public void ClearAllWords()
		{
			for(int i = 0; i < words.Count; ++i)
			{
				words[i].Clear();
			}
			count = 0;
		}

        public void ReadFromFile(string filename, bool has_hint = false)
        {
            FileInfo src = new FileInfo(filename);
            StreamReader ifs = src.OpenText();
            while(!ifs.EndOfStream)
            {
                string curr = ifs.ReadLine();
				if(curr.Length < 2)
				{
					continue;
				}
                string hint = string.Empty;
                if(has_hint)
                {
                    hint = ifs.ReadLine();
                }
                // do not accept non alphabetical items
                if(!is_alpha(curr))
                {
                    continue;
                }
				AddWord(curr, hint);
            }
            ifs.Close();
            ifs = null;
        }
    }
}
