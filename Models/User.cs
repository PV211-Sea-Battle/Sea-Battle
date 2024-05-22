namespace Models
{
    [Serializable]
    public class User
    {
        public int Id { get; set; }
        public string Login { get; set; } = null!;
        public string Password { get; set; } = null!;
        public bool IsReady { get; set; } = false;
        public bool IsWinner { get; set; } = false;
        public bool IsTurn { get; set; } = false;
        public override string ToString() => Login;
    }
}
