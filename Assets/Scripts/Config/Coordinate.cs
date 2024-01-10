using System;
using System.Collections.Generic;
using Blocks.Enum;
using UnityEngine;

namespace Config
{
    [Serializable]
    public class Coordinate
    {
        public BlockId startBlockType;
        public List<Vector2Int> matchPositions;
        
        [HideInInspector] public Vector2Int gridPosition;
        [HideInInspector] public Vector2 relativePosition;
        [HideInInspector] public Vector2 relativeSize;
        
        public Coordinate(Coordinate newCord)
        {
            startBlockType = newCord.startBlockType;
            relativePosition = newCord.relativePosition;
            relativeSize = newCord.relativeSize;
            gridPosition = newCord.gridPosition;
            matchPositions = newCord.matchPositions;
        }
        
        public Coordinate(int r, int c, Vector2 position, Vector2 size, BlockId startBlock)
        {
            relativePosition = position;
            relativeSize = size;
            startBlockType = startBlock;
            gridPosition = new Vector2Int(r, c);
        }
    }
}
