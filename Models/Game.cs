namespace Models
{
    [Serializable]
    public class Game
    {
        public int Id { get; set; }
        public string Name { get; set; } = null!;
        public bool IsPrivate { get; set; }
        public string? Password { get; set; }
        public int HostUserId { get; set; }
        public int? ClientUserId { get; set; }
        public int? HostFieldId { get; set; }
        public int? ClientFieldId { get; set; }

        public User HostUser { get; set; }
        public User ClientUser { get; set; }
        public Field HostField { get; set; }
        public Field ClientField { get; set; }
        public override string ToString() => Name;
    }
}
