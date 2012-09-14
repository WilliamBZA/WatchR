using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace WatchRServer.Models
{
    public class Share : IComparable, IEquatable<Share>
    {
        public string ShortCode { get; set; }

        public decimal LastTradePrice { get; set; }

        public decimal TradeVolume { get; set; }

        public bool Equals(Share other)
        {
            if (other == null)
            {
                return false;
            }

            return ShortCode.Equals(other.ShortCode);
        }

        public override bool Equals(object obj)
        {
            var other = obj as Share;

            if (other == null)
            {
                return false;
            }

            return ShortCode.Equals(other.ShortCode);
        }

        public int CompareTo(object obj)
        {
            var other = obj as Share;

            if (other == null)
            {
                return 1;
            }

            return ShortCode.CompareTo(other.ShortCode);
        }

        public override string ToString()
        {
            return string.Format("{0} - {1:C} ({2:N0})", ShortCode, LastTradePrice, TradeVolume);
        }

        public override int GetHashCode()
        {
            return ShortCode.GetHashCode();
        }
    }
}