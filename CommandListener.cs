using System;
using ReimuAPI.ReimuBase;
using ReimuAPI.ReimuBase.Interfaces;
using ReimuAPI.ReimuBase.TgData;
using TWBlackListSoamChecker.CommandObject;
using TWBlackListSoamChecker.DbManager;

namespace TWBlackListSoamChecker
{
    internal class CommandListener : ICommandReceiver
    {
        public CommandListener()
        {
            new DatabaseManager().checkdb();
        }

        public CallbackMessage OnGroupCommandReceive(TgMessage RawMessage, string JsonMessage, string Command)
        {
            return OnSupergroupCommandReceive(RawMessage, JsonMessage, Command);
        }

        public CallbackMessage OnPrivateCommandReceive(TgMessage RawMessage, string JsonMessage, string Command)
        {
            try
            {
                if (SharedCommand(RawMessage, JsonMessage, Command)) return new CallbackMessage();
                return new CallbackMessage();
            }
            catch (StopProcessException)
            {
                return new CallbackMessage {StopProcess = true};
            }
            catch (Exception e)
            {
                RAPI.GetExceptionListener().OnException(e, JsonMessage);
                throw e;
            }
        }

        public CallbackMessage OnSupergroupCommandReceive(TgMessage RawMessage, string JsonMessage, string Command)
        {
            try
            {
                GroupCfg cfg = Temp.GetDatabaseManager().GetGroupConfig(RawMessage.chat.id);
                if (cfg.AdminOnly == 0)
                {
                    if (TgApi.getDefaultApiConnection().checkIsAdmin(RawMessage.chat.id, RawMessage.from.id) ||
                        RAPI.getIsBotAdmin(RawMessage.from.id) || RAPI.getIsBotOP(RawMessage.from.id))
                    {
                    }
                    else
                    {
                        return new CallbackMessage {StopProcess = true};
                    }
                }
                if (SharedCommand(RawMessage, JsonMessage, Command)) return new CallbackMessage();
                switch (Command)
                {
                    
                    case "/leave":
                        new LeaveCommand().Leave(RawMessage);
                        break;
                    case "/soamenable":
                        new SoamManager().SoamEnable(RawMessage);
                        break;
                    case "/soamdisable":
                        new SoamManager().SoamDisable(RawMessage);
                        break;
                    case "/__get_exception":
                        throw new Exception();
                    case "/soamstat":
                    case "/soamstatus":
                        new SoamManager().SoamStatus(RawMessage);
                        break;
                    //case "/cnkick":
                    //    if (Temp.DisableBanList)
                    //    {
                    //        TgApi.getDefaultApiConnection().sendMessage(
                    //            RawMessage.chat.id,
                    //            "非常抱歉，目前版本已禁用封鎖用戶的功能，請聯絡管理員開啟此功能。",
                    //            RawMessage.message_id
                    //            );
                    //        break;
                    //    }
                    //    if (RawMessage.reply_to_message == null)
                    //    {
                    //        TgApi.getDefaultApiConnection().sendMessage(RawMessage.chat.id, "請回覆一則訊息", RawMessage.message_id);
                    //        return new CallbackMessage();
                    //    }
                    //    BanUser ban = Temp.GetDatabaseManager().GetUserBanStatus(RawMessage.reply_to_message.from.id);
                    //    if (ban.Ban == 0)
                    //    {
                    //        if (ban.Level == 0)
                    //        {
                    //            SetActionResult bkick_result = TgApi.getDefaultApiConnection().kickChatMember(
                    //                RawMessage.chat.id,
                    //                RawMessage.reply_to_message.from.id,
                    //                GetTime.GetUnixTime() + 86400
                    //                );
                    //            if (bkick_result.ok)
                    //            {
                    //                TgApi.getDefaultApiConnection().sendMessage(RawMessage.chat.id, "已移除", RawMessage.message_id);
                    //                return new CallbackMessage();
                    //            }
                    //            else
                    //            {
                    //                TgApi.getDefaultApiConnection().sendMessage(
                    //                    RawMessage.chat.id,
                    //                    "無法移除，可能是機器人沒有適當的管理員權限。",
                    //                    RawMessage.message_id
                    //                    );
                    //                return new CallbackMessage();
                    //            }
                    //        }
                    //        else
                    //        {
                    //            TgApi.getDefaultApiConnection().sendMessage(
                    //                RawMessage.chat.id,
                    //                "無法移除，因為此使用者不在黑名單，請您聯絡群組的管理員處理。" +
                    //                "如果你認為這位使用者將會影響大量群組，您可連絡 @" + Temp.MainChannelName + " 提供的群組。",
                    //                RawMessage.message_id
                    //                );
                    //            return new CallbackMessage();
                    //        }
                    //    }
                    //    else
                    //    {
                    //        TgApi.getDefaultApiConnection().sendMessage(
                    //            RawMessage.chat.id,
                    //            "無法移除，因為此使用者不在黑名單，請您聯絡群組的管理員處理。" +
                    //            "如果你認為這位使用者將會影響大量群組，您可連絡 @" + Temp.MainChannelName + " 提供的群組。",
                    //            RawMessage.message_id
                    //            );
                    //        return new CallbackMessage();
                    //    }
                }

                return new CallbackMessage();
            }
            catch (StopProcessException)
            {
                return new CallbackMessage {StopProcess = true};
            }
            catch (Exception e)
            {
                RAPI.GetExceptionListener().OnException(e, JsonMessage);
                throw e;
            }
        }

        private bool SharedCommand(TgMessage RawMessage, string JsonMessage, string Command)
        {
            switch (Command)
            {
                case "/user":
                    return new UserCommand().User(RawMessage);
                case "/lsop":
                    return new OP().lsOP(RawMessage);
                case "/help":
                    return new Help().HelpStatus(RawMessage);
                case "/banstat":
                case "/banstatus":
                case "/twbanstat":
                    if (Temp.DisableBanList)
                    {
                        TgApi.getDefaultApiConnection().sendMessage(
                            RawMessage.chat.id,
                            "非常抱歉，目前版本已禁用封鎖用戶的功能，請聯絡管理員開啟此功能。",
                            RawMessage.message_id
                        );
                        break;
                    }

                    return new BanStatus().banstatus(RawMessage);
                //case "/clickmetobesb"://垃圾功能，之後拔掉，希望不要爆炸！
                //    TgApi.getDefaultApiConnection().sendMessage(
                //        RawMessage.chat.id,
                //        "Success, now you are SB.",
                //        RawMessage.message_id
                //        );
                //    break;
            }

            return new AdminCommand().AdminCommands(RawMessage, JsonMessage, Command);
        }
    }
}