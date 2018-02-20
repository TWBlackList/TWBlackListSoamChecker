using ReimuAPI.ReimuBase;
using ReimuAPI.ReimuBase.TgData;
using TWBlackListSoamChecker.CommandObject;

namespace TWBlackListSoamChecker
{
    internal class AdminCommand
    {
        internal bool AdminCommands(TgMessage RawMessage, string JsonMessage, string Command)
        {
            if (RAPI.getIsBotOP(RawMessage.GetSendUser().id) || RAPI.getIsBotAdmin(RawMessage.GetSendUser().id))
            {
                if (!Temp.DisableBanList)
                    switch (Command)
                    {
                        case "/getspampoints":
                            new SpamStringManager().GetSpamPoints(RawMessage);
                            throw new StopProcessException();
                        case "/twban":
                        case "/ban":
                            new BanUserCommand().Ban(RawMessage, JsonMessage, Command);
                            throw new StopProcessException();
                        case "/twunban":
                        case "/unban":
                            new UnbanUserCommand().Unban(RawMessage);
                            throw new StopProcessException();
                    }
                if (RAPI.getIsBotAdmin(RawMessage.GetSendUser().id))
                {
                    if (!Temp.DisableBanList)
                        switch (Command)
                        {
                            case "/getallspamstr":
                                new SpamStringManager().GetAllInfo(RawMessage);
                                return true;
                            case "/addspamstr":
                                new SpamStringManager().Add(RawMessage);
                                throw new StopProcessException();
                            case "/delspamstr":
                                new SpamStringManager().Remove(RawMessage);
                                throw new StopProcessException();
                            case "/suban":
                                new BanMultiUserCommand().BanMulti(RawMessage, JsonMessage, Command);
                                throw new StopProcessException();
                            case "/suunban":
                                new UnBanMultiUserCommand().UnbanMulti(RawMessage);
                                throw new StopProcessException();
                            case "/getspamstr":
                                new SpamStringManager().GetName(RawMessage);
                                throw new StopProcessException();
                        }
                    switch (Command)
                    {
                        case "/say":
                            new BroadCast().BroadCast_Status(RawMessage);
                            throw new StopProcessException();
                        case "/sdall":
                            new OP().SDAll(RawMessage);
                            throw new StopProcessException();
                        case "/seall":
                            new OP().SEAll(RawMessage);
                            throw new StopProcessException();
                        case "/addop":
                            new OP().addOP(RawMessage);
                            throw new StopProcessException();
                        case "/delop":
                            new OP().delOP(RawMessage);
                            throw new StopProcessException();
                        case "/addwl":
                            new Whitelist().addWhitelist(RawMessage);
                            throw new StopProcessException();
                        case "/delwl":
                            new Whitelist().deleteWhitelist(RawMessage);
                            throw new StopProcessException();
                        case "/lswl":
                            new Whitelist().listWhitelist(RawMessage);
                            throw new StopProcessException();
                        case "/block":
                            new BlockGroup().addBlockGroup(RawMessage);
                            throw new StopProcessException();
                        case "/unblock":
                            new BlockGroup().deleteBlockGroup(RawMessage);
                            throw new StopProcessException();
                        case "/blocks":
                            new BlockGroup().listBlockGroup(RawMessage);
                            throw new StopProcessException();
                    }
                }
            }
            return false;
        }
    }
}