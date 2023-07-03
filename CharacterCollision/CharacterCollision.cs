using UnityEngine;

public class CharacterCollision: MonoBehaviour
{
    private Rigidbody rb;

    private void Awake()
    {
        rb = GetComponent<Rigidbody>();
    }

    private void OnCollisionEnter(Collision collision)
    {
        Debug.Log("Collision Detected");
        // Prevent the character from passing through colliders
        foreach (var contact in collision.contacts)
        {
            var normal = contact.normal;
            rb.position += normal * 0.01f; // Adjust the value as needed
        }
    }
}