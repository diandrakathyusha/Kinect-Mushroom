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
