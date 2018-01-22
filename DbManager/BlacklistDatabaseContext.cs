using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations;

namespace CNBlackListSoamChecker.DbManager
{
    internal class BlacklistDatabaseContext : DbContext
    {
        public DbSet<BanUser> BanUsers { get; set; }
        public DbSet<BanHistory> BanHistorys { get; set; }
        public DbSet<GroupCfg> GroupConfig { get; set; }
        public DbSet<UnbanRequest> UnbanRequests { get; set; }
        public DbSet<UnbanRequestCount> UnbanRequestCount { get; set; }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            optionsBuilder.UseSqlite("Data Source=" + ConfigManager.GetConfigPath() + "soamchecker.db");
        }
    }

    public class BanUser
    {
        [Key]
        public int UserID { get; set; }
        public int Ban { get; set; } = 1;
        public int Level { get; set; } = 0;
        public int ChannelMessageID { get; set; } = 0;
        public int ReasonMessageID { get; set; } = 0;
        public int HistoryID { get; set; } = 0;
        public long Expires { get; set; } = 0;
        public string Reason { get; set; }

        public string GetBanMessage()
        {
            string msg = "未封鎖";
            if (Ban == 0)
            {
                msg = "已封鎖";
                if (Level == 0)
                {
                    msg += "，處分為 : 封鎖";
                }
                else if (Level == 1)
                {
                    msg += "，處分為 : 警告";
                }
                else
                {
                    msg += "，處分為: " + Level + " (未知)";
                }
                msg += "，將於 " + GetTime.GetExpiresTime(Expires) + " 解除";
                msg += "\n原因是 : " + Reason;
                if (ChannelMessageID != 0) msg += "\n\n參考: https://t.me/" + Temp.MainChannelName + "/" + ChannelMessageID;
            }
            return msg;
        }
    }

    public class BanHistory
    {
        [Key]
        public int ID { get; set; }
        public int UserID { get; set; }
        public int Ban { get; set; } = 0;
        public int Level { get; set; } = 0;
        public int ChannelMessageID { get; set; } = 0;
        public int ReasonMessageID { get; set; } = 0;
        public int AdminID { get; set; } = 0;
        public long Expires { get; set; } = 0;
        public long BanTime { get; set; } = 0;
        public string Reason { get; set; }
    }

    public class UnbanRequest
    {
        [Key]
        public int ID { get; set; }
        public int UserID { get; set; }
        public int Pass { get; set; }
        public long UserReplyTime { get; set; }
        public long AdminReplyTime { get; set; }
        public string UserReplyText { get; set; }
        public string AdminReplyText { get; set; }
    }

    public class GroupCfg
    {
        [Key]
        public long GroupID { get; set; }
        public int AdminOnly { get; set; }
        public int BlackList { get; set; }
        public int AutoKick { get; set; }
        public int AntiHalal { get; set; }
        public int AutoDeleteCommand { get; set; }
        public int AutoDeleteSpamMessage { get; set; }
        public int SubscribeBanList { get; set; }
    }

    public class UnbanRequestCount
    {
        [Key]
        public int UserID { get; set; }
        public int RequestCount { get; set; }
        public int RequestLock { get; set; }
    }
}
