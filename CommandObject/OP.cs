using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading;
using Newtonsoft.Json;
using ReimuAPI.ReimuBase;
using ReimuAPI.ReimuBase.TgData;
using TWBlackListSoamChecker.DbManager;

namespace TWBlackListSoamChecker.CommandObject
{
    internal class OP
    {
        internal bool addOP(TgMessage RawMessage)
        {
            string UID_Value = RawMessage.text.Replace("/addop", "").Replace(" ", "");
            if (UID_Value.Length < 5)
            {
                TgApi.getDefaultApiConnection().sendMessage(RawMessage.chat.id, "使用方法 : /addop UID",
                    RawMessage.message_id);

                return false;
            }

            string json = File.ReadAllText("config.json");
            dynamic jsonObj = JsonConvert.DeserializeObject(json);
            jsonObj["op_list"].Add(Convert.ToInt32(UID_Value));
            string output =
                JsonConvert.SerializeObject(jsonObj, Formatting.Indented);
            File.WriteAllText("config.json", output);
            TgApi.getDefaultApiConnection().sendMessage(RawMessage.chat.id, "新增成功!", RawMessage.message_id);

            RAPI.reloadConfig();

            return true;
        }

        internal bool delOP(TgMessage RawMessage)
        {
            string UID_Value = RawMessage.text.Replace("/delop", "").Replace(" ", "");
            ;
            if (UID_Value.Length < 5)
            {
                TgApi.getDefaultApiConnection().sendMessage(RawMessage.chat.id, "使用方法 : /delop UID",
                    RawMessage.message_id);

                return false;
            }

            string json = File.ReadAllText("config.json");
            dynamic jsonObj = JsonConvert.DeserializeObject(json);

            int i = 0;
            bool found = false;

            foreach (var item in jsonObj["op_list"])
            {
                if (jsonObj["op_list"][i] == UID_Value)
                {
                    found = true;
                    break;
                }

                i = i + 1;
            }

            if (found)
            {
                jsonObj["op_list"].Remove(jsonObj["op_list"][i]);
                string output =
                    JsonConvert.SerializeObject(jsonObj, Formatting.Indented);
                File.WriteAllText("config.json", output);
                TgApi.getDefaultApiConnection().sendMessage(RawMessage.chat.id, "刪除成功!", RawMessage.message_id);

                RAPI.reloadConfig();
            }
            else
            {
                TgApi.getDefaultApiConnection()
                    .sendMessage(RawMessage.chat.id, "找不到OP!", RawMessage.message_id);
            }

            return true;
        }

        internal bool lsOP(TgMessage RawMessage)
        {
            string json = File.ReadAllText("config.json");
            dynamic jsonObj = JsonConvert.DeserializeObject(json);
            TgApi.getDefaultApiConnection().sendMessage(RawMessage.chat.id,
                "OP : \n" + string.Join("\n", jsonObj["op_list"]), RawMessage.message_id);
            return true;
        }

        internal bool SDAll(TgMessage RawMessage)
        {
            new Thread(delegate() { SoamDisable_All(RawMessage); }).Start();
            return true;
        }

        internal bool SEAll(TgMessage RawMessage)
        {
            new Thread(delegate() { SoamEnable_All(RawMessage); }).Start();
            return true;
        }

        internal bool SoamDisable_All(TgMessage RawMessage)
        {
            string enabled = "";
            string groupChatID = "";
            int AdminOnly = 3;
            int Blacklist = 3;
            int AutoKick = 3;
            int AntiHalal = 3;
            int AutoDeleteSpamMessage = 3;
            int AutoDeleteCommand = 3;
            int SubscribeBanList = 3;
            string text = RawMessage.text.ToLower();
            if (text.IndexOf(" adminonly") != -1)
            {
                AdminOnly = 1;
                enabled += " AdminOnly";
            }

            if (text.IndexOf(" blacklist") != -1)
            {
                Blacklist = 1;
                enabled += " Blacklist";
            }

            if (text.IndexOf(" autokick") != -1)
            {
                AutoKick = 1;
                enabled += " AutoKick";
            }

            if (text.IndexOf(" antihalal") != -1)
            {
                AntiHalal = 1;
                enabled += " AntiHalal";
            }

            if (text.IndexOf(" autodeletespammessage") != -1)
            {
                AutoDeleteSpamMessage = 1;
                enabled += " AutoDeleteSpamMessage";
            }

            if (text.IndexOf(" autodeletecommand") != -1)
            {
                AutoDeleteCommand = 1;
                enabled += " AutoDeleteCommand";
            }

            if (text.IndexOf(" subscribebanlist") != -1)
            {
                SubscribeBanList = 1;
                enabled += " SubscribeBanList";
            }

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
                    Temp.GetDatabaseManager().SetGroupConfig(
                        cfg.GroupID,
                        AdminOnly,
                        Blacklist,
                        AutoKick,
                        AntiHalal: AntiHalal,
                        AutoDeleteSpamMessage: AutoDeleteSpamMessage,
                        AutoDeleteCommand: AutoDeleteCommand,
                        SubscribeBanList: SubscribeBanList
                    );
                    if (enabled == "")
                        if (Temp.MainChannelName == null)
                            enabled = "";
                        else
                            enabled = "";
                    groupChatID = groupChatID + "\n" + cfg.GroupID;
                }

