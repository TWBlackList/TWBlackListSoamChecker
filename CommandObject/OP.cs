using CNBlackListSoamChecker.DbManager;
using ReimuAPI.ReimuBase;
using ReimuAPI.ReimuBase.TgData;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;

namespace CNBlackListSoamChecker.CommandObject {
    internal class OP {

        internal bool AddOP(TgMessage RawMessage){
            if (RAPI.getIsBotOP(RawMessage.GetSendUser().id)){
                string UID_Value = RawMessage.text.Replace("/addop","").Replace(" ","");
                if (UID_Value.Length < 5){
                    try{TgApi.getDefaultApiConnection().sendMessage(RawMessage.chat.id, "使用方法 : /addop UID", RawMessage.message_id);}catch{}
                    return false;
                }else{
                    string json = System.IO.File.ReadAllText("config.json");
                    dynamic jsonObj = Newtonsoft.Json.JsonConvert.DeserializeObject(json);
                    jsonObj["admin_list"].Add(System.Convert.ToInt32(UID_Value));
                    string output = Newtonsoft.Json.JsonConvert.SerializeObject(jsonObj, Newtonsoft.Json.Formatting.Indented);
                    System.IO.File.WriteAllText("config.json", output);
                    try{TgApi.getDefaultApiConnection().sendMessage(RawMessage.chat.id,"新增成功!",RawMessage.message_id);}catch{}
                    RAPI.reloadConfig();
                }
            }else{
                TgApi.getDefaultApiConnection().sendMessage(RawMessage.chat.id,"你沒有權限",RawMessage.message_id);
                return false;
            }
            return true;
        }

        internal bool DelOP(TgMessage RawMessage){
            if (RAPI.getIsBotOP(RawMessage.GetSendUser().id)){
                string UID_Value = RawMessage.text.Replace("/delop","").Replace(" ","");;
                if (UID_Value.Length < 5){
                    try{TgApi.getDefaultApiConnection().sendMessage(RawMessage.chat.id, "使用方法 : /delop UID", RawMessage.message_id);}catch{}
                    return false;
                }else{
                    string json = System.IO.File.ReadAllText("config.json");
                    dynamic jsonObj = Newtonsoft.Json.JsonConvert.DeserializeObject(json);
                            
                    int i = 0;
                    bool found = false;
                            
                    foreach (var item in jsonObj["admin_list"]){
                        if(jsonObj["admin_list"][i] == UID_Value){
                            found = true;
                            break;
                        }
                        i=i+1;
                    }

                    if(found){
                        jsonObj["admin_list"].Remove(jsonObj["admin_list"][i]);
                        string output = Newtonsoft.Json.JsonConvert.SerializeObject(jsonObj, Newtonsoft.Json.Formatting.Indented);
                        System.IO.File.WriteAllText("config.json", output);
                        try{TgApi.getDefaultApiConnection().sendMessage(RawMessage.chat.id,"刪除成功!",RawMessage.message_id);}catch{}
                        RAPI.reloadConfig();
                    }else{
                        try{TgApi.getDefaultApiConnection().sendMessage(RawMessage.chat.id,"找不到OP!",RawMessage.message_id);}catch{}
                    }
                }
            }else{
                TgApi.getDefaultApiConnection().sendMessage(RawMessage.chat.id,"你沒有權限拉",RawMessage.message_id);
                return false;
            }
            return true;
        }

        internal bool LsOP(TgMessage RawMessage){
            string json = System.IO.File.ReadAllText("config.json");
            dynamic jsonObj = Newtonsoft.Json.JsonConvert.DeserializeObject(json);
            TgApi.getDefaultApiConnection().sendMessage(RawMessage.chat.id,"SYSOP : \n" + System.String.Join("\n",jsonObj["admin_list"]),RawMessage.message_id);
            return true;
        }
        
