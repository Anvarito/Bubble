using UnityEngine;
using UnityEngine.UI;

public class Game : MonoBehaviour
{
    // Start is called before the first frame update
    [SerializeField] private Bubble BubblePrefab;
    [SerializeField] private Text time;
    [SerializeField] private Text currentScore;

    [SerializeField] private GameObject retryPanel;
    [SerializeField] private Text totalScore;


    private Bubble bubble;

    public float GameTime = 30;

    private float currentTime = 0;
    private float speedKoeff = 0;


    private int scorePoint = 0;
    private bool isTimeOut;

    private void Start()
    {
        GameStart();
    }

    public void GameStart()
    {
        retryPanel.SetActive(false);
        scorePoint = 0;
        currentScore.text = "0";
        time.text = "0";
        currentTime = 0;
        speedKoeff = 0;
        isTimeOut = false;
        //InvokeRepeating("Spawn", 0.2f, 0.2f);
    }

    [ContextMenu("Spawn")]
    public void Spawn()
    {
        bubble = Instantiate(BubblePrefab);
        bubble.SpeedModByTime(speedKoeff);
        BoxCollider boxCollider = bubble.GetBoxCollider();

        Vector3 localToWorldSize = new Vector3(boxCollider.bounds.size.x * bubble.transform.localScale.x,
                                                boxCollider.bounds.size.y * bubble.transform.localScale.y);

        Bounds bubbleBounds = new Bounds(boxCollider.bounds.center, localToWorldSize);
        Vector2 leftCorner = new Vector2(bubbleBounds.min.x, bubbleBounds.max.y / 2);
        Vector2 rightCorner = new Vector2(bubbleBounds.max.x, bubbleBounds.max.y / 2);
        float halfWidht = (rightCorner - leftCorner).x / 2;

        Camera cam = Camera.main;
        float h = cam.orthographicSize;
        float w = cam.aspect * h;
        Vector2 screenLeftBottom = new Vector2(-w, -h) + (Vector2)cam.transform.position;
        Vector2 screenRightBottom = new Vector2(w, -h) + (Vector2)cam.transform.position;

        var spawnAreaX = Random.Range(screenLeftBottom.x + halfWidht, screenRightBottom.x - halfWidht);
        var spawnAreaY = screenLeftBottom.y - halfWidht;
        bubble.transform.position = new Vector2(spawnAreaX, spawnAreaY);
    }
    float spawnTimer = 0;
    void Update()
    {
        if (!isTimeOut)
        {
            GameInput();
            TimerCounting();

            if (spawnTimer >= 0.2f)
            {
                Spawn();
                spawnTimer = 0;
            }
            else
            {
                spawnTimer += Time.deltaTime;
            }
        }
    }

    private void TimerCounting()
    {
        currentTime += Time.deltaTime;
        time.text = Mathf.Round(currentTime).ToString();
        speedKoeff = Mathf.Clamp01(currentTime / GameTime);

        if (currentTime >= GameTime)
        {
            isTimeOut = true;
            retryPanel.SetActive(true);
            totalScore.text = "Total: " + currentScore.text;
            CancelInvoke();
        }
    }

    private void GameInput()
    {
        foreach (var touch in Input.touches)
        {
            if (touch.phase == TouchPhase.Began)
            {
                RaycastHit hit;
                Ray ray = Camera.main.ScreenPointToRay(touch.position);
                if (Physics.Raycast(ray, out hit, float.MaxValue))
                {
                    Bubble hitBubble;
                    if (hit.transform.TryGetComponent(out hitBubble))
                    {
                        scorePoint += hitBubble.GetPoints();
                        currentScore.text = scorePoint.ToString();
                        Destroy(hit.collider.gameObject);
                    }

                }
            }
        }
    }
}
