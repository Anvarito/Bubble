using UnityEngine;
using System.Collections;
using UnityEngine.EventSystems;

//[RequireComponent(typeof(BoxCollider2D))]
public class Bubble : MonoBehaviour
{
    [SerializeField] private SpriteRenderer spriteRenderer;
    [SerializeField] private BoxCollider boxCollider;
    [SerializeField] private float speedMin = 2; // 2 , 4
    [SerializeField] private float speedMax = 4; // 2 , 4
    [SerializeField] private float speedAcceleration = 2; // 2 , 4

    private float currentSpeed;

    [SerializeField] private float lifeTime = 3;

    [SerializeField] private float minScale = 0.5f;
    [SerializeField] private float maxScale = 1.2f;

    private int point = 1;
    public BoxCollider GetBoxCollider()
    {
        return boxCollider;
    }

    public void SpeedModByTime(float speedKoeff)
    {
        var acceleration = Mathf.Lerp(0, speedAcceleration, speedKoeff);
        currentSpeed += acceleration;
    }

    public int GetPoints()
    {
        return point;
    }

    private void Awake()
    {
        var randomScale = Random.Range(minScale, maxScale);
        transform.localScale = new Vector2(randomScale, randomScale);

        var speedKoeff = (maxScale - randomScale) / (maxScale - minScale);
        currentSpeed = Mathf.Lerp(speedMin, speedMax, speedKoeff);
        point = (int)Mathf.Lerp(1, 5, speedKoeff);

        spriteRenderer.color = Random.ColorHSV();

        StartCoroutine(DestroyBubble());
    }

    void LateUpdate()
    {
        transform.Translate(Vector2.up * currentSpeed * Time.deltaTime);
    }
    private IEnumerator DestroyBubble()
    {
        yield return new WaitForSeconds(lifeTime);
        Destroy(gameObject);
    }
}
