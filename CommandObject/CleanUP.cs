using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using TWBlackListSoamChecker.DbManager;
using ReimuAPI.ReimuBase;
using ReimuAPI.ReimuBase.TgData;

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
                .sendMessage(RawMessage.chat.id, "處理中.........!" , RawMessage.message_id);
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
                    string groupInfo = null;
                    try{groupInfo = TgApi.getDefaultApiConnection().getChatInfo(cfg.GroupID).result.GetChatTextInfo(); } catch { }

                    if (groupInfo == null)
                    {
                        groups = groups + cfg.GroupID.ToString() + " : 無法取得聊天，";
                        if (Temp.GetDatabaseManager().RemoveGroupCfg(cfg.GroupID))
                        {
                            groups = groups + "移除成功\n";
                        }
                        else
                        {
                            groups = groups + "移除失敗\n";
                        }
                    }
                    else
                    {
                        groups = groups + cfg.GroupID.ToString() + " : 已取得聊天，略過\n";
                    }
                }
               
                var charlist = new List<string>();

                for (var i = 0; i < groups.Length; i += 4000)
                {
                    charlist.Add(groups.Substring(i, Math.Min(4000, groups.Length - i)));
                }

                foreach (string msg in charlist)
                {
                    TgApi.getDefaultApiConnection().sendMessage(
                        RawMessage.GetMessageChatInfo().id,
                        msg,
                        RawMessage.message_id,
                        TgApi.PARSEMODE_HTML
                    );
                }
            }
            return true;
        }
    }
}