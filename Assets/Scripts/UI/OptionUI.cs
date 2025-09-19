using UnityEngine;
using UnityEngine.UI;

using XPlan.UI;

public class OptionUI : UIBase
{
    [SerializeField] private Button initialBtn;
    [SerializeField] private Button stopBtn;
    [SerializeField] private Button backToBtn;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Awake()
    {
        initialBtn.interactable = false;
        stopBtn.interactable    = false;
        backToBtn.interactable  = false;

        RegisterButton(UIRequest.Reinitial, initialBtn);
        RegisterButton(UIRequest.StopMoving, stopBtn);
        RegisterButton(UIRequest.BackToHomedock, backToBtn);

        ListenCall<bool>(UICommand.RobotReady, (b) => 
        {
            initialBtn.interactable = !b;
            stopBtn.interactable    = b;
            backToBtn.interactable  = b;
        });
    }
}
