using CNBlackListSoamChecker.DbManager;
using ReimuAPI.ReimuBase;
using ReimuAPI.ReimuBase.TgData;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;

namespace CNBlackListSoamChecker.CommandObject {
    internal class BroadCast {
        internal bool BroadCast_Status(TgMessage RawMessage)
        {
            new Thread(delegate () { BC(RawMessage); }).Start();
            return true;
        }

        internal bool BC(TgMessage RawMessage){
            string Msg = RawMessage.text.Replace("/say","");
            if (RAPI.getIsBotOP(RawMessage.GetSendUser().id)){
                System.Console.WriteLine("Broadcasting " + Msg + " ......");
                using (var db = new BlacklistDatabaseContext()){
                    List<GroupCfg> groupCfg = null;
                    try
                    {
                        groupCfg = db.GroupConfig.ToList();
                    }
                    catch (InvalidOperationException)
                    {
                        return false;
                    }
                    if (groupCfg == null) return false;
                    foreach (GroupCfg cfg in groupCfg)
                    {
                        System.Console.WriteLine("Broadcasting " + Msg + " To Group ChatID : " + cfg.GroupID);
                        TgApi.getDefaultApiConnection().sendMessage(cfg.GroupID,Msg,ParseMode : TgApi.PARSEMODE_MARKDOWN);
                        Thread.Sleep(100);
                    }
                    TgApi.getDefaultApiConnection().sendMessage(RawMessage.chat.id,"有夠Highㄉ，傳送完畢!",RawMessage.message_id);
                }
            }else{
                TgApi.getDefaultApiConnection().sendMessage(RawMessage.chat.id,"你沒有權限",RawMessage.message_id);
                return false;
            }
            return true;
        }
    }
}
