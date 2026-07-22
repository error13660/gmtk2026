using UnityEngine;

public class TileManager : MonoBehaviour
{
    [SerializeField] private int mapWith;
    [SerializeField] private int mapHeight;
    private byte[,] tileIds;
    [SerializeField] private int mapSeed;
    [SerializeField] private TilePool[] tileSet; //links the tileId-s to the TIlePools that can spawn the tile models

    private void Awake()
    {
        tileIds = new byte[mapWith, mapHeight];
    }

    void Start()
    {
        
    }

    void Update()
    {
        
    }
}
