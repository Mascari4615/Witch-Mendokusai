using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StageManager : Singleton<StageManager>
{
    /*[SerializeField] private int adsasdasda = 1231245;
    [SerializeField] private Texture2D _light;

    [ContextMenu("Load")]
    public void Load()
    {
        LightmapData[] lightmaparray = LightmapSettings.lightmaps;
        LightmapData mapdata = new LightmapData();
        for (var i = 0; i < lightmaparray.Length; i++)
        {
            // mapdata.lightmapDir = _dir;
            mapdata.lightmapColor = _light;
            //   mapdata.shadowMask = _shadow;

            lightmaparray[i] = mapdata;
        }

        LightmapSettings.lightmaps = lightmaparray;
    }*/

    [SerializeField] private KeyCode[] asd;
    
    public void LoadStage(int stageId)
    {
    }
}