using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using TWBlackListSoamChecker.DbManager;
using ReimuAPI.ReimuBase;
using ReimuAPI.ReimuBase.TgData;

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
                foreach (GroupCfg cfg in groupCfg)
                {
                    var userInChatInfo = TgApi.getDefaultApiConnection().getChatMember(cfg.GroupID, user.UserID);
                    if (userInChatInfo.ok)
                        if (userInChatInfo.result.status == "member")
                        {
                            new Thread(delegate()
                            {
                                System.Console.Write("[SubscribeBanList] Ban " + user.UserID.ToString() + " in " + cfg.GroupID.ToString());
                                TgApi.getDefaultApiConnection().restrictChatMember(
                                    cfg.GroupID,
                                    user.UserID,
                                    GetTime.GetUnixTime() + 10,
                                    true,
                                    false);
                                SendMessageResult result = TgApi.getDefaultApiConnection().sendMessage(
                                    cfg.GroupID,
                                    "使用者 : " + user.UserID + "\n" + user.GetBanMessage() +
                                    "\n\n由於開啟了 SubscribeBanList ，已嘗試自動移除。" +
                                    "若要提出申訴，請至 @" + Temp.CourtGroupName + " 。"
                                );
                                Thread.Sleep(10000);
                                SetActionResult kickresult = TgApi.getDefaultApiConnection()
                                    .kickChatMember(cfg.GroupID, user.UserID, GetTime.GetUnixTime() + 60);
                                if(kickresult.ok)
                                    System.Console.WriteLine("...Done");
                                else
                                    System.Console.WriteLine("...Fail");
                                Thread.Sleep(10000);
                                TgApi.getDefaultApiConnection().deleteMessage(
                                    result.result.chat.id,
                                    result.result.message_id
                                );
                            }).Start();
                        }

                    Thread.Sleep(500);
                }
            }
        }
    }
}