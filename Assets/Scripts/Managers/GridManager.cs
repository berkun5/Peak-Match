using System;
using System.Collections.Generic;
using System.Linq;
using Blocks.Enum;
using Blocks.Interface;
using Commands;
using Config;
using GameServices;
using GameServices.ServiceLocator;
using GridExtensions;
using Managers.Base;
using Managers.UI;
using UnityEngine;
using Random = UnityEngine.Random;

namespace Managers
{
    public class GridManager : ManagerBase
    {
        /// <summary>
        /// from, > destination
        /// </summary>
        public event Action<Coordinate, Coordinate> CoordinateFillAnimated;
        public event Action<Coordinate> CoordinateFallAnimated;
        public event Action<Coordinate> CoordinateChanged;
        public event Action<Vector2Int, BlockId, bool, BlastReason> CoordinateBlasted;
        public event Action<BlockId, int> GoalValueChanged;
        public event Action<int> MovesValueChanged;
        public LevelConfig ActiveLevelConfig { get; private set; }
        
        private UIEntityManager _uiEntityManager;
        private CommandManager _commandManager;
        private GameData _gameData;
        private List<Coordinate> _activeCoordinates = new();
        private List <Coordinate> _directionalCoordinates = new();
        private int _remainingMoves;
        private readonly Dictionary<int, List<Coordinate>> _allRowCoordinates = new ();
        private readonly Dictionary<BlockId, int> _allCurrentGoals = new();
        
        public override void Init()
        {
            _gameData = GameServiceLocator.GetService<PersistentServiceProvider>()
                .GetManager<SettingsManager>().LocalData.GetData<GameData>();
        }

        public override void LateStart()
        {
            _commandManager = GameServiceLocator.GetService<CommandServiceProvider>().GetManager<CommandManager>();
        }

        public IBlockBehavior GetBlockBehaviour(BlockId id)
        {
            return GridExt.GetBlockBehaviour(id);
        }
		
        public List<Coordinate> GenerateLevelGrid(LevelConfig levelConfig, RectTransform canvasRect)
        {
            ActiveLevelConfig = levelConfig;
            _remainingMoves = levelConfig.MovesCount;
            SetGoals();
            
            _activeCoordinates = levelConfig.gridCoordinates.Select(cord => new Coordinate(cord)).ToList();
            var columnCount = levelConfig.ColumnCount;
            var rowCount = levelConfig.RowCount;
            var gridWidthOffset = _gameData.GridWidthOffset;
            var gridHeightOffset = _gameData.GridHeightOffset;
            
            var gridSize = GridExt.GetGridSize(canvasRect, gridWidthOffset, gridHeightOffset);
            var dynamicBlockSize = GridExt.GetGridElementSize(gridSize, rowCount, columnCount);
            var gridStartPos = GridExt.GetGridStartPosition(dynamicBlockSize, gridSize, rowCount, columnCount);
            
            
            foreach (var cord in _activeCoordinates)
            {
                cord.relativePosition = GridExt.GetPositionOnGrid(gridStartPos, dynamicBlockSize, cord.gridPosition);
                cord.relativeSize = new Vector2(dynamicBlockSize, dynamicBlockSize);
            }
            
            for (var i = 0; i < ActiveLevelConfig.RowCount; i++)
            {
                _allRowCoordinates.Add(i, GridExt.GetCoordinatesAtRow(i, _activeCoordinates));
            }
            
            RefreshLevelGrid();
            return _activeCoordinates;
        }

        private void SetGoals()
        {
            _allCurrentGoals.Clear();
            foreach (var goal in ActiveLevelConfig.goalValues)
            {
                _allCurrentGoals.Add(goal.GoalId, goal.Requirement);
            }
        }
        
        private void RefreshLevelGrid()
        {
            FillGrid();
            SetFall();
            SetMatches();
        }

