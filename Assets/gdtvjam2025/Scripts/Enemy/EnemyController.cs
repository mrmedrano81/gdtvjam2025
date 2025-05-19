using UnityEngine;
using UnityEngine.AI;

public class EnemyController : MonoBehaviour
{
    public Camera cam;

    public NavMeshAgent agent;

    private SimpleRtsCamera simpleRtsCamera;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        cam = FindFirstObjectByType<Camera>();  
        agent = GetComponent<NavMeshAgent>();
        simpleRtsCamera = FindFirstObjectByType<SimpleRtsCamera>();

        if (agent == null)
        {
            agent = GetComponentInChildren<NavMeshAgent>();
        }
    }

    // Update is called once per frame
    void Update()
    {
        if (simpleRtsCamera.RightMouseClicked())
        {
            Ray ray = cam.ScreenPointToRay(simpleRtsCamera.mousePosition);
            RaycastHit hit;

            if (Physics.Raycast(ray, out hit))
            {
                agent.SetDestination(hit.point);
            }
        }
    }

    private void OnCollisionEnter(Collision collision)
    {
        Debug.Log("Collision with: " + collision.gameObject.name);
    }
}
