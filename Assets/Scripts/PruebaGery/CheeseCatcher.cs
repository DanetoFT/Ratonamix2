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

    private Dictionary<GameObject, Coroutine> activeCoroutines = new Dictionary<GameObject, Coroutine>();// los diccionarios sirven como en este caso para poder saber si se esta procesando  y  asi poder cancelar la corrutina
    private Dictionary<GameObject, int> currentSpriteIndices = new Dictionary<GameObject, int>(); // sirve para poder guardar los cambios del queso y asi si se cancela y se saca poder guadra en el punto que se quedo y poder retomarlo
    private Dictionary<GameObject, float> originalDampings = new Dictionary<GameObject, float>();

    public bool IsProcessing (GameObject queso) // devuelve el queso si esta en proceso
    {
        return activeCoroutines.ContainsKey(queso);
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        var queso = other.gameObject.GetComponent<Queso>();
        if (queso != null && !activeCoroutines.ContainsKey(other.gameObject))//verifica so esta el queso
        {
            Rigidbody2D rb = other.GetComponent<Rigidbody2D>();
            if (rb != null)
            {
                if (!originalDampings.ContainsKey(other.gameObject))//guarda el valor original del drag o damping
                {
                    originalDampings[other.gameObject] = rb.linearDamping;
                }
                rb.linearDamping = 50f;//valor que de ralentizar
            }
            //empixa a inicira la corrutina de los spirtes
            int startIndex = currentSpriteIndices.TryGetValue(other.gameObject, out int idx) ? idx : 0;

            Coroutine coroutine = StartCoroutine(AcopleQueso(other.gameObject, startIndex));
            activeCoroutines.Add(other.gameObject, coroutine);
        }
        
      
    }

    private void OnTriggerExit2D(Collider2D other) // sirve para parpar el porceso cudo salga
    {
        var queso = other.gameObject.GetComponent<Queso>();
        if (queso != null && activeCoroutines.ContainsKey(other.gameObject))
        {
            StopProcessingQueso(other.gameObject); 
        }
    }

    public void StopProcessingQueso(GameObject queso)
    {
        if (activeCoroutines.TryGetValue(queso, out Coroutine coroutine)) //cancela todo el piorceso
        {
            StopCoroutine(coroutine);
            activeCoroutines.Remove(queso);
            queso.transform.SetParent(null);

            // destryte partículas
            foreach (Transform child in queso.transform)
            {
                if (child.gameObject.layer == LayerMask.NameToLayer("ParticleEffect"))
                {
                    Destroy(child.gameObject);
                }
            }
        }
        Rigidbody2D rb = queso.GetComponent<Rigidbody2D>();
        if (rb != null && originalDampings.ContainsKey(queso)) //vuelve el valor orignal del drag a 0
        {
            rb.linearDamping = originalDampings[queso];
            originalDampings.Remove(queso);//elimina el quso del diccionario
        }
    }

    IEnumerator AcopleQueso(GameObject queso, int startIndex)
    {
        Rigidbody2D rb = queso.GetComponent<Rigidbody2D>();
        if (rb != null)
        {
            if (!originalDampings.ContainsKey(queso))
            {
                originalDampings[queso] = rb.linearDamping;
            }
            rb.linearVelocity = Vector2.zero;
            rb.linearDamping = 50f;//srivve para ralentizar cuan
        }


        queso.transform.position = cheeseHoldPoint.position; //mueve a un punto x
        queso.transform.SetParent(cheeseHoldPoint);

        SpriteRenderer Sr = queso.GetComponent<SpriteRenderer>();
        if (Sr != null && cheeseSprites.Length > 0) //empieza el cambio de spirte paso a paso
        {
            
            for (int i = startIndex; i < cheeseSprites.Length; i++)
            {
                //pone un nuevo sprite
                currentSpriteIndices[queso] = i;
                Sr.sprite = cheeseSprites[i];

                GameObject particuleInstance = null; //iNstancia las particulas
                if (particuleSystem != null)
                {
                    Quaternion RotationParticule = Quaternion.Euler(-180f, 90f, 0f);
                    particuleInstance = Instantiate(particuleSystem,queso.transform.position,RotationParticule,queso.transform);
                    particuleInstance.layer = LayerMask.NameToLayer("ParticleEffect");
                }

                yield return new WaitForSeconds(particuleTime);// hace un timepo de espera

                if (particuleInstance != null)
                {
                    Destroy(particuleInstance);
                }

                float remainingDelay = spriteChangeDelay - particuleTime;
                if (remainingDelay > 0) yield return new WaitForSeconds(remainingDelay);
            }

          
            currentSpriteIndices.Remove(queso);
            if (originalDampings.ContainsKey(queso))
            {
                originalDampings.Remove(queso);
            }
            Destroy(queso);
            //lo destruye y lo saca del diccionario
           
        }
        
        activeCoroutines.Remove(queso);
    }
}