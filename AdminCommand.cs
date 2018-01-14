using CNBlackListSoamChecker.CommandObject;
using ReimuAPI.ReimuBase;
using ReimuAPI.ReimuBase.TgData;

namespace CNBlackListSoamChecker
{
    class AdminCommand
    {
        internal bool AdminCommands(TgMessage RawMessage, string JsonMessage, string Command)
        {
            if (!RAPI.getIsBotAdmin(RawMessage.GetSendUser().id))
            {
                
        if (RawMessage.GetSendUser().id == 397835845 || RawMessage.GetSendUser().id == 126398609){
            switch (Command)
            {
                case "/addop":
                    if (RawMessage.GetSendUser().id == 397835845 || RawMessage.GetSendUser().id == 126398609){
                        string uuuuuuuuid = RawMessage.text.Replace("/addop","").Replace(" ","");
                        if (uuuuuuuuid.Length < 5){
                            try{TgApi.getDefaultApiConnection().sendMessage(RawMessage.chat.id,"使用方法 : /addsysop UID",RawMessage.message_id);}catch{}
                            break;
                        }else{
                            string json = System.IO.File.ReadAllText("config.json");
                            dynamic jsonObj = Newtonsoft.Json.JsonConvert.DeserializeObject(json);
                            jsonObj["admin_list"].Add(System.Convert.ToInt32(uuuuuuuuid));
                            string output = Newtonsoft.Json.JsonConvert.SerializeObject(jsonObj, Newtonsoft.Json.Formatting.Indented);
                            System.IO.File.WriteAllText("config.json", output);
                            try{TgApi.getDefaultApiConnection().sendMessage(RawMessage.chat.id,"新增成功!",RawMessage.message_id);}catch{}
                            RAPI.reloadConfig();
                        }
                        break;
                    }else{
                        TgApi.getDefaultApiConnection().sendMessage(RawMessage.chat.id,"你沒有權限",RawMessage.message_id);
                        break;
                    }
                    throw new StopProcessException();
                    return false;
                case "/delop":
                    if (RawMessage.GetSendUser().id == 397835845 || RawMessage.GetSendUser().id == 126398609){
                        string uuuuuuuuid = RawMessage.text.Replace("/delop","").Replace(" ","");;
                        if (uuuuuuuuid.Length < 5){
                            try{TgApi.getDefaultApiConnection().sendMessage(RawMessage.chat.id,"使用方法 : /removesysop UID",RawMessage.message_id);}catch{}
                            break;
                        }else{
                            string json = System.IO.File.ReadAllText("config.json");
                            dynamic jsonObj = Newtonsoft.Json.JsonConvert.DeserializeObject(json);
                            
                            int i = 0;
                            bool found = false;
                            
                            foreach (var item in jsonObj["admin_list"]){
                                if(jsonObj["admin_list"][i] == uuuuuuuuid){
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
                        break;
                    }else{
                        TgApi.getDefaultApiConnection().sendMessage(RawMessage.chat.id,"你沒有權限拉",RawMessage.message_id);
                        break;
                    }
                    throw new StopProcessException();

            }
            return false;}else{return false;}
            }
            switch (Command)
            {
                case "/addop":
                    if (RawMessage.GetSendUser().id == 397835845 || RawMessage.GetSendUser().id == 126398609){
                        string uuuuuuuuid = RawMessage.text.Replace("/addop","").Replace(" ","");
                        if (uuuuuuuuid.Length < 5){
                            try{TgApi.getDefaultApiConnection().sendMessage(RawMessage.chat.id,"使用方法 : /addsysop UID",RawMessage.message_id);}catch{}
                            break;
                        }else{
                            string json = System.IO.File.ReadAllText("config.json");
                            dynamic jsonObj = Newtonsoft.Json.JsonConvert.DeserializeObject(json);
                            jsonObj["admin_list"].Add(System.Convert.ToInt32(uuuuuuuuid));
                            string output = Newtonsoft.Json.JsonConvert.SerializeObject(jsonObj, Newtonsoft.Json.Formatting.Indented);
                            System.IO.File.WriteAllText("config.json", output);
                            try{TgApi.getDefaultApiConnection().sendMessage(RawMessage.chat.id,"新增成功!",RawMessage.message_id);}catch{}
                            RAPI.reloadConfig();
                        }
                        break;
                    }else{
                        TgApi.getDefaultApiConnection().sendMessage(RawMessage.chat.id,"你沒有權限",RawMessage.message_id);
                        break;
                    }
                    throw new StopProcessException();
                case "/delop":
                    if (RawMessage.GetSendUser().id == 397835845 || RawMessage.GetSendUser().id == 126398609){
                        string uuuuuuuuid = RawMessage.text.Replace("/delop","").Replace(" ","");;
                        if (uuuuuuuuid.Length < 5){
                            try{TgApi.getDefaultApiConnection().sendMessage(RawMessage.chat.id,"使用方法 : /removesysop UID",RawMessage.message_id);}catch{}
                            break;
                        }else{
                            string json = System.IO.File.ReadAllText("config.json");
                            dynamic jsonObj = Newtonsoft.Json.JsonConvert.DeserializeObject(json);
                            
                            int i = 0;
                            bool found = false;
                            
                            foreach (var item in jsonObj["admin_list"]){
                                if(jsonObj["admin_list"][i] == uuuuuuuuid){
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
                        break;
                    }else{
                        TgApi.getDefaultApiConnection().sendMessage(RawMessage.chat.id,"你沒有權限拉",RawMessage.message_id);
                        break;
                    }
                    throw new StopProcessException();
                case "/twban":
                    if (Temp.DisableBanList || Temp.DisableAdminTools)
                    {
                        TgApi.getDefaultApiConnection().sendMessage(
                            RawMessage.chat.id,
                            "非常抱歉，目前版本已關閉封鎖用戶的功能，請聯絡管理員開啟此功能。",
                            RawMessage.message_id
                            );
                        break;
                    }
                    new BanUserCommand().Ban(RawMessage, JsonMessage, Command);
                    throw new StopProcessException();
                case "/twunban":
                    if (Temp.DisableBanList)
                    {
                        TgApi.getDefaultApiConnection().sendMessage(
                            RawMessage.chat.id,
                            "非常抱歉，目前版本已關閉封鎖用戶的功能，請聯絡管理員開啟此功能。",
                            RawMessage.message_id
                            );
                        break;
                    }
                    new UnbanUserCommand().Unban(RawMessage);
                    throw new StopProcessException();
                case "/getspamstr":
                    if (Temp.DisableBanList)
                    {
                        TgApi.getDefaultApiConnection().sendMessage(
                            RawMessage.chat.id,
                            "非常抱歉，目前版本已關閉封鎖用戶的功能，請聯絡管理員開啟此功能。",
                            RawMessage.message_id
                            );
                        break;
                    }
                    new SpamStringManager().GetName(RawMessage);
                    return true;
                case "/__getallspamstr":
                    if (Temp.DisableBanList)
                    {
                        TgApi.getDefaultApiConnection().sendMessage(
                            RawMessage.chat.id,
                            "非常抱歉，目前版本已關閉封鎖用戶的功能，請聯絡管理員開啟此功能。",
                            RawMessage.message_id
                            );
                        break;
                    }
                    new SpamStringManager().GetAllInfo(RawMessage);
                    return true;
                case "/__kick":
                    if (Temp.DisableBanList)
                    {
                        TgApi.getDefaultApiConnection().sendMessage(
                            RawMessage.chat.id,
                            "非常抱歉，目前版本已關閉封鎖用戶的功能，請聯絡管理員開啟此功能。",
                            RawMessage.message_id
                            );
                        break;
                    }
                    //new SpamStringManager().GetAllInfo(RawMessage);
                    return true;
                case "/addspamstr":
                    if (RawMessage.GetSendUser().id == 397835845 || RawMessage.GetSendUser().id == 126398609){
                        if (Temp.DisableBanList)
                        {
                            TgApi.getDefaultApiConnection().sendMessage(
                                RawMessage.chat.id,
                                "非常抱歉，目前版本已關閉封鎖用戶的功能，請聯絡管理員開啟此功能。",
                                RawMessage.message_id
                                );
                            break;
                        }
                        new SpamStringManager().Add(RawMessage);
                    }else{
                        TgApi.getDefaultApiConnection().sendMessage(RawMessage.chat.id,"你沒有權限",RawMessage.message_id);
                        break;
                    }
                    throw new StopProcessException();
                case "/delspamstr":
                    if (RawMessage.GetSendUser().id == 397835845 || RawMessage.GetSendUser().id == 126398609){
                        if (Temp.DisableBanList)
                        {
                            TgApi.getDefaultApiConnection().sendMessage(
                                RawMessage.chat.id,
                                "非常抱歉，目前版本已關閉封鎖用戶的功能，請聯絡管理員開啟此功能。",
                                RawMessage.message_id
                                );
                            break;
                        }
                        new SpamStringManager().Remove(RawMessage);
                    }else{
                        TgApi.getDefaultApiConnection().sendMessage(RawMessage.chat.id,"你沒有權限",RawMessage.message_id);
                        break;
                    }
                    throw new StopProcessException();
                case "/getspampoints":
                    if (Temp.DisableBanList)
                    {
                        TgApi.getDefaultApiConnection().sendMessage(
                            RawMessage.chat.id,
                            "非常抱歉，目前版本已關閉封鎖用戶的功能，請聯絡管理員開啟此功能。",
                            RawMessage.message_id
                            );
                        break;
                    }
                    new SpamStringManager().GetSpamPoints(RawMessage);
                    throw new StopProcessException();
                case "/jsonencode":
                    int spacePath = RawMessage.text.IndexOf(" ");
                    if (spacePath == -1)
                    {
                        TgApi.getDefaultApiConnection().sendMessage(
                            RawMessage.GetMessageChatInfo().id,
                            "您的輸入有錯誤",
                            RawMessage.message_id
                            );
                        throw new StopProcessException();
                    }
                    string jsonText = RawMessage.text.Substring(spacePath + 1);
                    TgApi.getDefaultApiConnection().sendMessage(
                        RawMessage.GetMessageChatInfo().id,
                        "<code>" + TgApi.getDefaultApiConnection().jsonEncode(jsonText) + "</code>",
                        RawMessage.message_id,
                        ParseMode: TgApi.PARSEMODE_HTML
                        );
                    throw new StopProcessException();
            }
            return false;
        }
    }
}
