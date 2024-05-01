using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Models
{
    [Serializable]
    public class Field
    {
        public int Id { get; set; }

        [ForeignKey(nameof(UserId))]
        public int UserId { get; set; }

        public List<Cell> Cells { get; set; } = new List<Cell>();
    }
}
