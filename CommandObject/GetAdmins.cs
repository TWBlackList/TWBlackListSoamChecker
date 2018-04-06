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
            
            string msg = "GID : " + gid.ToString() + "\n";
            string creator = "Creator : ";
            string admin_msg = "";
            foreach (var admin in admins)
            {
                if (admin.status == "creator")
                    if (admin.user.username != null)
                        creator = creator + admin.user.id.ToString() + " " + admin.user.full_name() + " @" +
                                  admin.user.username + "\n\n";
                    else
                        creator = creator + admin.user.id.ToString() + " " + admin.user.full_name() + "\n\n";
                else
                if (admin.user.username != null)
                    admin_msg = admin_msg + admin.user.id.ToString() + " " + admin.user.full_name() + " @" +
                                admin.user.username + "\n";
                else
                    admin_msg = admin_msg + admin.user.id.ToString() + " " + admin.user.full_name() + "\n";
            }

            string msg = msg + creator + admin_msg;
            
            TgApi.getDefaultApiConnection()
                .sendMessage(RawMessage.chat.id, msg+creator+admin_msg);

            
            return true;
        }
    }
}