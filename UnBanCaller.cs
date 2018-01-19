using CNBlackListSoamChecker.DbManager;
using ReimuAPI.ReimuBase;
using ReimuAPI.ReimuBase.TgData;
using ReimuAPI.ReimuBase.Interfaces;
using System;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using System.Collections.Generic;
using System.Runtime.Serialization.Json;

namespace CNBlackListSoamChecker
{
    internal class UnBanCaller
    {
        internal void UnBanCallerThread(int user)
        {
            new Thread(delegate () { CallGroups(user); }).Start();
        }

        internal void CallGroups(int user)
        {
            if (Temp.DisableAdminTools)
            {
                return;
            }
            using (var db = new BlacklistDatabaseContext())
            {
                List<GroupCfg> groupCfg = null;
                try
                {
                        groupCfg = db.GroupConfig.ToList();
                }
                catch (InvalidOperationException)
                {
                    return;
                }
                if (groupCfg == null) return;
                foreach (GroupCfg cfg in groupCfg)
                {
                    var userInChatInfo = TgApi.getDefaultApiConnection().getChatMember(cfg.GroupID, user);

                    if (!userInChatInfo.ok)
                    {
                        try{TgApi.getDefaultApiConnection().unbanChatMember(cfg.GroupID,user);}catch{}
                    }

                    try{TgApi.getDefaultApiConnection().restrictChatMember(cfg.GroupID,user,0,true,true,true,true);}catch{}

                    Thread.Sleep(3000);

                }
            }
        }
    }
}
