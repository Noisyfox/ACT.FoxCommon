using System.Diagnostics;
using System.IO;

namespace ACT.FoxCommon
{
    public class ProcessInfo
    {
        public uint Pid { get; }

        public string FileName { get; }

        public string FilePath { get; }

        public bool IsGameProcess { get; }

        public ProcessInfo(Process p)
        {
            Pid = (uint)p.Id;
            FilePath = p.MainModule.FileName;
            FileName = Path.GetFileName(FilePath);
            IsGameProcess = Utils.IsGameExePath(FilePath);
        }

        protected bool Equals(ProcessInfo other)
        {
            return Pid == other.Pid && FileName == other.FileName && FilePath == other.FilePath;
        }

        public override bool Equals(object obj)
        {
            if (ReferenceEquals(null, obj)) return false;
            if (ReferenceEquals(this, obj)) return true;
            if (obj.GetType() != this.GetType()) return false;
            return Equals((ProcessInfo)obj);
        }

        public override int GetHashCode()
        {
            unchecked
            {
                var hashCode = (int)Pid;
                hashCode = (hashCode * 397) ^ FileName.GetHashCode();
                hashCode = (hashCode * 397) ^ FilePath.GetHashCode();
                return hashCode;
            }
        }

        public static bool operator ==(ProcessInfo left, ProcessInfo right)
        {
            return Equals(left, right);
        }

        public static bool operator !=(ProcessInfo left, ProcessInfo right)
        {
            return !Equals(left, right);
        }

        public override string ToString()
        {
            return $"{nameof(Pid)}: {Pid}, {nameof(FileName)}: {FileName}, {nameof(FilePath)}: {FilePath}, {nameof(IsGameProcess)}: {IsGameProcess}";
        }
    }
}