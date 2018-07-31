using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Crossword
{
    [System.Serializable]
    public class Alphaword
    {
        [SerializeField]
        public string word;
        [SerializeField]
        public string hint;

        public int Length
        {
            get { return word.Length; }
        }

        public Alphaword(string w = "", string h = "")
        {
            word = w;
            hint = h;
        }
    }
}
