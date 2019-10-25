using System;
using UnityEngine;


[Serializable]
public struct GameConfig
{
    public float gameAreaWidth;
    public float gameAreaHeight;

    public float unitSpawnDelay;
    public int   numUnitsToSpawn;

    public float minUnitRadius;
    public float maxUnitRadius;

    public float minUnitSpeed;
    public float maxUnitSpeed;
}


[Serializable]
public struct GlobalConfig
{
    public GameConfig GameConfig;


    public static GlobalConfig Load(string assetName)
    {
        TextAsset textAsset = Resources.Load(assetName) as TextAsset;
        return JsonUtility.FromJson<GlobalConfig>(textAsset.text);
    }
}
