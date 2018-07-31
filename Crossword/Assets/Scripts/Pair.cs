using System.Collections;
using System.Collections.Generic;
namespace Crossword
{
    public class Pair<T, U>
    {
        public T Left { get; set; }
        public U Right { get; set; }

        public Pair(T lhs, U rhs)
        {
            Left = lhs;
            Right = rhs;
        }
    }

    public static class Pairs
    {
        public static Pair<T, U> MakePair<T, U>(T lhs, U rhs)
        {
            return new Pair<T, U>(lhs, rhs);
        }
    }
}