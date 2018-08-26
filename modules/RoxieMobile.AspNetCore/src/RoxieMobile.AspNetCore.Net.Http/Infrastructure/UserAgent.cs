using System;
using System.Collections.Generic;

namespace RoxieMobile.AspNetCore.Net.Http.Infrastructure
{
    public class UserAgent
    {
        private IDictionary<string, object> properties;


        /// <summary>
        /// The unknown.
        /// </summary>
        /// <returns>
        /// The <see cref="UserAgent"/>.
        /// </returns>
        public static UserAgent Unknown
        {
            get
            {
                return new UserAgent { Product = "unknown", Platform = "unknown", Version = "0.0", };
            }
        }
        
        public string Product { get; set; }

        public string Version { get; set; }

        public string Platform { get; set; }

        public string PlatformVersion { get; set; }

        public IDictionary<string, object> Properties
        {
            get
            {
                return this.properties
                       ?? (this.properties = new Dictionary<string, object>(StringComparer.OrdinalIgnoreCase));
            }
        }

        public object this[string key]
        {
            get
            {
                object result;
                this.Properties.TryGetValue(key, out result);
                return result;
            }

            set
            {
                if (this.Properties.ContainsKey(key))
                {
                    this.Properties.Remove(key);
                }

                this.Properties.Add(key, value);
            }
        }

        public override string ToString()
        {
            return $"{this.Product}/{this.Version} ({this.Platform} {this.PlatformVersion})";
        }
    }
}
