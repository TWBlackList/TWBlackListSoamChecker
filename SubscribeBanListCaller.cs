using CNBlackListSoamChecker.DbManager;
using ReimuAPI.ReimuBase;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;

namespace CNBlackListSoamChecker
{
    internal class SubscribeBanListCaller
    {
        internal void CallGroupsInThread(BanUser user)
        {
            new Thread(delegate () { CallGroups(user); }).Start();
        }

        internal void CallGroups(BanUser user)
        {
            if (Temp.DisableAdminTools)
            {
                return;
            }
            if (user.Level == 1)
                return;
            using (var db = new BlacklistDatabaseContext())
            {
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
                    var userInChatInfo = TgApi.getDefaultApiConnection().getChatMember(cfg.GroupID, user.UserID);
                    if (userInChatInfo.ok)
                    {
                        if (userInChatInfo.result.status == "member")
                        {
                            TgApi.getDefaultApiConnection().kickChatMember(cfg.GroupID, user.UserID, GetTime.GetUnixTime() + 86400);
                            TgApi.getDefaultApiConnection().sendMessage(
                                cfg.GroupID,
                                "新的被封鎖使用者 : " + user.UserID + "\n\n" + user.GetBanMessage() + "\n\n由于您訂閱了封鎖列表，已根據您的设定自動移除。"
                                );
                        }
                    }
                    Thread.Sleep(3000);
                }
            }
        }
    }
}
