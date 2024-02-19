using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class TileConstraint
{
    public GameObject tile;
    public List<GameObject> allowedUpNeighbors;
    public List<GameObject> allowedRightNeighbors;
    public List<GameObject> allowedDownNeighbors;
    public List<GameObject> allowedLeftNeighbors;
}