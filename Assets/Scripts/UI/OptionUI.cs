using UnityEngine;
using UnityEngine.UI;

using XPlan.UI;

public class OptionUI : UIBase
{
    [SerializeField] private Button stopBtn;
    [SerializeField] private Button backToBtn;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Awake()
    {
        RegisterButton(UIRequest.StopMoving, stopBtn);
        RegisterButton(UIRequest.BackToHomedock, backToBtn);
    }
}
