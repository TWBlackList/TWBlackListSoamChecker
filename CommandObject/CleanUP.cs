using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using ReimuAPI.ReimuBase;
using ReimuAPI.ReimuBase.TgData;
using TWBlackListSoamChecker.DbManager;

namespace TWBlackListSoamChecker.CommandObject
{
    internal class CleanUP
    {
        internal bool CleanUP_Status(TgMessage RawMessage)
        {
            new Thread(delegate() { CUP(RawMessage); }).Start();
            return true;
        }

        internal bool CUP(TgMessage RawMessage)
        {
            TgApi.getDefaultApiConnection()
                .sendMessage(RawMessage.chat.id, "處理中.........!", RawMessage.message_id);
            using (var db = new BlacklistDatabaseContext())
            {
                string groups = "";
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
                    bool status = false;
                    SendMessageResult result = TgApi.getDefaultApiConnection().sendMessage(
                        cfg.GroupID,
                        "測試訊息(不用理會此訊息)",
                        ParseMode: TgApi.PARSEMODE_MARKDOWN);
                    if (result.ok)
                    {
                        TgApi.getDefaultApiConnection().deleteMessage(cfg.GroupID, result.result.message_id);
                        status = true;
                        break;
                    }
                    
                    if (status)
                    {
                        groups = groups + cfg.GroupID + " : Bot是聊天成員，略過\n";
                    }
                    else
                    {
                        groups = groups + cfg.GroupID + " : Bot不是聊天成員，";
                        if (Temp.GetDatabaseManager().RemoveGroupCfg(cfg.GroupID))
                            groups = groups + "移除成功\n";
                        else
                            groups = groups + "移除失敗\n";
                    }ded
                }

                var charlist = new List<string>();

                for (var i = 0; i < groups.Length; i += 4000)
                    charlist.Add(groups.Substring(i, Math.Min(4000, groups.Length - i)));

                foreach (string msg in charlist)
                    TgApi.getDefaultApiConnection().sendMessage(
                        RawMessage.GetMessageChatInfo().id,
                        msg,
                        RawMessage.message_id,
                        TgApi.PARSEMODE_HTML
                    );
            }

            TgApi.getDefaultApiConnection().sendMessage(
                RawMessage.GetMessageChatInfo().id,
                "CleanUP 執行完畢",
                RawMessage.message_id,
                TgApi.PARSEMODE_HTML
            );
            return true;
        }
    }
}