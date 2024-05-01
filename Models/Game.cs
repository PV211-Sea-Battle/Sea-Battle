using System.ComponentModel.DataAnnotations.Schema;

namespace Models
{
    [Serializable]
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
