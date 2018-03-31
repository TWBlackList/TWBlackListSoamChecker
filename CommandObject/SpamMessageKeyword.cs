using System.Text.RegularExpressions;
using TWBlackListSoamChecker.DbManager;

// 这是迷之 Spam Message Checker

namespace TWBlackListSoamChecker.CommandObject
{
    internal class SpamMessageKeyword
    {
        public string GetEqualsKeyword(SpamMessageObj[] spamMessages, string text) // Mode 0 完全匹配
        {
            string totalPoints = "";
            foreach (SpamMessageObj msg in spamMessages)
                if (text.ToLower().Equals(msg.Message.ToLower()))
                    totalPoints = totalPoints + msg.Message.ToLower() + " : " + msg.Point.ToString() + "\n";
            return totalPoints;
        }

        public string GetRegexKeyword(SpamMessageObj[] spamMessages, string text) // Mode 1 正则
        {
            string totalPoints = "";
            foreach (SpamMessageObj msg in spamMessages)
                if (new Regex(msg.Message).Match(text).Success)
                    totalPoints = totalPoints + msg.Message + " : " + msg.Point.ToString() + "\n";
            return totalPoints;
        }

        public string GetSpamKeyword(SpamMessageObj[] spamMessages, string text) // Mode 2 迷之算法
        {
            string totalPoints = ""; // 总分，预定义，返回值用
            int textLen = text.Length - 1; // 被检测的消息的长度
            foreach (SpamMessageObj msg in spamMessages) // 已有的关键字循环
            {
                string targetStr = msg.Message; // 关键字
                int targetMsgLen = msg.Message.Length; // 关键字长度
                int lastPath = 0; // 最后一次检测消息时关键字所在长度
                int skipTo = 0;
                for (int nowPath = 0; nowPath < textLen; nowPath++) // 被检测消息被打断循环
                {
                    if (nowPath < skipTo) continue;
                    if (text[nowPath] == targetStr[lastPath])
                    {
                        // 如果被检测的消息的当前字符和当前关键字的字符匹配，则将关键字位置 +1
                        lastPath++;
                    }
                    else if (lastPath != 0) // 如果最后一次检测消息时关键字所在长度不是 0，则检查被检查消息的下一个字是否和当前关键字字符匹配
                    {
                        if (text[nowPath + 1] == targetStr[lastPath])
                        {
                            // 如果匹配则跳过两个字，并且将关键字位置 +1
                            skipTo = nowPath + 2;
                            lastPath++;
                        }
                    }
                    else
                    {
                        lastPath = 0;
                    }

                    if (lastPath >= targetMsgLen)
                    {
                        // 如果当前关键字位置超出范围则代表完全匹配，则加分

                        totalPoints = totalPoints + "迷之算法 : " + msg.Point.ToString() + "\n";
                        break;
                    }
                }
            }

            return totalPoints;
        }

        public string GetIndexOfKeyword(SpamMessageObj[] spamMessages, string text) // Mode 3 寻找匹配字符串
        {
            string totalPoints = "";
            foreach (SpamMessageObj msg in spamMessages)
                if (text.ToLower().IndexOf(msg.Message.ToLower()) != -1)
                    totalPoints = totalPoints + msg.Message.ToLower() + " : " + msg.Point.ToString() + "\n";
            return totalPoints;
        }

        public string GetHalalKeyword(string text) // Mode 4 清真
        {
            string totalPoints = "";
            int textLen = text.Length - 1;
            for (int nowPath = 0; nowPath < textLen; nowPath++)
            {
                char nowChar = text[nowPath];
                if (nowChar >= 0x0600 && nowChar <= 0x06FF)
                {
                    totalPoints = totalPoints + nowChar + " : 1\n";
                    continue;
                }

                if (nowChar >= 0x08A0 && nowChar <= 0x08FF)
                {
                    totalPoints = totalPoints + nowChar + " : 1\n";
                    continue;
                }

                if (nowChar >= 0xFB50 && nowChar <= 0xFDFF)
                {
                    totalPoints = totalPoints + nowChar + " : 1\n";
                    continue;
                }

                if (nowChar >= 0xFE70 && nowChar <= 0xFEFF) totalPoints = totalPoints + nowChar + " : 1\n";
            }

            return totalPoints;
        }

        public string GetIndiaKeyword(string text) // Mode 5 印度
        {
            string totalPoints = "";
            int textLen = text.Length - 1;
            for (int nowPath = 0; nowPath < textLen; nowPath++)
            {
                char nowChar = text[nowPath];
                if (nowChar >= 0x0900 && nowChar <= 0x097F)
                {
                    totalPoints = totalPoints + nowChar + " : 1\n";
                    continue;
                }

                if (nowChar >= 0xA8E0 && nowChar <= 0xA8FF)
                {
                    totalPoints = totalPoints + nowChar + " : 1\n";
                    continue;
                }

                if (nowChar >= 0x1CD0 && nowChar <= 0x1CFF) totalPoints = totalPoints + nowChar + " : 1\n";
            }

            return totalPoints;
        }

        public string GetContainsKeyword(SpamMessageObj[] spamMessages, string text) // Mode 6 如果包含
        {
            string totalPoints = "";
            int point = 0;
            foreach (SpamMessageObj msg in spamMessages)
                if (text.ToLower().Contains(msg.Message.ToLower()))
                {
                    point = msg.Point * (text.ToLower().Split(msg.Message.ToLower()).Length - 1);
                    totalPoints = totalPoints + msg.Message.ToLower() + " : " + point.ToString() + "\n";
                }
            return totalPoints;
        }

        public string GetRussiaKeyword(string text) // Mode 7 普丁
        {
            string totalPoints = "";
            int textLen = text.Length - 1;
            for (int nowPath = 0; nowPath < textLen; nowPath++)
            {
                char nowChar = text[nowPath];
                if (nowChar >= 0x0400 && nowChar <= 0x052F){ totalPoints = totalPoints + nowChar + " : 1\n"; }
            }

            return totalPoints;
        }
        
        public int GetNameKeyword(SpamMessageObj[] spamMessages, string name) // Mode 8 Name
        {
            string totalPoints = "";
            int point = 0;
            foreach (SpamMessageObj msg in spamMessages)
                if (name.ToLower().Contains(msg.Message.ToLower()))
                {
                    point = msg.Point * (name.ToLower().Split(msg.Message.ToLower()).Length - 1);
                    totalPoints = totalPoints + msg.Message.ToLower() + " : " + point.ToString() + "\n";
                }
            return totalPoints;
        }

    }
}