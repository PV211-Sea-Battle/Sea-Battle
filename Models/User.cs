namespace Models
{
    [Serializable]
    public class User
    {
        public int Id { get; set; }
        public string Login { get; set; } = null!;
        public string Password { get; set; } = null!;
        public bool IsReady { get; set; }
        public bool IsTurn { get; set; }
        public override string ToString() => Login;
    }
}
