using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using ReimuAPI.ReimuBase;
using ReimuAPI.ReimuBase.TgData;
using TWBlackListSoamChecker.DbManager;

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
                    string groupInfo = "無法取得";
                    try
                    {
                        groupInfo = TgApi.getDefaultApiConnection().getChatInfo(cfg.GroupID).result.GetChatTextInfo();
                    }
                    catch
                    {
                    }

                    groups = groups + cfg.GroupID + " : \n" + RAPI.escapeMarkdown(groupInfo) + "\n\n";

                    if (groups.Length > 3072)
                    {
                        TgApi.getDefaultApiConnection()
                            .sendMessage(RawMessage.chat.id, groups, ParseMode: TgApi.PARSEMODE_MARKDOWN);
                        groups = "";
                        Thread.Sleep(3000);
                    }
                }

                if (groups.Length > 0)
                    TgApi.getDefaultApiConnection()
                        .sendMessage(RawMessage.chat.id, groups, ParseMode: TgApi.PARSEMODE_MARKDOWN);

                TgApi.getDefaultApiConnection()
                    .sendMessage(RawMessage.chat.id, "Groups 輸出完畢!", RawMessage.message_id);
            }

            return true;
        }
    }
}