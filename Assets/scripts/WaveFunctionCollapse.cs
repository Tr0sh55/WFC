using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WaveFunctionCollapse : MonoBehaviour
{
    [SerializeField] private GameObject[] tilePrefabs;
    [SerializeField] private GameObject fillerTilePrefab;
    [SerializeField] private int gridWidth = 10;
    [SerializeField] private int gridHeight = 10;
    [SerializeField] private Transform tileParent;
    [SerializeField] private float tilePadding = 0.1f;
    [SerializeField] private float collapseDelay = 0.1f;

    private Cell[,] grid;
    private List<Vector2Int> activeCells = new List<Vector2Int>();
    
    private Vector2Int? lastCollapsedCell = null;

    private class Cell
    {
        public bool collapsed = false;
        public List<int> possibleTiles;
    }

    void Start()
    {
        InitializeGrid();
        CollapseRandomCell();
        while (ProcessCells()) { }
    }
    
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Space))
        {
            ClearGrid();
            StartCoroutine(GenerateCoroutine());
        }
    }

    private void ClearGrid()
    {
        foreach (Transform child in tileParent)
        {
            Destroy(child.gameObject);
        }
        InitializeGrid();
    }

    IEnumerator GenerateCoroutine()
    {
        while (ProcessCells())
        {
            yield return new WaitForSeconds(collapseDelay);
        }
    }

    void InitializeGrid()
    {
        grid = new Cell[gridWidth, gridHeight];
        for (int x = 0; x < gridWidth; x++)
        {
            for (int y = 0; y < gridHeight; y++)
            {
                grid[x, y] = new Cell { possibleTiles = new List<int>() };
                for (int i = 0; i < tilePrefabs.Length; i++)
                {
                    grid[x, y].possibleTiles.Add(i);
                }
                activeCells.Add(new Vector2Int(x, y));
            }
        }
    }

    void CollapseRandomCell()
    {
        int randomIndex = Random.Range(0, activeCells.Count);
        Vector2Int cellPos = activeCells[randomIndex];
        CollapseCell(cellPos.x, cellPos.y);
    }

    bool ProcessCells()
    {
        if (activeCells.Count == 0) return false;

        Vector2Int? cellToCollapse = ChooseCellWithLeastPossibilities();
        if (cellToCollapse.HasValue)
        {
            CollapseCell(cellToCollapse.Value.x, cellToCollapse.Value.y);
            return true;
        }

        return false;
    }

     Vector2Int? ChooseCellWithLeastPossibilities()
    {
        if (lastCollapsedCell.HasValue)
        {
            // Prioritize neighbors of the last collapsed cell
            var neighbors = GetNeighbors(lastCollapsedCell.Value);
            Vector2Int? leastNeighbor = null;
            int minPossibilities = int.MaxValue;
            foreach (var neighbor in neighbors)
            {
                int possibilities = grid[neighbor.x, neighbor.y].possibleTiles.Count;
                if (!grid[neighbor.x, neighbor.y].collapsed && possibilities < minPossibilities)
                {
                    minPossibilities = possibilities;
                    leastNeighbor = neighbor;
                }
            }
            if (leastNeighbor.HasValue)
            {
                return leastNeighbor;
            }
        }

        // Fallback to global minimum if no suitable neighbor is found
        return ChooseGlobalCellWithLeastPossibilities();
    }

    Vector2Int? ChooseGlobalCellWithLeastPossibilities()
    {
        int minPossibilities = int.MaxValue;
        Vector2Int? chosenCell = null;
        foreach (var cell in activeCells)
        {
            int possibilities = grid[cell.x, cell.y].possibleTiles.Count;
            if (!grid[cell.x, cell.y].collapsed && possibilities < minPossibilities)
            {
                minPossibilities = possibilities;
                chosenCell = cell;
            }
        }
        return chosenCell;
    }

    List<Vector2Int> GetNeighbors(Vector2Int cell)
    {
        List<Vector2Int> neighbors = new List<Vector2Int>();
        Vector2Int[] directions = { new Vector2Int(0, 1), new Vector2Int(1, 0), new Vector2Int(0, -1), new Vector2Int(-1, 0) };
        foreach (var dir in directions)
        {
            Vector2Int neighborPos = cell + dir;
            if (neighborPos.x >= 0 && neighborPos.x < gridWidth && neighborPos.y >= 0 && neighborPos.y < gridHeight)
            {
                neighbors.Add(neighborPos);
            }
        }
        return neighbors;
    }

    void CollapseCell(int x, int y)
    {
        var cell = grid[x, y];
        if (cell.possibleTiles.Count == 0)
        {
            Debug.LogWarning($"No possible tiles for cell ({x}, {y}), using filler tile.");
            InstantiateTile(fillerTilePrefab, x, y);
            cell.collapsed = true;
            activeCells.Remove(new Vector2Int(x, y));
            return;
        }

        int selectedIndex = Random.Range(0, cell.possibleTiles.Count);
        int tileIndex = cell.possibleTiles[selectedIndex];
        grid[x, y].collapsed = true;
        activeCells.Remove(new Vector2Int(x, y));
        lastCollapsedCell = new Vector2Int(x, y);

        InstantiateTile(tilePrefabs[tileIndex], x, y);
        UpdateNeighborPossibilities(x, y, tileIndex);
    }

    void UpdateNeighborPossibilities(int x, int y, int tileIndex)
    {
        TileDirectionSetter selectedTileDirections = tilePrefabs[tileIndex].GetComponent<TileDirectionSetter>();
        Vector2Int[] directions = { new Vector2Int(0, 1), new Vector2Int(1, 0), new Vector2Int(0, -1), new Vector2Int(-1, 0) };
        bool[] selectedTileOpenings = { selectedTileDirections.openUp, selectedTileDirections.openRight, selectedTileDirections.openDown, selectedTileDirections.openLeft };

        for (int i = 0; i < directions.Length; i++)
        {
            Vector2Int neighborPos = new Vector2Int(x + directions[i].x, y + directions[i].y);
            if (neighborPos.x >= 0 && neighborPos.x < gridWidth && neighborPos.y >= 0 && neighborPos.y < gridHeight && !grid[neighborPos.x, neighborPos.y].collapsed)
            {
                var neighborCell = grid[neighborPos.x, neighborPos.y];
                List<int> incompatibleTiles = new List<int>();
                foreach (int possibleTileIndex in neighborCell.possibleTiles)
                {
                    TileDirectionSetter possibleTileDirections = tilePrefabs[possibleTileIndex].GetComponent<TileDirectionSetter>();
                    bool[] possibleTileOpenings = { possibleTileDirections.openUp, possibleTileDirections.openRight, possibleTileDirections.openDown, possibleTileDirections.openLeft };
                    if (!AreDirectionsCompatible(selectedTileOpenings[i], possibleTileOpenings[(i + 2) % 4]))
                    {
                        incompatibleTiles.Add(possibleTileIndex);
                    }
                }
                foreach (int incompatibleTileIndex in incompatibleTiles)
                {
                    neighborCell.possibleTiles.Remove(incompatibleTileIndex);
                }

                // Fallback to filler tile if no possibilities remain after update
                if (neighborCell.possibleTiles.Count == 0)
                {
                    Debug.LogWarning($"No possible tiles remain for neighbor cell ({neighborPos.x}, {neighborPos.y}), using filler tile.");
                    InstantiateTile(fillerTilePrefab, neighborPos.x, neighborPos.y);
                    grid[neighborPos.x, neighborPos.y].collapsed = true;
                    activeCells.Remove(neighborPos);
                }
            }
        }
    }

    bool AreDirectionsCompatible(bool selectedTileSide, bool neighborTileSide)
    {
        return selectedTileSide == neighborTileSide;
    }

    void InstantiateTile(GameObject tilePrefab, int x, int y)
    {
        Instantiate(tilePrefab, new Vector3(x * (1 + tilePadding), y * (1 + tilePadding), 0), Quaternion.identity, tileParent);
    }
}
