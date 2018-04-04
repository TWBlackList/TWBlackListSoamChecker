using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using ReimuAPI.ReimuBase;
using ReimuAPI.ReimuBase.TgData;
using TWBlackListSoamChecker.DbManager;

namespace TWBlackListSoamChecker.CommandObject
{
    internal class BroadCast
    {
        internal bool BroadCast_Status(TgMessage RawMessage)
        {
            var saySpace = RawMessage.text.IndexOf(" ");
            if (saySpace == -1)
            {
                TgApi.getDefaultApiConnection().sendMessage(
                    RawMessage.GetMessageChatInfo().id,
                    "/say [g|group|groupid=1] [t|text=text]" +
                    "\ng=ChatID\nt=訊息",
                    RawMessage.message_id
                );
                return true;
            }

            var
                banValues = CommandDecoder.cutKeyIsValue(RawMessage.text.Substring(saySpace + 1));

            var text = new GetValues().GetText(banValues, RawMessage);

            if (text == null)
            {
                TgApi.getDefaultApiConnection().sendMessage(
                    RawMessage.GetMessageChatInfo().id,
                    "/say [g|group|groupid=1] [t|text=text]" +
                    "\ng=ChatID\nt=訊息",
                    RawMessage.message_id
                );
                return true;
            }

            var groupID = new GetValues().GetGroupID(banValues, RawMessage);

            if (groupID == 0)
            {
                new Thread(delegate() { BC(RawMessage, text); }).Start();
            }
            else
            {
                TgApi.getDefaultApiConnection()
                    .sendMessage(groupID, text, ParseMode: TgApi.PARSEMODE_MARKDOWN);
                TgApi.getDefaultApiConnection()
                    .sendMessage(RawMessage.chat.id, "傳送完畢!", RawMessage.message_id);
            }


            return true;
        }

        internal bool BC(TgMessage RawMessage, string Msg)
        {
            TgApi.getDefaultApiConnection()
                .sendMessage(RawMessage.chat.id, "傳送中.........!", RawMessage.message_id);
            Console.WriteLine("Broadcasting " + Msg + " ......");
            using (var db = new BlacklistDatabaseContext())
            {
                var groups = "";
                List<GroupCfg> groupCfg = null;
                try
                {
                    groupCfg = db.GroupConfig.ToList();
                }
                catch (InvalidOperationException)
                {
                    return false;
                }

                if (groupCfg == null) return false;
                foreach (var cfg in groupCfg)
                {
                    Console.WriteLine("Broadcasting " + Msg + " to group ChatID : " + cfg.GroupID);

                    var result = TgApi.getDefaultApiConnection()
                        .sendMessage(cfg.GroupID, Msg, ParseMode: TgApi.PARSEMODE_MARKDOWN);

                    if (result.ok)
                        groups = groups + "\n" + cfg.GroupID + " : 成功";
                    else
                        groups = groups + "\n" + cfg.GroupID + " : 失敗";

                    Thread.Sleep(500);
                }

                TgApi.getDefaultApiConnection()
                    .sendMessage(RawMessage.chat.id, "有夠Highㄉ，傳送完畢!" + groups, RawMessage.message_id);
            }

            return true;
        }
    }
}