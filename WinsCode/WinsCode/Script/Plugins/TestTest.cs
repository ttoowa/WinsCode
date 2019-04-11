using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using WinsCode.SubSystem.AutoComplete;

namespace WinsCode.Plugins
{
    public class TestTest : AutoCompleter
    {
        public string Identifier => "TestPlugin.TestTest";

        public uint Priority => 0;

        public AutoCompleteSet GetAutoCompleteSet(AutoCompleteContext context)
        {
            try {
                string prefix = context.Input;

                for (int index = context.Input.Length - 1; index >= 0; --index)
                    if (char.IsWhiteSpace(context.Input, index)) {
                        prefix = context.Input.Substring(index + 1);
                        break;
                    }

                List<string> list = new List<string>();
                DirectoryInfo dir = new DirectoryInfo(context.WorkingDirectory);

                foreach (var file in dir.GetFiles())
                    if (file.Name.StartsWith(prefix))
                        list.Add(file.Name.Substring(prefix.Length));

                foreach (var subdir in dir.GetDirectories())
                    if (subdir.Name.StartsWith(prefix))
                        list.Add(subdir.Name.Substring(prefix.Length));

                return new AutoCompleteSet(context, list);

            } catch {
                return null;
            }
        }
    }
}