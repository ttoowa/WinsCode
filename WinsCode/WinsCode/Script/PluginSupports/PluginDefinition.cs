using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WinsCode.Plugins {
    public class Instance {
        //이 클래스는 WinsCode 프로그램 인스턴스를 나타냅니다.
        //플러그인들은 이 인스턴스를 이용해 플러그인 매니저와 같은 내부 시스템에 접근합니다.
        public PluginManager pluginManager { get; }

        //테스트용으로 노출된 메소드입니다.
        //로그 스크린에 글자를 출력합니다.
        public void write(string content) {
            //Console.Write(content);
        }

        public void writeLine(string line) {
            //Console.WriteLine(content);
        }
    }

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

        public abstract bool initialize(Instance instance);
        public abstract void finalize(Instance instance);
    }

    public interface IPluginEventHandler {
        void handleReadLine(string sLine);
    }
}