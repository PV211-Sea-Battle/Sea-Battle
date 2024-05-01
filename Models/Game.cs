using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Models
{
    public class Game
    {
        public int Id { get; set; }
        public string Name { get; set; } = null!;
        public bool IsPrivate { get; set; }
        public string? Password { get; set; }
        public string? Winner { get; set; }
        public int HostUserId { get; set; }
        public int ClientUserId { get; set; } = -1; //пока второй игрок не присоединился, тут будет -1


        //место для будущих доп. настроек


        [ForeignKey(nameof(HostUserId))]
        public virtual User User { get; set; }

        public override string ToString()
        {
            return Name;
        }
    }
}
