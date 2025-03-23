using UnityEngine;

[CreateAssetMenu(fileName = "LevelData", menuName = "MushroomGame/LevelData", order = 1)]
public class LevelData : ScriptableObject
{
    public string levelName;
    public float myceliumThreshold;
    public int sporeCount;
}