                TgApi.getDefaultApiConnection().sendMessage(RawMessage.chat.id,
                    "有夠Highㄉ，處理完畢!　\n\nChat ID : \n" + groupChatID + "\n\n關閉的功能為:\n" + enabled,
                    RawMessage.message_id);
            }

            return true;
        }

        internal bool SoamEnable_All(TgMessage RawMessage)
        {
            string enabled = "";
            string otherMsg = "";
            string groupChatID = "";
            int AdminOnly = 3;
            int Blacklist = 3;
            int AutoKick = 3;
            int AntiHalal = 3;
            int AutoDeleteSpamMessage = 3;
            int AutoDeleteCommand = 3;
            int SubscribeBanList = 3;
            string text = RawMessage.text.ToLower();
            if (text.IndexOf(" adminonly") != -1)
            {
                AdminOnly = 0;
                enabled += " AdminOnly";
            }

            if (text.IndexOf(" blacklist") != -1)
            {
                Blacklist = 0;
                if (Temp.DisableBanList)
                    otherMsg += "\nBlackList 開啟失敗，目前版本未啟用此功能。。";
                else
                    enabled += " Blacklist";
            }

            if (text.IndexOf(" autokick") != -1)
            {
                AutoKick = 0;
                if (Temp.DisableBanList)
                    otherMsg += "\nAutoKick 開啟失敗，目前版本未啟用此功能。。";
                else
                    enabled += " AutoKick";
            }

            if (text.IndexOf(" antihalal") != -1)
            {
                AntiHalal = 0;
                enabled += " AntiHalal";
            }

            if (text.IndexOf(" autodeletespammessage") != -1)
            {
                AutoDeleteSpamMessage = 0;
                if (Temp.DisableBanList)
                    otherMsg += "\nAutoDeleteSpamMessage 開啟失敗，目前版本未啟用此功能。。";
                else
                    enabled += " AutoDeleteSpamMessage";
            }

            if (text.IndexOf(" autodeletecommand") != -1)
            {
                AutoDeleteCommand = 0;
                enabled += " AutoDeleteCommand";
            }

            if (text.IndexOf(" subscribebanlist") != -1)
            {
                SubscribeBanList = 0;
                if (Temp.DisableBanList)
                    otherMsg += "\nSubscribeBanList 開啟失敗，目前版本未啟用此功能。。";
                else
                    enabled += " SubscribeBanList";
                enabled += " SubscribeBanList";
            }

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
                    Temp.GetDatabaseManager().SetGroupConfig(
                        cfg.GroupID,
                        AdminOnly,
                        Blacklist,
                        AutoKick,
                        AntiHalal: AntiHalal,
                        AutoDeleteSpamMessage: AutoDeleteSpamMessage,
                        AutoDeleteCommand: AutoDeleteCommand,
                        SubscribeBanList: SubscribeBanList
                    );
                    if (enabled == "")
                        if (Temp.MainChannelName == null)
                            enabled = "";
                        else
                            enabled = "";
                    groupChatID = groupChatID + "\n" + cfg.GroupID;
                }

                TgApi.getDefaultApiConnection().sendMessage(RawMessage.chat.id,
                    "有夠Highㄉ，處理完畢!　\n\nChat ID : \n" + groupChatID + "\n\n開啟的功能為:\n" + enabled + otherMsg,
                    RawMessage.message_id);
            }

            return true;
        }
    }
}