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
                            SetActionResult result = TgApi.getDefaultApiConnection().kickChatMember(cfg.GroupID, user.UserID, GetTime.GetUnixTime() + 86400);
                            
                            if(result.ok){
                            TgApi.getDefaultApiConnection().sendMessage(
                                cfg.GroupID,
                                "新的被封鎖使用者 : " + user.UserID + "\n\n" + user.GetBanMessage() + "\n\n由於您訂閱了封鎖黑名單，已根據您的設定自動移除。"
                                );
                            }else{
                                TgApi.getDefaultApiConnection().sendMessage(
                                cfg.GroupID,
                                "新的被封鎖使用者 : " + user.UserID + "\n\n" + user.GetBanMessage() + "\n\n由於您訂閱了封鎖黑名單，可是似乎沒有權限(Ban User)移除，請設定正確的權限。"
                                );
                            }
                        }
                    }
                    Thread.Sleep(3000);
                }
            }
        }
    }
}
