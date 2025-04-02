using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;


public class UIClickDetection : MonoBehaviour
{
    public GraphicRaycaster raycaster;  // Assign your Canvas's GraphicRaycaster
    private PointerEventData pointerEventData;
    private EventSystem eventSystem;

    void Start()
    {
        eventSystem = EventSystem.current;
        if (raycaster == null)
        {
            raycaster = FindObjectOfType<GraphicRaycaster>();  // Auto-assign if not set
        }
    }

    void Update()
    {
        if (Input.GetMouseButtonDown(0))  // Detect left mouse click
        {
            DetectUIClick();
        }
    }

    void DetectUIClick()
    {
        pointerEventData = new PointerEventData(eventSystem);
        pointerEventData.position = Input.mousePosition;

        List<RaycastResult> results = new List<RaycastResult>();
        raycaster.Raycast(pointerEventData, results);

        foreach (RaycastResult result in results)
        {
            Debug.Log("Clicked on: " + result.gameObject.name);

            // If it's a collectible, trigger collection
            if (result.gameObject.CompareTag("Collectible"))
            {
                CollectItem(result.gameObject);
            }
        }
    }

    void CollectItem(GameObject collectible)
    {
        Debug.Log("Collected: " + collectible.name);
        Destroy(collectible); // Remove the collectible
    }
}
