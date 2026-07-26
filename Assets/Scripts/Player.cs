using UnityEngine;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using UnityEngine.SceneManagement;
using System.Collections;

[SelectionBase]
[RequireComponent(typeof(AudioSource))]
public class Player : MonoBehaviour
{
    public static Player Instance { get; private set; }

    public static Vector2Int intPos = Vector2Int.zero;
    public static Vector2 fPos = Vector2.zero;
    public static Vector2 mineFPos = Vector2.zero;
    public static bool isMining = false;
    public static float mineScale = 1;
    private Vector2 lookDirection = Vector2.zero;
    [SerializeField] private float moveSpeed = .1f;
    [SerializeField] private new Camera camera;
    [SerializeField] Transform mineMarker;
    [SerializeField] private AudioSource drillAudioSource;
    [SerializeField] private AudioSource musicAudioSource;
    private float musicVolume;
    private float audioEase = 0;
    private bool isControlLockout = false;
    [SerializeField] private GameObject augmentAquiredDisplay;

    public float timeRemaining = 60f;

    private void Awake()
    {
        Instance = this;

        drillAudioSource.volume = 0;
        musicVolume = musicAudioSource.volume;
    }

    private void Start()
    {
        augmentAquiredDisplay.SetActive(true);
    }

    void Update()
    {
        EaseAudio();
        SubtractTime();

        Vector2 direction = Vector2.zero;
        isMining = false;
        Vector2 newPos;

        if (isControlLockout) return;

        //move
        Ray ray = camera.ScreenPointToRay(Input.mousePosition);
        RaycastHit hit;
        if (Physics.Raycast(ray, out hit, 200, 1 << 6)
            && Input.GetKey(KeyCode.Mouse0))
        {
            isMining = true;
            Vector2 hitPos = hit.point;
            Vector2 playerPos = transform.position;
            direction = (hitPos - playerPos).normalized;
            lookDirection = direction;

            SetMineScale();
            newPos = transform.position;
            CollideAndSlide(direction * moveSpeed * Time.deltaTime * mineScale, 4);
            transform.position = newPos;
        }
        Vector2 minePos = new Vector2(transform.position.x, -(transform.position.y));
        mineFPos = minePos + (new Vector2(direction.x, -direction.y));


        //update player int and float position
        intPos = new Vector2Int((int)(transform.position.x + .5f), (int)(transform.position.y + .5f) * -1);
        fPos = new Vector2(transform.position.x, -transform.position.y);

        mineMarker.position = new Vector3(minePos.x, minePos.y * -1, 0) + (Vector3)direction;


        void CollideAndSlide(Vector2 travel, int depth)
        {
            if (depth <= 0) return;

            //move if possible
            RaycastHit2D hit;
            if (hit = Physics2D.CircleCast((Vector2)transform.position, .2f, travel.normalized, travel.magnitude, 1 << 7))
            {
                newPos += (travel.normalized * (hit.distance - .05f));
                Vector2 newDirection = Vector3.ProjectOnPlane(travel, hit.normal).normalized;
                CollideAndSlide(newDirection * (travel.magnitude - (hit.distance - .05f)), depth - 1);
                return;
            }
            else
            {
                newPos += travel;
                return;
            }
        }
    }

    private void SetMineScale()
    {
        mineScale = 1;
        if (Upgrades.isLevelFuelTank
            && lookDirection != Vector2.zero
            && Mathf.Acos(Mathf.Abs(Vector2.Dot(lookDirection, Vector2.right))) < 30f)
            mineScale *= 1.5f; //150% speed when level
        if (Upgrades.isStraightShooter
            && lookDirection != Vector2.zero
            && Mathf.Acos(Vector2.Dot(lookDirection, Vector2.down)) < 15f)
            mineScale *= 1.5f; //150% speed when going straight down
    }

    private void EaseAudio()
    {
        if (isMining) audioEase += .5f * Time.deltaTime;
        else audioEase -= .5f * Time.deltaTime;

        audioEase = Mathf.Clamp01(audioEase);
        drillAudioSource.volume = Mathf.SmoothStep(0, 1, audioEase);
        drillAudioSource.pitch = Mathf.SmoothStep(.7f, 1.1f, audioEase);
        musicAudioSource.volume = Mathf.SmoothStep(1, .7f, audioEase);
    }

    private void SubtractTime()
    {
        if (isControlLockout) return;

        timeRemaining -= Time.deltaTime;
        if (timeRemaining <= 0)
        {
            timeRemaining = 0;
            isControlLockout = true;
            StartCoroutine(ExitScene());
        }

        IEnumerator ExitScene()
        {
            float startTime = Time.time;
            while (Time.time - startTime < 5f)
            {
                float t = 1 - ((Time.time - startTime) / 5f);
                musicAudioSource.volume = musicVolume * t;
                yield return null;
            }
            SceneManager.LoadSceneAsync(0);
        }
    }
}
