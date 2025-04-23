using UnityEngine;
using UnityEngine.UIElements;
using UnityEngine.AI;
using NavMeshPlus.Extensions;

public class Ratoncillo : MonoBehaviour
{
    [SerializeField] Transform target;

    private Transform mouseTransform;

    NavMeshAgent agent;

    private Rigidbody2D rb;

    RotateAgentSmoothly rotate;

    [SerializeField] GameObject ratoncet;

    public float rotationSpeed;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        agent = GetComponent<NavMeshAgent>();
        agent.updateRotation = false;
        agent.updateUpAxis = false;
        mouseTransform = ratoncet.transform;
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    private void FixedUpdate()
    {
        Rotation();
    }

    void Rotation()
    {
        agent.SetDestination(target.position);

        Vector3 rotate = (agent.steeringTarget - transform.position).normalized;

        Quaternion targetRotation = Quaternion.LookRotation(transform.forward, rotate);
        Quaternion rotation = Quaternion.RotateTowards(transform.rotation, targetRotation, rotationSpeed * Time.deltaTime);
        rb.MoveRotation(rotation);

        //float angle = Mathf.Atan2(rotate.y, rotate.x) * Mathf.Rad2Deg;
        //mouseTransform.eulerAngles = new Vector3(0, 0, angle -90);
    }
}
