using CNBlackListSoamChecker.DbManager;
using ReimuAPI.ReimuBase;
using ReimuAPI.ReimuBase.TgData;

namespace CNBlackListSoamChecker.CommandObject {
    internal class OP {
        internal bool AddOP(TgMessage RawMessage){
            if (RAPI.getIsBotOP(RawMessage.GetSendUser().id)){
                string UID_Value = RawMessage.text.Replace("/addop","").Replace(" ","");
                if (UID_Value.Length < 5){
                    try{TgApi.getDefaultApiConnection().sendMessage(RawMessage.chat.id,"使用方法 : /addsysop UID",RawMessage.message_id);}catch{}
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
                    try{TgApi.getDefaultApiConnection().sendMessage(RawMessage.chat.id,"使用方法 : /removesysop UID",RawMessage.message_id);}catch{}
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
    }
}
