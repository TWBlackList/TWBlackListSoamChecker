using System;
using System.IO;
using Newtonsoft.Json;
using ReimuAPI.ReimuBase;
using ReimuAPI.ReimuBase.TgData;

namespace TWBlackListSoamChecker.CommandObject
{
    internal class HKWhitelist
    {
        internal bool addHKWhitelist(TgMessage RawMessage)
        {
            string UID_Value = RawMessage.text.Replace("/addhk", "").Replace(" ", "");
            if (UID_Value.Length < 5)
            {
                TgApi.getDefaultApiConnection().sendMessage(RawMessage.chat.id, "使用方法 : /addhk UID",
                    RawMessage.message_id);
                return false;
            }

            string json = File.ReadAllText("config.json");
            dynamic jsonObj = JsonConvert.DeserializeObject(json);

            int i = 0;
            bool found = false;
            foreach (var item in jsonObj["hk_whitelist"])
            {
                if (jsonObj["hk_whitelist"][i] == UID_Value)
                {
                    found = true;
                    break;
                }

                i = i + 1;
            }

            if (found)
            {
                TgApi.getDefaultApiConnection().sendMessage(RawMessage.chat.id, "已經在名單內了!", RawMessage.message_id);
                return false;
            }

            jsonObj["hk_whitelist"].Add(Convert.ToInt64(UID_Value));
            string output =
                JsonConvert.SerializeObject(jsonObj, Formatting.Indented);
            File.WriteAllText("config.json", output);
            TgApi.getDefaultApiConnection().sendMessage(RawMessage.chat.id, "新增成功!", RawMessage.message_id);

            RAPI.reloadConfig();

            return true;
        }

        internal bool deleteHKWhitelist(TgMessage RawMessage)
        {
            string UID_Value = RawMessage.text.Replace("/delhk", "").Replace(" ", "");
            ;
            if (UID_Value.Length < 5)
            {
                TgApi.getDefaultApiConnection().sendMessage(RawMessage.chat.id, "使用方法 : /delhk UID",
                    RawMessage.message_id);

                return false;
            }
            
            string json = File.ReadAllText("config.json");
            dynamic jsonObj = JsonConvert.DeserializeObject(json);

            int i = 0;
            bool found = false;

            foreach (var item in jsonObj["hk_whitelist"])
            {
                if (jsonObj["hk_whitelist"][i] == UID_Value)
                {
                    found = true;
                    break;
                }

                i = i + 1;
            }

            if (found)
            {
                jsonObj["hk_whitelist"].Remove(jsonObj["hk_whitelist"][i]);
                string output =
                    JsonConvert.SerializeObject(jsonObj, Formatting.Indented);
                File.WriteAllText("config.json", output);
                TgApi.getDefaultApiConnection().sendMessage(RawMessage.chat.id, "刪除成功!", RawMessage.message_id);
                RAPI.reloadConfig();
            }
            else
            {
                TgApi.getDefaultApiConnection()
                    .sendMessage(RawMessage.chat.id, "找不到User!", RawMessage.message_id);
            }

            return true;
        }

        internal bool listHKWhitelist(TgMessage RawMessage)
        {
            string json = File.ReadAllText("config.json");
            dynamic jsonObj = JsonConvert.DeserializeObject(json);
            TgApi.getDefaultApiConnection().sendMessage(RawMessage.chat.id,
                "HKWhitelist : \n" + string.Join("\n", jsonObj["hk_whitelist"]), RawMessage.message_id);
            return true;
        }
    }
}