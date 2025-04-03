using UnityEngine;
using System.Collections;

public class JumpTrigger : MonoBehaviour
{
    [Tooltip("Spore Prefab to Spawn")]
    public GameObject sporePrefab;

    [Tooltip("Spawn Area Size")]
    public Vector3 spawnArea = new Vector3(1f, 0.5f, 1f);

    [Tooltip("Maximum spores per trigger")]
    public int maxSporeCount = 5;

    [Tooltip("Spore launch force")]
    public float sporeLaunchForce = 1f;

    private int currentSporeCount = 0;
    private GameObject GameManager;

    void Start()
    {
        // Find the GameObject by name (or tag, etc.)
        GameManager = GameObject.Find("GameManager"); // Replace "MyTargetObject" with the actual name
    }
    void OnTriggerEnter(Collider other)
    {
        // Only trigger if a hand enters
        if (other.CompareTag("Hand"))
        {
            // Start animation (if any)
            Animation animation = gameObject.GetComponent<Animation>();
            if (animation != null && !animation.isPlaying)
            {
                animation.Play();
            }

            // Play audio (if any)
            AudioSource audioSrc = gameObject.GetComponent<AudioSource>();
            if (audioSrc != null && !audioSrc.isPlaying)
            {
                audioSrc.Play();
            }

            // Spawn spores if limit is not reached
            if (currentSporeCount < maxSporeCount)
            {
                SpawnSpore();
                currentSporeCount++;
            }
        }
    }

    void SpawnSpore()
    {
        MushroomController mushroomController = GameManager.GetComponent<MushroomController>();
        mushroomController.ReleaseSpores();
        // Spawn at the mushroom's position
        Vector3 spawnPos = transform.position;

        // Create the spore
        GameObject newSpore = Instantiate(sporePrefab, spawnPos, Quaternion.identity);

        // Add random force to spread the spore
        Rigidbody rb = newSpore.GetComponent<Rigidbody>();
        if (rb != null)
        {
            Vector3 randomDirection = new Vector3(
                Random.Range(-1f, 1f),   // Spread left/right
                Random.Range(0.5f, 1f),  // Always move upward
                Random.Range(-1f, 1f)    // Spread forward/backward
            ).normalized;

            rb.AddForce(randomDirection * sporeLaunchForce, ForceMode.Impulse);
        }
    }
}
