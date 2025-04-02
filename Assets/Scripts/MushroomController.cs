using UnityEngine;

public class MushroomController : MonoBehaviour
{
    private int sporeCount;
    private int releasedSpores = 0;
    public LevelManager levelManager;
    public bool fullSpores= false;

    private void Start()
    {
        sporeCount = levelManager.GetSporeCount();
    }

    private void OnEnable()
    {
        if (!fullSpores)
            MushroomGestureListener.OnTapGesture += ReleaseSpores;
    }

    private void OnDisable()
    {
        MushroomGestureListener.OnTapGesture -= ReleaseSpores;
    }

    private void ReleaseSpores()
    {
        releasedSpores += 1;
        Debug.Log("Spores Released: " + releasedSpores);

        if (releasedSpores >= sporeCount)
        {
            GameManager.Instance.OnMushroomGrowthComplete();
            releasedSpores = 0;
            fullSpores = true;
        }

    }
}