        //shifting down movement
        private void FillGrid()
        {
            var rowCount = ActiveLevelConfig.RowCount;
            
            for (var i = 0; i < rowCount; i++)
            {
                foreach (var rowPair in _allRowCoordinates)
                {
                    var rowBatch = rowPair.Value;
                    var startCord = -1;
                    var endCord = -1;
                    
                    for (var j = 0; j < rowBatch.Count - 1; j++)
                    {
                        var below = rowBatch[j + 1];
                        var current = rowBatch[j];

                        var canMove = current.startBlockType != BlockId.None;
                        var belowIsEmpty = below != null && below.startBlockType == BlockId.None;

                        if (!canMove || !belowIsEmpty)
                        {
                            continue;
                        }
                        
                        below.startBlockType = current.startBlockType;
                        current.startBlockType = BlockId.None;
                        
                        //finding start and end movement coordinates
                        if (startCord == -1)
                        {
                            startCord = j;
                        }
                        
                        endCord = j + 1;
                    }
                    
                    //after the inner loop, invoke the event for the moving blocks
                    if (startCord == -1)
                    {
                        continue;
                    }
                    
                    var firstBlock = rowBatch[startCord];
                    var lastBlock = rowBatch[endCord];
                    
                    CoordinateChanged?.Invoke(firstBlock);
                    CoordinateChanged?.Invoke(lastBlock);
                    CoordinateFillAnimated?.Invoke(firstBlock, lastBlock);
                }
            }
        }
        
        private void SetFall()
        {
            foreach (var rowPair in _allRowCoordinates)
            {
                var rowCoordinates = rowPair.Value;
                foreach (var cord in rowCoordinates)
                {
                    if (cord.startBlockType != BlockId.None)
                    {
                        continue;
                    }
                    
                    var randomType = ActiveLevelConfig.blockPool[Random.Range(0, ActiveLevelConfig.blockPool.Count)];
                    cord.startBlockType = randomType;
                    CoordinateChanged?.Invoke(cord);
                    CoordinateFallAnimated?.Invoke(cord);
                }
            }
        }
        
        private void SetMatches()
        {
            foreach (var cord in _activeCoordinates)
            {
                var cordId = cord.startBlockType;
                cord.matchPositions.Clear();

                var adjacentCoordinates = GridExt.GetLinkedCoordinates(cord, _activeCoordinates);

                foreach (var adjacentCord in adjacentCoordinates
                             .Where(adjacentCord => cordId == adjacentCord.startBlockType))
                {
                    cord.matchPositions.Add(adjacentCord.gridPosition);
                }
            }
        }
        
        public void TryMatchBlast(Coordinate blastCandidate)
        {
            if (blastCandidate.matchPositions.Count < GameData.MinimumMatchCount)
            {
                return;
            }

            if (_commandManager.HasActiveCommand())
            {
                return;
            }

            var isRocket = blastCandidate.matchPositions.Count >= _gameData.RocketMatchCount;
            
            for (var i = 0; i < blastCandidate.matchPositions.Count; i++)
            {
                var matchPos = blastCandidate.matchPositions[i];
                var blastedCord = GridExt.GetCoordinateAtGridPosition(matchPos, _activeCoordinates);
                var lastIteration = i == blastCandidate.matchPositions.Count - 1;
                var idBeforeBlast = blastedCord.startBlockType;
                
                var blastCommand = new BlastCommand(blastedCord, onComplete:() =>
                {
                    TryProgressGoal(idBeforeBlast);
                    CoordinateBlasted?.Invoke(matchPos, idBeforeBlast,
                        _allCurrentGoals.ContainsKey(idBeforeBlast),BlastReason.Match);
                    CoordinateChanged?.Invoke(blastedCord);

                    if (!lastIteration)
                    {
                        return;
                    }

                    if (isRocket)
                    {
                        var isVerticalDirection =  Random.Range(0, 2) < 1;
                        blastCandidate.startBlockType = isVerticalDirection ? BlockId.RocketVertical : BlockId.RocketHorizontal;
                        CoordinateChanged?.Invoke(blastCandidate);
                    }
                    
                    TryProgressMoves();
                    RefreshLevelGrid();
                });
                
                _commandManager.QueueCommand(blastCommand);
            }
        }
        
