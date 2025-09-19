using UnityEngine;
using UnityEngine.UI;

using XPlan.UI.Component;

public class POIItem : TableItem
{
    [SerializeField] private Button chooseBtn;
    [SerializeField] private Text showName;

    private void Awake()
    {
        chooseBtn.onClick.AddListener(() =>
        {
            DirectTrigger<string>(UIRequest.POIToMove, GetID());
        });
    }

    protected override void OnRefresh(TableItemInfo itemInfo)
    {
        POIInfo info            = (POIInfo)itemInfo;
        showName.text           = info.displayName;
        chooseBtn.interactable  = info.bEnable;
    }
}
