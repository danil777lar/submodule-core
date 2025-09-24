using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class GridLayoutFitter : MonoBehaviour
{
    [SerializeField] private bool fitOnUpdate;
    [Space]
    [SerializeField] private Vector2Int targetGridSize;
    [Header("Min Values")]
    [SerializeField] private Vector2 minCellSize;
    [SerializeField] private Vector2 minSpacing;
    [SerializeField] private RectOffset minPadding;

    private Vector2 _baseSize;
    private Vector2 _baseSpacing;
    private RectOffset _basePadding;
    
    private GridLayoutGroup _grid;
    private RectTransform _parentRect;
    
    private void Start()
    {
        _parentRect = (RectTransform)transform.parent;
        _grid = GetComponent<GridLayoutGroup>();
        
        _baseSize = _grid.cellSize;
        _baseSpacing = _grid.spacing;
        _basePadding = new RectOffset();
        CopyPadding(_grid.padding, _basePadding);
        
        Fit();
    }
    
    private void Update()
    {
        if (fitOnUpdate)
        {
            Fit();
        }
    }

    private void Fit()
    {
        TryFitPaddings();
        TryFitSpacings();
        TryFitSize();
    }
    
    private void TryFitPaddings()
    {
        Vector2 currentSize = GetCurrentFullSize();
        RectOffset padding = new RectOffset();
        CopyPadding(_grid.padding, padding);
        
        float sumPaddingX = (float)padding.left + (float)padding.right;
        float diffX = currentSize.x - _parentRect.rect.width;
        
        padding.left = (int)(padding.left - diffX * ((float)padding.left / (float)sumPaddingX));
        padding.left = Mathf.Clamp(padding.left, minPadding.left, _basePadding.left);

        padding.right = (int)(padding.right - diffX * ((float)padding.right / (float)sumPaddingX));
        padding.right = Mathf.Clamp(padding.right, minPadding.right, _basePadding.right);

        float sumPaddingY = (float)padding.top + (float)padding.bottom;
        float diffY = currentSize.y - _parentRect.rect.height;
        
        padding.top = (int)(padding.top - diffY * ((float)padding.top / (float)sumPaddingY));
        padding.top = Mathf.Clamp(padding.top, minPadding.top, _basePadding.top);
        
        padding.bottom = (int)(padding.bottom - diffY * ((float)padding.bottom / (float)sumPaddingY));
        padding.bottom = Mathf.Clamp(padding.bottom, minPadding.bottom, _basePadding.bottom);

        CopyPadding(padding, _grid.padding);
    }
    
    private void TryFitSpacings()
    {
        Vector2 currentSize = GetCurrentFullSize();
        Vector2 spacing = _grid.spacing;

        Vector2 diff = currentSize - new Vector2(_parentRect.rect.width, _parentRect.rect.height);
        spacing.x = Mathf.Clamp(spacing.x - diff.x / (targetGridSize.x - 1), minSpacing.x, _baseSpacing.x);
        spacing.y = Mathf.Clamp(spacing.y - diff.y / (targetGridSize.y - 1), minSpacing.y, _baseSpacing.y);

        _grid.spacing = spacing;
    }
    
    private void TryFitSize()
    {
        Vector2 currentSize = GetCurrentFullSize();
        Vector2 cellSize = _grid.cellSize;

        Vector2 diff = currentSize - new Vector2(_parentRect.rect.width, _parentRect.rect.height);
        cellSize.x = Mathf.Clamp(cellSize.x - diff.x / targetGridSize.x, minCellSize.x, _baseSize.x);
        cellSize.y = Mathf.Clamp(cellSize.y - diff.y / targetGridSize.y, minCellSize.y, _baseSize.y);

        _grid.cellSize = cellSize;
    }

    private void CopyPadding(RectOffset from, RectOffset to)
    {
        to.left = from.left;
        to.right = from.right;
        to.top = from.top;
        to.bottom = from.bottom;
    }

    private Vector2 GetCurrentFullSize()
    {
        Vector2 fullSize = Vector2.zero;
        
        fullSize.x = targetGridSize.x * _grid.cellSize.x;
        fullSize.x += (targetGridSize.x - 1) * _grid.spacing.x;
        fullSize.x += _grid.padding.left + _grid.padding.right;
        
        fullSize.y = targetGridSize.y * _grid.cellSize.y;
        fullSize.y += (targetGridSize.y - 1) * _grid.spacing.y;
        fullSize.y += _grid.padding.top + _grid.padding.bottom;

        return fullSize;
    }
}
