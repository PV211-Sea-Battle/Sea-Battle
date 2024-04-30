using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Models
{
    public class Field
    {
        public int Id { get; set; }
        public int UserId { get; set; }
        public bool IsActive { get; set; }

        public virtual ICollection<Cell> Cells { get; set; }
        public virtual User User { get; set; }
    }
}
