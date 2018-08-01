using System.Collections.Generic;
using System;

namespace Crossword
{
    public class Coordinates : Object
    {
        public int x, y;
        public Coordinates(int i = 0, int j = 0)
        {
            x = i;
            y = j;
        }

		public static Coordinates Zero
		{
			get { return new Coordinates(0, 0); }
		}

		public static int Cross(Coordinates u, Coordinates v)
		{
			return ((u.x * v.y) - (u.y * v.x));
		}

		public static int Dot(Coordinates u, Coordinates v)
		{
			return ((u.x * v.x) + (u.y * v.y));
		}

		public static int SqMagnitude(Coordinates u)
		{
			return Dot(u, u);
		}

		public override bool Equals(object obj)
		{
			if(obj == null || GetType() != obj.GetType())
			{
				return false;
			}
			Coordinates rhs = obj as Coordinates;
			return rhs.x == x && rhs.y == y;
		}

		public override int GetHashCode()
		{
			return base.GetHashCode();
		}
	}
}