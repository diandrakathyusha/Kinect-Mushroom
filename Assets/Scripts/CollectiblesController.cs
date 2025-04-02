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

    void Start()
    {
        // Find all collectibles in the scene
        collectibles = new List<GameObject>(GameObject.FindGameObjectsWithTag("Collectible"));
        UpdateCollectedText();
    }

    void Update()
    {
        if (handOverlayer == null) return;

        Vector3 cursorPos = handOverlayer.GetCursorPos();
        cursorPos = Camera.main.ViewportToScreenPoint(cursorPos);

        foreach (GameObject collectible in collectibles)
        {
            if (collectible == null) continue;

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
        collectibles.Remove(item);
        myceliumController.SpreadMycelium();
        Destroy(item);
        collectedCount++;
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
