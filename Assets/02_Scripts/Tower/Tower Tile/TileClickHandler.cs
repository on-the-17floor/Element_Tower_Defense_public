using UnityEngine;
using UnityEngine.EventSystems;

[RequireComponent(typeof(TowerTile))]
public class TileClickHandler : MonoBehaviour, IPointerClickHandler
{
    [SerializeField] private TowerTile tile;
    [SerializeField] private TowerRangeCircle tileRangeCircle;

    private TowerInfo towerInfoUI;
    private BuildButton buildButton;

    private void Start()
    {
        towerInfoUI = (TowerInfo)UIManager.Instance.PopupUIs[(int)PopupUIType.TowerInfo];
        buildButton = (BuildButton)UIManager.Instance.PopupUIs[(int)PopupUIType.BuildUI];
    }

    public void OnPointerClick(PointerEventData eventData)
    {

        // tileTower 없을 때 : 타워 생성 UI -> 타워 생성
        // tileTower 있을 때 : 타워 정보 출력, 사거리
        TowerManager.Instance.TileSelector.SelectTile(tile);
        SoundManager.Instance.SFXPlay(SFXType.UiClick, transform.position);

        if (tile.tower == null)
        {
            buildButton.Show();
        }

        else if (tile.tower != null)
        {
            towerInfoUI?.Show();

            int upgradeLevel = TowerManager.Instance.TowerUpgrade[tile.tower.Data.elementType];
            towerInfoUI?.SetTowerInfo(tile.tower.Data, upgradeLevel, tile.tower.DamageBuffRate, this);

            // 사거리 표시 ON
            OnAttackRange(true);
        }
    }

    public void OnAttackRange(bool active)
    {
        tileRangeCircle.RangeSphere.SetActive(active);
        if(active)
            tileRangeCircle.OnRange(tile.tower.Data.attackRange);
    }
}
