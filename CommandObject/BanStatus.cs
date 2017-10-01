using CNBlackListSoamChecker.DbManager;
using ReimuAPI.ReimuBase;
using ReimuAPI.ReimuBase.TgData;

namespace CNBlackListSoamChecker.CommandObject
{
    internal class BanStatus
    {
        internal bool banstatus(TgMessage RawMessage)
        {
            int banstatSpace = RawMessage.text.IndexOf(" ");
            if (banstatSpace == -1)
            {
                string banmsg = "";
                BanUser ban;
                ban = Temp.GetDatabaseManager().GetUserBanStatus(RawMessage.GetSendUser().id);
                banmsg = "发送者：\n" + RawMessage.GetSendUser().GetUserTextInfo() + "\n\n" + ban.GetBanMessage();
                if (ban.Ban == 0)
                {
                    banmsg += "\n\n对于被封禁的用户，您可以通过 [点击这里](https://t.me/" + TgApi.getDefaultApiConnection().getMe().username + "?start=soam_req_unban) 以请求解封。";
                }
                if (RawMessage.reply_to_message != null)
                {
                    ban = Temp.GetDatabaseManager().GetUserBanStatus(RawMessage.reply_to_message.GetSendUser().id);
                    banmsg += "\n\n------\n\n被回复的消息的原发送用户：\n" +
                        RawMessage.reply_to_message.GetSendUser().GetUserTextInfo() + "\n\n" +
                        ban.GetBanMessage();
                    if (RawMessage.reply_to_message.forward_from != null)
                    {
                        ban = Temp.GetDatabaseManager().GetUserBanStatus(RawMessage.reply_to_message.forward_from.id);
                        banmsg += "\n\n------\n\n被回复的消息转发自用户：\n" +
                            RawMessage.reply_to_message.forward_from.GetUserTextInfo() + "\n\n" +
                            ban.GetBanMessage();
                    }
                }
                TgApi.getDefaultApiConnection().sendMessage(RawMessage.GetMessageChatInfo().id, banmsg, RawMessage.message_id);
                return true;
            }
            else
            {
                if (int.TryParse(RawMessage.text.Substring(banstatSpace + 1), out int userid))
                {
                    BanUser ban = Temp.GetDatabaseManager().GetUserBanStatus(userid);
                    TgApi.getDefaultApiConnection().sendMessage(RawMessage.GetMessageChatInfo().id, "这位用户" + ban.GetBanMessage(), RawMessage.message_id);
                    return true;
                }
                else
                {
                    TgApi.getDefaultApiConnection().sendMessage(RawMessage.GetMessageChatInfo().id, "您的输入有误，请输入正确的 UID", RawMessage.message_id);
                    return true;
                }
            }
        }
    }
}
