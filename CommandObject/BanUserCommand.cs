using ReimuAPI.ReimuBase;
using ReimuAPI.ReimuBase.TgData;
using System.Collections.Generic;

namespace CNBlackListSoamChecker.CommandObject
{
    internal class BanUserCommand
    {
        internal bool Ban(TgMessage RawMessage, string JsonMessage, string Command)
        {
            int banSpace = RawMessage.text.IndexOf(" ");
            if (banSpace == -1)
            {
                TgApi.getDefaultApiConnection().sendMessage(
                    RawMessage.GetMessageChatInfo().id,
                    "/twban [i|id=1] [l|level=0] [m|minutes=0] [h|hours=0] [d|days=15] [f|from=f|fwd|r|reply] [halal [f|fwd|r|reply]]" +
                    " r|reason=\"asdfsadf asdfadsf\"\n\n" +
                    "m: 分鐘, h: 小時, d: 天\n" +
                    "from 選項僅在 id 未被定義時起作用\n" +
                    "ID 選擇優先度: 手動輸入 ID > 回覆的被轉發訊息 > 回覆的訊息\n" +
                    "選項優先度: 簡寫 > 全名\n" +
                    "halal 選項只能單獨使用，不能與其他選項共同使用，並且需要回覆一則訊息，否則將觸發異常。\n\n" +
                    "Example:\n" +
                    "/twban id=1 m=0 h=0 d=15 level=0 reason=\"aaa bbb\\n\\\"ccc\\\" ddd\"\n" +
                    "/twban halal\n" +
                    "/twban halal=reply",
                    RawMessage.message_id
                    );
                return true;
            }
            int BanUserId = 0;
            long ExpiresTime = 0;
            int Level = 0;
            string Reason = "";
            UserInfo BanUserInfo = null;
            string value = RawMessage.text.Substring(banSpace + 1);
            int valLen = value.Length;
            bool NotHalal = true;
            if (valLen >= 5)
            {
                if (value.Substring(0, 5) == "halal")
                {
                    NotHalal = false;
                    Reason = "Halal （台灣人無法理解的語言）";
                    if (valLen > 6)
                    {
                        if (value[5] != ' ')
                        {
                            TgApi.getDefaultApiConnection().sendMessage(
                                RawMessage.GetMessageChatInfo().id,
                                "您的輸入有錯誤，請檢查您的輸入，或使用 /twban 查詢幫助。 err_a1",
                                RawMessage.message_id
                                );
                            return true;
                        }
                        UserInfo tmpUinfo = new GetValues().GetByTgMessage(new Dictionary<string, string> { { "from" , value.Substring(6) } }, RawMessage);
                        if (tmpUinfo == null) return true; // 如果没拿到使用者信息则代表出现了异常
                        else
                        {
                            BanUserId = tmpUinfo.id;
                            if (tmpUinfo.language_code != null && tmpUinfo.language_code != "__CAN_NOT_GET_USERINFO__")
                            {
                                BanUserInfo = tmpUinfo;
                            }
                        }
                    }
                    else
                    {
                        UserInfo tmpUinfo = new GetValues().GetByTgMessage(new Dictionary<string, string> {  }, RawMessage);
                        if (tmpUinfo == null) return true; // 如果没拿到使用者信息则代表出现了异常
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
                    }
                }
            }
            if (NotHalal)
            {
                try
                {
                    Dictionary<string, string> banValues = CommandDecoder.cutKeyIsValue(value);
                    string tmpString = "";

                    // 获取使用者信息
                    UserInfo tmpUinfo = new GetValues().GetByTgMessage(banValues, RawMessage);
                    if (tmpUinfo == null) return true; // 如果没拿到使用者信息则代表出现了异常
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

                    // 获取 ExpiresTime
                    long tmpExpiresTime = new GetValues().GetBanUnixTime(banValues, RawMessage);
                    if (tmpExpiresTime == -1) return true; // 如果过期时间是 -1 则代表出现了异常
                    else ExpiresTime = tmpExpiresTime;

                    // 获取 Level
                    tmpString = banValues.GetValueOrDefault("l", "__invalid__");
                    if (tmpString == "__invalid__")
                    {
                        tmpString = banValues.GetValueOrDefault("level", "0");
                    }
                    if (!int.TryParse(tmpString, out Level))
                    {
                        TgApi.getDefaultApiConnection().sendMessage(
                            RawMessage.GetMessageChatInfo().id,
                            "您的輸入有錯誤，請檢查您的輸入，或使用 /twban 查詢幫助。 err8",
                            RawMessage.message_id
                            );
                        return true;
                    }

                    // 获取 Reason
                    Reason = new GetValues().GetReason(banValues, RawMessage);
                    if (Reason == null) return true; // 如果 Reason 是 null 则代表出现了异常
                    if (Reason.ToLower() == "halal")
                    {
                        Reason = "Halal （台灣人無法理解的語言）";
                    }
                }
                catch (DecodeException)
                {
                    TgApi.getDefaultApiConnection().sendMessage(
                        RawMessage.GetMessageChatInfo().id,
                        "您的輸入有錯誤，請檢查您的輸入，或使用 /twban 查詢幫助 err10",
                        RawMessage.message_id
                        );
                    return true;
                }
            }
            bool status;
            if (BanUserInfo == null)
            {
                status = Temp.GetDatabaseManager().BanUser(
                    RawMessage.GetSendUser().id,
                    BanUserId,
                    Level,
                    ExpiresTime,
                    Reason
                    );
            }
            else
            {
                status = Temp.GetDatabaseManager().BanUser(
                    RawMessage.GetSendUser().id,
                    BanUserId,
                    Level,
                    ExpiresTime,
                    Reason,
                    RawMessage.GetMessageChatInfo().id,
                    RawMessage.GetReplyMessage().message_id,
                    BanUserInfo
                    );
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
                    "操作成功。\n\n請注意 : 轉發使用者訊息到頻道或是發送使用者訊息到頻道失敗，請您手動發送至 @" + Temp.MainChannelName + " 。 err11",
                    RawMessage.message_id
                    );
                return true;
            }
            //return false;
        }
    }
}
