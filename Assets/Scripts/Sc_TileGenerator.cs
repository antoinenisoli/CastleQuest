using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.PlayerLoop;
using UnityEngine.SceneManagement;

public class Sc_TileGenerator : MonoBehaviour
{
    enum Direction
    {
        Horizontal,
        Vertical,
    }

    [SerializeField] List<GameObject> Tiles;
    [SerializeField] Vector2Int levelArray;
    GameObject[,] grid = new GameObject[5, 10];
    List<Sc_Tile> correctTiles = new List<Sc_Tile>();
    float distance;

    private void OnDrawGizmos()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireCube(transform.position + new Vector3(levelArray.x / 2, -levelArray.y / 2), (Vector2)levelArray);
    }

    private void Awake()
    {
        grid = new GameObject[levelArray.x, levelArray.y];
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
                grid[j, i].GetComponent<Sc_Tile>().coordinates = new Vector2(j, i);
                grid[j, i].name += " " + grid[j, i].GetComponent<Sc_Tile>().coordinates;
            }
        }
    }

    public Vector2 GetTileCoordinates(GameObject obj)
    {
        for (int i = 0; i < grid.GetLength(1); i++)
        {
            for (int j = 0; j < grid.GetLength(0); j++)
            {
                if (grid[j, i] == obj)
                {
                    return new Vector2(j, i);
                }
            }
        }

        return Vector2.zero;
    }

    public bool IsBlocked(Vector3 pos, Sc_Tile tile)
    {
        bool blocked = false;        
        return blocked;
    }

    public GameObject CreateTile(Vector3 newPos)
    {        
        bool done = false;
        while (!done)
        {
            GameObject newTile = Tiles[UnityEngine.Random.Range(0, Tiles.Count)];
            Sc_Tile toCompare = newTile.GetComponent<Sc_Tile>();

            if (!IsBlocked(newPos, toCompare))
            {
                done = true;
                return Instantiate(newTile, newPos, Quaternion.identity);
            }
            else
            {
                print("wrong");
            }
        }

        return null;
    }

    public void Swap(GameObject toSwap, Vector3 basePos)
    {
        GameObject obj = GetClosest(toSwap);
        if (obj == null)
            return;

        Vector2 objCoord = GetTileCoordinates(obj);
        Vector2 toSwapCoord = GetTileCoordinates(toSwap);

        grid[(int)objCoord.x, (int)objCoord.y] = toSwap;
        obj.GetComponent<Sc_Tile>().coordinates = toSwapCoord;

        grid[(int)toSwapCoord.x, (int)toSwapCoord.y] = obj;
        toSwap.GetComponent<Sc_Tile>().coordinates = objCoord;

        toSwap.transform.position = obj.transform.position;
        obj.transform.position = basePos;

        CheckDirection(toSwap.GetComponent<Sc_Tile>().coordinates, toSwap, true, Direction.Horizontal);
        CheckDirection(toSwap.GetComponent<Sc_Tile>().coordinates, toSwap, false, Direction.Horizontal);
        CheckDirection(toSwap.GetComponent<Sc_Tile>().coordinates, toSwap, true, Direction.Vertical);
        CheckDirection(toSwap.GetComponent<Sc_Tile>().coordinates, toSwap, false, Direction.Vertical);

        ClearGrid();
        StartCoroutine(ReplaceGrid());
    }

    void ClearGrid()
    {
        foreach (Sc_Tile tile in correctTiles)
        {
            Sc_Score.instance.ModifyScore(tile.scoreValue);
            Vector2 vec = GetTileCoordinates(tile.gameObject);
            Destroy(grid[(int)vec.x, (int)vec.y]);
        }

        correctTiles.Clear();
    }

    IEnumerator ReplaceGrid()
    {
        yield return new WaitForSeconds(0.5f);
        GenerateGrid(true);
    }

    void CheckDirection(Vector2 coords, GameObject toSwap, bool positive, Direction dir)
    {
        Sc_Tile checkedTile = null;
        List<Sc_Tile> tempValidTiles = new List<Sc_Tile>();

        for (int k = 0; k < 15; k++)
        {
            switch (dir)
            {
                case Direction.Horizontal:
                    if (positive)
                    {
                        if ((int)coords.x + k < grid.GetLength(0))
                        {
                            checkedTile = grid[(int)coords.x + k, (int)coords.y].GetComponent<Sc_Tile>();
                        }
                        else
                            break;
                    }
                    else
                    {
                        if ((int)coords.x - k > -1)
                        {
                            checkedTile = grid[(int)coords.x - k, (int)coords.y].GetComponent<Sc_Tile>();
                        }
                        else
                            break;
                    }

                    break;

                case Direction.Vertical:
                    if (positive)
                    {
                        if ((int)coords.y + k < grid.GetLength(1))
                        {
                            checkedTile = grid[(int)coords.x, (int)coords.y + k].GetComponent<Sc_Tile>();
                        }
                        else
                            break;
                    }
                    else
                    {
                        if ((int)coords.y - k > -1)
                        {
                            checkedTile = grid[(int)coords.x, (int)coords.y - k].GetComponent<Sc_Tile>();
                        }
                        else
                            break;
                    }

                    break;
            }

            if (checkedTile.spr.sprite == toSwap.GetComponent<Sc_Tile>().spr.sprite)
            {
                if (!tempValidTiles.Contains(checkedTile))
                    tempValidTiles.Add(checkedTile);
            }
            else
                break;
        }

        if (tempValidTiles.Count == 1 && tempValidTiles[0].coordinates == coords)
            tempValidTiles.RemoveAt(0);

        correctTiles.AddRange(tempValidTiles);
    }

    public GameObject GetClosest(GameObject objectToCheck)
    {
        float maxDist = Mathf.Infinity;
        GameObject closest = null;

        foreach (GameObject tile in grid)
        {
            distance = Vector3.Distance(objectToCheck.transform.position, tile.transform.position);

            if (distance < maxDist && objectToCheck.transform.position != tile.transform.position)
            {
                if (distance < 0.5f)
                {
                    Debug.DrawLine(objectToCheck.transform.position, tile.transform.position, Color.red);
                    maxDist = distance;
                    closest = tile;
                }
                else
                {
                    closest = objectToCheck;
                }
            }
        }

        return closest;
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.R))
        {
            SceneManager.LoadScene(0);
        }
    }
}
