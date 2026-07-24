using UnityEngine;

/// <summary>
/// Holds data that is needed to generate a Tile
/// </summary>
[CreateAssetMenu]
public class TileData : ScriptableObject
{
    public Mesh mesh;
    public GameObject extraDetail; //if the base mesh is not enough, ex. for animated components. an extra, not pooled object can be spawned
    public bool isRandom = true;
}
