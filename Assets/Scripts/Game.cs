using UnityEngine;

public class Game : MonoBehaviour
{
    [SerializeField] private int _width;
    [SerializeField] private int _height;
    [SerializeField] private int _mineCount = 16;

    private bool _gameOver = false;

    private Board _board;
    private Cell[,] _state;
    private Camera _cam;

    private System.Action _onWin;
    private System.Action _onLose;

    private void OnValidate()
    {
        _mineCount = Mathf.Clamp(_mineCount, 0, _width * _height);
    }


    private void Awake()
    {
        _cam = Camera.main;
        _board = GetComponentInChildren<Board>();
    }

    public void StartNewGame(System.Action onWin, System.Action onLose)
    {
        _onWin = onWin;
        _onLose = onLose;


        // 🧹 Clear existing board if any
        if (_board != null)
        {
            _board.Clear();
        }

        NewGame();
    }

    public void SetSettings(int width, int height, int mineCount)
    {
        _width = width;
        _height = height;
        _mineCount = mineCount;
    }

    private void NewGame()
    {
        _gameOver = false;
        _state = new Cell[_width, _height];

        GenerateCells();
        GenerateMines();
        GenerateNumbers();

        _board.Draw(_state);

        _cam.transform.position = new Vector3(_width / 2f, _height / 2f, -10f);

        float aspectRatio = (float)Screen.width / Screen.height;
        float verticalSize = _height / 2f + 1f;
        float horizontalSize = (_width / 2f + 1f) / aspectRatio;
        _cam.orthographicSize = Mathf.Max(verticalSize, horizontalSize);
    }

    private void GenerateCells()
    {
        for (int x = 0; x < _width; x++)
        {
            for (int y = 0; y < _height; y++)
            {
                Cell cell = new Cell();
                cell.position = new Vector3Int(x, y, 0);
                cell.type = Cell.Type.Empty;
                _state[x, y] = cell;
            }
        }
    }

    private void GenerateMines()
    {
        for (int i = 0; i < _mineCount; i++)
        {
            int x = Random.Range(0, _width);
            int y = Random.Range(0, _height);

            while (_state[x, y].type == Cell.Type.Mine)
            {
                x++;

                if (x >= _width)
                {
                    x = 0;
                    y++;
                }

                if (y >= _height)
                {
                    y = 0;
                }
            }

            _state[x, y].type = Cell.Type.Mine;
        }
    }

    private void GenerateNumbers()
    {
        for (int x = 0; x < _width; x++)
        {
            for (int y = 0; y < _height; y++)
            {
                Cell cell = _state[x, y];

                if (cell.type == Cell.Type.Mine)
                    continue;

                cell.number = CountMines(x, y);
                if (cell.number > 0)
                    cell.type = Cell.Type.Number;

                _state[x, y] = cell;
            }
        }
    }

    private int CountMines(int cellX, int cellY)
    {
        int count = 0;

        for (int adjacentX = -1; adjacentX <= 1; adjacentX++)
        {
            for (int adjacentY = -1; adjacentY <= 1; adjacentY++)
            {
                if (adjacentX == 0 && adjacentY == 0)
                    continue;

                int x = cellX + adjacentX;
                int y = cellY + adjacentY;

                if (GetCell(x, y).type == Cell.Type.Mine) count++;
            }
        }

        return count;
    }

    private void Update()
    {
        //if (Input.GetKeyDown(KeyCode.E))
        //    NewGame();
        if (!_gameOver)
        {
            if (Input.GetMouseButtonDown(1))
            {
                Flag();
            }
            else if (Input.GetMouseButtonDown(0))
            {
                Reveal();
            }
        }
    }

    private void Reveal()
    {
        Vector3 worldPosition = _cam.ScreenToWorldPoint(Input.mousePosition);
        Vector3Int cellPosition = _board.tileMap.WorldToCell(worldPosition);
        Cell cell = GetCell(cellPosition.x, cellPosition.y);

        if (cell.type == Cell.Type.Invalid || cell.revelead || cell.flagged) return;

        switch (cell.type)
        {
            case Cell.Type.Mine:
                Explode(cell);
                break;

            case Cell.Type.Empty:
                Flood(cell);
                CheckWinCondition();
                break;

            default:
                cell.revelead = true;
                _state[cellPosition.x, cellPosition.y] = cell;
                CheckWinCondition();
                break;
        }

        AudioManager.Instance.PlayClick();

        _board.Draw(_state);
    }

    private void Explode(Cell cell)
    {
        _gameOver = true;
        _onLose?.Invoke();
        cell.revelead = true;
        cell.exploded = true;

        _state[cell.position.x, cell.position.y] = cell;

        for(int x =0; x < _width; x++)
        {
            for(int y = 0; y < _height; y++)
            {
                cell = _state[x, y];

                if(cell.type == Cell.Type.Mine)
                {
                    cell.revelead = true;
                    _state[x, y] = cell;
                }
            }
        }
    }

    private void Flood(Cell cell)
    {
        if (cell.revelead) return;
        if (cell.type == Cell.Type.Mine || cell.type == Cell.Type.Invalid) return;

        cell.revelead = true;
        _state[cell.position.x, cell.position.y] = cell;

        if (cell.type == Cell.Type.Empty)
        {
            Flood(GetCell(cell.position.x - 1, cell.position.y));
            Flood(GetCell(cell.position.x + 1, cell.position.y));
            Flood(GetCell(cell.position.x, cell.position.y - 1));
            Flood(GetCell(cell.position.x, cell.position.y + 1));
        }
    }

    private void Flag()
    {
        Vector3 worldPosition = _cam.ScreenToWorldPoint(Input.mousePosition);
        Vector3Int cellPosition = _board.tileMap.WorldToCell(worldPosition);
        Cell cell = GetCell(cellPosition.x, cellPosition.y);

        if (cell.type == Cell.Type.Invalid || cell.revelead) return;

        cell.flagged = !cell.flagged;
        _state[cellPosition.x, cellPosition.y] = cell;

        AudioManager.Instance.PlayClick();

        _board.Draw(_state);
    }

    private void CheckWinCondition()
    {
        for (int x = 0; x < _width; x++)
        {
            for (int y = 0; y < _height; y++)
            {
                Cell cell = _state[x, y];
                if (cell.type != Cell.Type.Mine && !cell.revelead) return;
            }
        }

        Debug.Log("YOU WIN");
        _gameOver = true;
        _onWin?.Invoke();
        for (int x = 0; x < _width; x++)
        {
            for (int y = 0; y < _height; y++)
            {
                Cell cell = _state[x, y];

                if (cell.type == Cell.Type.Mine)
                {
                    cell.flagged = true;
                    _state[x, y] = cell;
                }
            }
        }
    }

    private Cell GetCell(int x,int y)
    {
        if (IsValid(x, y))
            return _state[x, y];
        else
        {
            return new Cell();
        }
    }

    private bool IsValid(int x,int y)
    {
        return x >= 0 && x < _width && y >= 0 && y < _height;
    }
}

[System.Serializable]
public struct GameSettings
{
    public int width;
    public int height;
    public int mineCount;

    public GameSettings(int w, int h, int mines)
    {
        width = w;
        height = h;
        mineCount = mines;
    }
}