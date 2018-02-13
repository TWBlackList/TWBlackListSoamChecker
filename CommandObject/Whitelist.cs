using CNBlackListSoamChecker.DbManager;
using ReimuAPI.ReimuBase;
using ReimuAPI.ReimuBase.TgData;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;

namespace TWBlackListSoamChecker.CommandObject {
    internal class Whitelist {

        internal bool addWhitelist(TgMessage RawMessage){
            if (RAPI.getIsBotOP(RawMessage.GetSendUser().id)){
                string UID_Value = RawMessage.text.Replace("/addwl","").Replace(" ","");
                if (UID_Value.Length < 5){
                    try{TgApi.getDefaultApiConnection().sendMessage(RawMessage.chat.id, "使用方法 : /addwl UID", RawMessage.message_id);}catch{}
                    return false;
                }else{
                    string json = System.IO.File.ReadAllText("config.json");
                    dynamic jsonObj = Newtonsoft.Json.JsonConvert.DeserializeObject(json);
                    jsonObj["whitelist"].Add(System.Convert.ToInt32(UID_Value));
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

        internal bool deleteWhitelist(TgMessage RawMessage){
            if (RAPI.getIsBotOP(RawMessage.GetSendUser().id)){
                string UID_Value = RawMessage.text.Replace("/delwl","").Replace(" ","");;
                if (UID_Value.Length < 5){
                    try{TgApi.getDefaultApiConnection().sendMessage(RawMessage.chat.id, "使用方法 : /delwl UID", RawMessage.message_id);}catch{}
                    return false;
                }else{
                    string json = System.IO.File.ReadAllText("config.json");
                    dynamic jsonObj = Newtonsoft.Json.JsonConvert.DeserializeObject(json);
                            
                    int i = 0;
                    bool found = false;
                            
                    foreach (var item in jsonObj["whitelist"]){
                        if(jsonObj["whitelist"][i] == UID_Value){
                            found = true;
                            break;
                        }
                        i=i+1;
                    }

                    if(found){
                        jsonObj["whitelist"].Remove(jsonObj["whitelist"][i]);
                        string output = Newtonsoft.Json.JsonConvert.SerializeObject(jsonObj, Newtonsoft.Json.Formatting.Indented);
                        System.IO.File.WriteAllText("config.json", output);
                        try{TgApi.getDefaultApiConnection().sendMessage(RawMessage.chat.id,"刪除成功!",RawMessage.message_id);}catch{}
                        RAPI.reloadConfig();
                    }else{
                        try{TgApi.getDefaultApiConnection().sendMessage(RawMessage.chat.id,"找不到User!",RawMessage.message_id);}catch{}
                    }
                }
            }else{
                TgApi.getDefaultApiConnection().sendMessage(RawMessage.chat.id,"你沒有權限拉",RawMessage.message_id);
                return false;
            }
            return true;
        }

        internal bool listWhitelist(TgMessage RawMessage){
            string json = System.IO.File.ReadAllText("config.json");
            dynamic jsonObj = Newtonsoft.Json.JsonConvert.DeserializeObject(json);
            TgApi.getDefaultApiConnection().sendMessage(RawMessage.chat.id,"SYSOP : \n" + System.String.Join("\n",jsonObj["whitelist"]),RawMessage.message_id);
            return true;
        }
        
    }
}
