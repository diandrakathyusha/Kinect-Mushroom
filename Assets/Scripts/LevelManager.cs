using UnityEngine;
using System.Collections;
using Cinemachine;

public class LevelManager : MonoBehaviour
{
    public GameObject[] lands;
    public CinemachineVirtualCamera cmUnderground;
    public CinemachineVirtualCamera cmSurface;
    public HandOverlayer handOverlayer;
    public LinePainter linePainter;
    public GameObject myceliumParticle;
    public MyceliumController myceliumController;
    public MushroomController mushroomController;
    public bool underground;
    private int currentLevel = 0;
    public LevelData[] levelDatas;  // Attach ScriptableObjects here


    public void InitializeLevels()
    {
        myceliumParticle.SetActive(false);

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
        yield return new WaitForSeconds(5f);
        GoToUnderground();
        //while (true)
        //{
           // yield return new WaitForSeconds(5f); // Wait for player input
            //GoToUnderground();
            //yield return new WaitForSeconds(4f); // Adjust timing
            //GoToSurface();
            //yield return new WaitForSeconds(4f);
            //SporeDispersalPhase();
            //NextLevel();
        //}
    }

    public float GetMyceliumThreshold()
    {
        return levelDatas[currentLevel].myceliumThreshold;
    }

    public int GetSporeCount()
    {
        return levelDatas[currentLevel].sporeCount;
    }

    private void GoToUnderground()
    {
        myceliumParticle.SetActive(true);
        handOverlayer.gameObject.SetActive(true);
        cmUnderground.Priority = 10;
        cmSurface.Priority = 5; // Underground view
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

    public void SmoothTransition(bool isUnderground)
    {
        if (isUnderground) GoToUnderground();
        else GoToSurface();
    }

    private void NextLevel()
    {
        //put smooth transition instead of whatever this is
        lands[currentLevel].SetActive(false);
        currentLevel = (currentLevel + 1) % lands.Length;
        lands[currentLevel].SetActive(true);
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
