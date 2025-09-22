using System.Collections.Generic;

using XPlan;
using XPlan.Observe;

public class AIController : LogicComponent
{
    public AIController(OpenAIRealtimeUnity aiRealTime) 
    {
        aiRealTime.OnAssistantTextDone -= HandleAITranscript;
        aiRealTime.OnAssistantTextDone += HandleAITranscript;

        AddUIListener(UIRequest.MicStart, () =>
        {
            aiRealTime.MicStart();
        });

        AddUIListener(UIRequest.MicStop, () =>
        {
            aiRealTime.MicStop();
        });
    }

    private void HandleAITranscript(string commandDesc)
    {
        SendMsg<RequestCommandMsg>(commandDesc);        
    }
}
