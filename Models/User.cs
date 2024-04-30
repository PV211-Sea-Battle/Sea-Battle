using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Models
{
    public class User
    {
       //я тут потренировался 
        public int Id { get; set; }
        public string Login { get; set; } = null!;
        public string Password { get; set; } = null!;

        public List<Field> Fields { get; set; } = new List<Field>();
    }
}
