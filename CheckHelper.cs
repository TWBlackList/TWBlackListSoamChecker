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
                console = console + "Getting chat administrator list CID : " + ChatID.ToString();
                foreach (var admin in admins)
                {
                    var result = TgApi.getDefaultApiConnection().getChatMember(Temp.ReportGroupID, admin.user.id);
                    console = console + "\nGetting user in report group UID : " + admin.user.id.ToString();
                    if (result.ok)
                        if(result.result.status != "left")
                        {
                            console = console + "\nUser in report group UID : " + admin.user.id.ToString();
                            status = true;
                            break;
                        }
                }
                
                System.Console.WriteLine(console);
                if (status)
                    Temp.adminInReport.Add(ChatID);

                return status;

            }
            else
            {
                return true;
            }
        }
    }
}