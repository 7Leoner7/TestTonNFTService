using Newtonsoft.Json.Linq;
using System;
using System.Threading.Tasks;
using TonSdk;
using TonSdk.Modules;

namespace TestTonClientQueries
{
    public class CommandsTonContract
    {
    
    public enum CommandsWallet
    {
        constructor,
        CreateClient,
        sendTransaction,
        GetClient,
        SetClient,
        GetBalance
    }

    public enum CommandsContract
    {
        constructor,
        AddUser,
        TransactionNFT,
        CreateNFT,
        GetCollNFT,
        GetNFT,
        GetOwnerCollNFT,
        GetAllCollNFT,
        sendTransaction,
        IsAuth
    }
        public string CommandsContractAddress { get; init; }
        public KeyPair _KeyPair { get; init; }
        public Abi _abi { get; init; }
        public string _tvc { get; init; }
        public Abi _tabi { get; init; }
        public string _tvcZ { get; init; }

        public CommandsTonContract(Abi abi, string tvc, Abi zero_abi, string zero_tvc, KeyPair key = null, string cca = null)
        {
            _KeyPair = key;
            _abi = abi;
            _tvc = tvc;
            _tabi = zero_abi;
            _tvcZ = zero_tvc;
            CommandsContractAddress = cca;
        }

        static public async Task<ResultOfProcessMessage> CCProcessMessage(ITonClient client, Abi abi, string debotAddr, KeyPair keypair, int command, JToken input, string tvc)
        {
            var _params = new ParamsOfProcessMessage()
            {
                SendEvents = false,
                MessageEncodeParams = new ParamsOfEncodeMessage()
                {
                    Abi = abi,
                    Address = debotAddr,
                    DeploySet = new DeploySet
                    {
                        Tvc = tvc
                    },
                    CallSet = new CallSet()
                    {
                        FunctionName = Enum.GetValues(typeof(CommandsContract)).GetValue(command).ToString(),
                        Input = input
                    },
                    Signer = new Signer.Keys() { KeysProperty = keypair }
                }
            };

            return await client.Processing.ProcessMessageAsync(_params);
        }

        static public async Task<ResultOfProcessMessage> CWProcessMessage(ITonClient client, string tvc, Abi abi, string targetAddr, KeyPair keypair, int command, JToken input)
        {
            var _params = new ParamsOfProcessMessage()
            {
                SendEvents = false,
                MessageEncodeParams = new ParamsOfEncodeMessage()
                {
                    Abi = abi,
                    DeploySet = new DeploySet
                    {
                        Tvc = tvc
                    },
                    Address = targetAddr,
                    CallSet = new CallSet()
                    {
                        FunctionName = Enum.GetValues(typeof(CommandsWallet)).GetValue(command).ToString(),
                        Input = input
                    },
                    Signer = new Signer.Keys() { KeysProperty = keypair }
                }
            };

            return await client.Processing.ProcessMessageAsync(_params);
        }

