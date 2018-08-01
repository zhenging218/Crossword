using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace Crossword
{
    public class Answer
    {
        WordBlock word;
        List<bool> correct;

        public Answer(WordBlock w)
        {
            word = w;
            correct = Enumerable.Repeat(false, word.Length).ToList();
        }

        public WordBlock Word
        {
            get { return word; }
        }

        public bool Solved
        {
            get
            {
                int total = 0;
                for(int i = 0; i < correct.Count; ++i)
                {
                    if(correct[i])
                    {
                        ++total;
                    }
                }
                return total == correct.Count;
            }
        }

        public bool Set(Coordinates coords, char c)
        {
            char mine = word[coords];
            bool ret = false;
            if(mine == Cell.Empty)
            {
                ret = false;
            }
            else if(mine == c)
            {
                ret = correct[word.IndexOf(coords)] = true;
            }
            else
            {
                ret = correct[word.IndexOf(coords)] = false;
            }
            return ret;
        }

        public void Reset()
        {
            for(int i = 0; i < correct.Count; ++i)
            {
                correct[i] = false;
            }
        }
    }

    public class Cell
    {
        List<int> solutionTo = null, firstOf = null;

        public bool HasWord
        {
            get { return !(solutionTo.Count == 0); }
        }

        public Cell()
        {
            solutionTo = new List<int>();
            firstOf = new List<int>();
        }

        public void AddSolution(int index, int is_first_of = -1)
        {
            solutionTo.Add(index);
            if(is_first_of >= 0)
            {
                firstOf.Add(is_first_of);
            }
        }

        public List<int> Solutions
        {
            get { return solutionTo; }
        }

        public List<int> FirstOf
        {
            get { return firstOf; }
        }

		public int FirstSolution
		{
			get { return Solutions.Count > 0 ? Solutions[0] : -1; }
		}

        public static char Empty {
            get { return ' '; }
        }
    }

    public class Board
	{
		List<Answer> words;
		int width, height;
		List<Cell> board;

		public Board(List<WordBlock> placement, int w, int h)
		{
            words = new List<Answer>();
            for(int i = 0; i < placement.Count; ++i)
            {
                words.Add(new Answer(placement[i]));
            }
			width = w;
			height = h;
			board = new List<Cell>();
			for(int i = 0; i < width * height; ++i)
			{
				board.Add(new Cell());
			}
			for(int i = 0; i < words.Count; ++i)
			{
				Coordinates start = words[i].Word.Start;
				Coordinates end = words[i].Word.End;
                board[start.y * width + start.x].AddSolution(i, i);
                if (words[i].Word.IsHorizontal)
				{
                    
					for(int x = start.x + 1; x <= end.x; ++x)
					{
						board[start.y * width + x].AddSolution(i);
					}
				} else
				{
					for (int y = start.y + 1; y <= end.y; ++y)
					{
						board[y * width + start.x].AddSolution(i);
					}
				}
			}
		}

		public int Width
		{
			get { return width; }
		}

		public int Height
		{
			get { return height; }
		}

		public char this[int i, int j]
		{
			get
			{
                char ret = Cell.Empty;
				if(board[j * width + i].HasWord)
				{
					ret = words[board[j * width +i].FirstSolution].Word[new Coordinates(i, j)];
				}
				return ret;
			}
		}

        public List<int> FirstOf(int i, int j)
        {
            if(board[j * width + i].HasWord)
            {
                return board[j * width + i].FirstOf;
            }
            return null;
        }

		public bool Solved
		{
			get
			{
				int total = 0;
				for (int i = 0; i < words.Count; ++i)
				{
					if (words[i].Solved)
					{
						++total;
					}
				}
				return total == words.Count;
			}
		}

        public List<WordBlock> Words
        {
            get
            {
                List<WordBlock> ret = new List<WordBlock>();
                for(int i = 0; i < words.Count; ++i)
                {
                    ret.Add(words[i].Word);
                }
                return ret;
            }
        }

        public bool Set(Coordinates coords, char c)
        {
            if(board[coords.y * width + coords.x].HasWord)
            {
                var solution = board[coords.y * width + coords.x].Solutions;
                bool ok = words[solution[0]].Set(coords, c);
                if (ok)
                {
                    for (int i = 1; i < solution.Count; ++i)
                    {
                        words[solution[i]].Set(coords, c);
                    }
                    return true;
                }
            }
            return false;
        }

        public void Reset()
        {
            for(int i = 0; i < words.Count; ++i)
            {
                words[i].Reset();
            }
        }

        public string Hints
        {
            get
            {
                string ret = string.Empty;
                for(int i = 0; i < words.Count; ++i)
                {
                    ret += (words[i].Word.IsHorizontal ? "H" : "V");
                    ret += i.ToString() + ": ";
                    string hint = words[i].Word.Hint;
                    if (hint.Length > 0)
                    {
                         ret += hint;
                    } else
                    {
                        ret += words[i].Word.Word;
                    }
                    ret += "\n\n";
                }
                return ret;
            }
        }

		public static string PrintBoard(Board b)
		{
			string ret = string.Empty;
			var words = b.words;
            ret += "board dimension is " + b.width.ToString() + ", " + b.height.ToString() + "\n";
			ret += "board has " + words.Count.ToString() + " words\n";
			for(int i = 0; i < words.Count; ++i)
			{
				ret += i.ToString() + ": " + words[i].Word.Word + "(" + words[i].Word.Length + ") occupies " + words[i].Word.Start.x.ToString() + ", " + words[i].Word.Start.y.ToString() + " to " + words[i].Word.End.x.ToString() + ", " + words[i].Word.End.y.ToString() + "\n";
			}

			for(int j = 0; j < b.height; ++j)
			{
				ret += "|";
				for(int i = 0; i < b.width; ++i)
				{
					char curr = b[i, j];
					ret = ret + curr.ToString() + "|";
				}
				ret += "\n";
			}
			Debug.Log(ret.Length.ToString());
			return ret;
		}
	}
}
