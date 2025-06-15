using UnityEngine;

public class ClickParticleSpawner : MonoBehaviour
{
    public GameObject particlePrefab; // Assign your particle prefab in the Inspector
    private Camera cam;

    void Start()
    {
        cam = Camera.main;
    }

    void Update()
    {
        if (Input.GetMouseButtonDown(0)) // Left-click
        {
            Vector3 mousePos = cam.ScreenToWorldPoint(Input.mousePosition);
            mousePos.z = 0f; // Keep on 2D plane

            GameObject particle = Instantiate(particlePrefab, mousePos, Quaternion.identity);

            // Destroy after particle duration
            float lifetime = particle.GetComponent<ParticleSystem>().main.duration
                             + particle.GetComponent<ParticleSystem>().main.startLifetime.constantMax;

            Destroy(particle, lifetime);
        }
    }
}
