using System;
using System.Collections.Generic;
using System.Linq;
using ReimuAPI.ReimuBase;
using ReimuAPI.ReimuBase.TgData;

namespace TWBlackListSoamChecker.CommandObject
{
    internal class GetAdmins
    {
        internal bool GetGroupAdmins(TgMessage RawMessage)
        {
            string[] values = RawMessage.text.Split(' ');

            if (values.Count() == 1)
            {
                TgApi.getDefaultApiConnection()
                    .sendMessage(RawMessage.chat.id, "輸入錯誤\n/groupadmin GID", RawMessage.message_id);
                return true;
            }

            long gid;

            if (!Int64.TryParse(values[1], out gid))
            {
                TgApi.getDefaultApiConnection()
                    .sendMessage(RawMessage.chat.id, "輸入錯誤\n/groupadmin GID", RawMessage.message_id);
                return true;
            }

            GroupUserInfo[] admins = TgApi.getDefaultApiConnection().getChatAdministrators(gid);
            

            string msg = TgApi.getDefaultApiConnection().getChatInfo(gid).result.title + "\nGID : `" + gid.ToString() + "`\n\n\n";

            string creatorMessage = "";
            
            string adminMessage = "\n\n\nAdmin\n";
              
            foreach (var admin in admins)
            {
                if (admin.status == "creator")
                    if (admin.user.username != null)
                        creatorMessage = string.Format("Creator\n`{0}` [{1}](https://t.me/{2})",
                            admin.user.id.ToString(), admin.user.full_name(), admin.user.username);
                    else
                        creatorMessage = string.Format("Creator\n`{0}` {1}",
                            admin.user.id.ToString(), RAPI.escapeMarkdown(admin.user.full_name()));
                else 
                if (admin.user.username != null)
                    adminMessage = string.Format("{0}\n`{1}` [{2}](https://t.me/{3})", adminMessage,
                        admin.user.id.ToString(), admin.user.full_name(), admin.user.username);
                else
                    adminMessage = string.Format("{0}\n`{1}` {2}", adminMessage,
                        admin.user.id.ToString(), RAPI.escapeMarkdown(admin.user.full_name()));
            }
            
            msg = msg + creatorMessage + adminMessage

            TgApi.getDefaultApiConnection()
                .sendMessage(RawMessage.chat.id, msg);


            
            return true;
        }
    }
}