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
            int saySpace = RawMessage.text.IndexOf(" ");
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

            Dictionary<string, string>
                banValues = CommandDecoder.cutKeyIsValue(RawMessage.text.Substring(saySpace + 1));

            string text = new GetValues().GetText(banValues, RawMessage);

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

            long groupID = new GetValues().GetGroupID(banValues, RawMessage);

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
            Console.WriteLine("Broadcasting " + Msg + " ......");
            using (var db = new BlacklistDatabaseContext())
            {
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
                foreach (GroupCfg cfg in groupCfg)
                {
                    Console.WriteLine("Broadcasting " + Msg + " To Group ChatID : " + cfg.GroupID);
                    TgApi.getDefaultApiConnection()
                        .sendMessage(cfg.GroupID, Msg, ParseMode: TgApi.PARSEMODE_MARKDOWN);
                    Thread.Sleep(100);
                }

                TgApi.getDefaultApiConnection()
                    .sendMessage(RawMessage.chat.id, "有夠Highㄉ，傳送完畢!", RawMessage.message_id);
            }

            return true;
        }
    }
}