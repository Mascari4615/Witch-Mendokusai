using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StageManager : Singleton<StageManager>
{
    private const string NoCollision = "NO_CLLISION";
    private const string Unit = "Unit";

    [SerializeField] private GameObject homeStageObject;
    [SerializeField] private Stage homeStage;

    public StageObject CurStageObject{ get; private set; }
    public Stage LastStage { get; private set; }
    public Stage CurStage { get; private set; }

    private Vector3 lastPos;
    
    private void Start()
    {
        CurStageObject = homeStageObject.GetComponent<StageObject>();
        CurStage = homeStage;
    }

    public void LoadStage(Stage stage, int spawnPortalIndex)
    {
        StartCoroutine(LoadStage_(stage, spawnPortalIndex));
    }
    
    private IEnumerator LoadStage_(Stage stage, int spawnPortalIndex)
    {
        // SpawnPortalIndex가 -1이면, 포탈 타기 전 마지막 위치로 돌아간다는 뜻
        
        // https://gall.dcinside.com/mgallery/board/view/?id=game_dev&no=99368
        PlayerController.Instance.SetInteractionColliderLayer(LayerMask.NameToLayer(NoCollision));
        yield return new WaitForFixedUpdate();

        CurStageObject.gameObject.SetActive(false);
        LastStage = CurStage;

        lastPos = PlayerController.Instance.transform.position;

        GameObject targetStage = stage.Prefab.gameObject;
        CurStageObject = ObjectManager.Instance.PopObject(targetStage).GetComponent<StageObject>();

        Vector3 targetPortalPos =
            (spawnPortalIndex == -1) ? lastPos : stage.Prefab.Portals[spawnPortalIndex].TpPos.position;
        CurStageObject.transform.position = PlayerController.Instance.transform.position - targetPortalPos;
        CurStageObject.gameObject.SetActive(true);
        CurStage = stage;

        yield return new WaitForFixedUpdate();
        PlayerController.Instance.SetInteractionColliderLayer(LayerMask.NameToLayer(Unit));
    }
}