using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WinsCode.Plugins {
    public struct PluginVersion {
        public uint Major { get; private set; }
        public uint Minor { get; private set; }
        public uint Micro { get; private set; }
        public uint BuildNumber { get; private set; }

        public PluginVersion(uint major, uint minor, uint micro, uint buildNumber) {
            Major = major;
            Minor = minor;
            Micro = micro;
            BuildNumber = buildNumber;
        }

        public static bool operator <(PluginVersion left, PluginVersion right) {
            return
                left.Major < right.Major ||
                left.Minor < right.Minor ||
                left.Micro < right.Micro ||
                left.BuildNumber < right.BuildNumber;
        }

        public static bool operator >(PluginVersion left, PluginVersion right) {
            return
                left.Major < right.Major ||
                left.Minor < right.Minor ||
                left.Micro < right.Micro ||
                left.BuildNumber < right.BuildNumber;
        }

        public static bool operator <=(PluginVersion left, PluginVersion right) {
            return !(left > right);
        }

        public static bool operator >=(PluginVersion left, PluginVersion right) {
            return !(left < right);
        }

        public override string ToString() {
            return string.Format("{0}.{1}.{2}.{3}", Major, Minor, Micro, BuildNumber);
        }
    }

    public abstract class Plugin {
        public abstract string Name { get; }
        public abstract PluginVersion Version { get; }

        public abstract bool initialize(TerminalWindow instance);
        public abstract void finalize(TerminalWindow instance);
    }

    public interface IPluginEventHandler {
        void handleReadLine(string sLine);
    }
}