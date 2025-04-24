using UnityEngine;
using System.Collections;
using System.Collections.Generic;


public class DragMouse : MonoBehaviour
{
    public float force;
    public LayerMask draggable;
    private Rigidbody2D Rb;
    private CheeseCatcher catcher;
    private Transform queso;
    [SerializeField] private Ratoncillo raton;
    


    private void Start()
    {
        catcher = FindAnyObjectByType<CheeseCatcher>();
        queso = FindAnyObjectByType<Queso>().transform;
    }

    private void FixedUpdate()
    {
        if (Rb)
        {
            Vector3 mousePos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
            mousePos.z = 0;
            Vector3 direction = (mousePos - Rb.transform.position);
            float effectiveForce = catcher.IsProcessing(Rb.gameObject) ? force * catcher.dragResistance : force;
            Rb.linearVelocity = direction * effectiveForce * Time.fixedDeltaTime;
        }
    }

    private void Update()
    {
        if (Input.GetMouseButtonDown(0))
        {
            Rb = GetRigidbodyFromMouseClick();
            if (Rb != null && catcher != null)
            {
                catcher.StopProcessingQueso(Rb.gameObject);
                raton.canRotate = true;
                raton.target = queso;
            }
        }
        if (Input.GetMouseButtonUp(0))
        {
            // Congela la posición al soltar
            if (Rb != null)
            {
                Rb.linearVelocity = Vector2.zero;
                Rb.angularVelocity = 0f;
                Rb.Sleep(); // Opcional: Detiene cálculos físicos
            }
            Rb = null;
        }
    }

    private Rigidbody2D GetRigidbodyFromMouseClick()
    {
        Vector2 clickPoint = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        RaycastHit2D hit = Physics2D.Raycast(clickPoint, Vector2.zero, Mathf.Infinity, draggable);

        if (hit.collider != null)
        {
            return hit.collider.GetComponent<Rigidbody2D>();
        }
        return null;
    }
}