        public async Task<ResultOfProcessMessage> GetBalance(ITonClient client, string targetAddr, JToken input, KeyPair key = null)
        {
            try
            {
                key = key != null ? key : _KeyPair;
                if (key == null) throw new Exception();
                return await CWProcessMessage(client, _tvcZ, _tabi, targetAddr, key, 5, input);
            }
            catch (Exception)
            {
                return null;
            }
        }
        /// <summary>
        /// GetClient()
        /// </summary>
        /// <param name="client"></param>
        /// <param name="targetAddr"></param>
        /// <param name="input"></param>
        /// <param name="key"></param>
        /// <returns></returns>
        public async Task<ResultOfProcessMessage> GetClient(ITonClient client, string targetAddr, JToken input, KeyPair key = null)
        {
            try
            {
                key = key != null ? key : _KeyPair;
                if (key == null) throw new Exception();
                return await CWProcessMessage(client, _tvcZ, _tabi, targetAddr, key, 3, input);
            }
            catch (Exception)
            {
                return null;
            }

        }
        /// <summary>
        /// sendTransaction(address dest, uint128 value, bool bounce)
        /// </summary>
        /// <param name="client"></param>
        /// <param name="targetAddr"></param>
        /// <param name="input"></param>
        /// <returns></returns>
        public async Task<ResultOfProcessMessage> SendTransaction(ITonClient client, string targetAddr, JToken input, KeyPair key = null)
        {
            try
            {
                key = key != null ? key : _KeyPair;
                if (key == null) throw new Exception(); 
                return await CWProcessMessage(client, _tvcZ, _tabi, targetAddr, key, 2, input);
            }
            catch (Exception)
            {
                return null;
            }
            
        }
        /// <summary>
        /// CreateClient(address addr, string likesNFT)
        /// </summary>
        /// <param name="client"></param>
        /// <param name="targetAddr"></param>
        /// <param name="input"></param>
        /// <returns></returns>
        public async Task<ResultOfProcessMessage> CreateClient(ITonClient client, string targetAddr, JToken input, KeyPair key = null)
        {
            try
            {
                key = key != null ? key : _KeyPair;
                if (key == null) throw new Exception();
                return await CWProcessMessage(client, _tvcZ, _tabi, targetAddr, key, 1, input);
            }
            catch (TonClientException)
            {
                return null;
            }
        }
        /// <summary>
        /// Конструктор Client
        /// </summary>
        /// <param name="client"></param>
        /// <param name="targetAddr"></param>
        /// <returns></returns>
        public async Task<ResultOfProcessMessage> ConstructorTonWallet(ITonClient client, string targetAddr, KeyPair key = null)
        {
            try
            {
                key = key != null ? key : _KeyPair;
                if (key == null) throw new Exception();
                return await CWProcessMessage(client, _tvcZ, _tabi, targetAddr, key, 0, new { }.ToJson());
            }
            catch (TonClientException)
            {
                return null;
            }
        }
        /// <summary>
        /// GetAllCollNFT()
        /// </summary>
        /// <param name="client"></param>
        /// <param name="debotAddr"></param>
        /// <param name="input"></param>
        /// <param name="key"></param>
        /// <returns></returns>
        public async Task<ResultOfProcessMessage> GetAllCollectionsNFT(ITonClient client, string debotAddr, JToken input, KeyPair key = null)
        { 
            try
            {
                key = key != null ? key : _KeyPair;
                if (key == null) throw new Exception();
                return await CCProcessMessage(client, _abi, debotAddr, key, 7, input, _tvc);
            }
            catch (Exception)
            {
                return null;
            }
        }
        /// <summary>
        /// GetOwnerCollNFT(OwnerNFT owner)
        /// </summary>
        /// <param name="client"></param>
        /// <param name="debotAddr"></param>
        /// <param name="input"></param>
        /// <param name="key"></param>
        /// <returns></returns>
        public async Task<ResultOfProcessMessage> GetOwnerCollNFT(ITonClient client, string debotAddr, JToken input, KeyPair key = null)
        {
            try
            {
                key = key != null ? key : _KeyPair;
                if (key == null) throw new Exception();
                return await CCProcessMessage(client, _abi, debotAddr, key, 6, input, _tvc);
            }
            catch (Exception)
            {
                return null;
            }
        }
        /// <summary>
        /// GetNFT(uint CollId, uint256 id)
        /// </summary>
        /// <param name="client"></param>
        /// <param name="debotAddr"></param>
        /// <param name="input"></param>
        /// <param name="key"></param>
        /// <returns></returns>
        public async Task<ResultOfProcessMessage> GetNFT(ITonClient client, string debotAddr, JToken input, KeyPair key = null)
        {
            try
            {
                key = key != null ? key : _KeyPair;
                if (key == null) throw new Exception();
                return await CCProcessMessage(client, _abi, debotAddr, key, 5, input, _tvc);
            }
            catch (Exception)
            {
                return null;
            }
        }
        /// <summary>
        /// GetCollNFT(uint256 id)
        /// </summary>
        /// <param name="client"></param>
        /// <param name="debotAddr"></param>
        /// <param name="input"></param>
        /// <param name="key"></param>
        /// <returns></returns>
        public async Task<ResultOfProcessMessage> GetCollNFT(ITonClient client, string debotAddr, JToken input, KeyPair key = null)
        {
            try
            {
                key = key != null ? key : _KeyPair;
                if (key == null) throw new Exception();
                return await CCProcessMessage(client, _abi, debotAddr, key, 4, input, _tvc);
            }
            catch (Exception)
            {
                return null;
            }
        }
        /// <summary>
        /// CreateNFT(OwnersCollectionNFT ListNFT)
        /// </summary>
        /// <param name="client"></param>
        /// <param name="debotAddr"></param>
        /// <param name="input"></param>
        /// <param name="key"></param>
        /// <returns></returns>
        public async Task<ResultOfProcessMessage> CreateNFT(ITonClient client,string debotAddr, JToken input, KeyPair key = null)
        {
            try
            {
                key = key != null ? key : _KeyPair;
                if (key == null) throw new Exception();
                return await CCProcessMessage(client, _abi, debotAddr, key, 3, input, _tvc);
            }
            catch (Exception)
            {
                return null;
            }
        }
        /// <summary>
        /// TransactionNFT(NFT nft, OwnerNFT buyer) returns(bool value)
        /// </summary>
        /// <param name="client"></param>
        /// <param name="debotAddr"></param>
        /// <param name="input"></param>
        /// <param name="key"></param>
        /// <returns></returns>
        public async Task<ResultOfProcessMessage> TransactionNFT(ITonClient client, string debotAddr, JToken input, KeyPair key = null)
        {
            try
            {
                key = key != null ? key : _KeyPair;
                if (key == null) throw new Exception();
                return await CCProcessMessage(client, _abi, debotAddr, key, 2, input, _tvc);
            }
            catch (Exception)
            {
                return null;
            }
        }
        /// <summary>
        /// AddUser(OwnerNFT own, uint128 bal)
        /// </summary>
        /// <param name="client"></param>
        /// <param name="debotAddr"></param>
        /// <param name="input"></param>
        /// <param name="key"></param>
        /// <returns></returns>
        public async Task<ResultOfProcessMessage> AddUser(ITonClient client, string debotAddr, JToken input, KeyPair key = null)
        {
            try
            {
                key = key != null ? key : _KeyPair;
                if (key == null) throw new Exception();
                return await CCProcessMessage(client, _abi, debotAddr, key, 1, input, _tvc);
            }
            catch (Exception)
            {
                return null;
            }
        }
        /// <summary>
        /// sendTransaction(address dest, uint128 value, bool bounce)
        /// </summary>
        /// <param name="client"></param>
        /// <param name="debotAddr"></param>
        /// <param name="input"></param>
        /// <param name="key"></param>
        /// <returns></returns>
        public async Task<ResultOfProcessMessage> sendTransaction(ITonClient client, string debotAddr, JToken input, KeyPair key = null)
        {
            try
            {
                key = key != null ? key : _KeyPair;
                if (key == null) throw new Exception();
                return await CCProcessMessage(client, _abi, debotAddr, key, 8, input, _tvc);
            }
            catch (Exception)
            {
                return null;
            }
        }
        /// <summary>
        /// IsAuth(OwnerNFT own) 
        /// </summary>
        /// <param name="client"></param>
        /// <param name="debotAddr"></param>
        /// <param name="input"></param>
        /// <param name="key"></param>
        /// <returns></returns>
        public async Task<ResultOfProcessMessage> IsAuth(ITonClient client, string debotAddr, JToken input, KeyPair key = null)
        {
            try
            {
                key = key != null ? key : _KeyPair;
                if (key == null) throw new Exception();
                return await CCProcessMessage(client, _abi, debotAddr, key, 9, input, _tvc);
            }
            catch (Exception)
            {
                return null;
            }
        }

        /// <summary>
        /// Конструктор Main
        /// </summary>
        /// <param name="client"></param>
        /// <param name="debotAddr"></param>
        /// <param name="key"></param>
        /// <returns></returns>
        public async Task<ResultOfProcessMessage> Constructor(ITonClient client, string debotAddr, KeyPair key = null)
        {
            try
            {
                key = key != null ? key : _KeyPair;
                if (key == null) throw new Exception();
                return await CCProcessMessage(client, _abi, debotAddr, key, 0, new { }.ToJson(), _tvc);
            }
            catch (Exception)
            {
                return null;
            }
        }
    }
}
