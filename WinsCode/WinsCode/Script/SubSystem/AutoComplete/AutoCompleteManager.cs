
using System;
using System.Collections.Generic;

namespace WinsCode.SubSystem.AutoComplete
{
    public class AutoCompleteManager {
        private TerminalWindow instance;

        private Dictionary<string, AutoCompleter> autoCompleterDict;
        private SortedList<uint, AutoCompleter> autoCompleterList;

        public AutoCompleteManager(TerminalWindow instance) {
            this.instance = instance;

            autoCompleterDict = new Dictionary<string, AutoCompleter>();
            autoCompleterList = new SortedList<uint, AutoCompleter>();
        }

        public void RegisterAutoCompleter(AutoCompleter autoCompleter) {
            if (autoCompleterDict.ContainsKey(autoCompleter.Identifier))
                throw new ArgumentException("already registered identifier.", "autoCompleter");

            autoCompleterDict.Add(autoCompleter.Identifier, autoCompleter);
            autoCompleterList.Add(autoCompleter.Priority, autoCompleter);
        }

        public void UnregisterAutoCompleter(string autoCompleterIdentifier) {
            AutoCompleter autoCompleter;

            if (!autoCompleterDict.TryGetValue(autoCompleterIdentifier, out autoCompleter))
                return;

            autoCompleterList.RemoveAt(autoCompleterList.IndexOfValue(autoCompleter));
        }

        public AutoCompleteSet GetAutoCompleteSet() {
            var context = new AutoCompleteContext(instance);

            foreach (var pair in autoCompleterList) {
                var autoCompleteSet = pair.Value.GetAutoCompleteSet(context);

                if (autoCompleteSet != null)
                    return autoCompleteSet;
            }

            return null;
        }
    }
}