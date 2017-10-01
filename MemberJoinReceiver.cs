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
                    "欢迎使用 @" + TgApi.getDefaultApiConnection().getMe().username + "\n" +
                    "请您进行一些设置：\n" +
                    "1.在您的群组中给予 @" + TgApi.getDefaultApiConnection().getMe().username + " 管理员权限\n" +
                    "2.使用 /soamenable 启用一些功能\n" +
                    "3.Enjoy it!\n\n" +
                    "注: 默认开启的功能有 BlackList AutoKick AntiHalal AutoDeleteSpamMessage 这 4 个，您可以根据您的需要来禁用或启用。",
                    RawMessage.message_id
                    );
                return new CallbackMessage();
            }
            if (Temp.DisableBanList)
            {
                return new CallbackMessage();
            }
            DatabaseManager dbmgr = Temp.GetDatabaseManager();
            if (RawMessage.GetMessageChatInfo().id == -1001079439348)
            {
                BanUser banUser = dbmgr.GetUserBanStatus(JoinedUser.id);
                if (banUser.Ban == 0)
                {
                    string resultmsg = "这位用户被封禁了";
                    if (banUser.ChannelMessageID != 0)
                    {
                        resultmsg += "， [原因请点击这里查看](https://t.me/" + Temp.MainChannelName + "/" + banUser.ChannelMessageID + ")";
                    }
                    else
                    {
                        resultmsg += "，原因是：\n" + banUser.Reason;
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
                    TgApi.getDefaultApiConnection().restrictChatMember(
                                RawMessage.GetMessageChatInfo().id,
                                JoinedUser.id,
                                GetTime.GetUnixTime() + 60
                                );
                    TgApi.getDefaultApiConnection().sendMessage(
                        RawMessage.GetMessageChatInfo().id,
                        "您未被封禁，请闲杂人等退群。如果您想加入这个群组，您可以去多点群发一些广告，然后您被 ban 了就能加入了。\n\n" +
                        "您将在 60 秒后自动退群。",
                        RawMessage.message_id,
                        ParseMode: TgApi.PARSEMODE_MARKDOWN
                        );
                    new Thread(delegate () {
                        Thread.Sleep(60000);
                        TgApi.getDefaultApiConnection().kickChatMember(
                            RawMessage.GetMessageChatInfo().id,
                            JoinedUser.id,
                            GetTime.GetUnixTime() + 60
                            );
                    }).Start();
                }
                return new CallbackMessage();
            }
            GroupCfg groupCfg = dbmgr.GetGroupConfig(RawMessage.GetMessageChatInfo().id);
            if (groupCfg.BlackList == 0)
            {
                BanUser banUser = dbmgr.GetUserBanStatus(JoinedUser.id);
                string resultmsg = "警告: ";
                if (banUser.Ban == 0)
                {
                    string banReason;
                    if (banUser.ChannelMessageID != 0)
                    {
                        banReason = "， [原因请点击这里查看](https://t.me/" + Temp.MainChannelName + "/" + banUser.ChannelMessageID + ")";
                    }
                    else
                    {
                        banReason = "\n\n原因是：\n" + banUser.Reason;
                    }
                    if (banUser.Level == 0)
                    {
                        resultmsg += "这位用户可能存在风险，已被封禁" + banReason + "\n\n" +
                            "对于被封禁的用户，您可以通过 [点击这里](https://t.me/" + TgApi.getDefaultApiConnection().getMe().username + "?start=soam_req_unban) 以请求解封。";
                        if (groupCfg.AutoKick == 0)
                        {
                            SetActionResult result = TgApi.getDefaultApiConnection().kickChatMember(
                                RawMessage.GetMessageChatInfo().id,
                                JoinedUser.id,
                                GetTime.GetUnixTime() + 86400
                                );
                            if (!result.ok)
                            {
                                resultmsg += "\n\n请注意: 您的群组当前打开了自动移除危险成员但机器人没有相应的管理员权限" +
                                    "，请您关闭此功能或者将机器人设置为管理员并给予相应的权限（Ban users）。";
                            }
                        }
                    }
                    else if (banUser.Level == 1)
                    {
                        resultmsg += "这位用户可能存在不良行为" + banReason  + "\n\n" +
                            "对于群组的管理员: 您可以观察这位用户在您的群组当中是否存在不良行为后再决定是否移除该成员\n"+
                            "对于被封禁的用户，您可以通过 [点击这里](https://t.me/" + TgApi.getDefaultApiConnection().getMe().username + "?start=soam_req_unban) 以请求解封。";
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
