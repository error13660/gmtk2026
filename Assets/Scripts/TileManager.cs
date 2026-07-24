using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
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

    private static Vector2Int[] OFFSETS =
        {
            new Vector2Int(-1,-1),
            new Vector2Int(0,-1),
            new Vector2Int(1,-1),
            new Vector2Int(-1,0),
            new Vector2Int(1,0),
            new Vector2Int(-1,1),
            new Vector2Int(0,1),
            new Vector2Int(1,1),
        };

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
                float height = pos.y + (int)(NoisePatterns.Instance.Pattern1(pos * 2) * 30f);

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

        //place clay veins

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

    private Vector2Int[] GetVeinBasePoints(int numberOfPoints)
    {
        var points = new Vector2Int[numberOfPoints];
        for (int i = 0; i < numberOfPoints; i++)
        {
            points[i] = new Vector2Int(
                ((mapWith / numberOfPoints) / 2) + ((mapWith / numberOfPoints) * i),
                0);
        }
        return points;
    }
    private Vector2Int[] GetNextVeinSpawnPoints(Vector2Int[] prev, Func<Vector2Int, Vector2Int> offsetFunc)
    {
        Vector2Int[] next = new Vector2Int[prev.Length];
        for (int i = 0; i < prev.Length; i++)
        {
            next[i] = offsetFunc.Invoke(prev[i]);
        }
        return next;
    }

    /// <summary>
    /// Generates a vein
    /// </summary>
    /// <param name="spawnPoint">The starting point of the vein generation</param>
    /// <param name="iterations">How many tiles to place</param>
    /// <param name="valueFunc">
    /// Vector2: direction from the spawn point
    /// int: number of neighboring vein tiles
    /// float: distance from spawn point
    /// returns: value (higher is better)
    /// </param>
    private void GenerateVein(Vector2Int spawnPoint, int iterations, int tileId, Func<Vector2, int, float, float> valueFunc)
    {
        List<Vector2Int> workingTiles = new List<Vector2Int>();
        void AddValidTiles(Vector2Int[] tiles)
        {
            for (int i = 0; i < tiles.Length; i++)
            {
                if (GetTileId(tiles[i]) == tileId) continue;
                if (workingTiles.Contains(tiles[i])) continue;
                workingTiles.Add(tiles[i]);
            }
        }

        SetTileId(spawnPoint, tileId);
        AddValidTiles(NeighboringTiles(spawnPoint));

        for (int i = 0; i < iterations; i++)
        {
            Vector2Int bestPos = workingTiles[0];
            float bestValue = valueFunc.Invoke(
                (Vector2)(workingTiles[0] - spawnPoint),
                NumberOfNeighboring(workingTiles[0], tileId),
                Vector2.Distance(spawnPoint, workingTiles[0]));

            //determine best tile placement position from working tiles
            for (int j = 1; j < workingTiles.Count; j++)
            {
                float value = valueFunc.Invoke(
                (Vector2)(workingTiles[i] - spawnPoint),
                NumberOfNeighboring(workingTiles[i], tileId),
                Vector2.Distance(spawnPoint, workingTiles[i]));
                if (value > bestValue) { bestValue = value; bestPos = workingTiles[i]; }
            }
            //place tile
            SetTileId(bestPos, tileId);
            AddValidTiles(NeighboringTiles(bestPos));
        }
    }

    /// <summary>
    /// Is 'pos' neighboring a tile with 'tileId' 
    /// diagonals allowed
    /// </summary>
    private bool IsNeighboring(Vector2Int pos, int tileId)
    {
        for (int i = 0; i < OFFSETS.Length; i++)
        {
            if (GetTileId(pos + OFFSETS[i]) == tileId) return true;
        }
        return false;
    }
    /// <summary>
    /// How many 'tileId' tiles are neighboring pos?
    /// </summary>
    private int NumberOfNeighboring(Vector2Int pos, int tileId)
    {
        int n = 0;
        for (int i = 0; i < OFFSETS.Length; i++)
        {
            if (GetTileId(pos + OFFSETS[i]) == tileId) n++;
        }
        return n;
    }

    private Vector2Int[] NeighboringTiles(Vector2Int pos)
    {
        Vector2Int[] tiles = new Vector2Int[OFFSETS.Length];
        for (int i = 0; i < OFFSETS.Length; i++)
        {
            tiles[i] = pos + OFFSETS[i];
        }
        return tiles;
    }
}
