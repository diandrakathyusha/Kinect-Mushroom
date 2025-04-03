using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;

public class CollectiblesController : MonoBehaviour
{
    [Tooltip("Reference to the Hand Overlayer script.")]
    public HandOverlayer handOverlayer;

    [Tooltip("Text UI to display collected items count.")]
    public Text collectedItemsText;

    private int collectedCount = 0;
    private List<GameObject> collectibles;
    public MyceliumController myceliumController;

    private Dictionary<GameObject, Color> originalColors = new Dictionary<GameObject, Color>();
    private HashSet<GameObject> collectedItems = new HashSet<GameObject>(); // Track collected items

    void Start()
    {
        // Find all collectibles in the scene
        collectibles = new List<GameObject>(GameObject.FindGameObjectsWithTag("Collectible"));

        // Store original colors
        foreach (GameObject collectible in collectibles)
        {
            Image image = collectible.GetComponent<Image>();
            if (image != null)
            {
                originalColors[collectible] = image.color;
            }
        }

        UpdateCollectedText();
    }

    void Update()
    {
        if (handOverlayer == null) return;

        Vector3 cursorPos = handOverlayer.GetCursorPos();
        cursorPos = Camera.main.ViewportToScreenPoint(cursorPos);

        foreach (GameObject collectible in collectibles)
        {
            if (collectible == null || collectedItems.Contains(collectible)) continue; // Skip if already collected

            RectTransform rectTransform = collectible.GetComponent<RectTransform>();
            if (rectTransform != null && RectTransformUtility.RectangleContainsScreenPoint(rectTransform, cursorPos))
            {
                if (handOverlayer.GetLastHandEvent() == InteractionManager.HandEventType.Grip)
                {
                    CollectItem(collectible);
                    break;
                }
            }
        }
    }

    void CollectItem(GameObject item)
    {
        if (collectedItems.Contains(item)) return; // Prevent repeated collection

        collectedItems.Add(item); // Mark as collected

        // Change UI Image color to indicate collection
        Image image = item.GetComponent<Image>();
        if (image != null)
        {
            image.color = new Color(1f, image.color.g, image.color.b, 0.5f); // Make it semi-transparent
        }

        // Disable the collider to prevent re-selection
        Collider2D collider = item.GetComponent<Collider2D>();
        if (collider != null)
        {
            collider.enabled = false;
        }

        collectedCount++;
        UpdateCollectedText();
        myceliumController.SpreadMycelium();

        // Check if all collectibles are collected
        if (collectedCount >= collectibles.Count)
        {
            ResetCollectibles();
        }
    }

    void ResetCollectibles()
    {
        collectedCount = 0;
        collectedItems.Clear(); // Clear collected items tracking

        foreach (GameObject collectible in collectibles)
        {
            if (collectible == null) continue;

            // Restore original color
            Image image = collectible.GetComponent<Image>();
            if (image != null && originalColors.ContainsKey(collectible))
            {
                image.color = originalColors[collectible];
            }

            // Reactivate collider
            Collider2D collider = collectible.GetComponent<Collider2D>();
            if (collider != null)
            {
                collider.enabled = true;
            }
        }

        UpdateCollectedText();
    }

    void UpdateCollectedText()
    {
        if (collectedItemsText != null)
        {
            collectedItemsText.text = "Collected: " + collectedCount;
        }
    }
}
