using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;

using XPlan;

public class SlamtecRobotManager : SystemBase
{
    [SerializeField] private OpenAIRealtimeUnity aiRealtimeUnity;
    [SerializeField] private string baseUrl     = "192.168.11.1";
    [SerializeField] private bool bIgnoreRobot  = false;

    protected override void OnInitialLogic()
    {
        RegisterLogic(new RobotController());
        RegisterLogic(new POIController(bIgnoreRobot));
        RegisterLogic(new CommandController(aiRealtimeUnity, bIgnoreRobot));
        RegisterLogic(new AIController(aiRealtimeUnity));
        RegisterLogic(new NetController(baseUrl));
    }
}
