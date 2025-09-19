using UnityEngine;
using UnityEngine.UI;

using XPlan.UI;

public class NLP_UI : UIBase
{
    [SerializeField] private Button micBtn;
    [SerializeField] private Text micTxt;
    [SerializeField] private Text talkTxt;

    private bool bIsPushed = false;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Awake()
    {
        bIsPushed           = false;
        micTxt.text         = "按下說話";
        micBtn.interactable = false;

        RegisterButton("", micBtn, () => 
        {
            bIsPushed = !bIsPushed;

            if(bIsPushed)
            {
                DirectTrigger(UIRequest.MicStart);
            }
            else
            {
                DirectTrigger(UIRequest.MicStop);
            }

            if (bIsPushed)
            {
                micTxt.text = "收音中…(點擊關)";
            }
            else
            {
                micTxt.text = "按下說話";
            }
        });

        ListenCall<bool>(UICommand.RobotReady, (b) =>
        {
            micBtn.interactable = b;
        });

        ListenCall<string>(UICommand.NLPToLocation, (locName) => 
        {
            if (talkTxt != null)
            {
                talkTxt.text = "前往 : " + locName;
            }
        });
    }
}
