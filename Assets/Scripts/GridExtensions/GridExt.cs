using System.Collections.Generic;
using System.Linq;
using Blocks;
using Blocks.Enum;
using Blocks.Interface;
using Config;
using UnityEngine;

namespace GridExtensions
{
    public static class GridExt 
    {
        private static readonly Vector2Int[] AdjacentDirections = { Vector2Int.right, Vector2Int.left, Vector2Int.up, Vector2Int.down };
        
        public static Vector2 GetGridStartPosition(float gridElementSize, Vector2 gridSize, int rowCount, int columnCount)
        {
            var halfBlockSize = gridElementSize / 2f;
            var halfGridSize = gridSize / 2;
			
            var startX = -halfGridSize.x + halfBlockSize;
            var startY = halfGridSize.y - halfBlockSize;
			
            var totalHeight = rowCount * gridElementSize;
            var totalWidth = columnCount * gridElementSize;

            // centering the grid to the canvas
            startX += (gridSize.x - totalWidth) / 2f;
            startY -= (gridSize.y - totalHeight) / 2f;
            return new Vector2(startX, startY);
        }
        
        public static Vector2 GetPositionOnGrid(Vector2 gridStartPosition, float gridElementSize, Vector2Int gridPosition)
        {
            var row = gridPosition.x;
            var column = gridPosition.y;
            
            var xPos = gridStartPosition.x + column * gridElementSize;
            var yPos = gridStartPosition.y - row * gridElementSize;
			
            return new Vector2(xPos, yPos);
        }
        
        public static Vector2 GetGridSize(RectTransform gridCanvasRect, float width, float height)
        {
            var rect = gridCanvasRect.rect;
            var canvasWidth = rect.width;
            var canvasHeight = rect.height;
			
            var widthPercentage = Mathf.Clamp01(width / 100f);
            var heightPercentage = Mathf.Clamp01(height / 100f);
			
            var widthOffsetRatio = canvasWidth * widthPercentage;
            var heightOffsetRatio = canvasHeight * heightPercentage;

            var widthOffset = canvasWidth - widthOffsetRatio;
            var heightOffset = canvasHeight - heightOffsetRatio;
            return new Vector2(widthOffset, heightOffset);
        }
        
        public static float GetGridElementSize(Vector2 gridSize, int rowCount, int columnCount)
        {
            var rowSize = gridSize.y / rowCount;
            var colSize = gridSize.x / columnCount;
			
            return Mathf.Min(rowSize, colSize);
        }

        public static Coordinate GetCoordinateAtGridPosition(Vector2Int gridPosition, IEnumerable<Coordinate> coordinates)
        {
            return coordinates.FirstOrDefault(t => t.gridPosition == gridPosition);
        }
        
        public static List<Coordinate> GetCoordinatesAtColumn(int targetColumn, IEnumerable<Coordinate> coordinates)
        {
            return coordinates.Where(coordinate => coordinate.gridPosition.x == targetColumn).ToList();
        }
        
        public static List<Coordinate> GetCoordinatesAtRow(int targetRow, IEnumerable<Coordinate> coordinates)
        {
            return coordinates.Where(coordinate => coordinate.gridPosition.y == targetRow).ToList();
        }
        
        public static IEnumerable<Coordinate> GetLinkedCoordinates(Coordinate centerCoordinate, IEnumerable<Coordinate> allCoordinates)
        {
            var  coordinatePositionResults = new HashSet<Vector2Int>();
            var  coordinateResults = new List<Coordinate>();
            
            var allCoordinatesList = allCoordinates.ToList();
            GetLinkedCoordinatesRecursive(centerCoordinate, allCoordinatesList, coordinatePositionResults, coordinateResults);
            return coordinateResults;
        }

        private static void GetLinkedCoordinatesRecursive(Coordinate currentCoordinate, IReadOnlyCollection<Coordinate> allCoordinates, ISet<Vector2Int> visitedCoordinates, ICollection<Coordinate> result)
        {
            visitedCoordinates.Add(currentCoordinate.gridPosition);
            result.Add(currentCoordinate);

            foreach (var direction in AdjacentDirections)
            {
                var linkedPosition = currentCoordinate.gridPosition + direction;
                var linkedCoordinate = GetCoordinateAtGridPosition(linkedPosition, allCoordinates);

                if (linkedCoordinate != null && !visitedCoordinates.Contains(linkedCoordinate.gridPosition) && currentCoordinate.startBlockType == linkedCoordinate.startBlockType)
                {
                    GetLinkedCoordinatesRecursive(linkedCoordinate, allCoordinates, visitedCoordinates, result);
                }
            }
        }

        public static ISet<Vector2Int> GetAdjacentPositions(Vector2Int gridPosition)
        {
            var  coordinatePositionResults = new HashSet<Vector2Int>();
            
            foreach (var direction in AdjacentDirections)
            {
                var position = gridPosition + direction; 
                coordinatePositionResults.Add(position);
            }

            return coordinatePositionResults;
        }
        
        //must be a pooling at some point or just creating garbage
        public static IBlockBehavior GetBlockBehaviour(BlockId id)
        {
            IBlockBehavior behaviour;
            switch (id)
            {   
                case BlockId.None:
                default:
                    behaviour = new ColorBlockBehaviour();
                    break;
                    
                case BlockId.Yellow: case BlockId.Red: case BlockId.Blue: 
                case BlockId.Green: case BlockId.Purple:
                    behaviour = new ColorBlockBehaviour();
                    break;
                    
                case BlockId.Balloon:
                    behaviour = new BalloonBlockBehaviour();
                    break;
                    
                case BlockId.Duck:
                    behaviour = new DuckBlockBehaviour();
                    break;
                    
                case BlockId.RocketVertical:
                    behaviour = new RocketBlockBehaviour(true);
                    break;
                
                case BlockId.RocketHorizontal:
                    behaviour = new RocketBlockBehaviour(false);
                    break;
            }

            return behaviour;
        }
    }
}
