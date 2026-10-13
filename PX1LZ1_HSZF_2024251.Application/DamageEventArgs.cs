using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PX1LZ1_HSZF_2024251.Application
{
    public class DamageEventArgs : EventArgs
    {
        public string WorldName { get; }
        public string Description { get; }
        public decimal Cost { get; }

        public DamageEventArgs(string worldName, string description, decimal cost)
        {
            WorldName = worldName;
            Description = description;
            Cost = cost;
        }
    }
}
