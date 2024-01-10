#if UNITY_EDITOR
using System.Collections.Generic;
using System.Linq;
using Blocks.Enum;
using Config;
using GridExtensions;
using UnityEngine;
using Random = UnityEngine.Random;

namespace EditorTools.LevelBuilder.Scripts
{
	[RequireComponent(typeof(Canvas))]
    public class LevelBuilder : MonoBehaviour
    {
	    public GameData gameData;
		public LevelConfig activeLevelConfig;
		public LevelBuilderBlock blockPrefab;
		[SerializeField] private RectTransform canvasRect;
		[SerializeField] private List<BlockId> randomizationBlockIdPool = new()
		{
			BlockId.Balloon, BlockId.Blue, BlockId.Green, BlockId.Duck, BlockId.Red,
			BlockId.RocketVertical, BlockId.RocketHorizontal, BlockId.Yellow, BlockId.Purple,
		};
		
		private bool _initialized;
		
		public void Init()
		{
			if (_initialized)
			{
				return;
			}
			
			_initialized = true;
			UpdateGrid(true);
		}
		
		//Expensive editor-only method, I'm using child and GetComponent in case of a user error,
		//The hierarchy might get manipulated by the user and cached list might get nullify,
		//It is safer to use Hierarchy as a list instead.
		public void UpdateGrid(bool initializeBlockIds = false, bool randomizeBlocks = false)
		{
			RegenerateGrid();
			var coordinates = activeLevelConfig.gridCoordinates;
			activeLevelConfig.blockPool = new List<BlockId>(randomizationBlockIdPool);
			
			for (var i = 0; i < coordinates.Count; i++)
			{
				LevelBuilderBlock childBlock;
				if (i < transform.childCount)
				{
					childBlock = transform.GetChild(i).GetComponent<LevelBuilderBlock>();
					childBlock.gameObject.SetActive(true);
				}
				else 
				{
					childBlock = Instantiate(blockPrefab, transform);
				}
				
				childBlock.Init(this);
				childBlock.OnCoordinatesChanged(coordinates[i]);

				if (randomizeBlocks)
				{
					coordinates[i].startBlockType = randomizationBlockIdPool[Random.Range(0, randomizationBlockIdPool.Count)];
					childBlock.Id = coordinates[i].startBlockType;
				}
				
				if (initializeBlockIds)
				{
					childBlock.Id = coordinates[i].startBlockType;
				}
			}
			
			for (var i = coordinates.Count; i < transform.childCount; i++)
			{
				var orphanChildBlock = transform.GetChild(i).GetComponent<LevelBuilderBlock>();
				orphanChildBlock.gameObject.SetActive(false);
				orphanChildBlock.Id = BlockId.None;
			}
		}

		private void RegenerateGrid()
		{
			var rowCount = activeLevelConfig.RowCount;
			var columnCount = activeLevelConfig.ColumnCount;

			var gridSize = GridExt.GetGridSize(canvasRect, gameData.GridWidthOffset, gameData.GridHeightOffset);
			var blockSize = GridExt.GetGridElementSize(gridSize, rowCount, columnCount);
			var gridStartPos = GridExt.GetGridStartPosition(blockSize, gridSize, rowCount, columnCount);
			var blockSizeVector = new Vector2(blockSize, blockSize);

			var updatedGridCoordinates = new List<Coordinate>();

			for (var row = 0; row < rowCount; row++)
			{
				for (var column = 0; column < columnCount; column++)
				{
					var gridPos = new Vector2Int(row, column);
					var existingCoordinate = activeLevelConfig.gridCoordinates.Find(cord => 
						cord.gridPosition == gridPos);
					
					var gridPosition = GridExt.GetPositionOnGrid(gridStartPos, blockSize, gridPos);
					
					if (existingCoordinate != null)
					{
						// Modify existing coordinate
						existingCoordinate.relativePosition = gridPosition;
						existingCoordinate.relativeSize = blockSizeVector;
						updatedGridCoordinates.Add(existingCoordinate);
					}
					else
					{
						// Create new coordinate
						var newCoordinate = new Coordinate(row, column, gridPosition, blockSizeVector, BlockId.None);
						updatedGridCoordinates.Add(newCoordinate);
					}
				}
			}

			// Remove orphan coordinates
			var expectedCount = rowCount * columnCount;
			if (activeLevelConfig.gridCoordinates.Count > expectedCount)
			{
				activeLevelConfig.gridCoordinates.RemoveRange(expectedCount, activeLevelConfig.gridCoordinates.Count - expectedCount);
			}

			activeLevelConfig.gridCoordinates = updatedGridCoordinates;
		}
		
		public void ClearGrid(bool keepCoordinates = true)
		{
			if (!keepCoordinates)
			{
				activeLevelConfig.gridCoordinates.Clear();
			}

			var children = (from Transform child in transform select child.gameObject).ToList();
			foreach (var child in children)
			{
				DestroyImmediate(child);
			}
		}
		
		public void ChangeBlockType(int index, BlockId id)
		{
			activeLevelConfig.gridCoordinates[index].startBlockType = id;
		}
	}
}
#endif