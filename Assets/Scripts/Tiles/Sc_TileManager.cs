using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using DG.Tweening;

public class Tile
{
    public GameObject prefab;
    public int probabilityRate = 20;
}

public class Sc_TileManager : MonoBehaviour
{
    [Header("Spawn Tiles")]
    [HideInInspector] public Sc_GameManager gameManager;
    [SerializeField] Vector2Int levelArray;
    [HideInInspector] public GameObject highlightedTile;
    Tile[] tiles;
    [SerializeField] GameObject[] allTiles = new GameObject[4];
    public GameObject[,] grid = new GameObject[5, 10];
    public bool canSwap = false;
    
    [Header("Spells")]
    [SerializeField] TileEffect_SpellIce iceSpell;
    Dictionary<SpellType, TileEffect_Spell> allSpellEffects = new Dictionary<SpellType, TileEffect_Spell>();

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
        allSpellEffects.Add(TileEffect_SpellIce.type, iceSpell);
        gameManager = FindObjectOfType<Sc_GameManager>();
        grid = new GameObject[levelArray.x, levelArray.y];
        Sc_EventManager.instance.onGameStart.AddListener(NewGame);
        Sc_EventManager.instance.onWin.AddListener(StopGame);
    }

    void NewGame()
    {
        StartCoroutine(GenerateGrid(false));
    }

    public void StopGame(bool b)
    {
        canSwap = false;
        Sc_EventManager.instance.onUpdateStats.Invoke();
        foreach (var item in grid)
        {
            item.GetComponent<Sc_Tile>().Death();
        }
    }

    public IEnumerator GenerateGrid(bool replace)
    {
        yield return new WaitForSeconds(1f);
        for (int i = 0; i < grid.GetLength(1); i++)
        {
            for (int j = 0; j < grid.GetLength(0); j++)
            {
                if (replace && grid[j, i] != null)
                    continue;

                grid[j, i] = CreateTile(j, i);
                if (j - 1 >= 0)
                {
                    while (true)
                    {
                        Sc_Tile adjacentTile = grid[j - 1, i].GetComponent<Sc_Tile>();
                        Sc_Tile currentTile = grid[j, i].GetComponent<Sc_Tile>();

                        if (currentTile.IsSameOf(adjacentTile))
                        {
                            Destroy(grid[j, i]);
                            grid[j, i] = CreateTile(j, i);
                        }
                        else
                            break;
                    }
                }

                if (i - 1 >= 0)
                {
                    while (true)
                    {
                        Sc_Tile adjacentTile = grid[j, i - 1].GetComponent<Sc_Tile>();
                        Sc_Tile currentTile = grid[j, i].GetComponent<Sc_Tile>();

                        if (currentTile.IsSameOf(adjacentTile))
                        {
                            Destroy(grid[j, i]);
                            grid[j, i] = CreateTile(j, i);
                        }
                        else
                            break;
                    }
                }

                Sc_Tile tile = grid[j, i].GetComponent<Sc_Tile>();
                tile.Creation();
                StartCoroutine(CheckThisTile(tile, 0.5f));
            }
        }

        canSwap = true;
        Sc_EventManager.instance.onUpdateStats.Invoke();
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

    public GameObject CreateTile(int j, int i)
    {
        int random = Random.Range(0, allTiles.Length);
        GameObject newTile = Instantiate(allTiles[random], transform.position + new Vector3(j, -i, 0), Quaternion.identity, transform);
        Sc_Tile tile = newTile.GetComponent<Sc_Tile>();
        tile.coordinates = new Vector2Int(j, i);
        tile.name = tile.ToString();
        return newTile;
    }

    public void Swap(Sc_Tile firstTile, Sc_Tile secondTile)
    {
        canSwap = false;
        Sc_EventManager.instance.onUpdateStats.Invoke();
        gameManager.ChangeAction(-1);
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

        StartCoroutine(CheckThisTile(firstTile, swapDuration));
        StartCoroutine(CheckThisTile(secondTile, swapDuration));
    }

    List<Sc_Tile> CheckLine(Sc_Tile startTile, Vector2Int direction)
    {
        Vector2Int lastCoord = new Vector2Int();
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
                    if (tile.IsSameOf(startTile))
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
                    if (tile.IsSameOf(startTile))
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

    IEnumerator CheckThisTile(Sc_Tile tileToCheck, float delay)
    {
        yield return new WaitForSeconds(delay);
        ClearLine(CheckLine(tileToCheck, Vector2Int.left));
        ClearLine(CheckLine(tileToCheck, Vector2Int.up));
    }

    void ClearLine(List<Sc_Tile> tiles)
    {
        foreach (var tile in tiles)
        {
            tile.Death();            
            SpellType spellType = tile.currentSpell;

            if (allSpellEffects.ContainsKey(spellType))
            {
                allSpellEffects[spellType].Effect(tiles);
                Sc_EventManager.instance.onUpdateStats.Invoke();
            }
        }

        for (int i = 2; i < tiles.Count; i++)
        {
            tiles[i].myTileEffect.Effect(tiles);            
            Sc_EventManager.instance.onUpdateStats.Invoke();
        }

        StartCoroutine(GenerateGrid(true));
    }

    public GameObject GetAdjacentCell(Vector2Int direction, Sc_Tile baseTile)
    {
        if (direction == Vector2Int.zero || !canSwap || !gameManager.canPlay)
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
        ManageTiles();
    }
}
