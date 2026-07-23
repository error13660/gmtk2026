using UnityEngine;

/// <summary>
/// Handles the graphics and collisions and return to the pool of a tile
/// </summary>
[RequireComponent(typeof(MeshFilter))]
[RequireComponent(typeof(BoxCollider2D))]
public class Tile : MonoBehaviour
{
    [SerializeField] private Vector2Int pos;

    private void Update()
    {
        //if player is too far -> return to pool
        if (Vector2Int.Distance(new Vector2Int((int)transform.position.x, (int)transform.position.y), Player.intPos) > TileManager.instance.spawnDistance * 1.2f)
        {
            TileManager.instance.DespawnTile(pos, this);
        }
    }

    public void SetMesh(Mesh mesh)
    {

    }

    public void SetExtraObject(GameObject eo)
    {

    }

    public void SetPos(Vector2Int pos)
    {

        this.pos = pos;
    }
}
