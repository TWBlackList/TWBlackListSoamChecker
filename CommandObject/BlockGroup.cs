using System;
using System.IO;
using Newtonsoft.Json;
using ReimuAPI.ReimuBase;
using ReimuAPI.ReimuBase.TgData;

namespace TWBlackListSoamChecker.CommandObject
{
    internal class BlockGroup
    {
        internal bool addBlockGroup(TgMessage RawMessage)
        {
            var ChatID_Value = RawMessage.text.Replace("/block", "").Replace(" ", "");
            if (ChatID_Value.Length < 10)
            {
                TgApi.getDefaultApiConnection().sendMessage(RawMessage.chat.id, "使用方法 : /block ChatID",
                    RawMessage.message_id);
                return false;
            }

            if (ChatID_Value.Length == 10 && Convert.ToInt64(ChatID_Value) > 0) ChatID_Value = "-100" + ChatID_Value;

            var json = File.ReadAllText("config.json");
            dynamic jsonObj = JsonConvert.DeserializeObject(json);

            var i = 0;
            var found = false;
            foreach (var item in jsonObj["blockgroup_list"])
            {
                if (jsonObj["blockgroup_list"][i] == ChatID_Value)
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

            jsonObj["blockgroup_list"].Add(Convert.ToInt64(ChatID_Value));
            string output =
                JsonConvert.SerializeObject(jsonObj, Formatting.Indented);
            File.WriteAllText("config.json", output);
            TgApi.getDefaultApiConnection().sendMessage(RawMessage.chat.id, "新增成功!", RawMessage.message_id);
            try
            {
                TgApi.getDefaultApiConnection().sendMessage(Convert.ToInt64(ChatID_Value), "此群組禁止使用本服務。");
                TgApi.getDefaultApiConnection().leaveChat(Convert.ToInt64(ChatID_Value));
            }
            catch
            {
            }

            RAPI.reloadConfig();

            return true;
        }

        internal bool deleteBlockGroup(TgMessage RawMessage)
        {
            var ChatID_Value = RawMessage.text.Replace("/unblock", "").Replace(" ", "");

            if (ChatID_Value.Length < 10)
            {
                TgApi.getDefaultApiConnection().sendMessage(RawMessage.chat.id, "使用方法 : /block ChatID",
                    RawMessage.message_id);
                return false;
            }

            if (ChatID_Value.Length == 10 && Convert.ToInt64(ChatID_Value) > 0) ChatID_Value = "-100" + ChatID_Value;

            var json = File.ReadAllText("config.json");
            dynamic jsonObj = JsonConvert.DeserializeObject(json);

            var i = 0;
            var found = false;

            foreach (var item in jsonObj["blockgroup_list"])
            {
                if (jsonObj["blockgroup_list"][i] == ChatID_Value)
                {
                    found = true;
                    break;
                }

                i = i + 1;
            }

            if (found)
            {
                jsonObj["blockgroup_list"].Remove(jsonObj["blockgroup_list"][i]);
                string output =
                    JsonConvert.SerializeObject(jsonObj, Formatting.Indented);
                File.WriteAllText("config.json", output);

                TgApi.getDefaultApiConnection().sendMessage(RawMessage.chat.id, "刪除成功!", RawMessage.message_id);
                RAPI.reloadConfig();
            }
            else
            {
                TgApi.getDefaultApiConnection().sendMessage(RawMessage.chat.id, "找不到ChatID!", RawMessage.message_id);
            }

            return true;
        }

        internal bool listBlockGroup(TgMessage RawMessage)
        {
            var json = File.ReadAllText("config.json");
            dynamic jsonObj = JsonConvert.DeserializeObject(json);
            TgApi.getDefaultApiConnection().sendMessage(RawMessage.chat.id,
                "Block List : \n" + string.Join("\n", jsonObj["blockgroup_list"]), RawMessage.message_id);
            return true;
        }
    }
}