using UnityEngine;

public class TrampaDardos : MonoBehaviour
{
    [Header("Detección")]
    public LayerMask playerLayer;

    [Header("Disparo")]
    public GameObject dartPrefab;
    public Transform firePoint;
    public Vector2 dartDirection = Vector2.right;
    public float dartSpeed = 10f;

    [Header("Cooldown")]
    public float fireRate = 1f;
    private float fireCooldown;

    private void OnTriggerStay2D(Collider2D other)
    {
        if (((1 << other.gameObject.layer) & playerLayer) != 0)
        {
            if (Time.time >= fireCooldown)
            {
                FireDart();
                fireCooldown = Time.time + fireRate;
            }
        }
    }

    void FireDart()
    {
        GameObject dart = Instantiate(dartPrefab, firePoint.position, Quaternion.identity);
        Rigidbody2D rb = dart.GetComponent<Rigidbody2D>();
        if (rb != null)
        {
            rb.linearVelocity = dartDirection.normalized * dartSpeed;
        }
    }
}
