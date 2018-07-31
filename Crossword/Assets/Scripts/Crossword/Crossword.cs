using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace Crossword
{
	// manager class. singleton.
	public class Crossword : MonoBehaviour
	{
		private static Crossword instance = null;
        
		public static Crossword Instance
		{
			get { return instance; }
		}

		void Awake()
		{
			if (instance != null && instance != this)
			{
				Destroy(this);
			}
			else
			{
				instance = this;
			}
		}

		void OnDestroy()
		{
			if (instance == this)
			{
				instance = null;
			}
		}

		List<WordBlock> words;
		public GameObject WordBlockPrefab;
		// keep the word list instance here.
		// load word list on instance = this.
	}
}