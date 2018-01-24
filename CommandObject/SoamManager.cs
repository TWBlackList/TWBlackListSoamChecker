using CNBlackListSoamChecker.DbManager;
using ReimuAPI.ReimuBase;
using ReimuAPI.ReimuBase.TgData;

namespace CNBlackListSoamChecker.CommandObject
{
    class SoamManager
    {
        public void SoamEnable(TgMessage message)
        {
            if (!TgApi.getDefaultApiConnection().checkIsAdmin(message.chat.id, message.from.id))
            {
                TgApi.getDefaultApiConnection().sendMessage(message.chat.id, "您不是這個群組的管理員，無法執行此操作。", message.message_id);
                return;
            }
            string enabled = "";
            string otherMsg = "";
            int AdminOnly = 3;
            int Blacklist = 3;
            int AutoKick = 3;
            int AntiHalal = 3;
            int AutoDeleteSpamMessage = 3;
            int AutoDeleteCommand = 3;
            int SubscribeBanList = 3;
            string text = message.text.ToLower();
            if (text.IndexOf(" adminonly") != -1)
            {
                AdminOnly = 0;
                enabled += " AdminOnly";
            }
            if (text.IndexOf(" blacklist") != -1)
            {
                Blacklist = 0;
                if (Temp.DisableBanList)
                {
                    otherMsg += "\nBlackList 開啟失敗，目前版本未啟用此功能。。";
                }
                else
                {
                    enabled += " Blacklist";
                }
            }
            if (text.IndexOf(" autokick") != -1)
            {
                AutoKick = 0;
                if (Temp.DisableBanList)
                {
                    otherMsg += "\nAutoKick 開啟失敗，目前版本未啟用此功能。。";
                }
                else
                {
                    enabled += " AutoKick";
                }
            }
            if (text.IndexOf(" antihalal") != -1)
            {
                AntiHalal = 0;
                enabled += " AntiHalal";
            }
            if (text.IndexOf(" autodeletespammessage") != -1)
            {
                AutoDeleteSpamMessage = 0;
                if (Temp.DisableBanList)
                {
                    otherMsg += "\nAutoDeleteSpamMessage 開啟失敗，目前版本未啟用此功能。。";
                }
                else
                {
                    enabled += " AutoDeleteSpamMessage";
                }
            }
            if (text.IndexOf(" autodeletecommand") != -1)
            {
                AutoDeleteCommand = 0;
                enabled += " AutoDeleteCommand";
            }
            if (text.IndexOf(" subscribebanlist") != -1)
            {
                SubscribeBanList = 0;
                if (Temp.DisableBanList)
                {
                    otherMsg += "\nSubscribeBanList 開啟失敗，目前版本未啟用此功能。。";
                }
                else
                {
                    enabled += " SubscribeBanList";
                }
                enabled += " SubscribeBanList";
            }
            Temp.GetDatabaseManager().SetGroupConfig(
                message.chat.id,
                AdminOnly: AdminOnly,
                BlackList: Blacklist,
                AutoKick: AutoKick,
                AntiHalal: AntiHalal,
                AutoDeleteSpamMessage: AutoDeleteSpamMessage,
                AutoDeleteCommand: AutoDeleteCommand,
                SubscribeBanList: SubscribeBanList
                );
            if (enabled == "")
            {
                if (Temp.MainChannelName  == null)
                {
                    enabled = "指令錯誤，請檢查\n\n請您使用 /soamenable [所需的功能] 來啟用您需要的功能。\n" +
                        "例如: \"/soamenable BlackList\" (不包含引號) 則可以使用黑名單列表警告。\n" +
                        "您也可以使用多個選項，例如: \"/soamenable BlackList AutoKick\" (不包含引號) " +
                        "則可以使用黑名單列表警告，在警告後還會將成員移出群組。\n\n" +
                        "您可以使用 /soamstatus 取得目前群組開啟或關閉的功能。";
                }
                else
                {
                    enabled = "指令錯誤，請檢查\n\n請您使用 /soamenable [所需的功能] 來啟用您需要的功能。\n" +
                        "例如: \"/soamenable BlackList\" (不包含引號) 則可以使用由 @" + Temp.MainChannelName + " 提供的黑名單列表警告。\n" +
                        "您也可以使用多個選項，例如: \"/soamenable BlackList AutoKick\" (不包含引號) " +
                        "則可以使用由 @" + Temp.MainChannelName + " 提供的黑名單列表警告，在警告後還會將成員移出群組。\n\n" +
                        "您可以使用 /soamstatus 取得目前群組開啟或關閉的功能。";
                }
                TgApi.getDefaultApiConnection().sendMessage(message.chat.id, "失敗， " + enabled + otherMsg, message.message_id);
            }else{
                TgApi.getDefaultApiConnection().sendMessage(message.chat.id, "成功，開啟的功能有: " + enabled + otherMsg, message.message_id);
            }
            
            return;
        }

