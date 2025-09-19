using XPlan;

public class AIController : LogicComponent
{
    private OpenAIRealtimeUnity aiRealTime;

    public AIController(OpenAIRealtimeUnity aiRealTime) 
    {
        AddUIListener(UIRequest.MicStart, () => 
        {
            aiRealTime.MicStart();
        });

        AddUIListener(UIRequest.MicStop, () =>
        {
            aiRealTime.MicStop();
        });

        this.aiRealTime = aiRealTime;

        aiRealTime.OnAssistantTextDelta += HandleAITranscript;
    }

    protected override void OnDispose(bool bAppQuit)
    {
        aiRealTime.OnAssistantTextDelta -= HandleAITranscript;
    }

    private void HandleAITranscript(string poiName)
    {
        // 顯示POI名稱
        DirectCallUI<string>(UICommand.AddMessage, $"[Log] 辨識出地點 {poiName}");

        // 比對POI 找出移動的地點
        SendMsg<POIToMoveMsg>(poiName);        
    }
}
