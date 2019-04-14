using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WinsCode.SubSystem.AutoComplete
{
    public interface AutoCompleter
    {
        string Identifier { get; }
        uint Priority { get; }

        AutoCompleteSet GetAutoCompleteSet(AutoCompleteContext context);
    }
}