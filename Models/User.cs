namespace Models
{
    [Serializable]
    public class User
    {
        public int Id { get; set; }
        public string Login { get; set; } = null!;
        public string Password { get; set; } = null!;
        public bool IsInGame { get; set; }

        public virtual ICollection<Field> Fields { get; set; }
        public virtual ICollection<Game> Games { get; set; }
    }
}
