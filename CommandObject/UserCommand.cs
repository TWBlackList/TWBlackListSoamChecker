using ReimuAPI.ReimuBase;
using ReimuAPI.ReimuBase.TgData;

namespace CNBlackListSoamChecker.CommandObject {
    internal class UserCommand {
        internal bool User(TgMessage RawMessage){
            TgApi.getDefaultApiConnection().sendMessage(RawMessage.GetSendUser().id.ToString(),RawMessage.message_id);
            return true;
        }
    }
}
