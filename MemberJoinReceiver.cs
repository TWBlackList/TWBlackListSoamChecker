using CNBlackListSoamChecker.DbManager;
using ReimuAPI.ReimuBase;
using ReimuAPI.ReimuBase.Interfaces;
using ReimuAPI.ReimuBase.TgData;
using System.Threading;

namespace CNBlackListSoamChecker
{
    class MemberJoinReceiver : IMemberJoinLeftListener
    {
        public CallbackMessage OnGroupMemberJoinReceive(TgMessage RawMessage, string JsonMessage, UserInfo JoinedUser)
        {
            return OnSupergroupMemberJoinReceive(RawMessage, JsonMessage, JoinedUser);
        }

        public CallbackMessage OnSupergroupMemberJoinReceive(TgMessage RawMessage, string JsonMessage, UserInfo JoinedUser)
        {
            if (JoinedUser.id == TgApi.getDefaultApiConnection().getMe().id)
            {
                TgApi.getDefaultApiConnection().sendMessage(
                    RawMessage.GetMessageChatInfo().id,
                    "觀迎使用 @" + TgApi.getDefaultApiConnection().getMe().username + "\n" +
                    "請您進行一些設定 : \n" +
                    "1.在您的群組中给予 @" + TgApi.getDefaultApiConnection().getMe().username + " 管理員權限\n" +
                    "2.使用 /soamenable 開啟一些功能\n" +
                    "3.Enjoy it!\n\n" +
                    "注: 預設開啟的功能有 BlackList AutoKick AntiHalal SubscribeBanList 這 4 個，您可以根據您的需要來關閉或啟用。",
                    RawMessage.message_id
                    );
                return new CallbackMessage();
            }
            if (Temp.DisableBanList)
            {
                return new CallbackMessage();
            }
            DatabaseManager dbmgr = Temp.GetDatabaseManager();
            if (RawMessage.GetMessageChatInfo().id == -1001132136235)
            {
                BanUser banUser = dbmgr.GetUserBanStatus(JoinedUser.id);
                if (banUser.Ban == 0)
                {
                    string resultmsg = "這位使用者被封鎖了";
                    if (banUser.ChannelMessageID != 0)
                    {
                        resultmsg += "， [原因請點選這裡查看](https://t.me/" + Temp.MainChannelName + "/" + banUser.ChannelMessageID + ")";
                    }
                    else
                    {
                        resultmsg += "，原因 : \n" + banUser.Reason + "\nID : " + JoinedUser.id;
                    }
                    TgApi.getDefaultApiConnection().sendMessage(
                        RawMessage.GetMessageChatInfo().id,
                        resultmsg,
                        RawMessage.message_id,
                        ParseMode: TgApi.PARSEMODE_MARKDOWN
                        );
                }
                else
                {
                    //TgApi.getDefaultApiConnection().restrictChatMember(
                    //            RawMessage.GetMessageChatInfo().id,
                    //            JoinedUser.id,
                    //            GetTime.GetUnixTime() + 60
                    //            );
                    //TgApi.getDefaultApiConnection().sendMessage(
                    //    RawMessage.GetMessageChatInfo().id,
                    //    "您未被封鎖，請閒雜等人退出群組。如果您想加入這個群組，您可以去多點群發一些廣告，然後您被 Ban 了就能加入了。\n\n" +
                    //    "您將在 60 秒後自動退出群組。",
                    //    RawMessage.message_id,
                    //    ParseMode: TgApi.PARSEMODE_MARKDOWN
                    //    );
                    //new Thread(delegate () {
                    //    Thread.Sleep(60000);
                    //    TgApi.getDefaultApiConnection().kickChatMember(
                    //        RawMessage.GetMessageChatInfo().id,
                    //        JoinedUser.id,
                    //        GetTime.GetUnixTime() + 60
                    //        );
                    //}).Start();
                }
                return new CallbackMessage();
            }
            GroupCfg groupCfg = dbmgr.GetGroupConfig(RawMessage.GetMessageChatInfo().id);
            if (groupCfg.BlackList == 0)
            {
                BanUser banUser = dbmgr.GetUserBanStatus(JoinedUser.id);
                string resultmsg = "";
                if (banUser.Ban == 0)
                {
                    string banReason;
                    if (banUser.ChannelMessageID != 0)
                    {
                        banReason = "， [原因請點選這裡查看](https://t.me/" + Temp.MainChannelName + "/" + banUser.ChannelMessageID + ")";
                    }
                    else
                    {
                        banReason = "\n原因 : " + banUser.Reason;
                    }
                    if (banUser.Level == 0)
                    {
                        resultmsg += "警告：這個使用者「將會」對群組造成負面影響，已自動封鎖" + banReason + "\n" +
                            "被封鎖的用戶，可以到 [這個群組](https://t.me/J_Court) 尋求申訴";
                        if (groupCfg.AutoKick == 0)
                        {
                            try{SetActionResult result = TgApi.getDefaultApiConnection().kickChatMember(
                                RawMessage.GetMessageChatInfo().id,
                                JoinedUser.id,
                                GetTime.GetUnixTime() + 86400
                                );
                                if (!result.ok){
                                    resultmsg += "\n注意：由於開啟了 AutoKick 但沒有 Ban Users 權限" +
                                            "，請關閉此功能或給予權限（Ban users）。";
                            }
                            }catch{}

                            
                        }
                    }
                    else if (banUser.Level == 1)
                    {
                        resultmsg += "警告：這個使用者「可能」對群組造成負面影響" + banReason  + "\n" +
                            "請群組管理員多加留意\n"+
                            "對於被警告的使用者，你可以通過 [這個群組](https://t.me/J_Court) 以請求解除。";

                    }

                }
                else
                {
                    return new CallbackMessage() {  };
                }
                TgApi.getDefaultApiConnection().sendMessage(
                    RawMessage.GetMessageChatInfo().id,
                    resultmsg,
                    RawMessage.message_id,
                    ParseMode : TgApi.PARSEMODE_MARKDOWN
                    );
                return new CallbackMessage() { StopProcess = true };
            }
            return new CallbackMessage();
        }

        public CallbackMessage OnGroupMemberLeftReceive(TgMessage RawMessage, string JsonMessage, UserInfo JoinedUser)
        {
            return new CallbackMessage();
        }

        public CallbackMessage OnSupergroupMemberLeftReceive(TgMessage RawMessage, string JsonMessage, UserInfo JoinedUser)
        {
            return new CallbackMessage();
        }
    }
}
