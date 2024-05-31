namespace Models
{
    [Serializable]
    public class Game
    {
        public int Id { get; set; }
        public string Name { get; set; } = null!;
        public bool IsPrivate { get; set; }
        public string? Password { get; set; }
        public User HostUser { get; set; } = null!;
        public User? ClientUser { get; set; }
        public Field? HostField { get; set; }
        public Field? ClientField { get; set; }
        public User? Winner { get; set; }
        public override string ToString() => Name;
    }
}