        public void SingleBlast(Coordinate blastCandidate, bool isSimultaneous = false)
        {
            var idBeforeBlast = blastCandidate.startBlockType;
            var blastCommand = new BlastCommand(blastCandidate, isSimultaneous, onComplete: () =>
            {
                TryProgressGoal(idBeforeBlast);
                CoordinateBlasted?.Invoke(blastCandidate.gridPosition, idBeforeBlast,
                    _allCurrentGoals.ContainsKey(idBeforeBlast),BlastReason.Single); 
                CoordinateChanged?.Invoke(blastCandidate); 
                RefreshLevelGrid();
            });
            
            _commandManager.QueueCommand(blastCommand);
        }

        //at this point I'm convinced that I need at least facade or even new manager for blasts
        public void BlastInDirection(Coordinate blastCandidate, BlastDirection direction, bool isSimultaneous = false)
        {
            const float blastDurationPerPosition = 0.05f;
            _directionalCoordinates.Clear();
            var startPosition = direction == BlastDirection.Vertical ? blastCandidate.gridPosition.y : blastCandidate.gridPosition.x;
            switch (direction)
            {
                case BlastDirection.Vertical:
                    SetRocketDirectionalCoordinates(GridExt.GetCoordinatesAtColumn(blastCandidate.gridPosition.x, _activeCoordinates), direction, startPosition);
                    break;

                case BlastDirection.Horizontal:
                    SetRocketDirectionalCoordinates(GridExt.GetCoordinatesAtRow(blastCandidate.gridPosition.y, _activeCoordinates), direction, startPosition);
                    break;

                case BlastDirection.None:
                default:
                    break;
            }
            
            for (var i = 0; i < _directionalCoordinates.Count; i++)
            {
                var blastingCord = _directionalCoordinates[i];
                var lastIteration = i == _directionalCoordinates.Count - 1;
                var idBeforeBlast = blastingCord.startBlockType;
                
                var blastCommand = new BlastCommand(blastingCord, setDelay: i * blastDurationPerPosition, onComplete:() =>
                {
                    TryProgressGoal(idBeforeBlast); 
                    CoordinateBlasted?.Invoke(blastingCord.gridPosition, idBeforeBlast,
                        _allCurrentGoals.ContainsKey(idBeforeBlast),BlastReason.Directional);
                    CoordinateChanged?.Invoke(blastingCord);

                    if (!lastIteration)
                    {
                        return;
                    }
                    
                    TryProgressMoves();
                    RefreshLevelGrid();
                });
                
                _commandManager.QueueCommand(blastCommand);
            }
            
        }
        
        private void SetRocketDirectionalCoordinates(IEnumerable<Coordinate> coordinates, BlastDirection direction, int rocketStartGridPosition = 0)
        {
            _directionalCoordinates.Clear();

            foreach (var cord in coordinates)
            {
                _directionalCoordinates.Add(cord);
            }

            //sounds complicated but Abs is just distance calculation and sort based on that,
            //_directionalCoordinates index - rocketStartGridPosition for each element = <<<Pos>>>
            _directionalCoordinates = direction switch
            {
                BlastDirection.Vertical => _directionalCoordinates.OrderBy(c => Math.Abs(c.gridPosition.y - rocketStartGridPosition)).ToList(),
                BlastDirection.Horizontal => _directionalCoordinates.OrderBy(c => Math.Abs(c.gridPosition.x - rocketStartGridPosition)).ToList(),
                _ => _directionalCoordinates
            };
        }
        
        private void TryProgressGoal(BlockId blastedId)
        {
            if (!_allCurrentGoals.ContainsKey(blastedId) || _allCurrentGoals[blastedId] <= 0)
            {
                return;
            }
            
            _allCurrentGoals[blastedId]--;
            GoalValueChanged?.Invoke(blastedId, _allCurrentGoals[blastedId]);
        }

        private void TryProgressMoves()
        {
            if (_remainingMoves <= 0)
            {
                return;
            }
            
            _remainingMoves--;
            MovesValueChanged?.Invoke(_remainingMoves);
        }
        
        public ISet<Vector2Int> GetAdjacentPositions(Vector2Int gridPosition)
        {
            return GridExt.GetAdjacentPositions(gridPosition);
        }
    }
}