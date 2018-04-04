using ReimuAPI.ReimuBase;
using ReimuAPI.ReimuBase.TgData;

namespace TWBlackListSoamChecker.CommandObject
{
    internal class LeaveCommand
    {
        internal bool Leave(TgMessage RawMessage)
        {
            var saySpace = RawMessage.text.IndexOf(" ");
            if (saySpace == -1)
                if (RAPI.getIsBotAdmin(RawMessage.GetSendUser().id) || RAPI.getIsBotOP(RawMessage.GetSendUser().id) ||
                    TgApi.getDefaultApiConnection().checkIsAdmin(RawMessage.chat.id, RawMessage.GetSendUser().id))
                {
                    TgApi.getDefaultApiConnection().sendMessage(RawMessage.GetMessageChatInfo().id,
                        "由 群管理 (" + RawMessage.GetSendUser().id + ") 請求離開群組", RawMessage.message_id);
                    TgApi.getDefaultApiConnection().leaveChat(RawMessage.chat.id);
                    return true;
                }

            if (TgApi.getDefaultApiConnection().checkIsAdmin(RawMessage.chat.id, RawMessage.GetSendUser().id))
            {
                var
                    banValues = CommandDecoder.cutKeyIsValue(RawMessage.text.Substring(saySpace + 1));

                var groupID = new GetValues().GetGroupID(banValues, RawMessage);

                if (groupID == 0)
                {
                    TgApi.getDefaultApiConnection()
                        .sendMessage(RawMessage.chat.id, "輸入錯誤!\n/leave [g=100000000]", RawMessage.message_id);
                }
                else
                {
                    TgApi.getDefaultApiConnection().sendMessage(groupID,
                        "由 Bot管理員 (" + RawMessage.GetSendUser().id + ") 請求離開群組", RawMessage.message_id);
                    TgApi.getDefaultApiConnection().leaveChat(groupID);
                    TgApi.getDefaultApiConnection().sendMessage(RawMessage.chat.id,
                        "由 Bot管理員 (" + RawMessage.GetSendUser().id + ") 請求離開群組 " + groupID, RawMessage.message_id);
                }

                return true;
            }

            TgApi.getDefaultApiConnection().sendMessage(RawMessage.GetMessageChatInfo().id, "您並不是群組管理或是 Bot 管理員",
                RawMessage.message_id);
            return true;
        }
    }
}