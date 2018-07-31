using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Crossword;

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
                var r = CellPrefab.transform as RectTransform;
                if(r == null)
                {
                    Debug.LogError("NO PROPER CELL PREFAB");
                    return;
                }
                float width = r.rect.width;
                float height = r.rect.height;
                Vector3 origin = Vector3.zero;
                if(BoardOrigin != null)
                {
                    origin = new Vector3(BoardOrigin.position.x + width / 2, BoardOrigin.position.y + height / 2, BoardOrigin.position.z);
                } else
                {
                    Debug.LogError("NO BOARD TO PLACE CELLS IN");
                    return;
                }

                var words = board.Words;

                for(int i = 0; i < words.Count; ++i)
                {
                    Coordinates start = words[i].Start, end = words[i].End;
                    // place cells on screen.
                    // parent to board origin.
                    if (words[i].IsHorizontal)
                    {
                        int a = 0;
                        for (int x = start.x; x <= end.x; ++x, ++a)
                        {
                            
                        }
                    }
                    else
                    {
                        int a = 0;
                        for (int y = start.y; y <= end.y; ++y, ++a)
                        {
                            
                        }
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

    bool Set(Coordinates coords, char c)
    {
        return board.Set(coords, c);
    }
}
