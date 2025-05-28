using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class TileSelector : MonoBehaviour
{
    [SerializeField] private TowerTile currentTile;
    public TowerTile CurrentTile => currentTile;

    public bool IsDragging { get; set; }

    // Manager
    private ButtonManager buttonManager;

    private void Start()
    {
        buttonManager = FindAnyObjectByType<ButtonManager>();
    }

    public void SelectTile(TowerTile tile)
    {
        // 이미 선택된 타일이 있으면 이전 타일 외곽선 끄기
        if(currentTile != null)
            currentTile.SetOutLineActive(false);

        currentTile = tile;
        currentTile.SetOutLineActive(true);
    }

    public void ResetTile()
    {
        if(currentTile == null) 
            return;

        currentTile.SetOutLineActive(false);
        currentTile = null;
    }
}
