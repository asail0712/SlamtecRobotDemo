using System.Collections.Generic;

using XPlan;
using XPlan.Observe;

public class GeneratePromptMsg : MessageBase
{
    public List<CommandInfo> commandList = new List<CommandInfo>();

    public GeneratePromptMsg(List<CommandInfo> commandList)
    {
        this.commandList = commandList;
    }
}

public class AIController : LogicComponent
{
    public AIController(OpenAIRealtimeUnity aiRealTime) 
    {
        aiRealTime.OnAssistantTextDelta -= HandleAITranscript;
        aiRealTime.OnAssistantTextDelta += HandleAITranscript;

        AddUIListener(UIRequest.MicStart, () =>
        {
            aiRealTime.MicStart();
        });

        AddUIListener(UIRequest.MicStop, () =>
        {
            aiRealTime.MicStop();
        });

        /**************************************
         * 等收到POI訊息後生成AI用的Prompt
         * ************************************/
        RegisterNotify<GeneratePromptMsg>((msg) => 
        {
            string commandStr = "你是一個命令匹配器。  \r\n任務：  \r\n1. 使用者會輸入一段文字。  \r\n2. 你有一份「命令清單」。  \r\n3. 你的工作是比對使用者輸入與命令清單，找出最接近的一個命令並回傳該命令本身。  \r\n4. 如果完全沒有接近或合理的匹配，直接回覆「找不到」。  \r\n5. 請只回傳命令，不要回傳其他文字或解釋。  \r\n\r\n命令清單：  ";

            for(int i = 0; i < msg.commandList.Count; ++i)
            {
                commandStr += $"{i + 1}.{msg.commandList[i]}。";
            }

            aiRealTime.basicInstructions = commandStr;
        });
    }

    private void HandleAITranscript(string commandDesc)
    {
        // 顯示POI名稱
        DirectCallUI<LogInfo>(UICommand.AddMessage, new LogInfo(LogType.Log, $"辨識出命令 {commandDesc}"));

        SendMsg<RequestCommandMsg>(commandDesc);        
    }
}
