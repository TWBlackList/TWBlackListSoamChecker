using TWBlackListSoamChecker.DbManager;
using ReimuAPI.ReimuBase.Interfaces;
using System.Collections.Generic;

namespace TWBlackListSoamChecker
{
    class ItemCleaner : IClearItemsReceiver
    {
        public void ClearItems()
        {
            Temp.spamMessageList = null;
            Temp.groupConfig = new Dictionary<long, GroupCfg>() { };
            Temp.bannedUsers = new Dictionary<int, BanUser>() { };
            return;
        }
    }
}
