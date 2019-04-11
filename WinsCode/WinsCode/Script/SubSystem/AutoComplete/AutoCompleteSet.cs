
using System.Collections.Generic;

namespace WinsCode.SubSystem.AutoComplete
{
    public class AutoCompleteSet
    {
        public AutoCompleteContext Context { get; private set; }
        public string Next {
            get {
                return enumerator.MoveNext() ? enumerator.Current : null;
            }
        }

        private SortedSet<string> autoCompleteSet;
        private IEnumerator<string> enumerator;

        public AutoCompleteSet(AutoCompleteContext context, ICollection<string> autoCompleteCollection) {
            Context = context;

            this.autoCompleteSet = new SortedSet<string>(autoCompleteCollection);
            enumerator = next();
        }

        private IEnumerator<string> next() {
            while (autoCompleteSet.Count != 0) {
                foreach (var next in autoCompleteSet) {
                    yield return next;
                }
            }
        }
    }
}