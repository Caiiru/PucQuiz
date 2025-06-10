using UnityEngine;

public class CursorParticleBurst : MonoBehaviour
{
    public ParticleSystem particleSystemPrefab; // Assign in Inspector
    private ParticleSystem particleInstance;
    private Camera cam;

    void Start()
    {
        cam = Camera.main;

        // Instantiate and disable emission at start
        particleInstance = Instantiate(particleSystemPrefab);
        particleInstance.Stop(true, ParticleSystemStopBehavior.StopEmittingAndClear);
    }

    void Update()
    {
        // Move the particle system to follow the cursor
        Vector3 mousePos = cam.ScreenToWorldPoint(Input.mousePosition);
        mousePos.z = 0f;
        particleInstance.transform.position = mousePos;

        if (Input.GetMouseButton(0))
        {
            // Emit a burst manually
            particleInstance.Play();
        }
    }
}
