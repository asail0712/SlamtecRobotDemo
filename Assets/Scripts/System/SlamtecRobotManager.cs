using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;

using XPlan;

public class SlamtecRobotManager : SystemBase
{
    [SerializeField] private OpenAIRealtimeUnity aiRealtimeUnity;
    [SerializeField] private string baseUrl             = "192.168.11.1";
    [SerializeField] private bool bIgnoreRobotInitial   = false;

    protected override void OnInitialLogic()
    {
        RegisterLogic(new RobotController(bIgnoreRobotInitial));
        RegisterLogic(new POIController(bIgnoreRobotInitial));
        RegisterLogic(new CommandController(aiRealtimeUnity, bIgnoreRobotInitial));
        RegisterLogic(new AIController(aiRealtimeUnity));
        RegisterLogic(new NetController(baseUrl));
    }
}
