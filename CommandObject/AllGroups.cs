using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using TWBlackListSoamChecker.DbManager;
using ReimuAPI.ReimuBase;
using ReimuAPI.ReimuBase.TgData;

namespace TWBlackListSoamChecker.CommandObject
{
    internal class AllGroups
    {
        internal bool Groups_Status(TgMessage RawMessage)
        {
            new Thread(delegate() { Groups(RawMessage); }).Start();
            return true;
        }

        internal bool Groups(TgMessage RawMessage)
        {
            using (var db = new BlacklistDatabaseContext())
            {
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
                    string groupInfo = "無法取得";
                    try{groupInfo = TgApi.getDefaultApiConnection().getChatInfo(cfg.GroupID).result.GetChatTextInfo(); } catch { }

                    TgApi.getDefaultApiConnection()
                        .sendMessage(RawMessage.chat.id, cfg.GroupID.ToString() + " : \n\n" + groupInfo, ParseMode: TgApi.PARSEMODE_MARKDOWN);
                    Thread.Sleep(500);
                }

                TgApi.getDefaultApiConnection()
                    .sendMessage(RawMessage.chat.id, "有夠Highㄉ，輸出完畢!", RawMessage.message_id);
            }

            return true;
        }
    }
}