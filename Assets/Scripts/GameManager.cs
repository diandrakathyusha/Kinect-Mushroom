using UnityEngine;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance;
    public LevelManager levelManager;

    private void Awake()
    {
        if (Instance == null) Instance = this;
        else Destroy(gameObject);
    }

    private void Start()
    {
        levelManager.InitializeLevels();
        StartLevelLoop();
    }

    public void StartLevelLoop()
    {
        StartCoroutine(levelManager.LoopLevels());
    }

    public void OnMyceliumComplete()
    {
        levelManager.GoToSurface();
    }

    public void OnMushroomGrowthComplete()
    {
        levelManager.ReleaseSpores();
    }
}
