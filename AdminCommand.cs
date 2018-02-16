using CNBlackListSoamChecker.CommandObject;
using ReimuAPI.ReimuBase;
using ReimuAPI.ReimuBase.TgData;

namespace TWBlackListSoamChecker
{
    internal class AdminCommand
    {
        private readonly string Disabled_Ban_Msg = "非常抱歉，目前版本已關閉封鎖用戶的功能，請聯絡管理員開啟此功能。";

        internal bool AdminCommands(TgMessage RawMessage, string JsonMessage, string Command)
        {
            if (!RAPI.getIsBotOP(RawMessage.GetSendUser().id))
                if (RAPI.getIsBotAdmin(RawMessage.GetSendUser().id))
                {
                    switch (Command)
                    {
                        case "/say":
                            new BroadCast().BroadCast_Status(RawMessage);
                            throw new StopProcessException();
                        case "/addop":
                            new OP().addOP(RawMessage);
                            throw new StopProcessException();
                        case "/delop":
                            new OP().delOP(RawMessage);
                            throw new StopProcessException();
                    }

                    return false;
                }
                else
                {
                    return false;
                }

            switch (Command)
            {
                case "/say":
                    new BroadCast().BroadCast_Status(RawMessage);
                    throw new StopProcessException();
                case "/sdall":
                    new OP().SDAll(RawMessage);
                    throw new StopProcessException();
                case "/seall":
                    new OP().SEAll(RawMessage);
                    throw new StopProcessException();
                case "/addop":
                    new OP().addOP(RawMessage);
                    throw new StopProcessException();
                case "/delop":
                    new OP().delOP(RawMessage);
                    throw new StopProcessException();
                case "/addwl":
                    new Whitelist().addWhitelist(RawMessage);
                    throw new StopProcessException();
                case "/delwl":
                    new Whitelist().deleteWhitelist(RawMessage);
                    throw new StopProcessException();
                case "/lswl":
                    new Whitelist().listWhitelist(RawMessage);
                    throw new StopProcessException();
                case "/block":
                    new BlockGroup().addBlockGroup(RawMessage);
                    throw new StopProcessException();
                case "/unblock":
                    new BlockGroup().deleteBlockGroup(RawMessage);
                    throw new StopProcessException();
                case "/blocks":
                    new BlockGroup().listBlockGroup(RawMessage);
                    throw new StopProcessException();
                case "/suban":
                    if (RAPI.getIsBotAdmin(RawMessage.GetSendUser().id))
                    {
                        if (Temp.DisableBanList)
                        {
                            TgApi.getDefaultApiConnection().sendMessage(RawMessage.chat.id, Disabled_Ban_Msg,
                                RawMessage.message_id);
                            break;
                        }

                        new BanMultiUserCommand().BanMulti(RawMessage, JsonMessage, Command);
                    }
                    else
                    {
                        TgApi.getDefaultApiConnection().sendMessage(RawMessage.chat.id, "你沒有權限", RawMessage.message_id);
                        break;
                    }

                    throw new StopProcessException();
                case "/ban":
                    if (Temp.DisableBanList || Temp.DisableAdminTools)
                    {
                        TgApi.getDefaultApiConnection()
                            .sendMessage(RawMessage.chat.id, Disabled_Ban_Msg, RawMessage.message_id);
                        break;
                    }

                    new BanUserCommand().Ban(RawMessage, JsonMessage, Command);
                    throw new StopProcessException();
                case "/cnban":
                    if (Temp.DisableBanList || Temp.DisableAdminTools)
                    {
                        TgApi.getDefaultApiConnection()
                            .sendMessage(RawMessage.chat.id, Disabled_Ban_Msg, RawMessage.message_id);
                        break;
                    }

                    new BanUserCommand().Ban(RawMessage, JsonMessage, Command);
                    throw new StopProcessException();
                case "/suunban":
                    if (RAPI.getIsBotAdmin(RawMessage.GetSendUser().id))
                    {
                        if (Temp.DisableBanList)
                        {
                            TgApi.getDefaultApiConnection().sendMessage(RawMessage.chat.id, Disabled_Ban_Msg,
                                RawMessage.message_id);
                            break;
                        }

                        new UnBanMultiUserCommand().UnbanMulti(RawMessage);
                    }
                    else
                    {
                        TgApi.getDefaultApiConnection().sendMessage(RawMessage.chat.id, "你沒有權限", RawMessage.message_id);
                        break;
                    }

                    throw new StopProcessException();
                case "/unban":
                    if (Temp.DisableBanList)
                    {
                        TgApi.getDefaultApiConnection()
                            .sendMessage(RawMessage.chat.id, Disabled_Ban_Msg, RawMessage.message_id);
                        break;
                    }

                    new UnbanUserCommand().Unban(RawMessage);
                    throw new StopProcessException();
                case "/cnunban":
                    if (Temp.DisableBanList)
                    {
                        TgApi.getDefaultApiConnection()
                            .sendMessage(RawMessage.chat.id, Disabled_Ban_Msg, RawMessage.message_id);
                        break;
                    }

                    new UnbanUserCommand().Unban(RawMessage);
                    throw new StopProcessException();
                case "/getspamstr":
                    if (RAPI.getIsBotAdmin(RawMessage.GetSendUser().id))
                    {
                        if (Temp.DisableBanList)
                        {
                            TgApi.getDefaultApiConnection().sendMessage(RawMessage.chat.id, Disabled_Ban_Msg,
                                RawMessage.message_id);
                            break;
                        }

                        new SpamStringManager().GetName(RawMessage);
                    }
                    else
                    {
                        TgApi.getDefaultApiConnection().sendMessage(RawMessage.chat.id, "你沒有權限", RawMessage.message_id);
                        break;
                    }

                    throw new StopProcessException();
                case "/__getallspamstr": //暫時不用
                    if (Temp.DisableBanList)
                    {
                        TgApi.getDefaultApiConnection()
                            .sendMessage(RawMessage.chat.id, Disabled_Ban_Msg, RawMessage.message_id);
                        break;
                    }

                    new SpamStringManager().GetAllInfo(RawMessage);
                    return true;
                case "/__kick": //暫時不用
                    if (Temp.DisableBanList)
                    {
                        TgApi.getDefaultApiConnection()
                            .sendMessage(RawMessage.chat.id, Disabled_Ban_Msg, RawMessage.message_id);
                        break;
                    }

                    //new SpamStringManager().GetAllInfo(RawMessage);
                    return true;
                case "/addspamstr":
                    if (RAPI.getIsBotAdmin(RawMessage.GetSendUser().id))
                    {
                        if (Temp.DisableBanList)
                        {
                            TgApi.getDefaultApiConnection().sendMessage(RawMessage.chat.id, Disabled_Ban_Msg,
                                RawMessage.message_id);
                            break;
                        }

                        new SpamStringManager().Add(RawMessage);
                    }
                    else
                    {
                        TgApi.getDefaultApiConnection().sendMessage(RawMessage.chat.id, "你沒有權限", RawMessage.message_id);
                        break;
                    }

                    throw new StopProcessException();
                case "/delspamstr":
                    if (RAPI.getIsBotAdmin(RawMessage.GetSendUser().id))
                    {
                        if (Temp.DisableBanList)
                        {
                            TgApi.getDefaultApiConnection().sendMessage(RawMessage.chat.id, Disabled_Ban_Msg,
                                RawMessage.message_id);
                            break;
                        }

                        new SpamStringManager().Remove(RawMessage);
                    }
                    else
                    {
                        TgApi.getDefaultApiConnection().sendMessage(RawMessage.chat.id, "你沒有權限", RawMessage.message_id);
                        break;
                    }

                    throw new StopProcessException();
                case "/getspampoints":
                    if (Temp.DisableBanList)
                    {
                        TgApi.getDefaultApiConnection()
                            .sendMessage(RawMessage.chat.id, Disabled_Ban_Msg, RawMessage.message_id);
                        break;
                    }

                    new SpamStringManager().GetSpamPoints(RawMessage);
                    throw new StopProcessException();
                case "/jsonencode": //這不知道是三小用的，CBLR也沒看過 ~"~
                    int spacePath = RawMessage.text.IndexOf(" ");
                    if (spacePath == -1)
                    {
                        TgApi.getDefaultApiConnection().sendMessage(
                            RawMessage.GetMessageChatInfo().id,
                            "您的輸入有錯誤",
                            RawMessage.message_id
                        );
                        throw new StopProcessException();
                    }

                    string jsonText = RawMessage.text.Substring(spacePath + 1);
                    TgApi.getDefaultApiConnection().sendMessage(
                        RawMessage.GetMessageChatInfo().id,
                        "<code>" + TgApi.getDefaultApiConnection().jsonEncode(jsonText) + "</code>",
                        RawMessage.message_id,
                        TgApi.PARSEMODE_HTML
                    );
                    throw new StopProcessException();
            }

            return false;
        }
    }
}