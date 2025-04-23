using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class CheeseCatcher : MonoBehaviour
{
    public Transform cheeseHoldPoint;
    public Sprite[] cheeseSprites;
    public float spriteChangeDelay;
    public float particuleTime;
    public GameObject particuleSystem;
    public float dragResistance = 0.5f;

    Ratoncillo raton;

    private Dictionary<GameObject, Coroutine> activeCoroutines = new Dictionary<GameObject, Coroutine>();
    private Dictionary<GameObject, int> currentSpriteIndices = new Dictionary<GameObject, int>(); // Nuevo: Guarda el índice actual

    private void Start()
    {
        raton = FindFirstObjectByType<Ratoncillo>();
    }

    public bool IsProcessing (GameObject queso)
    {
        return activeCoroutines.ContainsKey(queso);
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        raton.canRotate = false;
        var queso = other.gameObject.GetComponent<Queso>();
        if (queso != null && !activeCoroutines.ContainsKey(other.gameObject))
        {
            // Obtener el último índice o empezar desde 0
            int startIndex = currentSpriteIndices.TryGetValue(other.gameObject, out int idx) ? idx : 0;

            Coroutine coroutine = StartCoroutine(AcopleQueso(other.gameObject, startIndex));
            activeCoroutines.Add(other.gameObject, coroutine);
        }
    }

    private void OnTriggerExit2D(Collider2D other)
    {
        raton.canRotate = true;
        var queso = other.gameObject.GetComponent<Queso>();
        if (queso != null && activeCoroutines.ContainsKey(other.gameObject))
        {
            StopProcessingQueso(other.gameObject);
        }
    }

    public void StopProcessingQueso(GameObject queso)
    {
        if (activeCoroutines.TryGetValue(queso, out Coroutine coroutine))
        {
            StopCoroutine(coroutine);
            activeCoroutines.Remove(queso);
            queso.transform.SetParent(null);

            // Destruir partículas
            foreach (Transform child in queso.transform)
            {
                if (child.gameObject.layer == LayerMask.NameToLayer("ParticleEffect"))
                {
                    Destroy(child.gameObject);
                }
            }
        }
    }

    IEnumerator AcopleQueso(GameObject queso, int startIndex)
    {
        Rigidbody2D rb = queso.GetComponent<Rigidbody2D>();
        float originalDrag = 0;
        if (rb != null)
        {
            rb.linearVelocity = Vector2.zero;
            rb.linearDamping = 50f;
        }

        //transform.position = queso.transform.position;
        //queso.transform.position = cheeseHoldPoint.position;
        //queso.transform.SetParent(cheeseHoldPoint);

        SpriteRenderer Sr = queso.GetComponent<SpriteRenderer>();
        if (Sr != null && cheeseSprites.Length > 0)
        {
            // Comenzar desde el último índice guardado
            for (int i = startIndex; i < cheeseSprites.Length; i++)
            {
                // Actualizar el índice actual
                currentSpriteIndices[queso] = i;
                Sr.sprite = cheeseSprites[i];

                GameObject particuleInstance = null;
                if (particuleSystem != null)
                {
                    Quaternion RotationParticule = Quaternion.Euler(-180f, 90f, 0f);
                    particuleInstance = Instantiate(
                        particuleSystem,
                        queso.transform.position,
                        RotationParticule,
                        queso.transform
                    );
                    particuleInstance.layer = LayerMask.NameToLayer("ParticleEffect");
                }

                yield return new WaitForSeconds(particuleTime);

                if (particuleInstance != null)
                {
                    Destroy(particuleInstance);
                }

                float remainingDelay = spriteChangeDelay - particuleTime;
                if (remainingDelay > 0) yield return new WaitForSeconds(remainingDelay);
            }

            // Al completar todos los sprites
            currentSpriteIndices.Remove(queso);
            Destroy(queso);
           
        }
        if (rb = null)
        {
            rb.linearDamping = originalDrag;
        }
        activeCoroutines.Remove(queso);
    }
}