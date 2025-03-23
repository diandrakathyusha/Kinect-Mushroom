using UnityEngine;

public class MushroomController : MonoBehaviour
{
    private int sporeCount;
    private int releasedSpores = 0;

    private void Start()
    {
        sporeCount = GameManager.Instance.levelManager.GetSporeCount();
    }

    private void OnEnable()
    {
        MushroomGestureListener.OnTapGesture += ReleaseSpores;
    }

    private void OnDisable()
    {
        MushroomGestureListener.OnTapGesture -= ReleaseSpores;
    }

    private void ReleaseSpores()
    {
        releasedSpores++;
        Debug.Log("Spores Released: " + releasedSpores);

        if (releasedSpores >= sporeCount)
            GameManager.Instance.OnMushroomGrowthComplete();
    }
}
