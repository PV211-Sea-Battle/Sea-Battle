namespace Models
{
    [Serializable]
    public class Response
    {
        public string? ErrorMessage { get; set; }
        public User? User { get; set; }
        public Game? Game { get; set; }
        public List<Game>? Games { get; set; }
    }
}
