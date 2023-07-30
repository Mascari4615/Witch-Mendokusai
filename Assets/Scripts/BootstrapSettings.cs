using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = nameof(BootstrapSettings), menuName = "BootstrapSettings")]
public class BootstrapSettings : ScriptableObject
{
    [field: SerializeField] public DataManager DataManagerPrefab { get; private set; }
    [field: SerializeField] public AudioManager AudioManagerPrefab { get; private set; }
}

public static class Bootstrap
{
    [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.BeforeSceneLoad)]
    public static void OnBooting()
    {
        Debug.Log("SANS");

        var bootstrapStuff = Resources.Load<BootstrapSettings>(nameof(BootstrapSettings));
        if (bootstrapStuff == null)
        {
            Debug.LogError("BootStrapSettings not found");
            return;
        }

        Object.Instantiate(bootstrapStuff.DataManagerPrefab);
        Object.Instantiate(bootstrapStuff.AudioManagerPrefab);
    }
}

