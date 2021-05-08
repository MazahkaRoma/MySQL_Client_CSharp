using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WpfApp1
{
    public enum  Type
    { 
        INT,
        DATE,
        BOOLEAN
    }
    public class Column
    {
        
        public string Column_Name { get; set; }
        public bool Primary_Key { get; set; }
        public bool Unique { get; set; }
        public Type type { get; set; }
    }
}
