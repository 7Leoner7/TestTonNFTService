using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace NewNFTGel.Models
{
    public class ContactModel:AuthModel
    {
        public bool IsAdmin { get; set; }

        public string[] messages { get; set; }
    }
}
