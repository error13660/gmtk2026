using UnityEngine;
using System.Collections.Generic;
using System.Runtime.CompilerServices;

[SelectionBase]
public class Player : MonoBehaviour
{
    public static Vector2Int intPos = Vector2Int.zero;
    public static Vector2Int mineIntPos = Vector2Int.zero;
    public static bool isMining = false;
    [SerializeField] private float moveSpeed = .1f;
    [SerializeField] private new Camera camera;
    [SerializeField] Transform mineMarker;

    void Update()
    {
        Vector2 direction = Vector2.zero;
        isMining = false;

        //move
        Ray ray = camera.ScreenPointToRay(Input.mousePosition);
        RaycastHit hit;
        if (Physics.Raycast(ray, out hit, 200, 1 << 6)
            && Input.GetKey(KeyCode.Mouse0))
        {
            isMining = true;
            Vector2 minePos = new Vector2(transform.position.x, transform.position.y * -1f) + direction + (Vector2.one * .5f);
            mineIntPos = new Vector2Int((int)minePos.x, (int)minePos.y);

            Vector2 hitPos = hit.point;
            Vector2 playerPos = transform.position;
            direction = (hitPos - playerPos).normalized;

            //move if possible
            if (!Physics2D.CircleCast(transform.position, .5f, direction, moveSpeed * Time.deltaTime, 1 << 7))
            {
                transform.position += (Vector3)(direction * moveSpeed * Time.deltaTime);
            }
        }


        //update player int position
        intPos = new Vector2Int((int)(transform.position.x + .5f), (int)(transform.position.y + .5f) * -1);

        mineMarker.position = new Vector3(mineIntPos.x, mineIntPos.y * -1, 0);
    }
}
