﻿using System;
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
        public string Name { get; set; }
        public Favori()
        {

        }
        public Favori(string url, string name)
        {
            Name = name;
            Url = url;
        }
    }

}
