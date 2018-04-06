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

            string msg = "GID : " + gid.ToString() + "\nCreator : ";
            
            foreach (var admin in admins)
            {
                if (admin.user.username != null)
                    msg = msg + admin.user.id.ToString() + " " +  admin.user.full_name() + " @" + admin.user.username + "\n";
                else
                    msg = msg + admin.user.id.ToString() +  " " +  admin.user.full_name() + "\n";
            }

            TgApi.getDefaultApiConnection()
                .sendMessage(RawMessage.chat.id, msg);
            
            return true;
        }
    }
}