using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Crossword;

public class GameCell : MonoBehaviour {

	Coordinates coords;
	public InputField text = null;
	public GameCell()
	{
		coords = Coordinates.Zero;
	}

	public void Place(Coordinates c)
	{
		coords = c;
	}

	void Update()
	{
		if(text != null)
		{
			if(text.isFocused)
			{
				text.text = text.text.ToUpper();
			}
		}
	}

	public void OnInputEnd(string i)
	{
		if(i.Length == 1)
		{
			GameManager.Instance.Set(coords, i[0]);
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
