namespace Locksmith.Core
{
    using System;
    using System.Diagnostics.CodeAnalysis;

    public class Error : IEquatable<Error>
    {
        private readonly string value;

        public Error(string value)
        {
            this.value = value;
        }

        public override string ToString()
        {
            return this.value;
        }

        public override bool Equals(object other)
        {
            return other is Error otherError && this.Equals(otherError);
        }

        public bool Equals([AllowNull] Error other)
        {
            return !(other is null)
                && string.Equals(this.value, other.value);
        }

        public override int GetHashCode()
        {
            var hashcode = new HashCode();
            hashcode.Add(this.value);

            return hashcode.ToHashCode();
        }

        public static bool operator ==(Error first, Error second)
        {
            return first is null ? second is null : first.Equals(second);
        }

        public static bool operator !=(Error first, Error second)
        {
            return !(first == second);
        }
    }
}
