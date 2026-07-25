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

    private void OnEnable()
    {
        TileManager.instance.MineBroadcast += OnMineBroadcast;
    }

    private void OnDisable()
    {
        TileManager.instance.MineBroadcast -= OnMineBroadcast;
    }

    void OnMineBroadcast(Vector2Int pos)
    {
        if (pos == this.pos)
        {
            TileManager.instance.MineTile(pos);
            TileManager.instance.DespawnTile(pos, this);
        }
    }

    private void EvaluateMining()
    {
        if (!Player.isMining) { mineTime = 0; return; }

        switch (tileId)
        {
            case 0:
                {
                    //The player is mining. Is it mining this tile?
                    bool isInRange = (IsInRange(Player.mineFPos, .99f));
                    if (isInRange) mineTime += Time.deltaTime;
                    if (mineTime > .5f)
                    {
                        TileManager.instance.MineTile(pos);
                        TileManager.instance.DespawnTile(pos, this);
                        Upgrades.Instance.OnDustMined();
                    }
                    return;
                }
            case 1:
                {
                    //The player is mining. Is it mining this tile?
                    bool isInRange = (IsInRange(Player.mineFPos, .99f));
                    if (isInRange) mineTime += Time.deltaTime;
                    if (mineTime > 2f)
                    {
                        TileManager.instance.MineTile(pos);
                        TileManager.instance.DespawnTile(pos, this);
                        Upgrades.Instance.OnStoneMined();
                    }
                    return;
                }

            case 2:
                {
                    //The player is mining. Is it mining this tile?
                    bool isInRange = (IsInRange(Player.mineFPos, .99f));
                    if (isInRange) mineTime += Time.deltaTime;
                    if (mineTime > .25f)
                    {
                        TileManager.instance.MineTile(pos);
                        TileManager.instance.DespawnTile(pos, this);
                        Upgrades.Instance.OnSiltMined();
                    }
                    return;
                }
            case 3:
                {
                    //The player is mining. Is it mining this tile?
                    bool isInRange = (IsInRange(Player.mineFPos, .99f));
                    if (isInRange) mineTime += Time.deltaTime;
                    if (mineTime > .25f)
                    {
                        TileManager.instance.MineTile(pos);
                        TileManager.instance.DespawnTile(pos, this);
                        Upgrades.Instance.OnQuartzMined(pos);
                    }
                    return;
                }
            case 4:
                {
                    //The player is mining. Is it mining this tile?
                    bool isInRange = (IsInRange(Player.mineFPos, .99f));
                    if (isInRange) mineTime += Time.deltaTime;
                    if (mineTime > .25f)
                    {
                        TileManager.instance.MineTile(pos);
                        TileManager.instance.DespawnTile(pos, this);
                        Upgrades.Instance.OnBasaltMined(pos);
                    }
                    return;
                }
            case 5:
                {
                    //The player is mining. Is it mining this tile?
                    bool isInRange = (IsInRange(Player.mineFPos, .99f));
                    if (isInRange) mineTime += Time.deltaTime;
                    if (mineTime > .25f)
                    {
                        TileManager.instance.MineTile(pos);
                        TileManager.instance.DespawnTile(pos, this);
                        Upgrades.Instance.OnClayMined();
                    }
                    return;
                }
            case 6:
                {
                    //The player is mining. Is it mining this tile?
                    bool isInRange = (IsInRange(Player.mineFPos, .99f));
                    if (isInRange) mineTime += Time.deltaTime;
                    if (mineTime > .25f)
                    {
                        TileManager.instance.MineTile(pos);
                        TileManager.instance.DespawnTile(pos, this);
                        Upgrades.Instance.OnGraniteMined();
                    }
                    return;
                }


            default:
                break;
        }
    }

    public void SetMesh(Mesh mesh)
    {
        GetComponent<MeshFilter>().mesh = mesh;
    }

    public void SetExtraObject(GameObject eo, bool isRandom)
    {
        if (UnityEngine.Random.Range(0, 5) > 1) return;
        extraDetail = Instantiate(eo, transform);
        extraDetail.transform.localPosition = Vector3.zero;
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

    private bool IsInRange(Vector2 minePos, float range)
    {
        if (Mathf.Abs(minePos.x - (float)pos.x) > range) return false;
        if (Mathf.Abs(minePos.y - (float)pos.y) > range) return false;
        return true;
    }
}
