using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Crossword;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour {

    public enum confirm_state
    {
        NONE,
        RESET,
        GOMAINMENU,
        NEWBOARD
    }

    confirm_state state;

    static GameManager instance = null;
    public static GameManager Instance
    {
        get { return instance; }
    }

    [Range(2, 10)]
    public int TotalWords = 2;
    public char StartWith = 'a';
	public bool RandomStart = false;
    Board board = null;

    public GameObject CellPrefab;
    public RectTransform BoardOrigin;
    public RectTransform BoardBackground;

    public Text HintBox;

    public Button GameMenuButton;
    public GameObject PauseMenu;
    public GameObject WinMenu;
    public GameObject ConfirmMenu;

    public int LoadingLevel;
    public int MainMenuLevel;

    List<InputField> cells;

    void Awake()
    {
        if(instance && instance != this)
        {
            Destroy(gameObject);
            
        } else
        {
            instance = this;
            state = confirm_state.NONE;
            var db = WordDatabase.Load();
            if (db != null)
            {
				if (BoardGen.Instance == null)
				{
					if (RandomStart)
					{
						StartWith = (char)Random.Range('a', 'z' + 1);
					}
					var indices = db.GetRandomWordList(StartWith, TotalWords);
					if (indices == null)
					{
						Debug.LogError("NO WORDS STARTING WITH " + StartWith + " IN DATABASE!");
						return;
					}
					List<Alphaword> awords = new List<Alphaword>();
					for (int i = 0; i < indices.Count; ++i)
					{
						awords.Add(db[StartWith, indices[i]]);
					}
					board = LevelGenerator.Generate(awords);
				} else
				{
					board = BoardGen.Instance.Board;
				}
				var r = CellPrefab.GetComponent<GameCell>();
                var rt = CellPrefab.transform as RectTransform;
                if(r == null || rt == null)
                {
                    Debug.LogError("NO PROPER CELL PREFAB");
                    return;
                }
                float width = r.Width;
                float height = r.Height;
                
				Vector3 origin = new Vector3(BoardOrigin.position.x, BoardOrigin.position.y, 0);
                BoardBackground.sizeDelta = new Vector2(width * board.Width, height * board.Height);

                if(HintBox != null)
                {
					// paste hints onto hint box
                    HintBox.text = board.Hints;
                }
                
				// build cells
                cells = new List<InputField>();

                for (int j = 0; j < board.Height; ++j)
				{
					for(int i = 0; i < board.Width; ++i)
					{
						if(board[i,j] == Cell.Empty)
						{
							continue;
						}
						var curr_cell = Instantiate(CellPrefab);
						curr_cell.transform.position = origin;
						// put cells in the scroll view.
						curr_cell.transform.SetParent(BoardOrigin);
                        curr_cell.GetComponent<GameCell>().Place(new Coordinates(i, j), board.FirstOf(i, j));
                        cells.Add(curr_cell.GetComponent<InputField>());
                        cells[cells.Count - 1].interactable = true;
						// place cell at correct position based on size of cell and board coords.
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

    public void OnPause()
    {
        for (int i = 0; i < cells.Count; ++i)
        {
            cells[i].interactable = false;
        }
        PauseMenu.SetActive(true);
        WinMenu.SetActive(false);
        ConfirmMenu.SetActive(false);
        GameMenuButton.interactable = false;
        state = confirm_state.NONE;
    }

    public void OnUnpause()
    {
        for (int i = 0; i < cells.Count; ++i)
        {
            cells[i].interactable = true;
        }
        PauseMenu.SetActive(false);
        WinMenu.SetActive(false);
        ConfirmMenu.SetActive(false);
        GameMenuButton.interactable = true;
        state = confirm_state.NONE;
    }

    public void OnNewBoard()
    {
        SceneManager.LoadScene(LoadingLevel);
    }

    public void OnMainMenu()
    {
        SceneManager.LoadScene(MainMenuLevel);
    }

    public void OnReset()
    {
        for(int i = 0; i < cells.Count; ++i)
        {
            cells[i].text = string.Empty;
        }
        board.Reset();
    }

    public void OnConfirmationYes()
    {
        switch(state)
        {
            case confirm_state.GOMAINMENU:
                OnMainMenu();
                break;
            case confirm_state.NEWBOARD:
                OnNewBoard();
                break;
            case confirm_state.RESET:
                OnReset();
                OnUnpause();
                break;
        }
    }

    public void OnConfirmationNo()
    {
        OnUnpause();
    }

    public void OnResumeButton()
    {
        OnUnpause();
    }

    public void OnNewBoardButton()
    {
        PauseMenu.SetActive(false);
        WinMenu.SetActive(false);
        state = confirm_state.NEWBOARD;
        ConfirmMenu.SetActive(true);
    }

    public void OnMainMenuButton()
    {
        PauseMenu.SetActive(false);
        WinMenu.SetActive(false);
        state = confirm_state.GOMAINMENU;
        ConfirmMenu.SetActive(true);
    }

    public void OnResetButton()
    {
        PauseMenu.SetActive(false);
        WinMenu.SetActive(false);
        state = confirm_state.RESET;
        ConfirmMenu.SetActive(true);
    }

    public void OnGameMenuButton()
    {
		// game menu button should pause game.
		// any window closing should unpause (unless moving to another level).
        OnPause();
    }

    public bool Solved
    {
        get { return board.Solved; }
    }

    public bool Set(Coordinates coords, char c)
    {
        return board.Set(coords, c);
    }

	// if board is solved, disable all input.
    public void TrySolve()
    {
        if(board.Solved) {
            GameMenuButton.interactable = false;
            WinMenu.SetActive(true);
            for (int i = 0; i < cells.Count; ++i)
            {
                cells[i].interactable = false;
            }
        }
    }
}
