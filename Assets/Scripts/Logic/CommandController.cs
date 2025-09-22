using System.Collections.Generic;
using System.Xml;
using UnityEditor;
using UnityEngine;

using XPlan;
using XPlan.Observe;
using XPlan.Utility;

public enum CommandType
{
    MoveTo  = 0,
    Stop,
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

    public CommandController()
    {
        commandList = new List<CommandInfo>();

        // base command
        // 充電相關
        commandList.Add(new CommandInfo($"回到充電站", BATTERY));
        commandList.Add(new CommandInfo($"去充電", BATTERY));
        commandList.Add(new CommandInfo($"前往充電站", BATTERY));
        // Stop
        commandList.Add(new CommandInfo($"停止移動", STOP));
        commandList.Add(new CommandInfo($"停下", STOP));
        commandList.Add(new CommandInfo($"不要動", STOP));

        /**************************************
         * 等收到POI訊息後生成AI用的Prompt
         * ************************************/
        RegisterNotify<AddCommandMsg>((msg) =>
        {
            AddMoveToCommand(msg.param);

            SendMsg<GeneratePromptMsg>(commandList);
        });

        /**************************************
         * 收到Command後轉化成Robot行為
         * ************************************/
        RegisterNotify<RequestCommandMsg>((msg) => 
        {
            foreach(CommandInfo info in commandList)
            {
                if (msg.commandDesc.Contains(info.param)
                    || info.param.Contains(msg.commandDesc))
                {
                    if(info.param == BATTERY)
                    {
                        SendMsg<RobotChargingMsg>();
                    }
                    else if(info.param == STOP)
                    {
                        SendMsg<RobotStopMsg>();
                    }
                    else
                    {
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
}
