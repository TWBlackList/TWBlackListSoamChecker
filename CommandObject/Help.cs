using CNBlackListSoamChecker.DbManager;
using ReimuAPI.ReimuBase;
using ReimuAPI.ReimuBase.TgData;

namespace CNBlackListSoamChecker.CommandObject {
    internal class Help {
        internal bool HelpStatus(TgMessage RawMessage){
            string finalHelpMsg;
            string groupHelp =  "/soamenable - 啟用功能\n" +
                                "/soamdisable - 關閉功能\n" +
                                "/soamstatus - 取得目前群組開啟功能\n" +
                                "/twkick - 將一個已在封鎖列表內的使用這從群組中移除出去";
            string privateHelp = "";
            string sharedHelp = "/twbanstat - 查詢處分狀態\n" + 
                                "/lsop - Operator 名冊";
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
                finalHelpMsg = finalHelpMsg + "\n\nOP指令:\n" +
                                "/twban - 封鎖\n" +
                                "/twunban - 解除封鎖\n" +
                                "/getspampoints - 測試關鍵字\n\n" +
                                "Admin指令:\n" +
                                "/addspamstr - 新增 1 個自動規則\n" +
                                "/delspamstr - 刪除 1 個自動規則\n" +
                                "/getspamstr - 查看自動規則列表\n" +
                                "/say - 廣播\n" +
                                "/addop - 新增 Operator\n" +
                                "/delop - 解除 Operator\n";
            }
            TgApi.getDefaultApiConnection().sendMessage(RawMessage.GetMessageChatInfo().id,finalHelpMsg,RawMessage.message_id);
            return true;
        }
    }
}
