using CNBlackListSoamChecker.DbManager;
using ReimuAPI.ReimuBase;
using ReimuAPI.ReimuBase.TgData;
using System.Collections.Generic;
using System.IO;
using System.Runtime.Serialization.Json;
using System.Text;

namespace CNBlackListSoamChecker.CommandObject
{
    public class SpamStringManager
    {
        public static int SPAMSTR_TYPE_EQUALS = 0;
        public static int SPAMSTR_TYPE_REGEX = 1;
        public static int SPAMSTR_TYPE_SELFCHK = 2;
        public static int SPAMSTR_TYPE_HALAL = 3;
        public static int SPAMSTR_TYPE_INDIA = 4;
        public static int SPAMSTR_TYPE_CONTAINS = 5;

        public void GetAllInfo(TgMessage RawMessage)
        {
            string spamstrings = "<code>";
            List<SpamMessage> msgs = Temp.GetDatabaseManager().GetSpamMessageList();
            foreach (SpamMessage msg in msgs)
            {
                spamstrings += "- " + msg.FriendlyName + ":" +
                    "\n    Enabled: " + msg.Enabled +
                    "\n    Type: " + msg.Type +
                    "\n    AutoGlobalBlock: " + msg.AutoBlackList +
                    "\n    AutoDelete: " + msg.AutoDelete +
                    "\n    AutoKick: " + msg.AutoKick +
                    "\n    AutoMute: " + msg.AutoMute +
                    "\n    BanDays: " + msg.BanDays +
                    "\n    BanHours: " + msg.BanHours +
                    "\n    BanMinutes: " + msg.BanMinutes +
                    "\n    MinPoints: " + msg.MinPoints +
                    "\n    Messages: ";
                foreach (SpamMessageObj i in msg.Messages)
                {
                    spamstrings += "\n    - Message: " + TgApi.getDefaultApiConnection().jsonEncode(i.Message) +
                        "\n      Point: " + i.Point;
                }
                spamstrings += "\n\n";
            }
            spamstrings += "</code>";
            if (spamstrings == "<code></code>")
            {
                TgApi.getDefaultApiConnection().sendMessage(RawMessage.GetMessageChatInfo().id, "null", RawMessage.message_id);
                return;
            }
            TgApi.getDefaultApiConnection().sendMessage(
                RawMessage.GetMessageChatInfo().id,
                spamstrings,
                RawMessage.message_id,
                ParseMode: TgApi.PARSEMODE_HTML
                );
        }

        public void GetName(TgMessage RawMessage)
        {
            int spacePath = RawMessage.text.IndexOf(" ");
            string spamstrings = "";
            List<SpamMessage> msgs = Temp.GetDatabaseManager().GetSpamMessageList();
            if (spacePath == -1)
            {
                if (msgs.Count == 0)
                {
                    TgApi.getDefaultApiConnection().sendMessage(RawMessage.GetMessageChatInfo().id, "<code>null</code>", RawMessage.message_id);
                    return;
                }
                foreach (SpamMessage msg in msgs)
                {
                    spamstrings += "FriendlyName: <code>" + msg.FriendlyName + "</code>, Enabled: " + msg.Enabled + "\n";
                }
                spamstrings += "\n您可以使用 /getspamstr [FriendlyName] 來取得詳細訊息。";
            }
            else
            {
                string name = RawMessage.text.Substring(spacePath + 1);
                foreach (SpamMessage msg in msgs)
                {
                    if (name != msg.FriendlyName)
                    {
                        continue;
                    }
                    if (spamstrings != "")
                    {
                        spamstrings += "\n\n------\n\n";
                    }
                    spamstrings += "<code>- " + msg.FriendlyName + ":" +
                        "\n    Enabled: " + msg.Enabled +
                        "\n    Type: " + msg.Type +
                        "\n    AutoGlobalBlock: " + msg.AutoBlackList +
                        "\n    AutoDelete: " + msg.AutoDelete +
                        "\n    AutoKick: " + msg.AutoKick +
                        "\n    AutoMute: " + msg.AutoMute +
                        "\n    BanDays: " + msg.BanDays +
                        "\n    BanHours: " + msg.BanHours +
                        "\n    BanMinutes: " + msg.BanMinutes +
                        "\n    MinPoints: " + msg.MinPoints +
                        "\n    Messages: ";
                    foreach (SpamMessageObj i in msg.Messages)
                    {
                        spamstrings += "\n    - Message: " + TgApi.getDefaultApiConnection().jsonEncode(i.Message) +
                            "\n      Point: " + i.Point;
                    }
                    spamstrings += "</code>";
                }
                if (spamstrings == "")
                {
                    spamstrings = "沒有查到這筆紀錄，請檢查您的輸入。";
                }
            }
            TgApi.getDefaultApiConnection().sendMessage(
                RawMessage.GetMessageChatInfo().id,
                spamstrings,
                RawMessage.message_id,
                ParseMode: TgApi.PARSEMODE_HTML
                );
        }

