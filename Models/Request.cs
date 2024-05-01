using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Models
{
    [Serializable]
    public class Request
    {
        public string? ActionKey { get; set; }
        public User? User { get; set; }
    }
}
