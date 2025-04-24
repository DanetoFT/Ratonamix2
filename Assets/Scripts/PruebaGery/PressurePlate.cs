using UnityEngine;

public class PressurePlate : MonoBehaviour
{
    [Header("Configuración")]
    public LayerMask mouseLayer; 
    public Animator plateAnimator;
    public Animator doorAnimator;

    private void OnTriggerEnter2D(Collider2D other)
    {
        
        if ((mouseLayer.value & (1 << other.gameObject.layer)) != 0)// una movida q hay q hcaer para que detecte la capa del raton y lo chace con los index de los layers
        {
          
            plateAnimator.SetBool("Pressed", true);

            doorAnimator.SetBool("Pressed", true);
        }
    }

    private void OnTriggerExit2D(Collider2D other)
    {
        if ((mouseLayer.value & (1 << other.gameObject.layer)) != 0)
        {
           
            plateAnimator.SetBool("Pressed", false);
            doorAnimator.SetBool("Pressed", false);
        }
    }
}