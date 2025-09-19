using System.Collections.Generic;

using UnityEngine;

using XPlan.UI;
using XPlan.Utility;

public class LogUI : UIBase
{
    [SerializeField] private GameObject logPrefab;
    [SerializeField] private GameObject logAnchor;

    private List<GameObject> itemList;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Awake()
    {
        itemList = new List<GameObject>();

        ListenCall<string>(UICommand.AddMessage, (log) => 
        {
            GameObject logGO = GameObject.Instantiate(logPrefab);
            logAnchor.AddChild(logGO);
            logGO.transform.SetAsFirstSibling();

            LogItem logItem = logGO.GetComponent<LogItem>();
            logItem.SetLog(log);

            // log太多 就移除最舊的
            itemList.Add(logGO);
            while(itemList.Count > 15)
            {
                itemList.RemoveAt(0);
            }
        });
    }
}
