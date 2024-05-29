using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Mascari4615
{
	public class GridManager : MonoBehaviour
	{
		private InputManager InputManager => InputManager.Instance;

		[SerializeField]
		private Grid grid;

		[SerializeField]
		private GameObject gridVisualization;

		[SerializeField]
		private Animator marker;

		[SerializeField]
		private GameObject blockPrefab;

		private Vector3 targetCellPos;

		private GridData gridData;

		private void Start()
		{
			gridData = new();

			StopBuilding();
		}

		[ContextMenu(nameof(StartBuilding))]
		public void StartBuilding()
		{
			InputManager.OnClicked += ClickCell;
			gridVisualization.SetActive(true);
			marker.SetBool("ON", true);
		}

		[ContextMenu(nameof(StopBuilding))]
		public void StopBuilding()
		{
			InputManager.OnClicked -= ClickCell;
			gridVisualization.SetActive(false);
			marker.SetBool("ON", false);
		}

		private void Update()
		{
			UpdateCellPos();

			if (marker.transform.position != targetCellPos)
			{
				if (marker.GetBool("ON"))
				{
					marker.transform.position = targetCellPos;
					marker.SetTrigger("RESET");
				}
			}
		}

		private void UpdateCellPos()
		{
			Vector3 mousePosition = InputManager.MouseWorldPosition;
			Vector3Int gridPosition = grid.WorldToCell(mousePosition);
			Vector3 newTargetCellPos = grid.GetCellCenterWorld(gridPosition);
			newTargetCellPos.y = 0.01f;
			targetCellPos = newTargetCellPos;
		}

		private void ClickCell()
		{
			if (InputManager.IsPointerOverUI())
				return;

			if (gridData.TryGetObjectAt(grid.WorldToCell(targetCellPos), out GameObject obj))
			{
				gridData.RemoveObjectAt(grid.WorldToCell(targetCellPos));
				ObjectPoolManager.Instance.Despawn(obj);
				return;
			}
			else
			{
				GameObject block = ObjectPoolManager.Instance.Spawn(blockPrefab);
				block.transform.position = targetCellPos;
				block.SetActive(true);

				gridData.AddObjectAt(grid.WorldToCell(targetCellPos), block);
			}

			// buildingState.OnAction(gridPosition);
		}
	}
}