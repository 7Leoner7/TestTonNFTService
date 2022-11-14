using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Threading.Tasks;
using TonSdk;
using TonSdk.Modules;

namespace TestTonClientQueries
{
    public class NFTofJSON
    {
        [JsonProperty("url")]
        public string url { get; set; }

        [JsonProperty("isSelling")]
        public bool isSelling { get; set; }

        [JsonProperty("cost")]
        public uint cost { get; set; }

        [JsonProperty("descript")]
        public string descript { get; set; }

        [JsonProperty("IdColl")]
        public ulong IdColl { get; set; }

        [JsonProperty("idNFT")]
        public ulong idNFT { get; set; }
    }

    public class OwnerNFT
    {
        [JsonProperty("OwnAddrWallet")]
        public string OwnAddrWallet { get; set; }

        [JsonProperty("bal")]
        public ulong bal { get; set; }

        [JsonProperty("LikesNFTbyID")]
        public string LikesNFTbyID { get; set; }
    }

    public struct determinant
    {
        public ulong ColorSum;
        public ulong deltaColor;
        public string decomposition;
    }

    public struct NFT
    {
        public ulong IdColl;
        public ulong idNFT; 
        public string addrNFT; 
        public string PathNFT; 
        public OwnerNFT Owner; 
        public OwnerNFT Creater; 
        public string description; 
        public bool pay_able; 
        public uint cost; 
        public bool IsOriginal;    
        public determinant det; 
    }

    public struct OwnersCollectionNFT
    {
        public OwnerNFT Creator;
        public NFT[] CollNFT;
        public ulong IdColl;
    }

    internal class MessageInternal
    {
        [JsonProperty("id")]
        public string Id { get; set; }

        [JsonProperty("cell")]
        public string CellBase64 { get; set; }

        [JsonProperty("msg_type")]
        public MessageType Type { get; set; }

        [JsonProperty("value")]
        public string Value { get; set; }
    }

    public enum MessageType
    {
        Internal = 0,
        ExternalInbound,
        ExternalOutbound,
        Unknown
    }

    public static class TonClientNodeSeExtensions
    {

        public static Abi GiverAbi { get; set; }
        public static string _GiverAddr { get; set; }//"0:841288ed3b55d9cdafa806807f02a0ae0c169aa5edfe88a789a6482429756a94"
        public static ulong GiveAmount { get; set; }
        public static string GiveCommand { get; set; } //"sendGrams"
        public static bool GiverForMainContract { get; set; }

        public static async Task<ResultOfEncodeMessage> DeployWithGiverAsync(this ITonClient client, ParamsOfEncodeMessage @params)
        {
            var msg = await client.Abi.EncodeMessageAsync(@params);
            await client.GetGramsFromGiverAsync(msg.Address);
            await client.Processing.ProcessMessageAsync(new ParamsOfProcessMessage
            {
                MessageEncodeParams = @params,
                SendEvents = false
            });
            return msg;
        }

        public static async Task GetGramsFromGiverAsync(this ITonClient client, string account)
        {
            var runResult = await client.NetProcessFunctionAsync(_GiverAddr,
                GiverAbi,
                GiveCommand,
                new
                {
                    dest = account,
                    amount = GiveAmount
                }.ToJson(),
                new Signer.None());

            foreach (var outMessage in runResult.OutMessages)
            {
                var parsed = await client.Boc.ParseMessageAsync(new ParamsOfParse
                {
                    Boc = outMessage
                });

                var message = parsed.Parsed.ToObject<MessageInternal>();
                if (message?.Type == MessageType.Internal)
                {
                    await client.Net.WaitForCollectionAsync(new ParamsOfWaitForCollection
                    {
                        Collection = "transactions",
                        Filter = new
                        {
                            in_msg = new
                            {
                                eq = message.Id
                            }
                        }.ToJson(),
                        Result = "id"
                    });
                }
            }
        }

        public static async Task<ResultOfProcessMessage> NetProcessFunctionAsync(this ITonClient client,
            string address,
            Abi abi,
            string functionName,
            JToken input,
            Signer signer)
        {
            try
            {
                return await client
                .Processing.ProcessMessageAsync(new ParamsOfProcessMessage
                {
                    MessageEncodeParams = new ParamsOfEncodeMessage
                    {
                        Address = address, 
                        Abi = abi,
                        CallSet = new CallSet
                        {
                            FunctionName = functionName,
                            Input = input
                        },
                        Signer = signer
                    },
                    SendEvents = false
                });
            }
            catch (TonClientException)
            {
                return null;
            }
          
        }
    }
}
