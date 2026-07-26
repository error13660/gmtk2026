using UnityEngine;

public class PlayerModel : MonoBehaviour
{
    [SerializeField] private Transform drillHead;
    [SerializeField] private Transform leftLegs;
    [SerializeField] private Transform rightLegs;
    void Update()
    {
        drillHead.localRotation = Quaternion.Euler(0, Time.deltaTime * 100 * Player.Instance.audioEase, 0) * drillHead.localRotation;
        leftLegs.localRotation = Quaternion.Euler(0, 0, Mathf.Sin(Time.time * 10) * 15 * Player.Instance.audioEase);
        rightLegs.localRotation = Quaternion.Euler(0, 0, -Mathf.Sin(Time.time * 10) * 15 * Player.Instance.audioEase);
    }
}
