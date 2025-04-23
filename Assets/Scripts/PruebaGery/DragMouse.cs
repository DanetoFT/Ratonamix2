using UnityEngine;
using System.Collections;
using System.Collections.Generic;


public class DragMouse : MonoBehaviour
{
    public float force;
    public LayerMask draggable;
    private Rigidbody2D Rb;
    private CheeseCatcher catcher;
    


    private void Start()
    {
        catcher = FindAnyObjectByType<CheeseCatcher>();// busca el scpirt y lo guarda
    }

    private void FixedUpdate()
    {
        if (Rb)
        {
            Vector3 mousePos = Camera.main.ScreenToWorldPoint(Input.mousePosition);//calcula la posicion del mouse en el mundo
            mousePos.z = 0;// no lo queremos es 2D
            Vector3 direction = (mousePos - Rb.transform.position);// calcula la direccion desde el onjeto hast el mouse
            float effectiveForce = catcher.IsProcessing(Rb.gameObject) ? force * catcher.dragResistance : force;//aplica un a fuerza basada en la sirrecion, fuerza y si chater esta ejecutandose.DragResintance es para hacer resisitencia en cuando lo cojes del queso pero todavia no funciona
            Rb.linearVelocity = direction * effectiveForce * Time.fixedDeltaTime;
        }
    }

    private void Update()
    {
        if (Input.GetMouseButtonDown(0)) //si haces click
        {
            Rb = GetRigidbodyFromMouseClick(); // guarmos el rigi
            if (Rb != null && catcher != null)
            {
                catcher.StopProcessingQueso(Rb.gameObject);//se detenie el proceso que este haciendo cheeschater
            }
        }
        if (Input.GetMouseButtonUp(0))//si se suelta el click
        {
            
            if (Rb != null)
            {
                Rb.linearVelocity = Vector2.zero; //en estas lineas le quita velocidad y rotacion
                Rb.angularVelocity = 0f;
                Rb.Sleep(); // Lo duerme entre comillas para que no se procese fisicamenet y hacer asi q siga moviendose
            }
            Rb = null;
        }
    }

    private Rigidbody2D GetRigidbodyFromMouseClick()
    {
        Vector2 clickPoint = Camera.main.ScreenToWorldPoint(Input.mousePosition);// tu clicl se convierte en un posicion 2d en el mundo
        RaycastHit2D hit = Physics2D.Raycast(clickPoint, Vector2.zero, Mathf.Infinity, draggable); //el raycast sirve para detectar si hay un colidder en la capa dragable y

        if (hit.collider != null) // si ecuntra o hit algo  devuelve el Rb
        {
            return hit.collider.GetComponent<Rigidbody2D>();
        }
        return null;
    }
}