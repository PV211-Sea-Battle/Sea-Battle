using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Models
{
    public class Cell
    {
        public int Id { get; set; }
        public int FieldId { get; set; }
        public bool IsContainsShip { get; set; }
        public bool IsHit { get; set; }

        public virtual Field Field { get; set; }
    }
}
