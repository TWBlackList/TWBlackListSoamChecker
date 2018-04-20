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

                foreach (long i in Temp.adminChecking)
                    if (i == ChatID)
                        return true;
                
                Temp.adminChecking.Add(ChatID);
                
                bool status = false;
                GroupUserInfo[] admins = TgApi.getDefaultApiConnection().getChatAdministrators(ChatID,true);
                System.Console.WriteLine("[checkHelper] Getting Chat Administrator List ChatID : " + ChatID);
                foreach (var admin in admins)
                {
                    if (admin.user.id != TgApi.getDefaultApiConnection().getMe().id)
                    {
                        var result = TgApi.getDefaultApiConnection().getChatMember(Temp.ReportGroupID , admin.user.id);
                        if (result.ok)
                            if (result.result.status != "left")
                            {
                                status = true;  
                                break;
                            }
                    }
                }

                if(!status)
                    foreach (var admin in admins)
                    {
                        if (admin.user.id != TgApi.getDefaultApiConnection().getMe().id)
                        {
                            SendMessageResult result = TgApi.getDefaultApiConnection().sendMessage(
                                Temp.ReportGroupID,
                                "[加群測試(不用理會此訊息)](tg://user?id=" + admin.user.id.ToString() + ")",
                                ParseMode: TgApi.PARSEMODE_MARKDOWN);
                            if (result.ok)
                            {
                                TgApi.getDefaultApiConnection().deleteMessage(Temp.ReportGroupID, result.result.message_id);
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

                Temp.adminChecking.Remove(ChatID);
                
                return status;

            }
            else
            {
                return true;
            }
        }
    }
}