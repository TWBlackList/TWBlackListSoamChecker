using ReimuAPI.ReimuBase;
using ReimuAPI.ReimuBase.TgData;
using System.Collections.Generic;

namespace CNBlackListSoamChecker.CommandObject
{
    class UnBanMultiUserCommand
    {
        internal bool UnbanMulti(TgMessage RawMessage)
        {
            int banSpace = RawMessage.text.IndexOf(" ");
            if (banSpace == -1)
            {
                TgApi.getDefaultApiConnection().sendMessage(
                    RawMessage.GetMessageChatInfo().id,
                    "/unban [i|id=1] [f|from=f|fwd|r|reply]" +
                    " r|reason=\"asdfsadf asdfadsf\"\n\n" +
                    "from 選項僅在 id 未被定義時起作用\n" +
                    "ID 選擇優先度: 手動輸入 ID > 回覆的被轉發訊息 > 回覆的訊息\n" +
                    "選項優先度: 簡寫 > 全名\n" +
                    "Example:\n" +
                    "/unban id=1 reason=\"aaa bbb\\n\\\"ccc\\\" ddd\"\n" +
                    "/unban",
                    RawMessage.message_id
                    );
                return true;
            }
            int[] UsersArray = new int[]{};
            bool status = false;
            int BanUserId = 0;
            string Reason;
            try
            {
                Dictionary<string, string> banValues = CommandDecoder.cutKeyIsValue(RawMessage.text.Substring(banSpace + 1));

                // 获取使用者信息
                UsersArray = new GetValues().GetUserIDs(banValues, RawMessage); 
                
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


            foreach (int userid in UsersArray){
                BanUserId = userid;
                try
                {
                    status = Temp.GetDatabaseManager().UnbanUser(
                            RawMessage.GetSendUser().id,
                            BanUserId,
                            Reason
                            );
                }
                catch (System.InvalidOperationException)
                {
                    TgApi.getDefaultApiConnection().sendMessage(
                        RawMessage.GetMessageChatInfo().id,
                        "操作失敗，這位使用者(" + BanUserId.ToString() + ")目前可能没有被封鎖。",
                        RawMessage.message_id
                        );
                    return true;
                }
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
        }
    }
}
