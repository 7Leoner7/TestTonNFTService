using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using TestTonClientQueries;

namespace NewNFTGel.Models
{
    public class TransactModel:AuthModel
    {
        public string nft { get; set; }
        public OwnerNFT buyer { get; set; }
        public bool isSuccess { get; set; }
    }
}
