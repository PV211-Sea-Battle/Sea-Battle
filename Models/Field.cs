namespace Models
{
    [Serializable]
    public class Field
    {
        public int Id { get; set; }
        public List<Cell> Cells { get; set; } = [];
    }
}
