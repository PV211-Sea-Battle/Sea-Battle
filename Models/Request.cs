using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Models
{
    public class Request
    {
        public string Header { get; set; } = null!;
        public User? User { get; set; }
        public Game? Game { get; set; }
        public Field? Field { get; set; }
        public Cell? Cell { get; set; }
    }
}
