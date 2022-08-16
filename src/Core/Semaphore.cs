#nullable enable

namespace Locksmith.Core
{
    using Newtonsoft.Json;
    using Newtonsoft.Json.Serialization;
    using System.Collections.Generic;

    [JsonObject(NamingStrategyType = typeof(CamelCaseNamingStrategy))]
    public class Semaphore
    {
        [JsonIgnore]
        public long Version { get; set; } = 0;

        [JsonProperty("semaphore")]
        public int Value { get; set; } = 1;

        public int Max { get; set; } = 1;

        public IList<string> Holders { get; set; } = new List<string>();

        public void SetMax(int max)
        {
            var diff = this.Max - max;

            this.Value -= diff;
            this.Max -= diff;
        }

        public Error? AddHolder(string h)
        {
            if (this.Holders.Contains(h))
            {
                return Errors.ErrExist;
            }

            this.Holders.Add(h);

	        return null;
        }

        public Error? RemoveHolder(string h)
        {
            if (!this.Holders.Contains(h))
            {
                return Errors.ErrNotExist;
            }

            this.Holders.Remove(h);

            return null;
        }

        public Error? Lock(string h)
        {
            if (this.Value <= 0)
            {
                return new Error ($"semaphore is at {this.Value}");
            }

            var error = this.AddHolder(h);
            if (error != null)
            {
                return error;
            }

            this.Value--;

            return null;
        }

        public Error? Unlock(string h)
        {
            var error = this.RemoveHolder(h);
            if (error != null)
            {
                return error;
            }

            this.Value++;

            return null;
        }
    }
}
