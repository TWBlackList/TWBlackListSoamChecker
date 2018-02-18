using System;
using System.Collections.Generic;
using ReimuAPI.ReimuBase;
using ReimuAPI.ReimuBase.TgData;

/*
 * !!! DO NOT TOUCH, MAGIC !!!
 * !!! DO NOT TOUCH, MAGIC !!!
 * !!! DO NOT TOUCH, MAGIC !!!
 */

namespace TWBlackListSoamChecker.CommandObject
{
    internal class GetValues
    {
        internal string GetReason(Dictionary<string, string> banValues, TgMessage RawMessage)
        {
            string Reason = null;
            Reason = banValues.GetValueOrDefault("r", null);
            if (Reason == null) Reason = banValues.GetValueOrDefault("reason", null);
            if (Reason == null)
            {
                TgApi.getDefaultApiConnection().sendMessage(
                    RawMessage.GetMessageChatInfo().id,
                    "您的輸入有錯誤，請檢查您的輸入，或使用 /ban 查詢幫助。 err9"
                );
                return null;
            }

            return Reason;
        }
        internal long GetGroupID(Dictionary<string, string> banValues, TgMessage RawMessage)
        {
            string GroupID = "";
            GroupID = banValues.GetValueOrDefault("g", "__invalid__");
            if (GroupID == "__invalid__") GroupID = banValues.GetValueOrDefault("group", "__invalid__");
            if (GroupID == "__invalid__") GroupID = banValues.GetValueOrDefault("groupid", "__invalid__");
            if (GroupID == "__invalid__") return 0;
            long id = 0 ;
            if(System.Int64.TryParse(GroupID,out id))
            {
                return id;
            }else{
                return 0;
            }
        }

        internal string GetText(Dictionary<string, string> banValues, TgMessage RawMessage)
        {
            string Text = "";
            Text = banValues.GetValueOrDefault("t", "__invalid__");
            if (Text == "__invalid__") Text = banValues.GetValueOrDefault("text", "__invalid__");
            if (Text == "__invalid__") return null;
            return Text;
        }

        internal UserInfo GetUserInfo(TgMessage RawMessage, string from)
        {
            if (RawMessage.reply_to_message == null) return null;
            if (from == "r" || from == "reply")
                return RawMessage.GetReplyMessage().GetSendUser();
            if (from == "f" || from == "fwd") return RawMessage.GetReplyMessage().GetForwardedFromUser();
            return null;
        }

        internal UserInfo GetUserInfo(TgMessage RawMessage, string from)
        {
            if (RawMessage.reply_to_message == null) return null;
            if (from == "r" || from == "reply")
                return RawMessage.GetReplyMessage().GetSendUser();
            if (from == "f" || from == "fwd") return RawMessage.GetReplyMessage().GetForwardedFromUser();
            return null;
        }

        internal long GetBanUnixTime(Dictionary<string, string> banValues, TgMessage RawMessage)
        {
            string tmpString = "";
            int Minutes = 0;
            int Hours = 0;
            int Days = 0;
            long ExpiresTime = 0;
            tmpString = banValues.GetValueOrDefault("m", "__invalid__");
            if (tmpString == "__invalid__") tmpString = banValues.GetValueOrDefault("minutes", "0");
            if (!int.TryParse(tmpString, out Minutes))
            {
                TgApi.getDefaultApiConnection().sendMessage(
                    RawMessage.GetMessageChatInfo().id,
                    "您的輸入有錯誤，請檢查您的輸入，或使用 /ban 查詢幫助。 err4"
                );
                return -1;
            }

            tmpString = banValues.GetValueOrDefault("h", "__invalid__");
            if (tmpString == "__invalid__") tmpString = banValues.GetValueOrDefault("hours", "0");
            if (!int.TryParse(tmpString, out Hours))
            {
                TgApi.getDefaultApiConnection().sendMessage(
                    RawMessage.GetMessageChatInfo().id,
                    "您的輸入有錯誤，請檢查您的輸入，或使用 /ban 查詢幫助。 err5"
                );
                return -1;
            }

            tmpString = banValues.GetValueOrDefault("d", "__invalid__");
            if (tmpString == "__invalid__")
                if (Minutes != 0 || Hours != 0)
                    tmpString = banValues.GetValueOrDefault("days", "0");
                else
                    tmpString = banValues.GetValueOrDefault("days", "7");
            if (!int.TryParse(tmpString, out Days))
            {
                TgApi.getDefaultApiConnection().sendMessage(
                    RawMessage.GetMessageChatInfo().id,
                    "您的輸入有錯誤，請檢查您的輸入，或使用 /ban 查詢幫助。 err6"
                );
                return -1;
            }

            long totalTime = Minutes * 60 + Hours * 360 + Days * 86400;
            if (totalTime == 0)
            {
                ExpiresTime = 0;
            }
            else
            {
                if (totalTime > 31536000)
                {
                    TgApi.getDefaultApiConnection().sendMessage(
                        RawMessage.GetMessageChatInfo().id,
                        "最大時間不可超過 365 天，請重新輸入。你可以將 m, h, d 3個項目改成 0 以代表永久，您可以使用 /ban 取得帮助。 err7"
                    );
                    return -1;
                }

                ExpiresTime = GetTime.GetUnixTime() + totalTime;
            }

            return ExpiresTime;
        }

