using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Models
{
    [Serializable]
    public class CompletedGame
    {
        public int Id { get; set; }
        public string Name { get; set; } = null!;
        public bool IsPrivate { get; set; }
        public string? Password { get; set; }
        public string? Winner { get; set; }
        public int HostUserId { get; set; }
        public int ClientUserId { get; set; }
    }
}
