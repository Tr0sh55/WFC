using System.Collections.Generic;
using UnityEngine;
using Random = UnityEngine.Random;


namespace DefaultNamespace
{
    public class Cell
    {
        public int x;
        public int y;
        public bool collapsed;
        public List<GameObject> possibleTiles;
        
        public Cell()
        {
            collapsed = false;
            possibleTiles = new List<GameObject>();
        }

        public Cell(int x, int y, bool collapsed, List<GameObject> possibleTiles)
        {
            this.x = x;
            this.y = y;
            this.collapsed = collapsed;
            this.possibleTiles = possibleTiles;
        }
        
        public Cell(int x, int y, bool collapsed)
        {
            this.x = x;
            this.y = y;
            this.collapsed = collapsed;
        }
        
        
        public Cell(int x, int y)
        {
            this.x = x;
            this.y = y;
        }

        public int Px
        {
            get => x;
            set => x = value;
        }

        public int Py
        {
            get => y;
            set => y = value;
        }

        public bool Collapsed
        {
            get => collapsed;
            set => collapsed = value;
        }

        public List<GameObject> PossibleTiles
        {
            get => possibleTiles;
            set => possibleTiles = value;
        }
        
        public void AddPossibleTile(GameObject tile)
        {
            possibleTiles.Add(tile);
        }
        
        public void RemovePossibleTile(GameObject tile)
        {
            possibleTiles.Remove(tile);
        }
        
        public void ClearPossibleTiles()
        {
            possibleTiles.Clear();
        }
        
        public bool HasPossibleTiles()
        {
            return possibleTiles.Count > 0;
        }

        public GameObject Collapse()
        {
            GameObject selectedTile = possibleTiles[Random.Range(0, possibleTiles.Count)];
            possibleTiles.Clear();
            collapsed = true;
            return selectedTile;
        }

        public void AddPossibleTiles(GameObject[] tilePrefabs)
        {
            //Debug.Log($"AddPossibleTiles called with {tilePrefabs.Length} prefabs.");

            if (possibleTiles == null)
            {
                //Debug.LogError("possibleTiles is null. Initializing.");
                possibleTiles = new List<GameObject>();
            }

            foreach (GameObject tile in tilePrefabs)
            {
                if (tile != null)
                {
                    possibleTiles.Add(tile);
                    //Debug.Log($"Added {tile.name} to possibleTiles.");
                }
                else
                {
                    Debug.LogWarning("Encountered a null tile prefab.");
                }
            }
        }

        public List<GameObject> GetCompatibleTiles(Cell neighbor, GameObject[] tilePrefabs)
        {
            List<GameObject> compatibleTiles = new List<GameObject>();
            foreach (GameObject tile in tilePrefabs)
            {
                TileDirectionSetter tileDirectionSetter = tile.GetComponent<TileDirectionSetter>();
                if (tileDirectionSetter.openUp && neighbor.y > y)
                {
                    compatibleTiles.Add(tile);
                }
                if (tileDirectionSetter.openRight && neighbor.x > x)
                {
                    compatibleTiles.Add(tile);
                }
                if (tileDirectionSetter.openDown && neighbor.y < y)
                {
                    compatibleTiles.Add(tile);
                }
                if (tileDirectionSetter.openLeft && neighbor.x < x)
                {
                    compatibleTiles.Add(tile);
                }
            }
            return compatibleTiles;
        }
    }
}