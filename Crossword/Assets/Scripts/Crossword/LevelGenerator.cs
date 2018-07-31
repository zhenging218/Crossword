using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace Crossword
{
    public static class LevelGenerator
    {
        static Alphaword CopyWord(Alphaword word)
        {
            return new Alphaword(string.Copy(word.word), string.Copy(word.hint));
        }

        static List<Alphaword> CopyWordList(List<Alphaword> src)
        {
            List<Alphaword> dst = new List<Alphaword>();
            for (int i = 0; i < src.Count; ++i)
            {
                dst.Add(CopyWord(src[i]));
            }
            return dst;
        }

        static void SortWordListDescending(ref List<Alphaword> rhs)
        {
            rhs.Sort((left, right) => left.word.Length.CompareTo(right.word.Length));
            rhs.Reverse();
        }

		static Pair<WordBlock, int> PlaceVertical(ref List<WordBlock> data, Alphaword word, int i, int width, int height)
		{
			List<Pair<WordBlock, int>> results = new List<Pair<WordBlock, int>>();
			bool valid = true;
			for (int l = 0; l < data[i].Length && valid; ++l)
			{
				Coordinates cell = data[i][l];
                for (int j = 0; j < word.Length && valid; ++j)
				{
                    if (word.word[j] != data[i][cell])
                    {
                        // cant overlap
                        continue;
                    }
                    // check if vertical word can fit in grid.
                    Coordinates v_start = new Coordinates(cell.x, cell.y - j);
					Coordinates v_end = new Coordinates(cell.x, cell.y + (word.Length - j - 1));
					bool v_front_ok = !(v_start.y < 0);
					bool v_back_ok = !(v_end.y >= height);
					if (!v_front_ok)
					{
						// because word is pushed "upwards" to test whole word,
						// if word leaves top of board then it will definitely be
						// invalid for the rest.
						valid = false;
					}
					else if (!v_back_ok)
					{
						// no space below, try next new word letter.
						continue;
					}
					else
					{
						int score = 1;
						bool placeOK = true;
						// try placement. check if all other words overlap properly, if any overlap occurs.
						for (int w = 0; w < data.Count && placeOK; ++w)
						{
							if (w == i)
							{
								continue;
							}
                            // check if word clashes with other word
                            for (int y = 0; y < word.Length; ++y)
                            {
                                int x = cell.x;
                                Coordinates pos = new Coordinates(x, v_start.y + y);
                                char other = data[w][pos];
                                if (other == Cell.Empty)
                                {
                                    // not clashing
                                    Coordinates[] dir = new Coordinates[]
                                    {
                                        new Coordinates(pos.x + 1, pos.y),
                                        new Coordinates(pos.x - 1, pos.y),
                                        new Coordinates(pos.x, pos.y + 1),
                                        new Coordinates(pos.x, pos.y - 1)
                                    };
                                    bool near = false;
                                    // cannot be right beside the word.
                                    for (int d = 0; d < dir.Length && !near; ++d)
                                    {
                                        char test = data[w][dir[d]];
                                        if (test != Cell.Empty)
                                        {
                                            near = true;
                                        }
                                    }
                                    if (near && data[w].IsVertical)
                                    {
                                        placeOK = false;
                                    }
                                    continue;
                                }
                                else if (other == word.word[y] && data[w].IsHorizontal)
                                {
                                    // overlap same character
                                    break;
                                }
                                else
                                {
                                    // fail
                                    placeOK = false;
                                }
                            }
                        }
						if (placeOK)
						{
							// add word block and score.
							results.Add(Pairs.MakePair(new WordBlock(word, v_start, v_end), score));
						}
					}
				}
			}
			if(results.Count == 0)
			{
				return null;
			}
			results.Sort((x, y) => x.Right.CompareTo(y.Right));
			return results[results.Count - 1];
		}

		static Pair<WordBlock, int> PlaceHorizontal(ref List<WordBlock> data, Alphaword word, int i, int width, int height)
		{
			List<Pair<WordBlock, int>> results = new List<Pair<WordBlock, int>>();
			bool valid = true;
			for (int l = 0; l < data[i].Length && valid; ++l)
			{
				Coordinates cell = data[i][l];
				for (int j = 0; j < word.Length && valid; ++j)
				{
                    if(word.word[j] != data[i][cell])
                    {
                        // cant overlap
                        continue;
                    }
					// check if vertical word can fit in grid.
					Coordinates h_start = new Coordinates(cell.x - j, cell.y);
					Coordinates h_end = new Coordinates(cell.x + (word.Length - j - 1), cell.y);
					bool h_front_ok = !(h_start.x < 0);
					bool h_back_ok = !(h_end.x >= width);
					if (!h_front_ok)
					{
						// because word is pushed "leftwards" to test whole word,
						// if word leaves top of board then it will definitely be
						// invalid for the rest.
						valid = false;
					}
					else if (!h_back_ok)
					{
						// no space to the right, try next new word letter.
						continue;
					}
					else
					{
						int score = 1;
						bool placeOK = true;
						// try placement. check if all other words overlap properly, if any overlap occurs.
						for (int w = 0; w < data.Count && placeOK; ++w)
						{
                            if(w == i)
                            {
                                continue;
                            }
							// check if word clashes with other word
                            for(int x = 0; x < word.Length && placeOK; ++x)
                            {
                                int y = cell.y;
                                Coordinates pos = new Coordinates(h_start.x + x, y);
                                char other = data[w][pos];
                                if(other == Cell.Empty)
                                {
                                    // not clashing
                                    Coordinates[] dir = new Coordinates[]
                                    {
                                        new Coordinates(pos.x + 1, pos.y),
                                        new Coordinates(pos.x - 1, pos.y),
                                        new Coordinates(pos.x, pos.y + 1),
                                        new Coordinates(pos.x, pos.y - 1)
                                    };
                                    bool near = false;
                                    // cannot be right beside the word.
                                    for(int d = 0; d < dir.Length && !near; ++d)
                                    {
                                        char test = data[w][dir[d]];
                                        if(test != Cell.Empty)
                                        {
                                            near = true;
                                        }
                                    }
                                    if(near && data[w].IsHorizontal)
                                    {
                                        placeOK = false;
                                    }
                                    continue;
                                }
                                else if(other == word.word[x] && data[w].IsVertical)
                                {
                                    // overlap same character
                                    
                                    break;
                                } else
                                {
                                    // fail
                                    placeOK = false;
                                }
                            }
						}
						if (placeOK)
						{
							// add word block and score.
							results.Add(Pairs.MakePair(new WordBlock(word, h_start, h_end), score));
						}
					}
				}

			}
			if (results.Count == 0)
			{
				return null;
			}
			results.Sort((x, y) => x.Right.CompareTo(y.Right));
			return results[results.Count - 1];
		}

		static Pair<WordBlock, int> PlaceWord(ref List<WordBlock> data, Alphaword word, int width, int height)
        {
			List<Pair<WordBlock, int>> results = new List<Pair<WordBlock, int>>();
            for(int i = 0; i < data.Count; ++i) // loop through currently available words
            {
				if(data[i].IsHorizontal) // vertical new word
				{
					var curr_result = PlaceVertical(ref data, word, i, width, height);
					if(curr_result != null)
					{
						results.Add(curr_result);
					}
				}
				else if(data[i].IsVertical) // horizontal new word
				{
					var curr_result = PlaceHorizontal(ref data, word, i, width, height);
					if (curr_result != null)
					{
						results.Add(curr_result);
					}
				}
            }
			if (results.Count == 0) return null;
			results.Sort((x, y) => x.Right.CompareTo(y.Right));
			return results[results.Count - 1];
		}

        public static Board Generate(List<Alphaword> words, int max_tries = 1000)
        {
			int width = 0;
			int add = int.MaxValue;
			if(words.Count < 2)
			{
				Debug.Log("no words!");
				return null;
			}
			if(words.Count > 10)
			{
				words.RemoveRange(10, words.Count - 10);
			}
			for(int i = 0; i < words.Count; ++i)
			{
				width = Mathf.Max(width, words[i].Length);
				add = Mathf.Min(add, words[i].Length);
			}
			width += add;
			int height = width;
            List<Alphaword> sorted_words = CopyWordList(words);
            SortWordListDescending(ref sorted_words);
            int best_score = 0;
			List<WordBlock> best_gen_words = null;
            var rng = new System.Random();
            for (int i = 0; i < max_tries; ++i)
            {
                var result = sorted_words.OrderBy(x => rng.Next());
                sorted_words = new List<Alphaword>(result);
                List<WordBlock> gen_words = new List<WordBlock>();
				int curr_score = 0;
				bool curr_tryOK = true;
                for(int curr_word = 0; curr_word < sorted_words.Count && curr_tryOK; ++curr_word)
                {
                    if(gen_words.Count == 0)
                    {
                        // place first word
                        int dx = Random.Range(0, 2);
                        int dy = (dx == 0) ? 1 : 0;
						int range_x = width - sorted_words[curr_word].Length;
						int range_y = height - sorted_words[curr_word].Length;
						int start_x = Random.Range(0, range_x);
						int start_y = Random.Range(0, range_y);
						Coordinates first_start = new Coordinates(start_x, start_y);
						Coordinates first_end = new Coordinates(first_start.x + (sorted_words[curr_word].Length - 1) * dx, (first_start.y + sorted_words[curr_word].Length - 1) * dy);
						gen_words.Add(new WordBlock(sorted_words[curr_word], first_start, first_end));
                    } else
                    {
						// place every other word
						// test overlap
						var curr_result = PlaceWord(ref gen_words, sorted_words[curr_word], width, height);
						if(curr_result != null)
						{
							gen_words.Add(curr_result.Left);
							curr_score += curr_result.Right;
						} else
						{
							curr_tryOK = false;
						}
                    }
                }
				if(curr_tryOK)
				{
					if(curr_score > best_score)
					{
						best_gen_words = gen_words;
						best_score = curr_score;
					}
				}
            }
			// setup board using gen_words and return
			if(best_gen_words != null)
			{
				return new Board(best_gen_words, width, height);
			}
            return null;
        }
    }
}
