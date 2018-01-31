using ReimuAPI.ReimuBase;
using ReimuAPI.ReimuBase.TgData;

namespace TWBlackListSoamChecker.CommandObject {
    internal class UserCommand {
        internal bool User(TgMessage RawMessage){
            if(RawMessage.text.ToLower().Contains("username")){
                if (RawMessage.reply_to_message != null)
                    {
                        if (RawMessage.reply_to_message.forward_from != null)
                        {
                            TgApi.getDefaultApiConnection().sendMessage(RawMessage.GetMessageChatInfo().id,RawMessage.reply_to_message.forward_from.username.ToString(),RawMessage.message_id);
                            return true;
                        }
                        TgApi.getDefaultApiConnection().sendMessage(RawMessage.GetMessageChatInfo().id,RawMessage.reply_to_message.GetSendUser().username.ToString(),RawMessage.message_id);
                        return true;
                    }
                TgApi.getDefaultApiConnection().sendMessage(RawMessage.GetMessageChatInfo().id,RawMessage.GetSendUser().username.ToString(),RawMessage.message_id);
                return true;
            }else{
                if (RawMessage.reply_to_message != null)
                    {
                        if (RawMessage.reply_to_message.forward_from != null)
                        {
                            TgApi.getDefaultApiConnection().sendMessage(RawMessage.GetMessageChatInfo().id,RawMessage.reply_to_message.forward_from.id.ToString(),RawMessage.message_id);
                            return true;
                        }
                        TgApi.getDefaultApiConnection().sendMessage(RawMessage.GetMessageChatInfo().id,RawMessage.reply_to_message.GetSendUser().id.ToString(),RawMessage.message_id);
                        return true;
                    }
                TgApi.getDefaultApiConnection().sendMessage(RawMessage.GetMessageChatInfo().id,RawMessage.GetSendUser().id.ToString(),RawMessage.message_id);
                return true;
            }
        }
    }
}
