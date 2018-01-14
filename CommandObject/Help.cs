using CNBlackListSoamChecker.DbManager;
using ReimuAPI.ReimuBase;
using ReimuAPI.ReimuBase.TgData;

namespace CNBlackListSoamChecker.CommandObject {
    internal class Help {
        internal bool HelpStatus(TgMessage RawMessage){
            string finalHelpMsg;
            string groupHelp =  "/soamenable - 啟用一個功能\n" +
                                "/soamdisable - 關閉一個功能\n" +
                                "/soamstatus - 取得目前群組開啟功能\n" +
                                "/twkick - 將一個已在封鎖列表內的使用這從群組中移除出去";
            string privateHelp = "";
            string sharedHelp = "/twbanstat - 看看自己有沒有被封鎖";
            switch (RawMessage.chat.type){
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
                finalHelpMsg += "\n管理員指令: /twban /twunban /getspamstr /addspamstr /delspamstr /getspampoints /lsop /addop /delsysop";
            }
            TgApi.getDefaultApiConnection().sendMessage(RawMessage.GetMessageChatInfo().id,finalHelpMsg,RawMessage.message_id);
            return true;
        }
    }
}
