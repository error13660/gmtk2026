using System;
using System.Collections.Generic;
using UnityEngine;

public class TileManager : MonoBehaviour
{
    public static TileManager instance;

    [SerializeField] private int mapWith;
    [SerializeField] private int mapHeight;
    private byte[,] tilesData; //stores the tile ids and their alive state 0000000/0 id/state
    [SerializeField] private int mapSeed;
    [SerializeField] private TilePool tilePool;
    private HashSet<Vector2Int> minedTiles;

    [SerializeField] public int spawnDistance;

    private void Awake()
    {
        instance = this;
        tilesData = new byte[mapWith, mapHeight];
        minedTiles = new HashSet<Vector2Int>();

        //generate map ---
        //place base materials
        /*
         * Each biome has it's unique base materials.
         * The base material is determined by the 'height' value that is the sum of the
         * true height and a fractal noise function.
         * Special biomes can also be placed when a second fractal noise function ('weirdness') reaches a
         * high enough or specific value. 'height' is also a factor in this case.
         */
        for (int i = 0; i < mapWith; i++)
        {
            for (int j = 0; j < mapHeight; j++)
            {
                Vector2Int pos = new Vector2Int(i, j);
                float height = pos.y + (int)(NoisePatterns.Instance.Pattern1(pos*2) * 30f);

                //tid 0
                if (height < 30) SetTileId(pos, 0);
                else SetTileId(pos, 1);
            }
        }


        //place veins
        /*
         * On top of the base materials of biomes, biome specific vein-like clumps of special materials can appear.
         * Thes can be siginificantly harder or easier to mine, making optimal traversal more exciting.
         */
    }

    void Start()
    {
        for (int i = 0; i < mapWith; i++)
        {
            MineTile(new Vector2Int(i, i));
        }
    }

    void Update()
    {
        //spawn tiles (despawn is handled by the tiles themselves)
        for (int i = 0; i < mapWith; i++)
        {
            int jStart = Mathf.Max(0, Player.intPos.y - spawnDistance);
            int jEnd = Mathf.Min(mapHeight, Player.intPos.y + spawnDistance);
            for (int j = jStart; j < jEnd; j++)
            {
                //check for distance
                Vector2Int tilePos = new Vector2Int(i, j);
                if (Vector2Int.Distance(tilePos, Player.intPos) < spawnDistance
                    && !IsTileAlive(tilePos) //check status
                    && !IsTileMined(tilePos)) //check mined status
                    SpawnTile(tilePos);
            }
        }
    }

    private void SpawnTile(Vector2Int pos)
    {
        int id = GetTileId(pos);
        tilePool.RequestTileAt(pos, id);
        SetTileStatus(pos, true);
    }

    public void DespawnTile(Vector2Int pos, Tile tile)
    {
        SetTileStatus(pos, false);
        tilePool.ReturnTile(tile);
    }

    private bool IsTileAlive(Vector2Int pos)
    {
        byte data = tilesData[pos.x, pos.y];
        int state = data & 1; //get last bit
        return state == 1;
    }

    private int GetTileId(Vector2Int pos)
    {
        byte data = tilesData[pos.x, pos.y];
        int id = data & ~1; //get all but last bit
        return id >> 1; //shift to correct place
    }

    private void SetTileId(Vector2Int pos, int id)
    {
        if (id > 128) { Debug.LogError("Invalid tile id"); return; }

        byte data = (byte)(id << 1);
        data = (byte)(data & ~1);
        tilesData[pos.x, pos.y] = data;
    }

    public void SetTileStatus(Vector2Int pos, bool isAlive)
    {
        tilesData[pos.x, pos.y] = (byte)(tilesData[pos.x, pos.y] & ~1); //remove current state

        if (isAlive)
        {
            tilesData[pos.x, pos.y] = (byte)(tilesData[pos.x, pos.y] | 1); //set status to alive
        }
    }

    public void MineTile(Vector2Int pos)
    {
        minedTiles.Add(pos);
    }

    private bool IsTileMined(Vector2Int pos)
    {
        return minedTiles.Contains(pos);
    }
}
