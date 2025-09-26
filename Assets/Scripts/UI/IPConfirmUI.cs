using System.Net;
using System.Text.RegularExpressions;
using UnityEngine;
using UnityEngine.UI;
using XPlan.UI;

public class IPConfirmUI : UIBase
{
    [SerializeField] private Button okBtn;
    [SerializeField] private InputField inputTxt;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    private void Awake()
    {
        okBtn.interactable = IsIPv4(inputTxt.text);

        RegisterButton("", okBtn, () =>
        {
            DirectTrigger<string>(UIRequest.ConfirmIP, inputTxt.text);

            gameObject.SetActive(false);
        });

        RegisterText("", inputTxt, (ipStr) => 
        {
            okBtn.interactable = IsIPv4(ipStr);
        });
    }

    static readonly Regex IPv4Strict = new Regex(
        @"^(?:25[0-5]|2[0-4]\d|1\d\d|[1-9]?\d)" +
        @"(?:\.(?:25[0-5]|2[0-4]\d|1\d\d|[1-9]?\d)){3}$",
        RegexOptions.Compiled);

    bool IsIPv4(string s) => IPv4Strict.IsMatch(s);
}
