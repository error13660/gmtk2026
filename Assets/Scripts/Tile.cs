using UnityEngine;

/// <summary>
/// Handles the graphics and collisions and return to the pool of a tile
/// </summary>
[RequireComponent(typeof(MeshFilter))]
[RequireComponent(typeof(BoxCollider2D))]
public class Tile : MonoBehaviour
{
    [SerializeField] private Vector2Int pos;
    private GameObject extraDetail;
    private int tileId;
    private float mineTime = 0;

    private void Update()
    {
        //if player is too far -> return to pool
        if (Vector2Int.Distance(pos, Player.intPos) > TileManager.instance.spawnDistance * 1.2f)
        {
            TileManager.instance.DespawnTile(pos, this);
        }
        EvaluateMining();
    }

    private void EvaluateMining()
    {
        Vector2Int minePos = Player.mineIntPos;
        if (!Player.isMining) { mineTime = 0; return; }

        //The player is mining. Is it mining this tile?
        bool isInRange = IsInRange(minePos, 1);

        if (isInRange) mineTime += Time.deltaTime;
        if (mineTime > .5f)
        {
            TileManager.instance.MineTile(pos);
            TileManager.instance.DespawnTile(pos, this);
        }
    }

    public void SetMesh(Mesh mesh)
    {
        GetComponent<MeshFilter>().mesh = mesh;
    }

    public void SetExtraObject(GameObject eo)
    {
        extraDetail = Instantiate(eo, transform);
    }

    public void SetPos(Vector2Int pos)
    {
        transform.position = new Vector3(pos.x, pos.y * -1, 0);
        mineTime = 0;
        this.pos = pos;
    }

    public void SetTileId(int tileId) { this.tileId = tileId; }

    public void DestroyExtraDetail()
    {
        Destroy(extraDetail);
    }

    private bool IsInRange(Vector2Int minePos, int range)
    {
        if (Mathf.Abs(minePos.x - pos.x) > range) return false;
        if (Mathf.Abs(minePos.y - pos.y) > range) return false;
        return true;
    }
}
