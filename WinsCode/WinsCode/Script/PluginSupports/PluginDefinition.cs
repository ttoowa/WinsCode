using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WinsCode.Plugins {
    public interface IPluginEventHandler {
        void handleReadLine(string sLine);
    }
}