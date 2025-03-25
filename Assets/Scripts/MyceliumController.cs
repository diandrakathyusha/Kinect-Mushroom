using UnityEngine;

public class MyceliumController : MonoBehaviour
{
    private float spreadProgress = 0f;
    private float spreadThreshold;
    public LevelManager levelManager;
    public bool fullMycelium = false;

    private void Start()
    {
        spreadThreshold = levelManager.GetMyceliumThreshold();
    }

    private void OnEnable()
    {
        if (!fullMycelium)
        MushroomGestureListener.OnWaveGesture += SpreadMycelium;
    }

    private void OnDisable()
    {
        MushroomGestureListener.OnWaveGesture -= SpreadMycelium;
    }

    private void SpreadMycelium()
    {
        spreadProgress += 0.1f;
        UpdateVisuals();

        if (spreadProgress >= spreadThreshold)
        {
            GameManager.Instance.OnMyceliumComplete();
            spreadProgress = 0;
            fullMycelium = true;
        }

    }

    private void UpdateVisuals()
    {
        Debug.Log("Mycelium Spreading: " + spreadProgress);
    }
}
