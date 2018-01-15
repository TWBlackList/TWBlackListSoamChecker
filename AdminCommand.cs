using CNBlackListSoamChecker.CommandObject;
using ReimuAPI.ReimuBase;
using ReimuAPI.ReimuBase.TgData;

namespace CNBlackListSoamChecker
{
    class AdminCommand
    {
        string Disabled_Ban_Msg = "非常抱歉，目前版本已關閉封鎖用戶的功能，請聯絡管理員開啟此功能。";
        internal bool AdminCommands(TgMessage RawMessage, string JsonMessage, string Command)
        {
            if (!RAPI.getIsBotAdmin(RawMessage.GetSendUser().id))
            {
                
        if (RAPI.getIsBotOP(RawMessage.GetSendUser().id)){
            switch (Command)
            {
                case "/say":
                    new BroadCast().BroadCast_Status(RawMessage);
                    throw new StopProcessException();
                case "/addop":
                    if (Temp.DisableBanList)
                    {
                        TgApi.getDefaultApiConnection().sendMessage(RawMessage.chat.id,Disabled_Ban_Msg,RawMessage.message_id);
                        break;
                    }
                    new OP().AddOP(RawMessage);
                    throw new StopProcessException();
                case "/delop":
                    if (Temp.DisableBanList)
                    {
                        TgApi.getDefaultApiConnection().sendMessage(RawMessage.chat.id,Disabled_Ban_Msg,RawMessage.message_id);
                        break;
                    }
                    new OP().DelOP(RawMessage);
                    throw new StopProcessException();

            }
            return false;}else{return false;}
            }
            switch (Command)
            {
                case "/say":
                    new BroadCast().BroadCast_Status(RawMessage);
                    throw new StopProcessException();
                case "/sdall":
                    if (Temp.DisableBanList)
                    {
                        TgApi.getDefaultApiConnection().sendMessage(RawMessage.chat.id,Disabled_Ban_Msg,RawMessage.message_id);
                        break;
                    }
                    new OP().SDAll(RawMessage);
                    throw new StopProcessException();
                case "/seall":
                    if (Temp.DisableBanList)
                    {
                        TgApi.getDefaultApiConnection().sendMessage(RawMessage.chat.id,Disabled_Ban_Msg,RawMessage.message_id);
                        break;
                    }
                    new OP().SEAll(RawMessage);
                    throw new StopProcessException();
                case "/addop":
                    if (Temp.DisableBanList)
                    {
                        TgApi.getDefaultApiConnection().sendMessage(RawMessage.chat.id,Disabled_Ban_Msg,RawMessage.message_id);
                        break;
                    }
                    new OP().AddOP(RawMessage);
                    throw new StopProcessException();
                case "/delop":
                    if (Temp.DisableBanList)
                    {
                        TgApi.getDefaultApiConnection().sendMessage(RawMessage.chat.id,Disabled_Ban_Msg,RawMessage.message_id);
                        break;
                    }
                    new OP().DelOP(RawMessage);
                    throw new StopProcessException();
                case "/twban":
                    if (Temp.DisableBanList || Temp.DisableAdminTools)
                    {
                        TgApi.getDefaultApiConnection().sendMessage(RawMessage.chat.id,Disabled_Ban_Msg,RawMessage.message_id);
                        break;
                    }
                    new BanUserCommand().Ban(RawMessage, JsonMessage, Command);
                    throw new StopProcessException();
                case "/twunban":
                    if (Temp.DisableBanList)
                    {
                        TgApi.getDefaultApiConnection().sendMessage(RawMessage.chat.id,Disabled_Ban_Msg,RawMessage.message_id);
                        break;
                    }
                    new UnbanUserCommand().Unban(RawMessage);
                    throw new StopProcessException();
                case "/getspamstr":
                    if (RAPI.getIsBotOP(RawMessage.GetSendUser().id)){
                        if (Temp.DisableBanList)
                        {
                            TgApi.getDefaultApiConnection().sendMessage(RawMessage.chat.id,Disabled_Ban_Msg,RawMessage.message_id);
                            break;
                        }
                        new SpamStringManager().GetName(RawMessage);
                    }else{
                        TgApi.getDefaultApiConnection().sendMessage(RawMessage.chat.id,"你沒有權限",RawMessage.message_id);
                        break;
                    }
                    throw new StopProcessException();
                case "/__getallspamstr":
                    if (Temp.DisableBanList)
                    {
                        TgApi.getDefaultApiConnection().sendMessage(RawMessage.chat.id,Disabled_Ban_Msg,RawMessage.message_id);
                        break;
                    }
                    new SpamStringManager().GetAllInfo(RawMessage);
                    return true;
                case "/__kick":
                    if (Temp.DisableBanList)
                    {
                        TgApi.getDefaultApiConnection().sendMessage(RawMessage.chat.id,Disabled_Ban_Msg,RawMessage.message_id);
                        break;
                    }
                    //new SpamStringManager().GetAllInfo(RawMessage);
                    return true;
                case "/addspamstr":
                    if (RAPI.getIsBotOP(RawMessage.GetSendUser().id)){
                        if (Temp.DisableBanList)
                        {
                            TgApi.getDefaultApiConnection().sendMessage(RawMessage.chat.id,Disabled_Ban_Msg,RawMessage.message_id);
                            break;
                        }
                        new SpamStringManager().Add(RawMessage);
                    }else{
                        TgApi.getDefaultApiConnection().sendMessage(RawMessage.chat.id,"你沒有權限",RawMessage.message_id);
                        break;
                    }
                    throw new StopProcessException();
                case "/delspamstr":
                    if (RAPI.getIsBotOP(RawMessage.GetSendUser().id)){
                        if (Temp.DisableBanList)
                        {
                            TgApi.getDefaultApiConnection().sendMessage(RawMessage.chat.id,Disabled_Ban_Msg,RawMessage.message_id);
                            break;
                        }
                        new SpamStringManager().Remove(RawMessage);
                    }else{
                        TgApi.getDefaultApiConnection().sendMessage(RawMessage.chat.id,"你沒有權限",RawMessage.message_id);
                        break;
                    }
                    throw new StopProcessException();
                case "/getspampoints":
                    if (Temp.DisableBanList)
                    {
                        TgApi.getDefaultApiConnection().sendMessage(RawMessage.chat.id,Disabled_Ban_Msg,RawMessage.message_id);
                        break;
                    }
                    new SpamStringManager().GetSpamPoints(RawMessage);
                    throw new StopProcessException();
                case "/jsonencode":
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
                        ParseMode: TgApi.PARSEMODE_HTML
                        );
                    throw new StopProcessException();
            }
            return false;
        }
    }
}
