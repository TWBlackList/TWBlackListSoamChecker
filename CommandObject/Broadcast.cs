using CNBlackListSoamChecker.DbManager;
using ReimuAPI.ReimuBase;
using ReimuAPI.ReimuBase.TgData;

namespace CNBlackListSoamChecker.CommandObject {
    internal class BroadCast {
        internal bool BroadCast(TgMessage RawMessage){
            string Msg = RawMessage.text.Replace("/say","");
            if (RAPI.getIsBotOP(RawMessage.GetSendUser().id)){
                using (var db = new BlacklistDatabaseContext()){
                    List<GroupCfg> groupCfg = null;
                    try
                    {
                        groupCfg = db.GroupConfig
                        .Where(cfg => cfg.SubscribeBanList == 0)
                        .ToList();
                    }
                    catch (InvalidOperationException)
                    {
                        return;
                    }
                    if (groupCfg == null) return;
                    foreach (GroupCfg cfg in groupCfg)
                    {
                        TgApi.getDefaultApiConnection().sendMessage(cfg.GroupID,Msg,ParseMode : TgApi.PARSEMODE_MARKDOWN)
                    }
                }
            }else{
                TgApi.getDefaultApiConnection().sendMessage(RawMessage.chat.id,"你沒有權限",RawMessage.message_id);
                break;
            }
            return true;
        }
    }
}
