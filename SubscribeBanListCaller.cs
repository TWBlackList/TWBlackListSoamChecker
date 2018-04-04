using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using ReimuAPI.ReimuBase;
using TWBlackListSoamChecker.DbManager;

namespace TWBlackListSoamChecker
{
    internal class SubscribeBanListCaller
    {
        internal void CallGroupsInThread(BanUser user)
        {
            new Thread(delegate() { CallGroups(user); }).Start();
        }

        internal void CallGroups(BanUser user)
        {
            if (Temp.DisableAdminTools) return;
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
                foreach (var cfg in groupCfg)
                {
                    var userInChatInfo = TgApi.getDefaultApiConnection().getChatMember(cfg.GroupID, user.UserID);
                    if (userInChatInfo.ok)
                        if (userInChatInfo.result.status == "member")
                            new Thread(delegate()
                            {
                                Console.WriteLine("[SubscribeBanList] Ban " + user.UserID +
                                                  " in " + cfg.GroupID);
                                //TgApi.getDefaultApiConnection().restrictChatMember(
                                //    cfg.GroupID,
                                //    user.UserID,
                                //    GetTime.GetUnixTime() + 10,
                                //    false);
                                var result = TgApi.getDefaultApiConnection().sendMessage(
                                    cfg.GroupID,
                                    "使用者 : " + user.UserID + "\n" + user.GetBanMessage() +
                                    "\n\n由於開啟了 SubscribeBanList ，已嘗試自動移除。"
                                );
                                Thread.Sleep(5000);
                                TgApi.getDefaultApiConnection()
                                    .kickChatMember(cfg.GroupID, user.UserID, GetTime.GetUnixTime() + 1800);
                            }).Start();

                    Thread.Sleep(500);
                }
            }
        }
    }
}