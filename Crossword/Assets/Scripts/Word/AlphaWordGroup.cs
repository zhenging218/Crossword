using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Crossword
{
	[System.Serializable]
	public class AlphaWordGroup
	{
		[SerializeField]
		List<Alphaword> words;

		public void Sort()
		{
			words.Sort((lhs, rhs) => string.Compare(lhs.word, rhs.word));
		}

		public int Count
		{
			get { return words.Count; }
		}

		public Alphaword this[int i]
		{
			get { return words[i]; }
		}

		public void Add(string word, string hint)
		{
			words.Add(new Alphaword(word, hint));
			Sort();
		}

		public void Remove(string word)
		{
			for (int i = 0; i < words.Count; ++i)
			{
				if (words[i].word == word)
				{
					words.RemoveAt(i);
					Sort();
					break;
				}
			}
		}

		public void Remove(int index)
		{
			words.RemoveAt(index);
			Sort();
		}

		public void Clear()
		{
			words.Clear();
		}

		public bool Exists(string word)
		{
			return words.Exists(lhs => lhs.word == word);
		}

		public AlphaWordGroup()
		{
			words = new List<Alphaword>();
		}
	}
}
