using System.Collections.Generic;
using UnityEngine;


/// <summary>
/// Holds all the kinds of tiles in the same pool.
/// When a tile is requested, it grabs an unused dormant one and prepares it to function like the requested tile.
/// </summary>
public class TilePool : MonoBehaviour
{
    [SerializeField] private TileData[] tileset; //links the tileId-s to the TIlePools that can spawn the tile models
    [SerializeField] private Tile tilePrefab; //the generic prefab of a tile
    private List<Tile> pool;

    private void Awake()
    {
        pool = new List<Tile>();
    }

    public void RequestTileAt(Vector2Int pos, int tileId)
    {
        //get tile
        Tile tile;

        if (pool.Count > 0)
        {
            tile = pool[0];
            pool.RemoveAt(0);
        }
        else
        {
            tile = Instantiate(tilePrefab);
        }

        //prepare tile
        tile.SetMesh(tileset[tileId].mesh);
        if (tileset[tileId].extraDetail != null) tile.SetExtraObject(tileset[tileId].extraDetail);
        tile.transform.position = new Vector3(pos.x,pos.y,0);
        tile.SetPos(pos);
        tile.gameObject.SetActive(true);
    }

    public void ReturnTile(Tile tile)
    {
        tile.gameObject.SetActive(false);
        pool.Add(tile);
    }
}
