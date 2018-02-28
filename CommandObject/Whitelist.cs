using System;
using System.IO;
using Newtonsoft.Json;
using ReimuAPI.ReimuBase;
using ReimuAPI.ReimuBase.TgData;

namespace TWBlackListSoamChecker.CommandObject
{
    internal class Whitelist
    {
        internal bool addWhitelist(TgMessage RawMessage)
        {
            string UID_Value = RawMessage.text.Replace("/addwl", "").Replace(" ", "");
            if (UID_Value.Length < 5)
            {
                TgApi.getDefaultApiConnection().sendMessage(RawMessage.chat.id, "使用方法 : /addwl UID",
                    RawMessage.message_id);
                return false;
            }
            if (UID_Value.Length == 10 && Convert.ToInt64(UID_Value) > 0) UID_Value = "-100" + UID_Value;

            string json = File.ReadAllText("config.json");
            dynamic jsonObj = JsonConvert.DeserializeObject(json);

            int i = 0;
            bool found = false;
            foreach (var item in jsonObj["whitelist"])
            {
                if (jsonObj["whitelist"][i] == UID_Value)
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

            jsonObj["whitelist"].Add(Convert.ToInt64(UID_Value));
            string output =
                JsonConvert.SerializeObject(jsonObj, Formatting.Indented);
            File.WriteAllText("config.json", output);
            TgApi.getDefaultApiConnection().sendMessage(RawMessage.chat.id, "新增成功!", RawMessage.message_id);

            RAPI.reloadConfig();

            return true;
        }

        internal bool deleteWhitelist(TgMessage RawMessage)
        {
            string UID_Value = RawMessage.text.Replace("/delwl", "").Replace(" ", "");
            ;
            if (UID_Value.Length < 5)
            {
                TgApi.getDefaultApiConnection().sendMessage(RawMessage.chat.id, "使用方法 : /delwl UID",
                    RawMessage.message_id);

                return false;
            }
            if (UID_Value.Length == 10 && Convert.ToInt64(UID_Value) > 0) UID_Value = "-100" + UID_Value;

            string json = File.ReadAllText("config.json");
            dynamic jsonObj = JsonConvert.DeserializeObject(json);

            int i = 0;
            bool found = false;

            foreach (var item in jsonObj["whitelist"])
            {
                if (jsonObj["whitelist"][i] == UID_Value)
                {
                    found = true;
                    break;
                }

                i = i + 1;
            }

            if (found)
            {
                jsonObj["whitelist"].Remove(jsonObj["whitelist"][i]);
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

        internal bool listWhitelist(TgMessage RawMessage)
        {
            string json = File.ReadAllText("config.json");
            dynamic jsonObj = JsonConvert.DeserializeObject(json);
            TgApi.getDefaultApiConnection().sendMessage(RawMessage.chat.id,
                "Whitelist : \n" + string.Join("\n", jsonObj["whitelist"]), RawMessage.message_id);
            return true;
        }
    }
}