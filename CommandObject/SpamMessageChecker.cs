using CNBlackListSoamChecker.DbManager;
using ReimuAPI.ReimuBase;
using System.Text.RegularExpressions;

// 这是迷之 Spam Message Checker

namespace CNBlackListSoamChecker.CommandObject
{
    class SpamMessageChecker
    {
        public int GetEqualsPoints(SpamMessageObj[] spamMessages, string text) // Mode 0 完全匹配
        {
            int totalPoints = 0;
            foreach (SpamMessageObj msg in spamMessages)
            {
                if (text.ToLower().Equals(msg.Message.ToLower()))
                {
                    totalPoints += msg.Point;
                }
            }
            return totalPoints;
        }

        public int GetRegexPoints(SpamMessageObj[] spamMessages, string text) // Mode 1 正则
        {
            int totalPoints = 0;
            foreach (SpamMessageObj msg in spamMessages)
            {
                if (new Regex(msg.Message).Match(text).Success)
                {
                    totalPoints += msg.Point;
                }
            }
            return totalPoints;
        }

        public int GetSpamPoints(SpamMessageObj[] spamMessages, string text) // Mode 2 迷之算法
        {
            int totalPoints = 0; // 总分，预定义，返回值用
            int textLen = text.Length - 1; // 被检测的消息的长度
            foreach (SpamMessageObj msg in spamMessages) // 已有的关键字循环
            {
                string targetStr = msg.Message; // 关键字
                int targetMsgLen = msg.Message.Length; // 关键字长度
                int lastPath = 0; // 最后一次检测消息时关键字所在长度
                int skipTo = 0;
                for (int nowPath = 0; nowPath < textLen; nowPath++) // 被检测消息被打断循环
                {
                    if (nowPath < skipTo)
                    {
                        continue;
                    }
                    if (text[nowPath] == targetStr[lastPath])
                    { // 如果被检测的消息的当前字符和当前关键字的字符匹配，则将关键字位置 +1
                        lastPath++;
                    }
                    else if (lastPath != 0) // 如果最后一次检测消息时关键字所在长度不是 0，则检查被检查消息的下一个字是否和当前关键字字符匹配
                    {
                        if (text[nowPath + 1] == targetStr[lastPath])
                        { // 如果匹配则跳过两个字，并且将关键字位置 +1
                            skipTo = nowPath + 2;
                            lastPath++;
                        }
                    }
                    else
                    {
                        lastPath = 0;
                    }
                    if (lastPath >= targetMsgLen)
                    { // 如果当前关键字位置超出范围则代表完全匹配，则加分
                        totalPoints += msg.Point;
                        break;
                    }
                }
            }
            return totalPoints;
        }


        public int GetContainsPoints(SpamMessageObj[] spamMessages, string text) // Mode 6 如果包含
        {
            int totalPoints = 0;
            foreach (SpamMessageObj msg in spamMessages)
            {
                if (text.ToLower().Contains(msg.Message.ToLower()))
                {
                    totalPoints += msg.Point;
                }
            }
            return totalPoints;
        }


        public int GetIndexOfPoints(SpamMessageObj[] spamMessages, string text) // Mode 3 寻找匹配字符串
        {
            int totalPoints = 0;
            foreach (SpamMessageObj msg in spamMessages)
            {
                if (text.ToLower().IndexOf(msg.Message.ToLower()) != -1)
                {
                    totalPoints += msg.Point;
                }
            }
            return totalPoints;
        }

        public int GetHalalPoints(string text) // Mode 4 清真
        {
            int totalPoints = 0;
            int textLen = text.Length - 1;
            for (int nowPath = 0; nowPath < textLen; nowPath++)
            {
                if (totalPoints >= 230)
                {
                    return 230;
                }
                char nowChar = text[nowPath];
                if (nowChar >= 0x0600 && nowChar <= 0x06FF)
                {
                    totalPoints++;
                    continue;
                }
                if (nowChar >= 0x08A0 && nowChar <= 0x08FF)
                {
                    totalPoints++;
                    continue;
                }
                if (nowChar >= 0xFB50 && nowChar <= 0xFDFF)
                {
                    totalPoints++;
                    continue;
                }
                if (nowChar >= 0xFE70 && nowChar <= 0xFEFF)
                {
                    totalPoints++;
                    continue;
                }
            }
            return totalPoints;
        }

        public int GetIndiaPoints(string text) // Mode 5 印度
        {
            int totalPoints = 0;
            int textLen = text.Length - 1;
            for (int nowPath = 0; nowPath < textLen; nowPath++)
            {
                if (totalPoints >= 230)
                {
                    return 230;
                }
                char nowChar = text[nowPath];
                if (nowChar >= 0x0900 && nowChar <= 0x097F)
                {
                    totalPoints++;
                    continue;
                }
                if (nowChar >= 0xA8E0 && nowChar <= 0xA8FF)
                {
                    totalPoints++;
                    continue;
                }
                if (nowChar >= 0x1CD0 && nowChar <= 0x1CFF)
                {
                    totalPoints++;
                    continue;
                }
            }
            return totalPoints;
        }
    }
}
