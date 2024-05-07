namespace Models
{
    [Serializable]
    public class User
    {
        public int Id { get; set; }
        public string Login { get; set; } = null!;
        public string Password { get; set; } = null!;
        public int Victories { get; set; } = 0;
        public int Defeats { get; set; } = 0;

        public virtual List<Field> Fields { get; set; }
        public virtual List<Game> Games { get; set; }
    }
}
