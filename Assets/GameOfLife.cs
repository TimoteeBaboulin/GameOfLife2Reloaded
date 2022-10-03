using System.Collections;
using TMPro;
using UnityEngine;
using Random = UnityEngine.Random;

public class GameOfLife : MonoBehaviour
{
    private Grid _cellGrid;
    [SerializeField] private UIInputs UIScript;
    [SerializeField] private TextMeshProUGUI _textMeshPro;

    private Coroutine _running;

    private void Start()
    {
        _cellGrid = new Grid(100,150);
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
}

public class Grid
{
    public static Grid Instance;

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
                GenerateGrid();
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
                GenerateGrid();
                return;
            }
            Debug.Log("Can't use negative or null values.");
        }
    }

    private bool[][] _cells;
    private GameObject[][] _cubes;

    public Grid()
    {
        _x = 10;
        _y = 10;
        Instance = this;

        _cells = new bool[_x][];
        GenerateGrid();
    }

    public Grid(int x, int y)
    {
        _x = x;
        _y = y;
        Instance = this;

        _cells = new bool[_x][];
        GenerateGrid();
    }

    public Grid(Vector2Int vector)
    {
        _x = vector.x;
        _y = vector.y;
        
        Instance = this;

        _cells = new bool[_x][];
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
    }
    
    private void DestroyCubes()
    {
        if (_cubes == null)
            return;
        
        for (int x = 0; x < _cubes.Length; x++)
        {
            for (int y = 0; y < _cubes[x].Length; y++)
            {
                GameObject.Destroy(_cubes[x][y]);
            }
        }
    }
    
    private void GenerateCubes()
    {
        _cubes = new GameObject[_x][];

        for (int x = 0; x < _cells.Length; x++)
        {
            _cubes[x] = new GameObject[_y];
            for (int y = 0; y < _cells[x].Length; y++)
            {
                //Generate GameObject and keep it in the array
                GameObject newCell = GameObject.CreatePrimitive(PrimitiveType.Cube);
                _cubes[x][y] = newCell;
                
                //Set cube's position based on his place in the arrays
                newCell.transform.position = new Vector3(x, y, 0);

                //Set cube's color based on current cell
                if (_cells[x][y])
                {
                    newCell.GetComponent<MeshRenderer>().material.color = Color.white;
                }
                else
                {
                    newCell.GetComponent<MeshRenderer>().material.color = Color.black;
                }
            }
        }
    }

    public void CalculateNextStates()
    {
        bool[][] nextGrid = new bool[_x][];
        for (int x = 0; x < _cells.Length; x++)
        {
            nextGrid[x] = new bool[_y];
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
                } else {
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
                if (cellY + y < 0 || cellY + y >= _cells[cellX + x].Length || (y==0 && x==0))
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
        for (int x = 0; x < _x; x++)
        {
            for (int y = 0; y < _y; y++)
            {
                if (_cells[x][y])
                    _cubes[x][y].GetComponent<MeshRenderer>().material.color = Color.white;
                else
                    _cubes[x][y].GetComponent<MeshRenderer>().material.color = Color.black;
            }
        }
    }

    public void InverseCell(int x, int y)
    {
        if (x < 0 || y < 0 || x >= _x || y >= _y) return;

        _cells[x][y] = !_cells[x][y];
        
        if (_cells[x][y])
            _cubes[x][y].GetComponent<MeshRenderer>().material.color =  Color.white;
        else
            _cubes[x][y].GetComponent<MeshRenderer>().material.color =  Color.black;
    }
}
