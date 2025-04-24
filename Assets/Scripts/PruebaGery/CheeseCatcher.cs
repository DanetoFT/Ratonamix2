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

    Ratoncillo raton;

    Ratoncillo raton;

    private Dictionary<GameObject, Coroutine> activeCoroutines = new Dictionary<GameObject, Coroutine>();
    private Dictionary<GameObject, int> currentSpriteIndices = new Dictionary<GameObject, int>();
    private Dictionary<GameObject, RigidbodyType2D> initialBodyTypes = new Dictionary<GameObject, RigidbodyType2D>();

<<<<<<< HEAD
    public Dictionary<GameObject, int> CurrentSpriteIndices => currentSpriteIndices;
=======
    private void Start()
    {
        raton = FindFirstObjectByType<Ratoncillo>();
    }

    private void Start()
    {
        raton = FindFirstObjectByType<Ratoncillo>();
    }

    public bool IsProcessing (GameObject queso)
    {
        return activeCoroutines.ContainsKey(queso);
    }
>>>>>>> 685dbc9 (Navmesh)

    private void OnTriggerEnter2D(Collider2D other)
    {
        raton.canRotate = false;
        var queso = other.gameObject.GetComponent<Queso>();
        if (queso != null && !activeCoroutines.ContainsKey(other.gameObject))
        {
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

            if (initialBodyTypes.TryGetValue(queso, out RigidbodyType2D initialType))
            {
                Rigidbody2D rb = queso.GetComponent<Rigidbody2D>();
                if (rb != null) rb.bodyType = initialType;
                initialBodyTypes.Remove(queso);
            }

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
        if (rb != null)
        {
            initialBodyTypes[queso] = rb.bodyType;
            rb.bodyType = RigidbodyType2D.Kinematic;
            rb.velocity = Vector2.zero;
        }

        //transform.position = queso.transform.position;
        //queso.transform.position = cheeseHoldPoint.position;
        //queso.transform.SetParent(cheeseHoldPoint);

        SpriteRenderer Sr = queso.GetComponent<SpriteRenderer>();
        if (Sr != null && cheeseSprites.Length > 0)
        {
            for (int i = startIndex; i < cheeseSprites.Length; i++)
            {
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

                if (particuleInstance != null) Destroy(particuleInstance);

                float remainingDelay = spriteChangeDelay - particuleTime;
                if (remainingDelay > 0) yield return new WaitForSeconds(remainingDelay);
            }

            if (rb != null && initialBodyTypes.ContainsKey(queso))
            {
                rb.bodyType = initialBodyTypes[queso];
                initialBodyTypes.Remove(queso);
            }
            Destroy(queso);
        }
        currentSpriteIndices.Remove(queso);
        activeCoroutines.Remove(queso);
    }
}