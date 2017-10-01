using ReimuAPI.ReimuBase;
using ReimuAPI.ReimuBase.Interfaces;
using ReimuAPI.ReimuBase.TgData;

namespace CNBlackListSoamChecker
{
    class HelpMessage : IHelpMessage
    {
        public string GetHelpMessage(TgMessage RawMessage, string MessageType)
        {
            string finalHelpMsg;
            string groupHelp = "/soamenable - 启用一个功能\n" +
                "/soamdisable - 禁用一个功能\n" +
                "/soamstatus - 查看当前群组开启了的功能\n" +
                "/bkick - 将一个已在封禁列表中的用户从群组中移除出去";
            string privateHelp = "";
            string sharedHelp = "/banstat - 看看自己有没有被 Ban";
            switch (MessageType)
            {
                case "group":
                case "supergroup":
                    finalHelpMsg =  groupHelp + "\n" + sharedHelp;
                    break;
                case "private":
                    finalHelpMsg = privateHelp + "\n" + sharedHelp;
                    break;
                default:
                    finalHelpMsg =  sharedHelp;
                    break;
            }
            if (RAPI.getIsBotAdmin(RawMessage.from.id))
            {
                finalHelpMsg += "\n管理员指令: /ban /unban /getspamstr /addspamstr /delspamstr /getspampoints";
            }
            return finalHelpMsg;
        }
    }
}
