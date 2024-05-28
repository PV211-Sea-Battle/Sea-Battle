namespace Models
{
    [Serializable]
    public class Request
    {
        public string Header { get; set; } = null!;
        public User? User { get; set; }
        public Game? Game { get; set; }
        public Field? Field { get; set; }
        public Cell? Cell { get; set; }
        public string? EnteredGamePassword { get; set; }
    }
}
