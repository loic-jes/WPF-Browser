using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Braawser.Model
{
    [Serializable]
    public class Favori
    {
        public string Url { get; set; }
        public Favori()
        {

        }
        public Favori(string url)
        {
            Url = url;
        }
    }

}
