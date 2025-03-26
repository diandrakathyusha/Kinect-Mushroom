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
    public bool underground;

    private int currentLevel = 0;
    public LevelData[] levelDatas;

    private Vector3 initialBackgroundPos;

    void Start()
    {
        InitializeLevels();
        StartCoroutine(LoopLevels());
    }

    public void InitializeLevels()
    {
        for (int i = 0; i < lands.Length; i++)
        {
            lands[i].SetActive(i == currentLevel);
        }
    }

    public IEnumerator LoopLevels()
    {
        yield return new WaitForSeconds(5f);  // Initial delay before starting the first level
        GoToUnderground();
    }

    public float GetMyceliumThreshold() => levelDatas[currentLevel].myceliumThreshold;
    public int GetSporeCount() => levelDatas[currentLevel].sporeCount;

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

    private void NextLevel()
    {
        StartCoroutine(TransitionToNextLevel());
    }

    private IEnumerator TransitionToNextLevel()
    {
        int nextLevel = (currentLevel + 1) % lands.Length;
        lands[nextLevel].SetActive(true);

        float transitionTime = 0f;
        float duration = 2f;

        // Initial positions for cyclic movement
        Vector3[] initialLandPositions = new Vector3[lands.Length];
        for (int i = 0; i < lands.Length; i++)
        {
            initialLandPositions[i] = lands[i].transform.position;
        }

        Vector2 initialBgAPos = backgroundA.anchoredPosition;
        Vector2 initialBgBPos = backgroundB.anchoredPosition;

        while (transitionTime < duration)
        {
            transitionTime += Time.deltaTime;

            // Move lands to the left
            float landMoveAmount = landMoveSpeed * Time.deltaTime;
            for (int i = 0; i < lands.Length; i++)
            {
                lands[i].transform.position -= new Vector3(landMoveAmount, 0f, 0f);

                // Loop lands cyclically
                if (lands[i].transform.position.x <= -landWidth)
                {
                    lands[i].transform.position += new Vector3(landWidth * lands.Length, 0f, 0f);
                }
            }

            // Scroll the background
            float backgroundMoveAmount = backgroundScrollSpeed * Time.deltaTime;

            backgroundA.anchoredPosition -= new Vector2(backgroundMoveAmount, 0f);
            backgroundB.anchoredPosition -= new Vector2(backgroundMoveAmount, 0f);

            // Loop the background for seamless effect
            if (backgroundA.anchoredPosition.x <= -backgroundA.rect.width)
            {
                backgroundA.anchoredPosition = new Vector2(backgroundB.anchoredPosition.x + backgroundB.rect.width, backgroundA.anchoredPosition.y);
            }

            if (backgroundB.anchoredPosition.x <= -backgroundB.rect.width)
            {
                backgroundB.anchoredPosition = new Vector2(backgroundA.anchoredPosition.x + backgroundA.rect.width, backgroundB.anchoredPosition.y);
            }

            yield return null;
        }

        // Deactivate the previous land after transition
        lands[currentLevel].SetActive(false);
        currentLevel = nextLevel;
        StartCoroutine(LoopLevels());

    }
    public void ReleaseSpores()
    {
        Debug.Log("Spores Released!");
        myceliumController.fullMycelium = false;
        mushroomController.fullSpores = false;
        NextLevel();
    }
}
