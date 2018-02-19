using System.Threading.Tasks;
using ReimuAPI.ReimuBase;
using ReimuAPI.ReimuBase.Interfaces;
using ReimuAPI.ReimuBase.TgData;
using TWBlackListSoamChecker.DbManager;
using System.Threading;

namespace TWBlackListSoamChecker
{
    internal class MemberJoinReceiver : IMemberJoinLeftListener
    {
        public CallbackMessage OnGroupMemberJoinReceive(TgMessage RawMessage, string JsonMessage, UserInfo JoinedUser)
        {
            return OnSupergroupMemberJoinReceive(RawMessage, JsonMessage, JoinedUser);
        }

        public CallbackMessage OnSupergroupMemberJoinReceive(TgMessage RawMessage, string JsonMessage,
            UserInfo JoinedUser)
        {
            DatabaseManager dbmgr = Temp.GetDatabaseManager();
            GroupCfg groupCfg = dbmgr.GetGroupConfig(RawMessage.GetMessageChatInfo().id);

            if (groupCfg.AntiBot == 0 && JoinedUser.is_bot && !TgApi.getDefaultApiConnection()
                    .checkIsAdmin(RawMessage.GetMessageChatInfo().id, RawMessage.from.id))
            {
                SetActionResult result = TgApi.getDefaultApiConnection()
                    .kickChatMember(RawMessage.GetMessageChatInfo().id, JoinedUser.id, GetTime.GetUnixTime() + 86400);
                if (result.ok)
                    TgApi.getDefaultApiConnection().sendMessage(
                        RawMessage.GetMessageChatInfo().id,
                        "機器人 : " + JoinedUser.GetUserTextInfo() + "\n由於開啟了 AntiBot ，已自動移除機器人。"
                    );
                else
                    TgApi.getDefaultApiConnection().sendMessage(
                        RawMessage.GetMessageChatInfo().id,
                        "機器人 : " + JoinedUser.GetUserTextInfo() + "\n由於開啟了 AntiBot ，但沒有 (Ban User) 權限，請設定正確的權限。"
                    );

                new Task(() =>
                {
                    long banUtilTime = GetTime.GetUnixTime() + 86400;
                    Temp.GetDatabaseManager().BanUser(
                        0,
                        RawMessage.GetSendUser().id,
                        0,
                        banUtilTime,
                        "自動封鎖 - 拉入機器人",
                        RawMessage.GetMessageChatInfo().id,
                        0,
                        RawMessage.GetSendUser()
                    );
                }).Start();
            }

            if (JoinedUser.id == TgApi.getDefaultApiConnection().getMe().id)
            {
                if(RAPI.getIsBlockGroup(RawMessage.GetMessageChatInfo().id)){
                    new Thread(delegate()
                    {
                        TgApi.getDefaultApiConnection().sendMessage(RawMessage.GetMessageChatInfo().id, "此群組禁止使用本服務。");
                        Thread.Sleep(2000);
                        TgApi.getDefaultApiConnection().leaveChat(RawMessage.GetMessageChatInfo().id);
                    }).Start();
                    return new CallbackMessage();
                }
                TgApi.getDefaultApiConnection().sendMessage(
                    RawMessage.GetMessageChatInfo().id,
                    "歡迎使用 @" + TgApi.getDefaultApiConnection().getMe().username + "\n" +
                    "1.請在群組中给予 @" + TgApi.getDefaultApiConnection().getMe().username + " 管理員權限\n" +
                    "2.使用 /help 可查閱使用說明\n" +
                    "預設開啟的功能有 BlackList AutoKick AntiHalal SubscribeBanList，可以根據需要來調整。\n\n" +
                    "注意 : 加入機器人即同意讓渡部分 Ban Users 權限予本項目組，並授權本組依據 @J_Court 置頂規定，代表群管理對群組內成員逕行封鎖\n" +
                    "如不同意請立即移除此機器人，且禁止違背群主意願私自添加",
                    RawMessage.message_id
                );
                return new CallbackMessage();
            }

            if (Temp.DisableBanList) return new CallbackMessage();
            if (Temp.CourtGroupName != null && RawMessage.GetMessageChatInfo().username == Temp.CourtGroupName)
            {
                BanUser banUser = dbmgr.GetUserBanStatus(JoinedUser.id);
                if (banUser.Ban == 0)
                {
                    string resultmsg = "這位使用者被封鎖了";
                    resultmsg += "，原因 : \n" + RAPI.escapeMarkdown(banUser.Reason) + "\nID : " + JoinedUser.id;
                    if (banUser.ChannelMessageID != 0 && Temp.MainChannelName != null)
                        resultmsg += "\n參考 : https://t.me/" + RAPI.escapeMarkdown(Temp.MainChannelName) + "/" + banUser.ChannelMessageID ;
                    TgApi.getDefaultApiConnection().sendMessage(
                        RawMessage.GetMessageChatInfo().id,
                        resultmsg,
                        RawMessage.message_id,
                        TgApi.PARSEMODE_MARKDOWN
                    );
                }

                return new CallbackMessage();
            }

            if (groupCfg.BlackList == 0)
            {
                BanUser banUser = dbmgr.GetUserBanStatus(JoinedUser.id);
                string resultmsg = "";
                if (banUser.Ban == 0)
                {
                    string banReason;
                    if (banUser.ChannelMessageID != 0 && Temp.MainChannelName != null)
                        banReason = " [原因請點選這裡查看](https://t.me/" + Temp.MainChannelName + "/" +
                                    banUser.ChannelMessageID + ")\n";
                    else
                        banReason = "\n原因 : " + RAPI.escapeMarkdown(banUser.Reason) + "\n";
                    if (banUser.Level == 0)
                    {
                        resultmsg += "警告 : 這個使用者「將會」對群組造成負面影響\n" +
                                     banReason + 
                                     "若有開啟 AutoKick 功能，將會自動踢出使用者\n" +
                                     "被封鎖的用戶，可以到 [這個群組](https://t.me/" + Temp.CourtGroupName + ") 尋求申訴";
                        if (groupCfg.AutoKick == 0)
                            try
                            {
                                SetActionResult result = TgApi.getDefaultApiConnection().kickChatMember(
                                    RawMessage.GetMessageChatInfo().id,
                                    JoinedUser.id,
                                    GetTime.GetUnixTime() + 86400
                                );
                                if (!result.ok)
                                    resultmsg += "\n注意 : 由於開啟了 AutoKick 但沒有 Ban Users 權限" +
                                                 "，請關閉此功能或給予權限（Ban users）。";
                            }
                            catch
                            {
                            }
                    }
                    else if (banUser.Level == 1)
                    {
                        resultmsg += "警告 : 這個使用者「可能」對群組造成負面影響" + RAPI.escapeMarkdown(banReason) + "\n" +
                                     "請群組管理員多加留意\n" +
                                     "對於被警告的使用者，你可以通過 [這個群組](https://t.me/" + Temp.CourtGroupName + ") 以請求解除。";
                    }
                }
                else
                {
                    return new CallbackMessage();
                }

                TgApi.getDefaultApiConnection().sendMessage(
                    RawMessage.GetMessageChatInfo().id,
                    resultmsg,
                    RawMessage.message_id,
                    TgApi.PARSEMODE_MARKDOWN
                );
                return new CallbackMessage {StopProcess = true};
            }

            return new CallbackMessage();
        }

        public CallbackMessage OnGroupMemberLeftReceive(TgMessage RawMessage, string JsonMessage, UserInfo JoinedUser)
        {
            return new CallbackMessage();
        }

        public CallbackMessage OnSupergroupMemberLeftReceive(TgMessage RawMessage, string JsonMessage,
            UserInfo JoinedUser)
        {
            return new CallbackMessage();
        }
    }
}