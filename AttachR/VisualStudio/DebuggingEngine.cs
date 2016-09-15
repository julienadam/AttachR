using System;

namespace AttachR.VisualStudio
{
    public class DebuggingEngine : IEquatable<DebuggingEngine>
    {
        public string Name { get; }
        public Guid Id { get; }

        public DebuggingEngine(string name, Guid id)
        {
            Name = name;
            Id = id;
        }

        public bool Equals(DebuggingEngine other)
        {
            if (ReferenceEquals(null, other)) return false;
            if (ReferenceEquals(this, other)) return true;
            return string.Equals(Name, other.Name) && Id.Equals(other.Id);
        }

        public override bool Equals(object obj)
        {
            if (ReferenceEquals(null, obj)) return false;
            if (ReferenceEquals(this, obj)) return true;
            if (obj.GetType() != GetType()) return false;
            return Equals((DebuggingEngine) obj);
        }

        public override int GetHashCode()
        {
            unchecked
            {
                return ((Name?.GetHashCode() ?? 0)*397) ^ Id.GetHashCode();
            }
        }

        public static bool operator ==(DebuggingEngine left, DebuggingEngine right)
        {
            return Equals(left, right);
        }

        public static bool operator !=(DebuggingEngine left, DebuggingEngine right)
        {
            return !Equals(left, right);
        }
    }
}