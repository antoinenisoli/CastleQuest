using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using DG.Tweening;

public class Sc_TileGenerator : MonoBehaviour
{
    enum Direction
    {
        Horizontal,
        Vertical,
    }

    [SerializeField] Sprite[] allSprites;
    [SerializeField] GameObject tilePrefab;
    [SerializeField] Vector2Int levelArray;
    public GameObject highlightedTile;
    public GameObject[,] grid = new GameObject[5, 10];
    List<Sc_Tile> correctTiles = new List<Sc_Tile>();

    private void OnDrawGizmos()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireCube(transform.position + new Vector3(levelArray.x / 2, -levelArray.y / 2), (Vector2)levelArray);
    }

    private void Awake()
    {
        grid = new GameObject[levelArray.x, levelArray.y];
        GenerateGrid(false);
    }

    public void GenerateGrid(bool replace)
    {
        for (int i = 0; i < grid.GetLength(1); i++)
        {
            for (int j = 0; j < grid.GetLength(0); j++)
            {
                if (replace && grid[j, i] != null)
                    continue;

                grid[j, i] = CreateTile(transform.position + new Vector3(j, -i, 0));
                Sc_Tile tile = grid[j, i].GetComponent<Sc_Tile>();
                tile.coordinates = new Vector2Int(j, i);
                tile.name = tile.myType + " " + tile.coordinates;
                System.Array array = System.Enum.GetValues(typeof(TileType));

                if (j - 1 >= 0)
                {
                    while (tile.IsSameOf(grid[j - 1, i].GetComponent<Sc_Tile>()))
                    {
                        int random = Random.Range(0, allSprites.Length);
                        tile.myType = (TileType)array.GetValue(random);
                    }
                }

                if (i - 1 >= 0)
                {
                    while (tile.IsSameOf(grid[j, i - 1].GetComponent<Sc_Tile>()))
                    {
                        int random = Random.Range(0, allSprites.Length);
                        tile.myType = (TileType)array.GetValue(random);
                    }
                }

                tile.Creation(allSprites[(int)tile.myType], (int)tile.myType);
            }
        }
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
        int random = Random.Range(0, allSprites.Length);
        Sc_Tile tile = newTile.GetComponent<Sc_Tile>();
        tile.myType = (TileType)array.GetValue(random);
        return newTile;
    }

    public void Swap(Sc_Tile firstTile, Sc_Tile secondTile)
    {
        Vector2Int objCoord = GetTileCoordinates(firstTile.gameObject);
        Vector2Int toSwapCoord = GetTileCoordinates(secondTile.gameObject);

        Vector2 objPosition = firstTile.transform.position;
        Vector2 swapPosition = secondTile.transform.position;

        grid[objCoord.x, objCoord.y] = secondTile.gameObject;
        firstTile.coordinates = toSwapCoord;

        grid[toSwapCoord.x, toSwapCoord.y] = firstTile.gameObject;
        secondTile.coordinates = objCoord;

        float delay = 0.5f;
        secondTile.transform.DOMove(objPosition, delay);
        firstTile.transform.DOMove(swapPosition, delay);

        StartCoroutine(CheckLine(secondTile.coordinates, secondTile.gameObject, true, Direction.Horizontal, delay));

        ClearGrid();
        StartCoroutine(ReplaceGrid());
    }

    void ClearGrid()
    {
        foreach (Sc_Tile tile in correctTiles)
        {
            Sc_Score.instance.ModifyScore(tile.scoreValue);
            Vector2 vec = GetTileCoordinates(tile.gameObject);
            grid[(int)vec.x, (int)vec.y].GetComponent<Sc_Tile>().Death();
        }

        correctTiles.Clear();
    }

    IEnumerator ReplaceGrid()
    {
        yield return new WaitForSeconds(0.5f);
        GenerateGrid(true);
    }

    IEnumerator CheckLine(Vector2Int coords, GameObject toSwap, bool positive, Direction dir, float delay)
    {
        yield return new WaitForSeconds(delay);
        Sc_Tile checkedTile = null;
        List<Sc_Tile> tempValidTiles = new List<Sc_Tile>();

        for (int k = 0; k < 15; k++)
        {
            switch (dir)
            {
                case Direction.Horizontal:
                    if (positive)
                    {
                        if (coords.x + k < grid.GetLength(0))
                        {
                            checkedTile = grid[coords.x + k, coords.y].GetComponent<Sc_Tile>();
                        }
                        else
                            break;
                    }
                    else
                    {
                        if (coords.x - k > -1)
                        {
                            checkedTile = grid[coords.x - k, coords.y].GetComponent<Sc_Tile>();
                        }
                        else
                            break;
                    }

                    break;

                case Direction.Vertical:
                    if (positive)
                    {
                        if (coords.y + k < grid.GetLength(1))
                        {
                            checkedTile = grid[coords.x, coords.y + k].GetComponent<Sc_Tile>();
                        }
                        else
                            break;
                    }
                    else
                    {
                        if (coords.y - k > -1)
                        {
                            checkedTile = grid[coords.x, coords.y - k].GetComponent<Sc_Tile>();
                        }
                        else
                            break;
                    }

                    break;
            }

            if (checkedTile.IsSameOf(toSwap.GetComponent<Sc_Tile>()))
            {
                if (!tempValidTiles.Contains(checkedTile))
                {
                    tempValidTiles.Add(checkedTile);
                    print(checkedTile);
                }
            }
            else
                break;
        }

        if (tempValidTiles.Count == 1 && tempValidTiles[0].coordinates == coords)
            tempValidTiles.RemoveAt(0);

        correctTiles.AddRange(tempValidTiles);
    }

    public GameObject GetAdjacentCell(Vector2Int direction, Sc_Tile baseTile)
    {
        if (direction == Vector2Int.zero)
            return null;

        Vector2Int result = new Vector2Int(baseTile.coordinates.x + direction.x, baseTile.coordinates.y + direction.y);
        GameObject tile = null;
        if (result.x < grid.GetLength(0) && result.x >= 0)
            if (result.y < grid.GetLength(1) && result.y >= 0)
                tile = grid[baseTile.coordinates.x + direction.x, baseTile.coordinates.y + direction.y];

        return tile;
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.R))
        {
            SceneManager.LoadScene(0);
        }

        foreach (var item in grid)
        {
            Sc_Tile tile = item.GetComponent<Sc_Tile>();
            tile.highlight = item == highlightedTile;
        }
    }
}
