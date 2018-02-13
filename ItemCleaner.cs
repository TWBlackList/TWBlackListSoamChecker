using System.Collections.Generic;
using ReimuAPI.ReimuBase.Interfaces;
using TWBlackListSoamChecker.DbManager;

namespace TWBlackListSoamChecker
{
    internal class ItemCleaner : IClearItemsReceiver
    {
        public void ClearItems()
        {
            Temp.spamMessageList = null;
            Temp.groupConfig = new Dictionary<long, GroupCfg>();
            Temp.bannedUsers = new Dictionary<int, BanUser>();
        }
    }
}