using ReimuAPI.ReimuBase;
using ReimuAPI.ReimuBase.TgData;

namespace TWBlackListSoamChecker.CommandObject {
    internal class UserCommand {
        internal bool User(TgMessage RawMessage){
            TgApi.getDefaultApiConnection().sendMessage(RawMessage.GetMessageChatInfo().id,RawMessage.GetSendUser().id.ToString(),RawMessage.message_id);
            return true;
        }
    }
}
