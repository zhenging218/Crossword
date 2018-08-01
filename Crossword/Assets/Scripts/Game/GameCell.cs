using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Crossword;

public class GameCell : MonoBehaviour {

	Coordinates coords;
	public InputField text = null;
    public Text celltext = null;
	public GameCell()
	{
		coords = Coordinates.Zero;
	}

	// setup game cell.
	// remember cell coords on board and whether this cell is the first cell of any words.
	public void Place(Coordinates c, List<int> firstOf = null)
	{
		coords = c;

        if (firstOf != null)
        {
            string index = string.Empty;
            for (int i = 0; i < firstOf.Count; ++i)
            {
                index += firstOf[i].ToString();
                if(i + 1 < firstOf.Count)
                {
                    index += ",";
                }
            }
            celltext.text = index;
            celltext.gameObject.SetActive(true);
        } else
        {
            celltext.gameObject.SetActive(false);
        }
	}

	void Update()
	{
		if(text != null)
		{
			// show input as only uppercase.
			if(text.isFocused)
			{
				text.text = text.text.ToUpper();
			}
		}
	}

	// only lock in input when player loses focus of cell.
	public void OnInputEnd(InputField i)
	{
		if(i.text.Length == 1)
		{
			GameManager.Instance.Set(coords, i.text.ToLower()[0]);
            GameManager.Instance.TrySolve();
		}
	}

	public float Width
	{
		get
		{
			var t = transform as RectTransform;
			if(t != null)
			{
				return t.rect.width;
			}
			return 0.0f;
		}
	}

	public float Height
	{
		get
		{
			var t = transform as RectTransform;
			if (t != null)
			{
				return t.rect.height;
			}
			return 0.0f;
		}
	}

	
}
