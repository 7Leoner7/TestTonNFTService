using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace NewNFTGel.Models
{
    public class ModelLoaderNFTofJSON : AuthModel
    {
        public string JSONofNFT { get; set; }

        public bool succesfull { get; set; }
    }
}
