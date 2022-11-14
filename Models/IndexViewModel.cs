using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using TestTonClientQueries;

namespace NewNFTGel.Models
{
    public class IndexViewModel : AuthModel
    {
        public List<NFT> ListOfNFT;

        public ulong number { get; set; }
    }
}
