using UnityEngine;

public class MyceliumController : MonoBehaviour
{
    private float spreadProgress = 0f;
    private float spreadThreshold;

    private void Start()
    {
        spreadThreshold = GameManager.Instance.levelManager.GetMyceliumThreshold();
    }

    private void OnEnable()
    {
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
            GameManager.Instance.OnMyceliumComplete();
    }

    private void UpdateVisuals()
    {
        Debug.Log("Mycelium Spreading: " + spreadProgress);
    }
}
