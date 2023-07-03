using UnityEngine;
using UnityEngine.AI;

public class NavMeshCharacterController : MonoBehaviour
{
    [SerializeField] private Camera _camera;
    private NavMeshAgent _navMeshAgent;
    private bool isMoving;

    private void Start()
    {
        _navMeshAgent = GetComponent<NavMeshAgent>();
    }

    // Update is called once per frame
    private void Update()
    {
        if (Input.GetMouseButtonUp(0))
        {
            // Perform a raycast to the ground
            var ray = _camera.ScreenPointToRay(Input.mousePosition);
            RaycastHit hit;

            if (Physics.Raycast(ray, out hit))
            {
                // Set the destination for the NavMeshAgent
                _navMeshAgent.SetDestination(hit.point);
                isMoving = true;
            }
        }

        // Check if the character has reached the destination
        if (isMoving && !_navMeshAgent.pathPending && _navMeshAgent.remainingDistance <= _navMeshAgent.stoppingDistance)
            isMoving = false;
        // Perform any actions when the character reaches the destination
    }
}