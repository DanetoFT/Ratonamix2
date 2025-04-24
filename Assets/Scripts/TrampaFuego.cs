using UnityEngine;

public class TrampaFuego : MonoBehaviour
{
    [Header("Parámetros de tiempo")]
    public float fireOnDuration = 2f;
    public float fireOffDuration = 2f;

    [Header("Daño")]
    public int damageAmount = 1;
    public LayerMask playerLayer;

    [Header("Referencias")]
    public GameObject fireEffect;
    public Collider2D damageCollider;

    private bool isFireOn = false;
    private float timer = 0f;

    void Start()
    {
        SetFireState(false);
    }

    void Update()
    {
        timer -= Time.deltaTime;

        if (isFireOn && timer <= 0)
        {
            SetFireState(false);
            timer = fireOffDuration;
        }
        else if (!isFireOn && timer <= 0)
        {
            SetFireState(true);
            timer = fireOnDuration;
        }
    }

    void SetFireState(bool state)
    {
        isFireOn = state;
        if (fireEffect != null)
            fireEffect.SetActive(state);

        if (damageCollider != null)
            damageCollider.enabled = state;
    }

    public void TriggerEntered(Collider2D other)
    {
        Debug.Log("Algo tocó el fuego: " + other.name);

        if (!isFireOn) return;

        if (((1 << other.gameObject.layer) & playerLayer) != 0)
        {
            Debug.Log("[¡Jugador murió al tocar el fuego!");
        }
    }
}
