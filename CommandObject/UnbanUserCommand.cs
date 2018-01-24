using ReimuAPI.ReimuBase;
using ReimuAPI.ReimuBase.TgData;
using System.Collections.Generic;

namespace CNBlackListSoamChecker.CommandObject
{
    class UnbanUserCommand
    {
        internal bool Unban(TgMessage RawMessage)
        {
            int banSpace = RawMessage.text.IndexOf(" ");
            if (banSpace == -1)
            {
                TgApi.getDefaultApiConnection().sendMessage(
                    RawMessage.GetMessageChatInfo().id,
                    "/twunban [i|id=1] [f|from=f|fwd|r|reply]" +
                    " r|reason=\"asdfsadf asdfadsf\"\n\n" +
                    "from 選項僅在 id 未被定義時起作用\n" +
                    "ID 選擇優先度: 手動輸入 ID > 回覆的被轉發訊息 > 回覆的訊息\n" +
                    "選項優先度: 簡寫 > 全名\n" +
                    "Example:\n" +
                    "/twunban id=1 reason=\"aaa bbb\\n\\\"ccc\\\" ddd\"\n" +
                    "/twunban",
                    RawMessage.message_id
                    );
                return true;
            }
            int BanUserId = 0;
            string Reason;
            UserInfo BanUserInfo = null;
            try
            {
                Dictionary<string, string> banValues = CommandDecoder.cutKeyIsValue(RawMessage.text.Substring(banSpace + 1));

                // 获取使用者信息
                UserInfo tmpUinfo = new GetValues().GetByTgMessage(banValues, RawMessage);
                if (tmpUinfo == null) return true; // 如果没拿到使用者信息則代表出现了异常
                else
                {
                    BanUserId = tmpUinfo.id;
                    if (tmpUinfo.language_code != null)
                    {
                        if (tmpUinfo.language_code != "__CAN_NOT_GET_USERINFO__")
                        {
                            BanUserInfo = tmpUinfo;
                        }
                    }
                    else
                    {
                        BanUserInfo = tmpUinfo;
                    }
                }

                // 获取 Reason
                Reason = new GetValues().GetReason(banValues, RawMessage);
                if (Reason == null) return true; // 如果 Reason 是 null 則代表出现了异常
            }
            catch (DecodeException)
            {
                TgApi.getDefaultApiConnection().sendMessage(
                    RawMessage.GetMessageChatInfo().id,
                    "您的輸入有錯誤，請檢查您的輸入，或使用 /ban 取得幫助 err10",
                    RawMessage.message_id
                    );
                return true;
            }
            bool status;
            try
            {
                if (BanUserInfo == null)
                {
                    status = Temp.GetDatabaseManager().UnbanUser(
                        RawMessage.GetSendUser().id,
                        BanUserId,
                        Reason
                        );
                }
                else
                {
                    status = Temp.GetDatabaseManager().UnbanUser(
                        RawMessage.GetSendUser().id,
                        BanUserId,
                        Reason,
                        BanUserInfo
                        );
                }
            }
            catch (System.InvalidOperationException)
            {
                TgApi.getDefaultApiConnection().sendMessage(
                    RawMessage.GetMessageChatInfo().id,
                    "操作失敗，這位使用者目前可能没有被封鎖。",
                    RawMessage.message_id
                    );
                return true;
            }
            if (status)
            {
                TgApi.getDefaultApiConnection().sendMessage(
                    RawMessage.GetMessageChatInfo().id,
                    "操作成功。",
                    RawMessage.message_id
                    );
                return true;
            }
            else
            {
                TgApi.getDefaultApiConnection().sendMessage(
                    RawMessage.GetMessageChatInfo().id,
                    "操作成功。\n\n請注意 : 轉發使用者訊息到頻道或是發送使用者訊息到頻道失敗，請您手動發送至  @" + Temp.MainChannelName + " 。 err11",
                    RawMessage.message_id
                    );
                return true;
            }
            //return false;
        }

        private UserInfo GetUserInfo(TgMessage RawMessage, string from)
        {
            if (RawMessage.reply_to_message == null)
            {
                return null;
            }
            if (from == "r" || from == "reply")
            {
                return RawMessage.GetReplyMessage().GetSendUser();
            }
            else if (from == "f" || from == "fwd")
            {
                return RawMessage.GetForwardedFromUser();
            }
            return null;
        }
    }
}
