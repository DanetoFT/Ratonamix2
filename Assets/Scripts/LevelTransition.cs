using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class LevelTransition : MonoBehaviour
{
    public Image blackScreen;
    public Transform raton;

    public Transform[] puntosDeNivel;
    public GameObject[] nivelesADesactivar;

    public float fadeDuration = 1f;
    private bool enTransicion = false;

    private int nivelActual = 0;

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (!enTransicion && other.gameObject.layer == LayerMask.NameToLayer("nextLevel"))
        {
            if (nivelActual < puntosDeNivel.Length)
            {
                StartCoroutine(TransicionCoroutine(nivelActual));
                nivelActual++; 
            }
            else
            {
                Debug.LogWarning("No hay más niveles definidos.");
            }
        }
    }

    IEnumerator TransicionCoroutine(int indice)
    {
        enTransicion = true;

        yield return StartCoroutine(FadeToBlack());

        raton.position = puntosDeNivel[indice].position;

        if (indice < nivelesADesactivar.Length && nivelesADesactivar[indice] != null)
        {
            nivelesADesactivar[indice].SetActive(false);
        }

        yield return new WaitForSeconds(0.2f);

        yield return StartCoroutine(FadeFromBlack());

        enTransicion = false;
    }
    IEnumerator FadeToBlack()
    {
        float tiempo = 0f;
        while (tiempo < fadeDuration)
        {
            tiempo += Time.deltaTime;
            float alpha = Mathf.Lerp(0, 1, tiempo / fadeDuration);
            blackScreen.color = new Color(0, 0, 0, alpha);
            yield return null;
        }
    }
    IEnumerator FadeFromBlack()
    {
        float tiempo = 0f;
        while (tiempo < fadeDuration)
        {
            tiempo += Time.deltaTime;
            float alpha = Mathf.Lerp(1, 0, tiempo / fadeDuration);
            blackScreen.color = new Color(0, 0, 0, alpha);
            yield return null;
        }
    }
}
