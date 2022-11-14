using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using TestTonClientQueries;

namespace NewNFTGel.Models
{
    public class PersonAreaViewModel : AuthModel
    {
        [JsonProperty("ListOfCollections")]
        public List<OwnersCollectionNFT> ListOfCollections;

        public string ToAddress { get; set; }

        public bool IsOwner { get; set; }
    }
}
