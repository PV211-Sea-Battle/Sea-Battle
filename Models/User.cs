namespace Models
{
    [Serializable]
    public class User
    {
        public int Id { get; set; }
        public string Login { get; set; } = null!;
        public string Password { get; set; } = null!;

        public virtual List<Field> Fields { get; set; }
        public virtual List<Game> Games { get; set; }
        public override string ToString() => Login;
    }
}
