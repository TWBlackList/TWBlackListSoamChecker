using CNBlackListSoamChecker.CommandObject;
using CNBlackListSoamChecker.DbManager;
using ReimuAPI.ReimuBase;
using ReimuAPI.ReimuBase.Interfaces;
using ReimuAPI.ReimuBase.TgData;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace CNBlackListSoamChecker
{
    class SpamMessageDeleter : IOtherMessageReceiver
    {
        public CallbackMessage ReceiveAllNormalMessage(TgMessage BaseMessage, string JsonMessage)
        {
            if (BaseMessage.chat.type != "group" && BaseMessage.chat.type != "supergroup")
                return new CallbackMessage();
            string chatText = null;
            if (BaseMessage.text != null)
            {
                chatText = BaseMessage.text.ToLower();
            }
            else if (BaseMessage.caption != null)
            {
                chatText = BaseMessage.caption.ToLower();
            }
            else
            {
                return new CallbackMessage();
            }
            // Call Admin START
            int atAdminPath = chatText.IndexOf("@admin");
            if (atAdminPath != -1)
            {
                int textLen = chatText.Length;
                if (textLen == 6)
                {
                    CallAdmin(BaseMessage);
                }
                else if (textLen >= 8)
                {
                    if (atAdminPath == 0)
                    {
                        if (chatText[7] == ' ')
                        {
                            CallAdmin(BaseMessage);
                        }
                    }
                    else if (atAdminPath == textLen - 6)
                    {
                        if (chatText[textLen - 7] == ' ')
                        {
                            CallAdmin(BaseMessage);
                        }
                    }
                    else
                    {
                        if (chatText[atAdminPath - 1] == ' ' && chatText[atAdminPath + 7] == ' ')
                        {
                            CallAdmin(BaseMessage);
                        }
                    }
                }
            }
            // Call Admin END

            if (TgApi.getDefaultApiConnection().checkIsAdmin(BaseMessage.chat.id, BaseMessage.from.id))
            {
                return new CallbackMessage();
            }

            // ALTI HALAL AND INDIA START
            GroupCfg cfg = Temp.GetDatabaseManager().GetGroupConfig(BaseMessage.chat.id);
            if (cfg.AntiHalal == 0)
            {
                List<SpamMessage> spamMsgList = Temp.GetDatabaseManager().GetSpamMessageList();
                int halalPoints = new SpamMessageChecker().GetHalalPoints(chatText);
                int indiaPoints = new SpamMessageChecker().GetIndiaPoints(chatText);
                if (halalPoints >= 8 || indiaPoints >= 16)
                {

                    
                    //If not in ban status , ban user.
                    if (Temp.GetDatabaseManager().GetUserBanStatus(BaseMessage.from.id).Ban != 0)
                    {
                        new Task(() =>
                        {
                            Temp.GetDatabaseManager().BanUser(
                                    0,
                                    BaseMessage.from.id,
                                    1,
                                    0,
                                    "\n自動封鎖 : 台灣人無法理解的語言",
                                    BaseMessage.GetMessageChatInfo().id,
                                    BaseMessage.message_id,
                                    BaseMessage.from
                                    );
                        }).Start();
                    }

                    new Task(() => {
                        TgApi.getDefaultApiConnection().forwardMessage(
                            Temp.ReasonChannelID,
                            BaseMessage.GetMessageChatInfo().id,
                            BaseMessage.message_id);
                    }).Start();

                    //Kick user and delete spam message
                    new Task(() =>
                    {
                        TgApi.getDefaultApiConnection().kickChatMember(BaseMessage.chat.id, BaseMessage.from.id, 0);
                        TgApi.getDefaultApiConnection().deleteMessage(BaseMessage.chat.id, BaseMessage.message_id);
                    }).Start();

                    BanUser banstat = Temp.GetDatabaseManager().GetUserBanStatus(BaseMessage.GetSendUser().id);

                    if (banstat.Ban == 0)
                    {
                        TgApi.getDefaultApiConnection().kickChatMember(
                            BaseMessage.GetMessageChatInfo().id,
                            BaseMessage.GetSendUser().id,
                            GetTime.GetUnixTime() + 86400
                            );
                    }

                    //Send alert and delete alert after 60 second
                    new Thread(delegate () {
                        SendMessageResult autodeletespammessagesendresult = TgApi.getDefaultApiConnection().sendMessage(
                            BaseMessage.GetMessageChatInfo().id,
                            "偵測到台灣人無法理解的語言，已自動回報，如有誤報請加入 @" + Temp.ReportGroupName + " 以報告誤報。"
                            );
                        Thread.Sleep(60000);
                        TgApi.getDefaultApiConnection().deleteMessage(
                            autodeletespammessagesendresult.result.chat.id,
                            autodeletespammessagesendresult.result.message_id
                            );
                    }).Start();
                    return new CallbackMessage() { StopProcess = true };
                }
            }
            // ALTI HALAL AND INDIA END

            // AUTO DELETE SPAM MESSAGE START
            if (Temp.DisableBanList == false && cfg.AutoDeleteSpamMessage == 0)
            {
                List<SpamMessage> spamMsgList = Temp.GetDatabaseManager().GetSpamMessageList();
                int points = 0;
                foreach (SpamMessage smsg in spamMsgList)
                {
                    switch (smsg.Type)
                    {
                        case 0:
                            points =+ new SpamMessageChecker().GetEqualsPoints(smsg.Messages, chatText);
                            break;
                        case 1:
                            points =+ new SpamMessageChecker().GetRegexPoints(smsg.Messages, chatText);
                            break;
                        case 2:
                            points =+ new SpamMessageChecker().GetSpamPoints(smsg.Messages, chatText);
                            break;
                        case 3:
                            points =+ new SpamMessageChecker().GetIndexOfPoints(smsg.Messages, chatText);
                            break;
                        case 4:
                            points =+ new SpamMessageChecker().GetHalalPoints(chatText);
                            break;
                        case 5:
                            points =+ new SpamMessageChecker().GetIndiaPoints(chatText);
                            break;
                        case 6:
                            points = new SpamMessageChecker().GetContainsPoints(smsg.Messages, chatText);
                            break;
                    }
                    if (points >= smsg.MinPoints)
                    {

                        new Task(() => {
                            TgApi.getDefaultApiConnection().forwardMessage(
                                Temp.ReasonChannelID,
                                BaseMessage.GetMessageChatInfo().id,
                                BaseMessage.message_id);

                        }).Start();
                        //ProcessMessage (Ban Blacklist Delete kick mute)
                        ProcessMessage(smsg, BaseMessage.message_id, BaseMessage.GetMessageChatInfo().id, BaseMessage.GetSendUser());

                        BanUser banstat = Temp.GetDatabaseManager().GetUserBanStatus(BaseMessage.GetSendUser().id);

                        if (banstat.Ban == 0)
                        {
                            TgApi.getDefaultApiConnection().kickChatMember(
                                BaseMessage.GetMessageChatInfo().id,
                                BaseMessage.GetSendUser().id,
                                GetTime.GetUnixTime() + 86400
                                );
                        }
                        //Send alert and delete alert after 60 second
                        new Thread(delegate () {
                            SendMessageResult autodeletespammessagesendresult = TgApi.getDefaultApiConnection().sendMessage(
                            BaseMessage.GetMessageChatInfo().id,
                            "偵測到 " + smsg.FriendlyName +
                            " ，已自動回報，如有誤報請加入 @" + Temp.ReportGroupName + " 以報告誤報。"
                            );
                            Thread.Sleep(60000);
                            TgApi.getDefaultApiConnection().deleteMessage(
                                autodeletespammessagesendresult.result.chat.id,
                                autodeletespammessagesendresult.result.message_id
                                );
                        }).Start();
                        return new CallbackMessage() { StopProcess = true };
                    }
                }
            }
            // AUTO DELETE SPAM MESSAGE END

            // Auto DELETE Command START
            if (cfg.AutoDeleteCommand == 0)
            {
                if (BaseMessage.entities != null)
                {
                    ContentEntities tmpEntities = BaseMessage.entities[0];
                    Log.i(tmpEntities.type + "" + tmpEntities.offset);
                    if (tmpEntities.type == "bot_command" && tmpEntities.offset == 0)
                    {
                        new Thread(delegate () {
                            SendMessageResult autodeletecommandsendresult = TgApi.getDefaultApiConnection().sendMessage(
                                BaseMessage.GetMessageChatInfo().id,
                                "請您不要亂玩機器人的指令，有問題請聯絡群組管理員。"
                                );
                            Thread.Sleep(60000);
                            TgApi.getDefaultApiConnection().deleteMessage(
                                autodeletecommandsendresult.result.chat.id,
                                autodeletecommandsendresult.result.message_id
                                );
                        }).Start();
                        TgApi.getDefaultApiConnection().deleteMessage(BaseMessage.chat.id, BaseMessage.message_id);
                    }
                }
            }
            // Auto DELETE Command END
            
            // Admin ONLY START
            if (cfg.AdminOnly == 0)
            {
                throw new StopProcessException();
            }
            // Admin ONLY END
            return new CallbackMessage();
        }

        private void ProcessMessage(SpamMessage smsg, int MsgID, long ChatID, UserInfo SendUserInfo)
        {
            long banUtilTime;
            if (smsg.BanDays == 0 && smsg.BanHours == 0 && smsg.BanMinutes == 0)
            {
                banUtilTime = 0;
            }
            else
            {
                banUtilTime = GetTime.GetUnixTime() + (smsg.BanDays * 86400) + (smsg.BanHours * 3600) + (smsg.BanMinutes * 60);
            }
            if (smsg.AutoBlackList)
            {
                if (Temp.GetDatabaseManager().GetUserBanStatus(SendUserInfo.id).Ban == 0) return;
                new Task(() =>
                {
                    Temp.GetDatabaseManager().BanUser(
                            0,
                            SendUserInfo.id,
                            smsg.BanLevel,
                            banUtilTime,
                            "\n自動封鎖 : " + smsg.FriendlyName,
                            ChatID,
                            MsgID,
                            SendUserInfo
                            );
                }).Start();
            }
            if (smsg.AutoDelete)
                new Task(() =>
                {
                    TgApi.getDefaultApiConnection().deleteMessage(ChatID, MsgID);
                }).Start();
            if (smsg.AutoKick)
                new Task(() =>
                {
                    TgApi.getDefaultApiConnection().kickChatMember(ChatID, SendUserInfo.id, banUtilTime);
                }).Start();
            if (smsg.AutoMute)
                TgApi.getDefaultApiConnection().restrictChatMember(
                    ChatID,
                    SendUserInfo.id,
                    banUtilTime,
                    SendMessage: true,
                    SendMedia: false
                    );
        }

        private void CallAdmin(TgMessage msg)
        {
            GroupUserInfo[] admins = TgApi.getDefaultApiConnection().getChatAdministrators(msg.chat.id);
            string retmsg = "";
            foreach (GroupUserInfo i in admins)
            {
                if (i.user.username != null)
                {
                    retmsg += "@" + i.user.username;
                }
                else
                {
                    string userRealName = "[NO_NAME]";
                    if (i.user.first_name != null)
                    {
                        userRealName = i.user.first_name;
                    }
                    if (i.user.last_name != null)
                    {
                        userRealName = " " + i.user.last_name;
                    }
                    retmsg += "<a href=\"tg://user?id=" + i.user.id + "\">" + userRealName + "</a>";
                }
                retmsg += " , ";
            }
            TgApi.getDefaultApiConnection().sendMessage(
                msg.chat.id,
                retmsg,
                msg.message_id
                );
        }

        public CallbackMessage ReceiveOtherMessage(TgMessage RawMessage, string JsonMessage)
        {
            throw new System.NotImplementedException();
        }

        public CallbackMessage ReceiveUnknownBaseMessage(TgBaseMessage BaseMessage, string JsonMessage)
        {
            throw new System.NotImplementedException();
        }
    }
}
