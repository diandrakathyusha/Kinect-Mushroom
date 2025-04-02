using UnityEngine;
using System.Collections;
using Cinemachine;

public class LevelManager : MonoBehaviour
{
    public GameObject[] lands;  // All land objects
    public float landMoveSpeed = 5f;

    [Header("UI Background Settings")]
    public RectTransform backgroundA;
    public RectTransform backgroundB;
    public float backgroundScrollSpeed = 100f;
    public float landWidth = 10f;

    public CinemachineVirtualCamera cmUnderground;
    public CinemachineVirtualCamera cmSurface;
    public HandOverlayer handOverlayer;
    public LinePainter linePainter;
    public GameObject myceliumParticle;
    public MyceliumController myceliumController;
    public MushroomController mushroomController;
    public int totalCollectibles;

    public bool underground;

    private int currentLevel = 0;

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
            lands[i].transform.position = new Vector3(i * landWidth, 0f, 0f);
        }
    }

    public IEnumerator LoopLevels()
    {
        yield return new WaitForSeconds(5f);
        GoToUnderground();
    }

    // Fetch mycelium threshold from the active land's LevelData
    public float GetMyceliumThreshold()
    {
        Land currentLand = lands[currentLevel].GetComponent<Land>();
        return currentLand != null && currentLand.levelData != null ? currentLand.levelData.myceliumThreshold : 0f;
    }

    // Fetch spore count from the active land's LevelData
    public int GetSporeCount()
    {
        Land currentLand = lands[currentLevel].GetComponent<Land>();
        return currentLand != null && currentLand.levelData != null ? currentLand.levelData.sporeCount : 0;
    }

    public void CheckCollectionCompletion(int collectedCount)
    {
        if (collectedCount >= totalCollectibles)
        {
            GoToSurface();
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

    private float CalculateLandWidth(GameObject land)
    {
        BoxCollider collider = land.GetComponent<BoxCollider>();
        if (collider != null) return collider.bounds.size.x;

        Renderer renderer = land.GetComponent<Renderer>();
        if (renderer != null) return renderer.bounds.size.x;

        Debug.LogWarning("Land width not found. Using default width.");
        return 10f;
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

            foreach (GameObject land in lands)
            {
                land.transform.position -= new Vector3(moveAmount, 0f, 0f);
            }

            RepositionLands();
            ScrollBackground();

            yield return null;
        }

        currentLevel = (currentLevel + 1) % lands.Length;
        StartCoroutine(LoopLevels());
    }

    private void RepositionLands()
    {
        float minX = float.MaxValue;
        float maxX = float.MinValue;
        GameObject leftmostLand = null;

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
