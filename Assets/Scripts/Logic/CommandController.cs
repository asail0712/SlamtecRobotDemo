using System.Collections.Generic;
using System.Xml;
using UnityEditor;
using UnityEngine;

using XPlan;
using XPlan.Observe;
using XPlan.Utility;

public enum CommandType
{
    None    = 0,
    MoveTo,  
    Stop,
}

public class ClearAllCommandMsg : MessageBase
{
    public ClearAllCommandMsg()
    {

    }
}

public class AddCommandMsg : MessageBase
{
    public CommandType type;
    public string param;

    public AddCommandMsg(CommandType type, string param)
    {
        this.type   = type;
        this.param  = param;
    }
}

public class RequestCommandMsg : MessageBase
{
    public string commandDesc;

    public RequestCommandMsg(string commandDesc)
    {
        this.commandDesc    = commandDesc;
    }
}

public class CommandInfo
{
    public string commandStr;
    public string param;

    public CommandInfo(string commandStr, string param)
    {
        this.commandStr = commandStr;
        this.param      = param;
    }
}

public class CommandController : LogicComponent
{
    private List<CommandInfo> commandList;

    static private string BATTERY   = "Battery";
    static private string STOP      = "Stop";
    static private string NONE      = "None";

    public CommandController(OpenAIRealtimeUnity aiRealTime, bool bIgnoreRobot)
    {
        commandList = new List<CommandInfo>();

        // base command
        // 找不到
        commandList.Add(new CommandInfo($"找不到", NONE));
        // 充電相關
        commandList.Add(new CommandInfo($"回到充電站", BATTERY));
        commandList.Add(new CommandInfo($"去充電", BATTERY));
        commandList.Add(new CommandInfo($"前往充電站", BATTERY));
        commandList.Add(new CommandInfo($"回家", BATTERY));
        commandList.Add(new CommandInfo($"回去", BATTERY));
        // Stop
        commandList.Add(new CommandInfo($"停止移動", STOP));
        commandList.Add(new CommandInfo($"停下", STOP));
        commandList.Add(new CommandInfo($"不要動", STOP));
        commandList.Add(new CommandInfo($"別動", STOP));

        const int BasicCommandCount = 10;

        if (bIgnoreRobot)
        {
            InitialPrompt(aiRealTime);
        }

        /**************************************
         * 等收到POI訊息後生成AI用的Prompt
         * ************************************/
        RegisterNotify<ClearAllCommandMsg>((dummy) =>
        {
            if(commandList.Count > BasicCommandCount)
            {
                // 前10個為基本指令
                commandList.RemoveRange(BasicCommandCount, commandList.Count - (BasicCommandCount + 1));
            }

            InitialPrompt(aiRealTime);
        });

        RegisterNotify<AddCommandMsg>((msg) =>
        {
            AddMoveToCommand(msg.param);

            InitialPrompt(aiRealTime);
        });

        /**************************************
         * 收到Command後轉化成Robot行為
         * ************************************/
        RegisterNotify<RequestCommandMsg>((msg) => 
        {           
            foreach (CommandInfo info in commandList)
            {
                if (msg.commandDesc.Contains(info.commandStr)
                    || info.commandStr.Contains(msg.commandDesc))
                {
                    if (info.param == NONE)
                    {
                        DirectCallUI<LogInfo>(UICommand.AddMessage, new LogInfo(LogType.Log, $"無法辨識命令"));
                    }
                    else if (info.param == BATTERY)
                    {
                        DirectCallUI<LogInfo>(UICommand.AddMessage, new LogInfo(LogType.Log, $"AI辨識出命令 ({msg.commandDesc})"));
                        SendMsg<RobotChargingMsg>();
                    }
                    else if (info.param == STOP)
                    {
                        DirectCallUI<LogInfo>(UICommand.AddMessage, new LogInfo(LogType.Log, $"AI辨識出命令 ({msg.commandDesc})"));
                        SendMsg<RobotStopMsg>();
                    }
                    else
                    {
                        DirectCallUI<LogInfo>(UICommand.AddMessage, new LogInfo(LogType.Log, $"AI辨識出命令 ({msg.commandDesc})"));
                        SendMsg<RobotMoveMsg>(info.param);
                    }

                    break;
                }
            }
        });
    }

    private void AddMoveToCommand(string goalName)
    {
        commandList.Add(new CommandInfo($"前往{goalName}", goalName));        
        commandList.Add(new CommandInfo($"移動到{goalName}", goalName));
        commandList.Add(new CommandInfo($"往{goalName}移動", goalName));
        commandList.Add(new CommandInfo($"回到{goalName}", goalName));
    }

    private void InitialPrompt(OpenAIRealtimeUnity aiRealTime)
    {
        string commandStr = "你是一個命令匹配器。  \r\n任務：  \r\n1. 使用者會輸入一段可能是中文或是英文的文字。  \r\n2. 你有一份「命令清單」。  \r\n3. 你的工作是比對使用者輸入與命令清單，找出最接近的一個命令並回傳該命令本身。  \r\n4. 如果完全沒有接近或合理的匹配，直接回覆「找不到」。  \r\n5. 請只回傳命令，不要回傳其他文字或解釋。  \r\n\r\n命令清單：  ";

        for (int i = 0; i < commandList.Count; ++i)
        {
            commandStr += $"{i + 1}.{commandList[i].commandStr}。";
        }

        aiRealTime.basicInstructions = commandStr;
    }
}
