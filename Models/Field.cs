namespace Models
{
    [Serializable]
    public class Field
    {
        public int Id { get; set; }
        public int GameId { get; set; }
        public bool IsBelongToHost { get; set; }
        public List<Cell> Cells { get; set; }
    }
}