        public void GetByID(TgMessage RawMessage)
        {
            int spacePath = RawMessage.text.IndexOf(" ");
            if (spacePath == -1)
            {
                TgApi.getDefaultApiConnection().sendMessage(
                    RawMessage.GetMessageChatInfo().id,
                    "你的輸入有錯誤，請在最後面加上規則的 ID，您可以用 /getallspamstr 取得。",
                    RawMessage.message_id,
                    ParseMode: TgApi.PARSEMODE_MARKDOWN
                    );
                return;
            }
            string name = RawMessage.text.Substring(spacePath + 1);
            string spamstrings = "";
            List<SpamMessage> msgs = Temp.GetDatabaseManager().GetSpamMessageList();
            foreach (SpamMessage msg in msgs)
            {
                if (name != msg.FriendlyName)
                {
                    continue;
                }
                spamstrings += "- " + msg.FriendlyName + ":" +
                    "\n    Enabled: " + msg.Enabled +
                    "\n    Type: " + msg.Type +
                    "\n    AutoGlobalBlock: " + msg.AutoBlackList +
                    "\n    AutoDelete: " + msg.AutoDelete +
                    "\n    AutoKick: " + msg.AutoKick +
                    "\n    AutoMute: " + msg.AutoMute +
                    "\n    BanDays: " + msg.BanDays +
                    "\n    BanHours: " + msg.BanHours +
                    "\n    BanMinutes: " + msg.BanMinutes +
                    "\n    MinPoints: " + msg.MinPoints +
                    "\n    Messages: ";
                foreach (SpamMessageObj i in msg.Messages)
                {
                    spamstrings += "\n    - Message: " + TgApi.getDefaultApiConnection().jsonEncode(i.Message) +
                        "\n      Point: " + i.Point;
                }
                spamstrings += "\n\n";
            }
            if (spamstrings == "")
            {
                TgApi.getDefaultApiConnection().sendMessage(RawMessage.GetMessageChatInfo().id, "null", RawMessage.message_id);
                return;
            }
            TgApi.getDefaultApiConnection().sendMessage(
                RawMessage.GetMessageChatInfo().id,
                spamstrings,
                RawMessage.message_id
                );
        }

        public void Get(TgMessage RawMessage)
        {
            int spacePath = RawMessage.text.IndexOf(" ");
            if (spacePath == -1)
            {
                TgApi.getDefaultApiConnection().sendMessage(
                    RawMessage.GetMessageChatInfo().id,
                    "你的輸入有錯誤，請在最後面加上規則的 ID，您可以用 /getallspamstr 取得。",
                    RawMessage.message_id,
                    ParseMode: TgApi.PARSEMODE_MARKDOWN
                    );
                return;
            }
            string jsonText = RawMessage.text.Substring(spacePath + 1);
            string spamstrings = "";
            List<SpamMessage> msgs = Temp.GetDatabaseManager().GetSpamMessageList();
            foreach (SpamMessage msg in msgs)
            {
                spamstrings += "- " + msg.FriendlyName + "\n";
            }
            if (spamstrings == "")
            {
                TgApi.getDefaultApiConnection().sendMessage(RawMessage.GetMessageChatInfo().id, "null", RawMessage.message_id);
                return;
            }
            TgApi.getDefaultApiConnection().sendMessage(
                RawMessage.GetMessageChatInfo().id,
                spamstrings,
                RawMessage.message_id
                );
        }

