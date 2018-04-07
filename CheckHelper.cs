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
                    var result = TgApi.getDefaultApiConnection().getChatMember(Temp.ReportGroupID, admin.user.id);
                    if (result.ok)
                        if(result.result.status != "left"  && result.result.user.id != TgApi().getDefaultApiConnection().getMe().id)
                        {
                            status = true;
                            break;
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