using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using System.Linq;
using UnityEngine.Tilemaps;
using Random = UnityEngine.Random;

public class GameOfLife : MonoBehaviour
{
    private CellGrid _cellGrid;
    [SerializeField] private UIInputs UIScript;
    [SerializeField] private TextMeshProUGUI _textMeshPro;

    public Tile tile;

    private Coroutine _running;

    private void Start()
    {
        _cellGrid = new CellGrid(100,150);
        Camera.main.transform.position = new Vector3(_cellGrid.X / 2, _cellGrid.Y / 2, -10);
    }

    private void Update()
    {
        if (_textMeshPro != null)
            _textMeshPro.text = "Generations:\n" + _cellGrid.Generations;

        if (_running == null && !UIScript.Pause)
            _running = StartCoroutine(CalculateCoroutine());

    }

    IEnumerator CalculateCoroutine()
    {
        while (true)
        {
            if (!UIScript.Pause)
            {
                _cellGrid.CalculateNextStates();
                yield return new WaitForSeconds(1 / UIScript.GenerationSpeed);
                continue;
            }

            yield return null;
        }
    }
    
    public void ResetGrid()
    {
        _cellGrid.GenerateGrid();
    }

    public void BackIntime()
    {
        _cellGrid.BackInTime();
    }

    public void ForwardOneGeneration()
    {
        _cellGrid.CalculateNextStates();
    }
}

public class CellGrid
{
    public static CellGrid Instance;

    private float _RNG = 0f;

    public float RNG
    {
        get => _RNG;
        set
        {
            if (0 > value || value > 1)
                return;
            _RNG = value;
        }
    }

    private int _generations = 0;

    public int Generations
    {
        get => _generations;
    }

    private int _x;
    private int _y;

    public int X
    {
        get => _x;
        set
        {
            if (value > 0)
            {
                _x = value;
                return;
            }

            Debug.Log("Can't use negative or null values.");
        }
    }

    public int Y
    {
        get => _y;
        set
        {
            if (value > 0)
            {
                _y = value;
                return;
            }

            Debug.Log("Can't use negative or null values.");
        }
    }

    private bool[][] _cells;
    private List<bool[][]> _history;

    private Tilemap _cubes;
    private Tile _white = Resources.Load<Tile>("White");
    private Tile _black = Resources.Load<Tile>("Black");

    public CellGrid()
    {
        _x = 10;
        _y = 10;
        Instance = this;

        _cells = new bool[_x][];
        _history = new List<bool[][]>();
        GenerateGrid();
    }

    public CellGrid(int x, int y)
    {
        _x = x;
        _y = y;
        Instance = this;

        _cells = new bool[_x][];
        _history = new List<bool[][]>();
        GenerateGrid();
    }

    public CellGrid(Vector2Int vector)
    {
        _x = vector.x;
        _y = vector.y;

        Instance = this;

        _cells = new bool[_x][];
        _history = new List<bool[][]>();
        GenerateGrid();
    }

    public void GenerateGrid()
    {
        Camera.main.transform.position = new Vector3(_x / 2, _y / 2, -10);
        _generations = 0;
        DestroyCurrentGrid();

        if (_x % 2 == 1)
        {
            _x++;
        }

        if (_y % 2 == 1)
        {
            _y++;
        }


        for (int x = 0; x < _x; x++)
        {
            _cells[x] = new bool[_y];
            for (int y = 0; y < _y; y++)
            {
                float rng = Random.Range(0f, 1f);
                if (rng >= _RNG)
                    _cells[x][y] = false;
                else
                    _cells[x][y] = true;
            }
        }

        GenerateCubes();
    }

    private void DestroyCurrentGrid()
    {
        DestroyCubes();

        _cells = new bool[_x][];
        _history.Clear();
    }

    private void DestroyCubes()
    {
        if (_cubes == null) return;
        _cubes.ClearAllTiles();
    }

    private void GenerateCubes()
    {
        if (_cubes == null)
        {
            var newObject = new GameObject();
            newObject.AddComponent<UnityEngine.Grid>();
            var tilemap = new GameObject();
            tilemap.transform.SetParent(newObject.transform);
            _cubes = tilemap.AddComponent<Tilemap>();
            tilemap.AddComponent<TilemapRenderer>();

            //_cubes = tilemap.GetComponent<Tilemap>();
        }

        for (int x = 0; x < _cells.Length; x++)
        {
            for (int y = 0; y < _cells[x].Length; y++)
            {
                //Set cube's color based on current cell
                if (_cells[x][y])
                {
                    _cubes.SetTile(new Vector3Int(x, y, 0), _white);
                }
                else
                {
                    _cubes.SetTile(new Vector3Int(x, y, 0), _black);
                }
            }
        }
    }

    public void CalculateNextStates()
    {
        _history.Add(_cells);
        bool[][] nextGrid = new bool[_cells.Length][];
        for (int x = 0; x < _cells.Length; x++)
        {
            nextGrid[x] = new bool[_cells[x].Length];
        }

        for (int x = 0; x < _cells.Length; x++)
        {
            for (int y = 0; y < _cells[x].Length; y++)
            {
                int neighbors = CalculateNeighbors(x, y);

                if (_cells[x][y])
                {
                    if (neighbors == 2 || neighbors == 3)
                        nextGrid[x][y] = true;
                    else
                        nextGrid[x][y] = false;
                }
                else
                {
                    if (neighbors == 3)
                        nextGrid[x][y] = true;
                    else
                        nextGrid[x][y] = false;
                }
            }
        }

        _cells = nextGrid;

        UpdateCubes();
    }

    private int CalculateNeighbors(int cellX, int cellY)
    {
        int count = 0;

        for (int x = -1; x < 2; x++)
        {
            if (cellX + x < 0 || cellX + x >= _cells.Length)
                continue;
            for (int y = -1; y < 2; y++)
            {
                if (cellY + y < 0 || cellY + y >= _cells[cellX + x].Length || (y == 0 && x == 0))
                    continue;

                if (_cells[cellX + x][cellY + y])
                    count++;
            }
        }

        return count;
    }

    private void UpdateCubes()
    {
        _generations++;
        for (int x = 0; x < _cells.Length; x++)
        {
            for (int y = 0; y < _cells[x].Length; y++)
            {
                if (_cells[x][y] )
                    _cubes.SetTile(new Vector3Int(x, y, 0), _white);
                else
                    _cubes.SetTile(new Vector3Int(x, y, 0), _black);
            }
        }
    }

    public void InverseCell(int x, int y)
    {
        if (x < 0 || y < 0 || x >= _cells.Length || y >= _cells[x].Length) return;

        _cells[x][y] = !_cells[x][y];

        if (_cells[x][y])
            _cubes.SetTile(new Vector3Int(x, y, 0), _white);
        else
            _cubes.SetTile(new Vector3Int(x, y, 0), _black);
    }

    public void BackInTime()
    {
        if (_history.Count <= 0) return;

        int compteur = 1;

        while (compteur < _history.Count - 1 && Equals(_history[^compteur]))
        {
            compteur++;
        }

        _cells = _history[^compteur];
        _history.RemoveRange(_history.Count - compteur, compteur);
        _generations -= compteur + 1;

        UpdateCubes();
    }

    private bool Equals(bool[][] array)
    {
        if (_cells.Length != array.Length) return false;
        for (int x = 0; x < _cells.Length; x++)
        {
            if (_cells[x].Length != array[x].Length) return false;
            for (int y = 0; y < _cells[x].Length; y++)
            {
                if (_cells[x][y] != array[x][y]) return false;
            }
        }

        return true;
    }
}