        public void Add(TgMessage RawMessage)
        {

            string HelpContent =
                    "解析 JSON 时出现错误，請参考下面的例子 : \n```\n" +
                    "{\n    " +
                    "\"FriendlyName\": \"規則名稱\",\n    " +
                    "\"Enabled\": true,\n    " +
                    "\"Type\": 0,\n    " +
                    "\"AutoBlackList\": false,\n    " +
                    "\"AutoDelete\": true,\n    " +
                    "\"AutoKick\": false,\n    " +
                    "\"AutoMute\": false,\n    " +
                    "\"BanLevel\": 1,\n    " +
                    "\"BanDays\": 1,\n    " +
                    "\"BanHours\": 0,\n    " +
                    "\"BanMinutes\": 0,\n    " +
                    "\"MinPoints\": 1,\n    " +
                    "\"Messages\": " +
                    "[\n        " +
                    "{\n            " +
                    "\"Message\": \"__THIS_IS_A_TEST_SPAM_MESSAGE__\",\n            " +
                    "\"Point\": 1\n        " +
                    "}\n    " +
                    "]\n}" +
                    "\n```\n" +
                    "关于 Type 的说明 : \n完全匹配 = 0" +
                    "\n正則表达式 = 1" +
                    "\n使用迷之算法匹配 = 2" +
                    "\nstring.IndexOf(\"target\")!=-1 = 3" +
                    "\n清真 = 4" +
                    "\n印度 = 5" +
                    "\n包含 = 6";
            int spacePath = RawMessage.text.IndexOf(" ");
            if (spacePath == -1)
            {
                TgApi.getDefaultApiConnection().sendMessage(
                    RawMessage.GetMessageChatInfo().id,
                    HelpContent,
                    RawMessage.message_id,
                    ParseMode: TgApi.PARSEMODE_MARKDOWN
                    );
                return;
            }
            string jsonText = RawMessage.text.Substring(spacePath + 1);
            SpamMessage smsg;
            try {
                smsg = (SpamMessage)new DataContractJsonSerializer(
                      typeof(SpamMessage)
                  ).ReadObject(
                      new MemoryStream(
                          Encoding.UTF8.GetBytes(jsonText)
                      )
                  );
            } catch (System.Exception)
            {
                TgApi.getDefaultApiConnection().sendMessage(
                    RawMessage.GetMessageChatInfo().id,
                    HelpContent,
                    RawMessage.message_id,
                    ParseMode: TgApi.PARSEMODE_MARKDOWN
                    );
                return;
            }
            Temp.GetDatabaseManager().AddSpamMessage(smsg);
            TgApi.getDefaultApiConnection().sendMessage(
                RawMessage.GetMessageChatInfo().id,
                "ok",
                RawMessage.message_id,
                ParseMode: TgApi.PARSEMODE_MARKDOWN
                );
        }

        public void Remove(TgMessage RawMessage)
        {
            int spacePath = RawMessage.text.IndexOf(" ");
            if (spacePath == -1)
            {
                TgApi.getDefaultApiConnection().sendMessage(
                    RawMessage.GetMessageChatInfo().id,
                    "你的輸入有錯誤，請在指令後增加 FriendlyName",
                    RawMessage.message_id
                    );
                return;
            }
            string RuleFriendlyName = RawMessage.text.Substring(spacePath + 1);
            int count = Temp.GetDatabaseManager().DeleteSpamMessage(RuleFriendlyName);
            TgApi.getDefaultApiConnection().sendMessage(
                RawMessage.GetMessageChatInfo().id,
                "删除了 " + count + " 项",
                RawMessage.message_id
                );
        }

        public void GetSpamPoints(TgMessage RawMessage)
        {
            int spacePath = RawMessage.text.IndexOf(" ");
            if (spacePath == -1)
            {
                TgApi.getDefaultApiConnection().sendMessage(
                    RawMessage.GetMessageChatInfo().id,
                    "/getspampoints text=\"被檢測訊息，如果包含英文與數字以外的文字需要加引號\"" +
                    " rule=\"規則的暱稱，如果包含英文與數字以外的文字需要加引號\"",
                    RawMessage.message_id
                    );
                return;
            }
            Dictionary<string, string> banValues = CommandDecoder.cutKeyIsValue(RawMessage.text.Substring(spacePath + 1));
            string text = banValues.GetValueOrDefault("text", null);
            string rule = banValues.GetValueOrDefault("rule", null);
            if (text == null || rule == null)
            {
                TgApi.getDefaultApiConnection().sendMessage(
                    RawMessage.GetMessageChatInfo().id,
                    "你的輸入有錯誤",
                    RawMessage.message_id
                    );
                return;
            }
            SpamMessage smsg = Temp.GetDatabaseManager().GetSpamRule(rule);
            if (smsg == null)
            {
                TgApi.getDefaultApiConnection().sendMessage(
                    RawMessage.GetMessageChatInfo().id,
                    "没有找到您指定的规則，請重新指定。您可使用 /getspamstr 獲取所以規則。",
                    RawMessage.message_id
                    );
                return;
            }
            int points = 0;
            switch (smsg.Type)
            {
                case 0:
                    points = new SpamMessageChecker().GetEqualsPoints(smsg.Messages, text);
                    break;
                case 1:
                    points = new SpamMessageChecker().GetRegexPoints(smsg.Messages, text);
                    break;
                case 2:
                    points = new SpamMessageChecker().GetSpamPoints(smsg.Messages, text);
                    break;
                case 3:
                    points = new SpamMessageChecker().GetIndexOfPoints(smsg.Messages, text);
                    break;
                case 4:
                    points = new SpamMessageChecker().GetHalalPoints(text);
                    break;
                case 5:
                    points = new SpamMessageChecker().GetIndiaPoints(text);
                    break;
                case 6:
                    points = new SpamMessageChecker().GetContainsPoints(smsg.Messages, text);
                    break;
            }
            TgApi.getDefaultApiConnection().sendMessage(
                RawMessage.GetMessageChatInfo().id,
                "得分: " + points,
                RawMessage.message_id
                );
        }
    }
}
