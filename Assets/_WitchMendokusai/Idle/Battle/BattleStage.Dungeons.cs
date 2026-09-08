using UnityEngine;
using WitchMendokusai.DomainSDK.Idle;

namespace WitchMendokusai.Idle
{
	public sealed partial class BattleStage
	{
		private DungeonCatalogSO dungeonCatalog;
		private DungeonSO dungeonLook;
		private int difficultyShown;
		private bool lookReady;

		public void SetDungeonCatalog(DungeonCatalogSO catalog) => dungeonCatalog = catalog;

		private void ApplyDungeonLook(IdleSnapshot snapshot)
		{
			DungeonSO wanted = snapshot.DungeonRun.Active
				? dungeonCatalog.DungeonOf(snapshot.DungeonRun.Kind) : null;
			int difficulty = snapshot.DungeonRun.Active ? snapshot.DungeonRun.Difficulty : 0;
			if (lookReady && wanted == dungeonLook && difficulty == difficultyShown) { return; }
			lookReady = true;
			dungeonLook = wanted;
			difficultyShown = difficulty;
			entities.SetFoeLook(wanted);
			groundRest = wanted != null
				? Color.Lerp(wanted.FloorColor, Color.black, Mathf.Clamp01(difficulty * presentationAsset.DungeonDifficultyShade))
				: presentationAsset.GroundColor;
			groundMaterial.color = groundRest;
			foreach (Transform prop in scenery) { BattleVisualFactory.Kill(prop.gameObject); }
			scenery.Clear();
			sceneryMeshes.Clear();
			sceneryShape = (Geometry.Shape)(-1);
			if (wanted == null || wanted.SceneryPrefabs.Count == 0)
			{
				BuildScenery();
				return;
			}

			for (int index = 0; index < presentationAsset.SceneryCount; index++)
			{
				GameObject prop = Instantiate(wanted.SceneryPrefabs[index % wanted.SceneryPrefabs.Count], worldRoot, false);
				float side = index % 2 == 0 ? 1f : -1f;
				prop.transform.localPosition = new Vector3(
					index * presentationAsset.ScenerySpacing + presentationAsset.SceneryStartX,
					0f, side * presentationAsset.SceneryLaneOffset);
				scenery.Add(prop.transform);
			}
		}
	}
}
