using System;
using System.IO;

namespace AttachR.VisualStudio
{
    public class AvailableVisualStudioInstall : IEquatable<AvailableVisualStudioInstall>
    {
        public Version Version { get; }
        public FileInfo Executable { get; }

        public AvailableVisualStudioInstall(Version version, FileInfo executable)
        {
            Version = version;
            Executable = executable;
        }

        public bool Equals(AvailableVisualStudioInstall other)
        {
            if (ReferenceEquals(null, other)) return false;
            if (ReferenceEquals(this, other)) return true;
            return Equals(Version, other.Version) && Equals(Executable, other.Executable);
        }

        public override bool Equals(object obj)
        {
            if (ReferenceEquals(null, obj)) return false;
            if (ReferenceEquals(this, obj)) return true;
            if (obj.GetType() != GetType()) return false;
            return Equals((AvailableVisualStudioInstall) obj);
        }

        public override int GetHashCode()
        {
            unchecked
            {
                return ((Version?.GetHashCode() ?? 0)*397) ^ (Executable?.GetHashCode() ?? 0);
            }
        }

        public static bool operator ==(AvailableVisualStudioInstall left, AvailableVisualStudioInstall right)
        {
            return Equals(left, right);
        }

        public static bool operator !=(AvailableVisualStudioInstall left, AvailableVisualStudioInstall right)
        {
            return !Equals(left, right);
        }
    }
}