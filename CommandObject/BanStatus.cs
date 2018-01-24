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
                        banmsg = "發送者 : " + RawMessage.GetSendUser().GetUserTextInfo() + "\n" + ban.GetBanMessage();
                        if (ban.Ban == 0)
                        {
                            banmsg += "\n對於被封鎖的使用者，你可以通過 [點選這裡](https://t.me/" + TgApi.getDefaultApiConnection().getMe().username + "?start=soam_req_unban) 以請求解除。";
                        }
                        if (RawMessage.reply_to_message != null)
                        {
                            ban = Temp.GetDatabaseManager().GetUserBanStatus(RawMessage.reply_to_message.GetSendUser().id);
                            banmsg += "\n\n被回覆的訊息的原發送使用者 : " +
                                RawMessage.reply_to_message.GetSendUser().GetUserTextInfo() + "\n" +
                                ban.GetBanMessage();
                            if (RawMessage.reply_to_message.forward_from != null)
                            {
                                ban = Temp.GetDatabaseManager().GetUserBanStatus(RawMessage.reply_to_message.forward_from.id);
                                banmsg += "\n\n被回覆的訊息轉發自使用者 : " +
                                    RawMessage.reply_to_message.forward_from.GetUserTextInfo() + "\n" +
                                    ban.GetBanMessage();
                            }
                        }
                        TgApi.getDefaultApiConnection().sendMessage(RawMessage.GetMessageChatInfo().id, banmsg, RawMessage.message_id,ParseMode : TgApi.PARSEMODE_MARKDOWN);
                        return true;
                    }
                    else
                    {
                        if (int.TryParse(RawMessage.text.Substring(banstatSpace + 1), out int userid))
                        {
                            BanUser ban = Temp.GetDatabaseManager().GetUserBanStatus(userid);
                            TgApi.getDefaultApiConnection().sendMessage(RawMessage.GetMessageChatInfo().id, "這位使用者" + ban.GetBanMessage(), RawMessage.message_id);
                            return true;
                        }
                        else
                        {
                            TgApi.getDefaultApiConnection().sendMessage(RawMessage.GetMessageChatInfo().id, "您的輸入有錯誤，請輸入正確的 UserID", RawMessage.message_id);
                            return true;
                        }
                    }
                }
            }
        }
