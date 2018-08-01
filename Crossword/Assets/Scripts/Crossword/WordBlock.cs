using System.Collections;
using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace Crossword
{
	public class WordBlock
	{
		public enum IntersectionType
		{
			None = -1,
			Once = 0,
			Collinear = 1,
			Different = 2,
			Disjoint = 3,
			Parallel = 4
		}

		Coordinates start, end;
		Alphaword word;

		public int Length
		{
			get { return word.Length; }
		}

		public bool Placed
		{
			get { return !(start.Equals(Coordinates.Zero) && end.Equals(Coordinates.Zero)); }
		}

		public void Reset()
		{
			start = end = Coordinates.Zero;
		}

		public void Place(Coordinates s, Coordinates e)
		{
			start = s;
			end = e;
		}

		public string Word
		{
			get { return word.word; }
		}

		public string Hint
		{
			get { return word.hint; }
		}

		public Coordinates Start
		{
			get { return start; }
		}

		public Coordinates End
		{
			get { return end; }
		}

		public Coordinates this[int i]
		{

			get
			{
				int dx = (start.x - end.x) == 0 ? 0 : 1;
				int dy = dx == 1 ? 0 : 1;
				return new Coordinates(start.x + (i * dx), start.y + (i * dy));
				
			}
		}

		public char this[Coordinates i, bool debug = false]
		{
			get
			{
                if (debug)
                {
                    Debug.Log("start is " + start.x + ", " + start.y + " and end is " + end.x + ", " + end.y + " checking with " + i.x + ", " + i.y);
                }
				if(!(i.x >= start.x && i.x <= end.x && i.y >= start.y && i.y <= end.y))
				{
                    return Cell.Empty;
				}
				int dx = (end.x - start.x == 0) ? 0 : 1;
				int dy = (dx == 1) ? 0 : 1;
				int pos = (i.x - start.x) * dx + (i.y - start.y) * dy;
				return word.word[pos];
			}
		}

        public int IndexOf(Coordinates i)
        {
            if (!(i.x >= start.x && i.x <= end.x && i.y >= start.y && i.y <= end.y))
            {
                return -1;
            }
            int dx = (end.x - start.x == 0) ? 0 : 1;
            int dy = (dx == 1) ? 0 : 1;
            return (i.x - start.x) * dx + (i.y - start.y) * dy;
        }

		public bool IsHorizontal
		{
			get
			{
				return ((start.x - end.x) != 0) && ((start.y - end.y) == 0);
			}
		}

		public bool IsVertical
		{
			get
			{
				return ((start.x - end.x) == 0) && ((start.y - end.y) != 0);
			}
		}

		public WordBlock(Alphaword w)
		{
			start = end = Coordinates.Zero;
			word = w;
		}

		public WordBlock(Alphaword w, Coordinates s, Coordinates e)
		{
			start = s;
			end = e;
			word = w;
		}
	}
}