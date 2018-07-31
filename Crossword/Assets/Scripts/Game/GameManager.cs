using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Crossword;
using UnityEngine.UI;

public class GameManager : MonoBehaviour {

    static GameManager instance = null;
    public static GameManager Instance
    {
        get { return instance; }
    }

    [Range(2, 10)]
    public int TotalWords = 2;
    public char StartWith = 'a';

    Board board = null;

    public GameObject CellPrefab;
    public RectTransform BoardOrigin;

	public Text t;

    void Awake()
    {
        if(instance && instance != this)
        {
            Destroy(gameObject);
            
        } else
        {
            instance = this;
            var db = WordDatabase.Load();
            if (db != null)
            {
                var indices = db.GetRandomWordList(StartWith, TotalWords);
                List<Alphaword> awords = new List<Alphaword>();
                for (int i = 0; i < indices.Count; ++i)
                {
                    awords.Add(db['a', indices[i]]);
                }
                board = LevelGenerator.Generate(awords);
				if(t != null)
				{
					t.text = Board.PrintBoard(board);
				}
				var r = CellPrefab.GetComponent<GameCell>();
                if(r == null)
                {
                    Debug.LogError("NO PROPER CELL PREFAB");
                    return;
                }
                float width = r.Width;
                float height = r.Height;

				Vector3 origin = new Vector3(BoardOrigin.position.x - width / 2, BoardOrigin.position.y + height / 2, 0);
				BoardOrigin.sizeDelta = new Vector2(width * board.Width, height * board.Height);

				for(int j = 0; j < board.Height; ++j)
				{
					for(int i = 0; i < board.Width; ++i)
					{
						if(board[i,j] == Cell.Empty)
						{
							continue;
						}
						var curr_cell = Instantiate(CellPrefab);
						curr_cell.transform.position = origin;
						curr_cell.transform.SetParent(BoardOrigin);
						curr_cell.GetComponent<GameCell>().Place(new Coordinates(i, j));
						(curr_cell.transform as RectTransform).localPosition = new Vector2((i + 1) * width, -(j + 1) * height);
					}
				}
            }
        }
    }

    void OnDestroy()
    {
        if(instance == this)
        {
            instance = null;
        }
    }

    // Use this for initialization
    void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
		
	}

    public bool Solved
    {
        get { return board.Solved; }
    }

    public bool Set(Coordinates coords, char c)
    {
        return board.Set(coords, c);
    }
}
