using UnityEngine;
using UnityEngine.UI;

using XPlan.UI;

public class OptionUI : UIBase
{
    [SerializeField] private Button initialBtn;
    [SerializeField] private Button stopBtn;
    [SerializeField] private Button backToBtn;
    [SerializeField] private Button refreshPOIBtn;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Awake()
    {
        initialBtn.interactable     = false;
        stopBtn.interactable        = false;
        backToBtn.interactable      = false;
        refreshPOIBtn.interactable  = false;

        RegisterButton(UIRequest.RobotInitial, initialBtn, () => 
        {
            initialBtn.interactable = false;
        });
        RegisterButton(UIRequest.StopMoving, stopBtn);
        RegisterButton(UIRequest.BackToHomedock, backToBtn);
        RegisterButton(UIRequest.RefreshPOI, refreshPOIBtn);

        ListenCall<bool>(UICommand.RobotReady, (b) => 
        {
            initialBtn.interactable     = !b;
            stopBtn.interactable        = b;
            backToBtn.interactable      = b;
            refreshPOIBtn.interactable  = b;
        });
    }
}
