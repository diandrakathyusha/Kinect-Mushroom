using UnityEngine;
using System.Collections;
using Cinemachine;

public class LevelManager : MonoBehaviour
{
    public GameObject[] lands;  // All land objectsl
    public float landMoveSpeed = 5f;  // Speed of moving lands
    [Header("UI Background Settings")]
    public RectTransform backgroundA;
    public RectTransform backgroundB;
    public float backgroundScrollSpeed = 100f;  // Scrolling speed for the background
    public float landWidth = 10f;  // Width of each land

    public CinemachineVirtualCamera cmUnderground;
    public CinemachineVirtualCamera cmSurface;
    public HandOverlayer handOverlayer;
    public LinePainter linePainter;
    public GameObject myceliumParticle;
    public MyceliumController myceliumController;
    public MushroomController mushroomController;
    public int totalCollectibles;  // Set the total required collectibles in the Inspector

    public bool underground;

    private int currentLevel = 0;
    public LevelData[] levelDatas;

    private Vector3 initialBackgroundPos;

    void Start()
    {
        InitializeLevels();
        StartCoroutine(LoopLevels());
    }

    void InitializeLevels()
    {
        landWidth = CalculateLandWidth(lands[0]);
        Debug.Log("Land Width: " + landWidth);
        for (int i = 0; i < lands.Length; i++)
        {
            lands[i].SetActive(true);
            lands[i].transform.position = new Vector3(i * landWidth, 0f, 0f);  // Arrange lands in a row
        }
    }

    public IEnumerator LoopLevels()
    {
        yield return new WaitForSeconds(5f);  // Initial delay before starting the first level
        GoToUnderground();
    }

    public float GetMyceliumThreshold() => levelDatas[currentLevel].myceliumThreshold;
    public int GetSporeCount() => levelDatas[currentLevel].sporeCount;


    public void CheckCollectionCompletion(int collectedCount)
    {
        if (collectedCount >= totalCollectibles)
        {
            GoToSurface();  // Transition back to the surface
        }
    }
    private void GoToUnderground()
    {
        myceliumParticle.SetActive(true);
        handOverlayer.gameObject.SetActive(true);
        cmUnderground.Priority = 10;
        cmSurface.Priority = 5;
        underground = true;
    }

    public void GoToSurface()
    {
        myceliumParticle.SetActive(false);
        linePainter.DeleteAllLines();
        handOverlayer.gameObject.SetActive(false);
        cmUnderground.Priority = 5;
        cmSurface.Priority = 10;
        underground = false;
    }

    // Function to determine land width
    private float CalculateLandWidth(GameObject land)
    {
        BoxCollider collider = land.GetComponent<BoxCollider>();
        if (collider != null)
        {
            return collider.bounds.size.x;
        }

        Renderer renderer = land.GetComponent<Renderer>();
        if (renderer != null)
        {
            return renderer.bounds.size.x;
        }

        Debug.LogWarning("Land width not found. Using default width.");
        return 10f;  // Fallback value
    }

    private void NextLevel()
    {
        StartCoroutine(TransitionToNextLevel());
    }

    private IEnumerator TransitionToNextLevel()
    {
        float transitionTime = 0f;
        float duration = 2f;

        while (transitionTime < duration)
        {
            transitionTime += Time.deltaTime;
            float moveAmount = landMoveSpeed * Time.deltaTime;

            // Move all lands to the left
            for (int i = 0; i < lands.Length; i++)
            {
                lands[i].transform.position -= new Vector3(moveAmount, 0f, 0f);
            }

            // Reposition the leftmost land to the far right
            RepositionLands();

            // Scroll the background
            ScrollBackground();

            yield return null;
        }

        // Move to the next level
        currentLevel = (currentLevel + 1) % lands.Length;
        StartCoroutine(LoopLevels());

    }

    private void RepositionLands()
    {
        float minX = float.MaxValue;
        float maxX = float.MinValue;
        GameObject leftmostLand = null;

        // Find the leftmost and rightmost lands
        foreach (GameObject land in lands)
        {
            float landX = land.transform.position.x;

            if (landX < minX)
            {
                minX = landX;
                leftmostLand = land;
            }

            if (landX > maxX)
            {
                maxX = landX;
            }
        }

        // If the leftmost land moves too far left, reposition it to the right
        if (leftmostLand != null && minX <= -landWidth)
        {
            leftmostLand.transform.position = new Vector3(maxX + landWidth, 0f, 0f);
        }
    }

    private void ScrollBackground()
    {
        float backgroundMoveAmount = backgroundScrollSpeed * Time.deltaTime;

        backgroundA.anchoredPosition -= new Vector2(backgroundMoveAmount, 0f);
        backgroundB.anchoredPosition -= new Vector2(backgroundMoveAmount, 0f);

        // Loop the background seamlessly
        if (backgroundA.anchoredPosition.x <= -backgroundA.rect.width)
        {
            backgroundA.anchoredPosition = new Vector2(backgroundB.anchoredPosition.x + backgroundB.rect.width, backgroundA.anchoredPosition.y);
        }

        if (backgroundB.anchoredPosition.x <= -backgroundB.rect.width)
        {
            backgroundB.anchoredPosition = new Vector2(backgroundA.anchoredPosition.x + backgroundA.rect.width, backgroundB.anchoredPosition.y);
        }
    }
    public void ReleaseSpores()
    {
        Debug.Log("Spores Released!");
        myceliumController.fullMycelium = false;
        mushroomController.fullSpores = false;
        NextLevel();
    }
}
