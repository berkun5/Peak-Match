using System.Collections.Generic;
using Blocks.View;
using Config;
using UnityEngine;

namespace UI.Grid
{
    public class GridWindow : UIEntity
    {
        [SerializeField] private RectTransform gridCanvasRect;
        [SerializeField] private RectTransform gridBackground;
        private readonly List<BlockView> _blockViewInstances = new();
        private IGridWindowViewModel _viewModel;
        private BlocksData _blocksData;
        public void Init(IGridWindowViewModel viewModel)
        {
            _viewModel = viewModel;
            _blocksData = viewModel.BlocksData;

            gridBackground.sizeDelta = viewModel.GridSize(gridCanvasRect);
            CreateInitialBlockViews();
        }
        
        private void CreateInitialBlockViews()
        {
            var gridBlockViews = _viewModel.GridBlockViewModels;
            for (int i = 0; i < gridBlockViews.Count; i++)
            {
                var hasViewInstanceForThisIndex = _blockViewInstances.Count > i;
                var viewModel = gridBlockViews[i];
                var view = hasViewInstanceForThisIndex ? _blockViewInstances[i] : Instantiate(_blocksData.GetBlockPrefab(), transform);
                view.Init(viewModel);
                view.gameObject.SetActive(true);
                if (!hasViewInstanceForThisIndex)
                {
                    _blockViewInstances.Add(view);
                }
            }
        }
    }
}
