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
                string input = context.Input;

                //Get last black space substring of the input.
                for (int index = context.Input.Length - 1; index >= 0; --index)
                    if (char.IsWhiteSpace(context.Input, index)) {
                        input = context.Input.Substring(index + 1);
                        break;
                    }

                var prefix = Path.GetFileName(input);
                var rootPath = Path.GetDirectoryName(Path.Combine(context.WorkingDirectory + '/', input));
                var root = new DirectoryInfo(rootPath.Length == 0 ? Path.Combine(context.WorkingDirectory + '/', input) : rootPath);

                var files = from file in root.GetFiles()
                            where file.Name.StartsWith(prefix)
                            select file.Name.Substring(prefix.Length);

                var dirs = from dir in root.GetDirectories()
                           where dir.Name.StartsWith(prefix)
                           select dir.Name.Substring(prefix.Length);

                var list = files.Concat(dirs).ToList();

                if (files.Count() == 0 && dirs.Count() == 1)
                    list[0] += '/';
                
                return new AutoCompleteSet(context, list);

            } catch {
                return null;
            }
        }
    }
}