        public void SoamDisable(TgMessage message)
        {
            if (!TgApi.getDefaultApiConnection().checkIsAdmin(message.chat.id, message.from.id))
            {
                TgApi.getDefaultApiConnection().sendMessage(message.chat.id, "你不是這個群組的管理員，無法執行此操作。", message.message_id);
                return;
            }
            string enabled = "";
            int AdminOnly = 3;
            int Blacklist = 3;
            int AutoKick = 3;
            int AntiHalal = 3;
            int AutoDeleteSpamMessage = 3;
            int AutoDeleteCommand = 3;
            int SubscribeBanList = 3;
            string text = message.text.ToLower();
            if (text.IndexOf(" adminonly") != -1)
            {
                AdminOnly = 1;
                enabled += " AdminOnly";
            }
            if (text.IndexOf(" blacklist") != -1)
            {
                Blacklist = 1;
                enabled += " Blacklist";
            }
            if (text.IndexOf(" autokick") != -1)
            {
                AutoKick = 1;
                enabled += " AutoKick";
            }
            if (text.IndexOf(" antihalal") != -1)
            {
                AntiHalal = 1;
                enabled += " AntiHalal";
            }
            if (text.IndexOf(" autodeletespammessage") != -1)
            {
                AutoDeleteSpamMessage = 1;
                enabled += " AutoDeleteSpamMessage";
            }
            if (text.IndexOf(" autodeletecommand") != -1)
            {
                AutoDeleteCommand = 1;
                enabled += " AutoDeleteCommand";
            }
            if (text.IndexOf(" subscribebanlist") != -1)
            {
                SubscribeBanList = 1;
                enabled += " SubscribeBanList";
            }
            Temp.GetDatabaseManager().SetGroupConfig(
                message.chat.id,
                AdminOnly: AdminOnly,
                BlackList: Blacklist,
                AutoKick: AutoKick,
                AntiHalal: AntiHalal,
                AutoDeleteSpamMessage: AutoDeleteSpamMessage,
                AutoDeleteCommand: AutoDeleteCommand,
                SubscribeBanList: SubscribeBanList
                );
            if (enabled == "")
            {
                if (Temp.MainChannelName == null)
                {
                    enabled = "指令錯誤，請檢查\n\n請您使用 /soamdisable [要關閉的功能] 來關閉您需要的功能。\n" +
                    "例如: \"/soamdisable BlackList\" (不包含引號)  則可以關閉黑名單列表警告。\n" +
                    "您也可以使用多個選項，例如: \"/soamdisable BlackList AutoKick\" (不包含引號) " +
                    "則可以關閉黑名單列表警告，並關閉在警告後將成員移出群組的功能。" +
                    "您可以使用 /soamstatus 取得目前群組開啟或關閉的功能。";
                }
                else
                {
                    enabled = "指令錯誤，請檢查\n\n請您使用 /soamdisable [要關閉的功能] 來關閉您需要的功能。\n" +
                    "例如: \"/soamdisable BlackList\" (不包含引號) 則可以關閉由 @" + Temp.MainChannelName + " 提供的黑名單列表警告。\n" +
                    "您也可以使用多個選項，例如: \"/soamdisable BlackList AutoKick\" (不包含引號) " +
                    "則可以關閉由 @" + Temp.MainChannelName + " 提供的黑名單列表警告，並關閉在警告後將成員移出群組的功能。" +
                    "您可以使用 /soamstatus 取得目前群組開啟或關閉的功能。";
                }
                TgApi.getDefaultApiConnection().sendMessage(message.chat.id, "失敗， " + enabled , message.message_id);
            }else{
                TgApi.getDefaultApiConnection().sendMessage(message.chat.id, "成功，關閉的功能有: " + enabled , message.message_id);
            }
            return;
        }

        public void SoamStatus(TgMessage message)
        {
            string byChannelName = "";
            if (Temp.MainChannelName != null)
            {
                byChannelName = " (by @TWBlackList )";
            }
            GroupCfg gc = Temp.GetDatabaseManager().GetGroupConfig(message.chat.id);
            TgApi.getDefaultApiConnection().sendMessage(
                message.chat.id,
                "BlackList" + byChannelName + ": " + (gc.BlackList == 0) + "\n" +
                "AutoKick: " + (gc.AutoKick == 0) + "\n" +
                "AntiHalal: " + (gc.AntiHalal == 0) + "\n" +
                "AutoDeleteSpamMessage: " + (gc.AutoDeleteSpamMessage == 0) + "\n" +
                "AutoDeleteCommand: " + (gc.AutoDeleteCommand == 0) + "\n" +
                "AdminOnly: " + (gc.AdminOnly == 0) + "\n" +
                "SubscribeBanList: " + (gc.SubscribeBanList == 0),
                message.message_id
                );
            return;
        }
    }
}
