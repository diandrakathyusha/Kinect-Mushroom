using UnityEngine;
using System.Collections;

public class LevelManager : MonoBehaviour
{
    public GameObject[] lands;
    public Camera mainCamera;
    private int currentLevel = 0;
    public LevelData[] levelDatas;  // Attach ScriptableObjects here


    public void InitializeLevels()
    {
        for (int i = 0; i < lands.Length; i++)
        {
            lands[i].SetActive(false);
            // Assign the LevelData to each land
            lands[i].GetComponent<Land>().levelData = levelDatas[i];
        }
        lands[currentLevel].SetActive(true);
    }


    public IEnumerator LoopLevels()
    {
        while (true)
        {
            yield return new WaitForSeconds(2f); // Wait for player input
            MyceliumPhase();
            yield return new WaitForSeconds(4f); // Adjust timing
            MushroomPhase();
            yield return new WaitForSeconds(4f);
            SporeDispersalPhase();
            NextLevel();
        }
    }

    public float GetMyceliumThreshold()
    {
        return levelDatas[currentLevel].myceliumThreshold;
    }

    public int GetSporeCount()
    {
        return levelDatas[currentLevel].sporeCount;
    }

    private void MyceliumPhase()
    {
        mainCamera.transform.position = new Vector3(0, -5, -10);  // Underground view
    }

    private void MushroomPhase()
    {
        mainCamera.transform.position = new Vector3(0, 5, -10);   // Surface view
    }

    private void SporeDispersalPhase()
    {
        // Trigger spore dispersal
        GameManager.Instance.OnMushroomGrowthComplete();
    }

    private void NextLevel()
    {
        lands[currentLevel].SetActive(false);
        currentLevel = (currentLevel + 1) % lands.Length;
        lands[currentLevel].SetActive(true);
    }

    public void GoToSurface()
    {
        mainCamera.transform.position = new Vector3(0, 5, -10);
    }

    public void ReleaseSpores()
    {
        Debug.Log("Spores Released!");
    }
}
