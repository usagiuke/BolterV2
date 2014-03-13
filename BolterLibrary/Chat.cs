using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace BolterLibrary
{
    public class Chat
    {
        public bool IsNewLine()
        {
            return Funcs.IsNewChatLine();
        }

        public string GetChatLine()
        {
            var DirtyName = Regex.Replace(Funcs.GetNameOfSender(), @"[^\x20-\x7E]", String.Empty);
            var CleanText = Regex.Replace(Funcs.GetChatLine(), @"[^\x20-\x7E]", String.Empty);

            if (Regex.IsMatch(CleanText, @"'(?<name>[A-Z][a-z']+\s[A-Z][a-z']+)\k<name>'"))
            {
                foreach (var s in new Regex(@"'(?<name>[A-Z][a-z']+\s[A-Z][a-z']+)\k<name>'").Match(CleanText).Captures)
                {
                    CleanText = CleanText.Replace(s.ToString(), s.ToString().Substring(0, s.ToString().Length / 2)).Replace("'","");
                }
                return String.Format("{0}{1}", String.IsNullOrEmpty(DirtyName) ? null : "<" + DirtyName.Substring(0, DirtyName.Length / 2).Replace("'", "") + "> ", CleanText);
            }
            return String.Format("{0}{1}", String.IsNullOrEmpty(DirtyName) ? null : "<" + DirtyName.Substring(0, DirtyName.Length/2).Replace("'","") + "> ", CleanText);
        }

        public string GetRawChat()
        {
            return Funcs.GetChatLine();
        }

        public string GetRawName()
        {
            return Funcs.GetNameOfSender();
        }

        public static void SendCommand(string TextCommand)
        {
            Funcs.SendCommand(TextCommand);
        }
    }
}