        internal bool SDAll(TgMessage RawMessage)
        {
            new Thread(delegate () { SoamDisable_All(RawMessage); }).Start();
            return true;
        }        
        
        internal bool SEAll(TgMessage RawMessage)
        {
            new Thread(delegate () { SoamEnable_All(RawMessage); }).Start();
            return true;
        }

        internal bool SoamDisable_All(TgMessage RawMessage){
            if (RAPI.getIsBotOP(RawMessage.GetSendUser().id)){
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

                using (var db = new BlacklistDatabaseContext()){
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
                            AdminOnly: AdminOnly,
                            BlackList: Blacklist,
                            AutoKick: AutoKick,
                            AntiHalal: AntiHalal,
                            AutoDeleteSpamMessage: AutoDeleteSpamMessage,
                            AutoDeleteCommand: AutoDeleteCommand,
                            SubscribeBanList: SubscribeBanList
                            );
                        if (enabled == "")
                        {
                            if (Temp.MainChannelName == null)
                            {
                                enabled = "";
                            }
                            else
                            {
                                enabled = "";
                            }
                        }
                        groupChatID = groupChatID + "\n" + cfg.GroupID;
                        
                        }
                    TgApi.getDefaultApiConnection().sendMessage(RawMessage.chat.id,"有夠Highㄉ，處理完畢!　\n\nChat ID : \n" + groupChatID + "\n\n關閉的功能為:\n" + enabled,RawMessage.message_id);
                }
            }else{
                TgApi.getDefaultApiConnection().sendMessage(RawMessage.chat.id,"你沒有權限拉",RawMessage.message_id);
                return false;
            }
            return true;
        }

        internal bool SoamEnable_All(TgMessage RawMessage){
            if (RAPI.getIsBotOP(RawMessage.GetSendUser().id)){
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
                    {
                        otherMsg += "\nBlackList 開啟失敗，目前版本未啟用此功能。。";
                    }
                    else
                    {
                        enabled += " Blacklist";
                    }
                }
                if (text.IndexOf(" autokick") != -1)
                {
                    AutoKick = 0;
                    if (Temp.DisableBanList)
                    {
                        otherMsg += "\nAutoKick 開啟失敗，目前版本未啟用此功能。。";
                    }
                    else
                    {
                        enabled += " AutoKick";
                    }
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
                    {
                        otherMsg += "\nAutoDeleteSpamMessage 開啟失敗，目前版本未啟用此功能。。";
                    }
                    else
                    {
                        enabled += " AutoDeleteSpamMessage";
                    }
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
                    {
                        otherMsg += "\nSubscribeBanList 開啟失敗，目前版本未啟用此功能。。";
                    }
                    else
                    {
                        enabled += " SubscribeBanList";
                    }
                    enabled += " SubscribeBanList";
                }

                    using (var db = new BlacklistDatabaseContext()){
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
                                AdminOnly: AdminOnly,
                                BlackList: Blacklist,
                                AutoKick: AutoKick,
                                AntiHalal: AntiHalal,
                                AutoDeleteSpamMessage: AutoDeleteSpamMessage,
                                AutoDeleteCommand: AutoDeleteCommand,
                                SubscribeBanList: SubscribeBanList
                                );
                            if (enabled == "")
                            {
                                if (Temp.MainChannelName == null)
                                {
                                    enabled = "";
                                }
                                else
                                {
                                    enabled = "";
                                }
                            }
                            groupChatID = groupChatID + "\n" + cfg.GroupID;
                        }
                    TgApi.getDefaultApiConnection().sendMessage(RawMessage.chat.id,"有夠Highㄉ，處理完畢!　\n\nChat ID : \n" + groupChatID + "\n\n開啟的功能為:\n" + enabled + otherMsg,RawMessage.message_id);
                }
            }else{
                TgApi.getDefaultApiConnection().sendMessage(RawMessage.chat.id,"你沒有權限拉",RawMessage.message_id);
                return false;
            }
            return true;
        }

    }
}
