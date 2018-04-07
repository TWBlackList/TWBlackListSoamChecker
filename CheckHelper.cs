using ReimuAPI.ReimuBase;
using ReimuAPI.ReimuBase.TgData;

namespace TWBlackListSoamChecker
{
    public class CheckHelper
    {
        public bool CheckAdminInReportGroup(long ChatID)
        {
            if (Temp.ReportGroupID != 0)
            {
                foreach (long i in Temp.adminInReport)
                    if (i == ChatID)
                        return true;
                string console = "";
                bool status = false;
                GroupUserInfo[] admins = TgApi.getDefaultApiConnection().getChatAdministrators(ChatID, true);
                foreach (var admin in admins)
                {
                    if (admin.user.id != TgApi().getDefaultApiConnection().getMe().id)
                    {
                        var result = TgApi.getDefaultApiConnection().sendMessage(
                            Temp.ReportGroupID,
                            "[加群測試(不用理會此訊息)](tg://user?id=" + admin.user.id.ToString() + ")",
                            ParseMode: TgApi.PARSEMODE_MARKDOWN);
                        if (result.ok)
                        {
                            TgApi.deleteMessage(Temp.ReportGroupID, result.message_id);
                            status = true;
                            break;
                        }
                    }
                }

                if (status)
                {
                    System.Console.WriteLine("[checkHelper] Admin in report group GID : " + ChatID.ToString());
                    Temp.adminInReport.Add(ChatID);
                }
                else
                    System.Console.WriteLine("[checkHelper] Admin not in report group GID : " + ChatID.ToString());

                return status;

            }
            else
            {
                return true;
            }
        }
    }
}