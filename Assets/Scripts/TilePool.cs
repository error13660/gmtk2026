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
        int poolSize = (int)Mathf.Pow(
            (TileManager.instance.spawnDistance * 1.2f * 2), 2);

        pool = new List<Tile>(poolSize);
        for (int i = 0; i < poolSize; i++)
        {
            pool.Add(Instantiate(tilePrefab, transform));
        }
        Debug.Log("pool size created: " + poolSize);
    }

    public void RequestTileAt(Vector2Int pos, int tileId)
    {
        //get tile
        Tile tile;
        tile = pool[pool.Count - 1];
        pool.RemoveAt(pool.Count - 1);

        //prepare tile
        tile.SetMesh(tileset[tileId].mesh);
        if (tileset[tileId].extraDetail != null) tile.SetExtraObject(tileset[tileId].extraDetail, tileset[tileId].isRandom);
        tile.SetPos(pos);
        tile.SetTileId(tileId);
        tile.gameObject.SetActive(true);
    }

    public void ReturnTile(Tile tile)
    {
        tile.DestroyExtraDetail();
        tile.gameObject.SetActive(false);
        pool.Add(tile);
    }
}
