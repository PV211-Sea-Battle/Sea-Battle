namespace Models
{
    [Serializable]
    public class User
    {
        public int Id { get; set; }
        public string Login { get; set; } = null!;
        public string Password { get; set; } = null!;
        public bool IsInGame { get; set; }

        public List<Field> Fields { get; set; }
        public List<Game> Games { get; set; }
    }
}
