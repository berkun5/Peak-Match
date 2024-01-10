using System.Collections.Generic;
using Blocks.Interface;
using Config;
using UnityEngine;

namespace UI.Grid
{
    public interface IGridWindowViewModel
    {
        List<IBlockViewModel> GridBlockViewModels { get; }
        BlocksData BlocksData { get; }
        Vector2 GridSize(RectTransform canvasRect);
    }
}
