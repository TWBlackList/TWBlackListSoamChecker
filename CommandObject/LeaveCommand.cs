﻿using ReimuAPI.ReimuBase;
using ReimuAPI.ReimuBase.TgData;

namespace TWBlackListSoamChecker.CommandObject
{
    internal class LeaveCommand
    {
        internal bool Leave(TgMessage RawMessage)
        {
            if (RAPI.getIsBotAdmin(RawMessage.GetSendUser().id) || RAPI.getIsBotOP(RawMessage.GetSendUser().id) ||
                TgApi.getDefaultApiConnection().checkIsAdmin(RawMessage.chat.id, RawMessage.GetSendUser().id))
            {
                TgApi.getDefaultApiConnection().sendMessage(RawMessage.GetMessageChatInfo().id,
                    "由 使用者 (" + RawMessage.GetSendUser().id + ") 請求離開群組", RawMessage.message_id);
                TgApi.getDefaultApiConnection().leaveChat(RawMessage.chat.id);
                return true;
            }

            TgApi.getDefaultApiConnection().sendMessage(RawMessage.GetMessageChatInfo().id, "您並不是群組管理或是 Bot 管理員",
                RawMessage.message_id);
            return true;
        }
    }
}