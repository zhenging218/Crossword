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

        public bool Check(Coordinates coords, char c)
        {
            char mine = word[coords];
            bool ret = false;
            if(mine == Cell.Empty)
            {
                return false;
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
    }

    public class Cell
    {
        List<int> solutionTo;

        public bool HasWord
        {
            get { return !(solutionTo.Count == 0); }
        }

        public Cell()
        {
            solutionTo = new List<int>();
            solutionTo.Clear();
        }

        public void AddSolution(int index)
        {
            solutionTo.Add(index);
        }

        public List<int> Solutions
        {
            get { return solutionTo; }
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
				if(words[i].Word.IsHorizontal)
				{
					int a = 0;
					for(int x = start.x; x <= end.x; ++x, ++a)
					{
						board[start.y * width + x].AddSolution(i);
					}
				} else
				{
					int a = 0;
					for (int y = start.y; y <= end.y; ++y, ++a)
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
					var solution = board[j * width + i].Solutions;
					int index = solution[0];
					ret = words[index].Word[new Coordinates(i, j)];
				}
				return ret;
			}
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

        public bool Check(Coordinates coords, char c)
        {
            if(board[coords.y * width + coords.x].HasWord)
            {
                var solution = board[coords.y * width + coords.x].Solutions;
                bool ok = words[0].Check(coords, c);
                if (ok)
                {
                    for (int i = 1; i < solution.Count; ++i)
                    {
                        words[i].Check(coords, c);
                    }
                    return true;
                }
            }
            return false;
        }

		public static string PrintBoard(Board b)
		{
			string ret = string.Empty;
			var words = b.words;
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
