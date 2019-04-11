
namespace WinsCode.SubSystem.AutoComplete
{
    public class AutoCompleteContext
    {
        public string WorkingDirectory { get; private set; }
        public string Input { get; private set; }

        public AutoCompleteContext(TerminalWindow instance) {
            WorkingDirectory = instance.WorkingDirectory;
            Input = instance.Input;
        }
    }
}