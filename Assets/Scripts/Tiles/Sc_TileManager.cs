using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using DG.Tweening;

public class Sc_TileManager : MonoBehaviour
{
    [HideInInspector] public Sc_GameManager gameManager;
    [SerializeField] GameObject tilePrefab;
    [SerializeField] Vector2Int levelArray;
    [HideInInspector] public GameObject highlightedTile;
    public GameObject[,] grid = new GameObject[5, 10];
    float offset;
    [SerializeField] bool canSwap = false;
    Dictionary<TileType, TileEffect> allEffects = new Dictionary<TileType, TileEffect>();

    [Header("Tile tweens")]
    [Range(0,1)] public float tileDeathDuration = 0.3f;
    [Range(0, 1)] public float tileBirthDuration = 0.3f;
    [SerializeField] float swapDuration = 0.2f;

    private void OnDrawGizmos()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireCube(transform.position + new Vector3(levelArray.x / 2, -levelArray.y / 2), (Vector2)levelArray);
    }

    private void Start()
    {
        allEffects.Add(TileEffect_Red.type, new TileEffect_Red());
        allEffects.Add(TileEffect_Blue.type, new TileEffect_Blue());
        allEffects.Add(TileEffect_Green.type, new TileEffect_Green());
        allEffects.Add(TileEffect_Yellow.type, new TileEffect_Yellow());

        gameManager = FindObjectOfType<Sc_GameManager>();
        grid = new GameObject[levelArray.x, levelArray.y];
        StartCoroutine(GenerateGrid(false, 0));
        Sc_EventManager.instance.onWin.AddListener(StopGame);
    }

    public void StopGame(bool b)
    {
        canSwap = false;
    }

    public IEnumerator GenerateGrid(bool replace, float delay)
    {
        yield return new WaitForSeconds(delay + 1f);
        for (int i = 0; i < grid.GetLength(1); i++)
        {
            for (int j = 0; j < grid.GetLength(0); j++)
            {
                if (replace && grid[j, i] != null)
                    continue;

                grid[j, i] = CreateTile(transform.position + new Vector3(j, -i, 0));
                Sc_Tile tile = grid[j, i].GetComponent<Sc_Tile>();
                tile.coordinates = new Vector2Int(j, i);                
                System.Array array = System.Enum.GetValues(typeof(TileType));

                if (j - 1 >= 0)
                {
                    while (tile.IsSameOf(grid[j - 1, i].GetComponent<Sc_Tile>()))
                    {
                        int random = Random.Range(0, array.Length);
                        tile.myType = (TileType)array.GetValue(random);
                    }
                }

                if (i - 1 >= 0)
                {
                    while (tile.IsSameOf(grid[j, i - 1].GetComponent<Sc_Tile>()))
                    {
                        int random = Random.Range(0, array.Length);
                        tile.myType = (TileType)array.GetValue(random);
                    }
                }

                tile.Creation((int)tile.myType);
                tile.name = tile.ToString();
            }
        }

        canSwap = true;
    }

    public Vector2Int GetTileCoordinates(GameObject obj)
    {
        for (int i = 0; i < grid.GetLength(1); i++)
        {
            for (int j = 0; j < grid.GetLength(0); j++)
            {
                if (grid[j, i] == obj)
                    return new Vector2Int(j, i);
            }
        }

        return Vector2Int.zero;
    }

    public GameObject CreateTile(Vector3 newPos)
    {
        GameObject newTile = Instantiate(tilePrefab, newPos, Quaternion.identity, transform);
        System.Array array = System.Enum.GetValues(typeof(TileType));
        int random = Random.Range(0, array.Length);
        Sc_Tile tile = newTile.GetComponent<Sc_Tile>();
        tile.myType = (TileType)array.GetValue(random);
        return newTile;
    }

    public void Swap(Sc_Tile firstTile, Sc_Tile secondTile)
    {
        canSwap = false;
        gameManager.RemoveAction();
        Vector2Int objCoord = GetTileCoordinates(firstTile.gameObject);
        Vector2Int toSwapCoord = GetTileCoordinates(secondTile.gameObject);

        Vector2 objPosition = firstTile.transform.position;
        Vector2 swapPosition = secondTile.transform.position;

        grid[objCoord.x, objCoord.y] = secondTile.gameObject;
        firstTile.coordinates = toSwapCoord;

        grid[toSwapCoord.x, toSwapCoord.y] = firstTile.gameObject;
        secondTile.coordinates = objCoord;

        secondTile.transform.DOMove(objPosition, swapDuration);
        firstTile.transform.DOMove(swapPosition, swapDuration);
        secondTile.gameObject.name = secondTile.ToString();
        firstTile.gameObject.name = firstTile.ToString();

        Sc_Tile[] array = new Sc_Tile[] { firstTile, secondTile };
        StartCoroutine(CheckThisTile(array, swapDuration));
    }

    List<Sc_Tile> CheckLine(Sc_Tile startTile, Vector2Int direction)
    {
        Vector2Int lastCoord = new Vector2Int();
        TileType tileType = startTile.myType;
        bool outB = false;
        List<Sc_Tile> tempValidTiles = new List<Sc_Tile>();
        for (int i = 0; i < grid.GetLength(0); i++)
        {
            for (int j = 0; j < grid.GetLength(1); j++)
            {
                Vector2Int computeCoordinates = startTile.coordinates + (direction * i);
                bool inBound = computeCoordinates.x >= 0 && computeCoordinates.x < grid.GetLength(0) && computeCoordinates.y >= 0 && computeCoordinates.y < grid.GetLength(1);
                if (inBound && grid[computeCoordinates.x, computeCoordinates.y] != null)
                {
                    Sc_Tile tile = grid[computeCoordinates.x, computeCoordinates.y].GetComponent<Sc_Tile>();
                    if (tile.myType.Equals(tileType))
                    {
                        lastCoord = computeCoordinates;
                    }
                    else
                    {
                        outB = true;
                        break;
                    }
                }
            }

            if (outB)
            {
                outB = false;
                break;
            }
        }

        for (int i = 0; i < grid.GetLength(0); i++)
        {
            for (int j = 0; j < grid.GetLength(1); j++)
            {
                Vector2Int computeCoordinates = lastCoord - (direction * i);
                bool inBound = computeCoordinates.x >= 0 && computeCoordinates.x < grid.GetLength(0) && computeCoordinates.y >= 0 && computeCoordinates.y < grid.GetLength(1);
                if (inBound && grid[computeCoordinates.x, computeCoordinates.y] != null)
                {
                    Sc_Tile tile = grid[computeCoordinates.x, computeCoordinates.y].GetComponent<Sc_Tile>();
                    if (tile.myType.Equals(tileType))
                    {
                        if (!tempValidTiles.Contains(tile))
                            tempValidTiles.Add(tile);

                        if (!tempValidTiles.Contains(startTile))
                            tempValidTiles.Add(startTile);
                    }
                    else
                    {
                        outB = true;
                        break;
                    }
                }
            }

            if (outB)
                break;
        }

        if (tempValidTiles.Count < 3)
            tempValidTiles.Clear();

        return tempValidTiles;
    }

    IEnumerator CheckThisTile(Sc_Tile[] tilesToCheck, float delay)
    {
        yield return new WaitForSeconds(delay);
        foreach (var tile in tilesToCheck)
        {
            StartCoroutine(ClearLine(CheckLine(tile, Vector2Int.left)));
            StartCoroutine(ClearLine(CheckLine(tile, Vector2Int.up)));
        }

        StartCoroutine(GenerateGrid(true, offset));
    }

    IEnumerator ClearLine(List<Sc_Tile> tiles)
    {
        TileType thisType = TileType.Attack;
        foreach (var tile in tiles)
        {
            thisType = tile.myType;
            yield return new WaitForSeconds(offset);
            tile.Death();
            offset += 0.05f;
        }

        for (int i = 2; i < tiles.Count; i++)
        {
            allEffects[thisType].Effect(tiles);
            print(thisType.ToString());
            Sc_EventManager.instance.onUpdateStats.Invoke();
        }

        offset = 0;
    }

    public GameObject GetAdjacentCell(Vector2Int direction, Sc_Tile baseTile)
    {
        if (direction == Vector2Int.zero || !canSwap)
            return null;

        Vector2Int result = new Vector2Int(baseTile.coordinates.x + direction.x, baseTile.coordinates.y + direction.y);
        GameObject tile = null;
        if (result.x < grid.GetLength(0) && result.x >= 0)
            if (result.y < grid.GetLength(1) && result.y >= 0)
                tile = grid[baseTile.coordinates.x + direction.x, baseTile.coordinates.y + direction.y];

        return tile;
    }

    void ManageTiles()
    {
        foreach (var item in grid)
        {
            if (item != null)
            {
                Sc_Tile tile = item.GetComponent<Sc_Tile>();
                tile.highlight = item == highlightedTile;
            }
        }
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.R))
            SceneManager.LoadScene(0);

        ManageTiles();
    }
}
