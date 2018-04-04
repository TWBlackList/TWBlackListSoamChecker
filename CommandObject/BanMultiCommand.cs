using System.Collections.Generic;
using System.Threading;
using ReimuAPI.ReimuBase;
using ReimuAPI.ReimuBase.TgData;

namespace TWBlackListSoamChecker.CommandObject
{
    internal class BanMultiUserCommand
    {
        internal bool BanMulti(TgMessage RawMessage, string JsonMessage, string Command)
        {
            var banSpace = RawMessage.text.IndexOf(" ");
            if (banSpace == -1)
            {
                TgApi.getDefaultApiConnection().sendMessage(
                    RawMessage.GetMessageChatInfo().id,
                    "/suban [i|id=1] [l|level=0] [m|minutes=0] [h|hours=0] [d|days=15] [f|from=f|fwd|r|reply] [halal [f|fwd|r|reply]]" +
                    " r|reason=\"asdfsadf asdfadsf\"\n\n" +
                    "m: 分鐘, h: 小時, d: 天\n" +
                    "from 選項僅在 id 未被定義時起作用\n" +
                    "ID 選擇優先度: 手動輸入 ID > 回覆的被轉發訊息 > 回覆的訊息\n" +
                    "選項優先度: 簡寫 > 全名\n" +
                    "halal 選項只能單獨使用，不能與其他選項共同使用，並且需要回覆一則訊息，否則將觸發異常。\n\n" +
                    "Example:\n" +
                    "/suban id=1 m=0 h=0 d=15 level=0 reason=\"aaa bbb\\n\\\"ccc\\\" ddd\"\n" +
                    "/suban halal\n" +
                    "/suban halal=reply",
                    RawMessage.message_id
                );
                return true;
            }

            var BanUserId = 0;
            int[] UsersArray = { };
            long ExpiresTime = 0;
            var Level = 0;
            var Reason = "";
            var value = RawMessage.text.Substring(banSpace + 1);
            var valLen = value.Length;
            var NotHalal = true;
            var status = false;

            if (valLen >= 5)
                if (value.Substring(0, 5) == "halal")
                {
                    NotHalal = false;
                    Reason = "無法理解的語言";
                    if (valLen > 6)
                    {
                        if (value[5] != ' ')
                        {
                            TgApi.getDefaultApiConnection().sendMessage(
                                RawMessage.GetMessageChatInfo().id,
                                "您的輸入有錯誤，請檢查您的輸入，或使用 /cnban 查詢幫助。 err_a1",
                                RawMessage.message_id
                            );
                            return true;
                        }

                        UsersArray =
                            new GetValues().GetUserIDs(new Dictionary<string, string> {{"from", value.Substring(6)}},
                                RawMessage);
                    }
                    else
                    {
                        UsersArray = new GetValues().GetUserIDs(new Dictionary<string, string>(), RawMessage);
                    }
                }

            if (NotHalal)
                try
                {
                    var banValues = CommandDecoder.cutKeyIsValue(value);
                    var tmpString = "";

                    // 获取使用者
                    UsersArray = new GetValues().GetUserIDs(banValues, RawMessage);

                    // 获取 ExpiresTime
                    var tmpExpiresTime = new GetValues().GetBanUnixTime(banValues, RawMessage);
                    if (tmpExpiresTime == -1) return true; // 如果过期时间是 -1 则代表出现了异常
                    ExpiresTime = tmpExpiresTime;

                    // 获取 Level
                    tmpString = banValues.GetValueOrDefault("l", "__invalid__");
                    if (tmpString == "__invalid__") tmpString = banValues.GetValueOrDefault("level", "0");
                    if (!int.TryParse(tmpString, out Level))
                    {
                        TgApi.getDefaultApiConnection().sendMessage(
                            RawMessage.GetMessageChatInfo().id,
                            "您的輸入有錯誤，請檢查您的輸入，或使用 /cnban 查詢幫助。 err8",
                            RawMessage.message_id
                        );
                        return true;
                    }

                    // 获取 Reason
                    Reason = new GetValues().GetReason(banValues, RawMessage);
                    if (Reason == null) return true; // 如果 Reason 是 null 则代表出现了异常
                    if (Reason.ToLower() == "halal") Reason = "無法理解的語言";
                }
                catch (DecodeException)
                {
                    TgApi.getDefaultApiConnection().sendMessage(
                        RawMessage.GetMessageChatInfo().id,
                        "您的輸入有錯誤，請檢查您的輸入，或使用 /cnban 查詢幫助 err10",
                        RawMessage.message_id
                    );
                    return true;
                }

            new Thread(delegate()
            {
                foreach (var userid in UsersArray)
                {
                    BanUserId = userid;
                    status = Temp.GetDatabaseManager().BanUser(
                        RawMessage.GetSendUser().id,
                        BanUserId,
                        Level,
                        ExpiresTime,
                        Reason
                    );
                    if (RAPI.getIsInWhitelist(BanUserId))
                        TgApi.getDefaultApiConnection().sendMessage(
                            RawMessage.GetSendUser().id,
                            "操作失敗 : 使用者在白名單 UID" + BanUserId,
                            RawMessage.message_id
                        );
                    Thread.Sleep(3500);
                }

                //if (status)
                //{
                TgApi.getDefaultApiConnection().sendMessage(
                    RawMessage.GetMessageChatInfo().id,
                    "操作成功。",
                    RawMessage.message_id
                );
                //}
                //else
                //{
                //    TgApi.getDefaultApiConnection().sendMessage(
                //        RawMessage.GetMessageChatInfo().id,
                //        "操作成功。\n\n請注意 : 轉發使用者訊息到頻道或是發送使用者訊息到頻道失敗，請您手動發送至 @" + Temp.MainChannelName + " 。 err11",
                //        RawMessage.message_id
                //        );
                //    return true;
                //}
                //return false;
            }).Start();
            return true;
        }
    }
}