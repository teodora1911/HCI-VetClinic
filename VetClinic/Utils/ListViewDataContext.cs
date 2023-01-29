using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace VetClinic.Utils
{
    public class ListViewDataContext<T>
    {
        public IList<T> Items { get; set; }
        public T SelectedItem { get; set; }
        public Lang Language { get; set; }
    }
}