        internal UserInfo GetByTgMessage(Dictionary<string, string> banValues, TgMessage RawMessage)
        {
            UserInfo BanUserInfo = null;
            string tmpString = "";
            tmpString = banValues.GetValueOrDefault("i", "__invalid__");
            if (tmpString == "__invalid__")
            {
                tmpString = banValues.GetValueOrDefault("id", "__invalid__");
                if (tmpString == "__invalid__")
                {
                    UserInfo tmpUserInfo;
                    tmpString = banValues.GetValueOrDefault("f", "__invalid__");
                    if (tmpString == "__invalid__")
                    {
                        tmpString = banValues.GetValueOrDefault("from", "__invalid__");
                        if (tmpString == "__invalid__")
                        {
                            tmpUserInfo = GetUserInfo(RawMessage, "fwd");
                            if (tmpUserInfo == null)
                            {
                                tmpUserInfo = GetUserInfo(RawMessage, "reply");
                                if (tmpUserInfo == null)
                                {
                                    TgApi.getDefaultApiConnection().sendMessage(
                                        RawMessage.GetMessageChatInfo().id,
                                        "没有找到任何使用者 ID，請檢查您的輸入，或使用 /ban 查看帮助。 err1"
                                    );
                                    return null;
                                }
                            }

                            BanUserInfo = tmpUserInfo;
                        }
                        else
                        {
                            tmpUserInfo = GetUserInfo(RawMessage, tmpString);
                            if (tmpUserInfo == null)
                            {
                                TgApi.getDefaultApiConnection().sendMessage(
                                    RawMessage.GetMessageChatInfo().id,
                                    "未檢查到您指定的回覆訊息的 ID，請檢查您的輸入，或使用 /ban 查看帮助。 err2"
                                );
                                return null;
                            }

                            BanUserInfo = tmpUserInfo;
                        }
                    }
                    else
                    {
                        tmpUserInfo = GetUserInfo(RawMessage, tmpString);
                        if (tmpUserInfo == null)
                        {
                            TgApi.getDefaultApiConnection().sendMessage(
                                RawMessage.GetMessageChatInfo().id,
                                "未檢查到您指定的回覆訊息的 ID，請檢查您的輸入，或使用 /ban 查看帮助。 err2"
                            );
                            return null;
                        }

                        BanUserInfo = tmpUserInfo;
                    }
                }
            }

            if (BanUserInfo == null)
            {
                int BanUserId;
                if (!int.TryParse(tmpString, out BanUserId))
                {
                    TgApi.getDefaultApiConnection().sendMessage(
                        RawMessage.GetMessageChatInfo().id,
                        "您的輸入有錯誤，請檢查您的輸入，或使用 /ban 查詢幫助。 err3"
                    );
                    return null;
                }

                return new UserInfo {id = BanUserId, language_code = "__CAN_NOT_GET_USERINFO__"};
            }

            return BanUserInfo;
        }

        internal int[] GetUserIDs(Dictionary<string, string> banValues, TgMessage RawMessage)
        {
            string tmpString = "";
            tmpString = banValues.GetValueOrDefault("i", "__invalid__");
            if (tmpString == "__invalid__") tmpString = banValues.GetValueOrDefault("id", "__invalid__");
            int[] users = Array.ConvertAll<string, int>(tmpString.Split(","), int.Parse);

            return users;
        }
    }
}