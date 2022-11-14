using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using NewNFTGel.Models;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Threading.Tasks;
using TestTonClientQueries;
using TonSdk;
using TonSdk.Modules;

namespace NewNFTGel.Controllers
{
    public class MainController : Controller
    {

        static public List<Tuple<JObject, OwnersCollectionNFT>> NCSteck;

        public MainController()
        {
           
        }

        static public async Task<bool> authorize(AuthModel vm, bool IsAuthClient)
        {
            try
            {
                if (IsAuthClient)
                {
                    var client = await Startup._client.Crypto.SignAsync(new ParamsOfSign() { Keys = new KeyPair() { Secret = vm.secret_key, Public = vm.public_key }, Unsigned = "" });
                    var res = await Startup._client.Crypto.VerifySignatureAsync(new ParamsOfVerifySignature() { Public = vm.public_key, Signed = client.Signed });
                }
                var t = await Startup.ctc.IsAuth(Startup._client, Startup.ctc.CommandsContractAddress,
                new
                {
                    own = new OwnerNFT() { bal = 0, LikesNFTbyID = "", OwnAddrWallet = vm.address }
                }.ToJson());
                if (t == null) throw new Exception();
                bool B = t.Decoded.Output.Value<bool>("value0");
                if(vm.address != Startup.ctc.CommandsContractAddress)if (!B) throw new Exception();
                return true;
            }
            catch
            {
                return false;
            }
        }

#pragma warning disable CS1998 // В асинхронном методе отсутствуют операторы await, будет выполнен синхронный метод
        public async Task<IActionResult> ContactAsync(ContactModel vm)
        {
            if (Startup.ctc == null) return View(vm);
            //vm.IsFinaleAuth = await authorize(vm, false);
            vm.IsAdmin = vm.address == Startup.ctc.CommandsContractAddress;
            return View(vm);
        }

        public async Task<IActionResult> IndexAsync(IndexViewModel vm)
        {
            try
            {
                if (Startup.ctc == null) return View(vm);
                //vm.IsFinaleAuth = await authorize(vm, false);
                var list = await Startup.ctc.GetAllCollectionsNFT(Startup._client, Startup.ctc.CommandsContractAddress, new { }.ToJson());
                NFT[] res = { };

                if (list != null)
                {
                    var json = JObject.Parse(list.Decoded.Output.ToString());
                    List<NFT> tempList = new List<NFT>();
                   
                    foreach (var e in json)
                    {
                        var temp = e.Value.ToObject<OwnersCollectionNFT[]>();
                        for (int j = 0; j< temp.Length; j++) { 
                            for (int i = 0; i < temp[j].CollNFT.Length; i++)
                                tempList.Add(new NFT
                                {
                                    addrNFT = vm.address,
                                    pay_able = temp[j].CollNFT[i].pay_able,
                                    cost = temp[j].CollNFT[i].cost,
                                    description = temp[j].CollNFT[i].description,
                                    Creater = temp[j].CollNFT[i].Creater,
                                    IdColl = temp[j].CollNFT[i].IdColl,
                                    idNFT = temp[j].CollNFT[i].idNFT,
                                    IsOriginal = temp[j].CollNFT[i].IsOriginal,
                                    Owner = temp[j].CollNFT[i].Owner,
                                    PathNFT = temp[j].CollNFT[i].PathNFT,
                                    det = temp[j].CollNFT[i].det
                                }); 
                        }
                    }
                    vm.ListOfNFT = tempList;
                    vm.number = (ulong)vm.ListOfNFT.Count;
                }        
                return View(vm);
            }
            catch (Exception)
            {
                return View(vm);
            }

        }

        public IActionResult Privacy()
        {
            return View();
        }

        public async Task<IActionResult> PersonAreaAsync(PersonAreaViewModel vm)
        {
            try
            {
                if (vm.ToAddress == null) vm.ToAddress = vm.address;
                vm.ListOfCollections = new List<OwnersCollectionNFT>();
                vm.IsOwner = vm.ToAddress == vm.address;
                if (Startup.ctc == null) return View(vm);
                vm.IsFinaleAuth = await authorize(vm, false);
                if(!vm.IsFinaleAuth) throw new Exception();
                var list = await Startup.ctc.GetOwnerCollNFT(Startup._client, Startup.ctc.CommandsContractAddress, new { owner = new OwnerNFT() { OwnAddrWallet = vm.ToAddress, bal = 0, LikesNFTbyID = "" } }.ToJson());

                if (list == null) throw new Exception();
               
                var json = JObject.Parse(list.Decoded.Output.ToString());
                    List<OwnersCollectionNFT> tempList = new List<OwnersCollectionNFT>();
                  
                    foreach (var e in json)
                    {
                        var temp = e.Value.ToObject<OwnersCollectionNFT[]>();
                        for (int j = 0; j < temp.Length; j++)
                        tempList.Add(temp[j]);
                    }
                    vm.ListOfCollections = new List<OwnersCollectionNFT>(tempList);
                return View(vm);
            }
            catch (Exception)
            {
                return View(vm);
            }
        }

        [HttpPost]
        async public Task<IActionResult> LoaderNFTofJSON(ModelLoaderNFTofJSON vm)
        {
            vm.succesfull = false;
            if (Startup.ctc == null) return View(vm);
            var t = authorize(vm, false);
            vm.IsFinaleAuth = await t;
            try
            {
                var json = JObject.Parse(JToken.Parse(vm.JSONofNFT).ToString());
                OwnersCollectionNFT ocn = new();
                ocn.IdColl = 1;
                ocn.Creator = new OwnerNFT() { OwnAddrWallet = vm.address, bal = 2, LikesNFTbyID = "" };
                
                NCSteck.Add(new Tuple<JObject, OwnersCollectionNFT>(json, ocn));
                vm.succesfull = true;
                return View(vm);
            }
            catch (Exception)
            {
                return View(vm);
            }
        }

        [HttpPost]
        async public Task<IActionResult> Auth(IndexViewModel vm)
        {
            if (Startup.ctc == null) return View(vm);
            vm.IsFinaleAuth = await authorize(vm, true);
            return View(vm);
        }

        [HttpPost]
        async public Task<IActionResult> Reg()
        {
            IndexViewModel vm = new IndexViewModel();
            try
            {    
                if (Startup.ctc == null) return View(vm);
                var keypair = await Startup._client.Crypto.GenerateRandomSignKeysAsync();

                var @params = new ParamsOfEncodeMessage() //Запуск контракта и регистрация
                {
                    Abi = Startup.ctc._tabi,
                    DeploySet = new DeploySet
                    {
                        Tvc = Startup.ctc._tvcZ
                    },
                    Signer = new Signer.Keys
                    {
                        KeysProperty = keypair
                    },
                    CallSet = new CallSet
                    {
                        FunctionName = "constructor",
                        Input = new
                        {

                        }.ToJson()
                    }
                };
                var msg = await Startup._client.Abi.EncodeMessageAsync(@params);
                OwnerNFT own = new();
                own.bal = 0;
                own.LikesNFTbyID = "";
                own.OwnAddrWallet = msg.Address;
                var code = await Startup._client.Boc.GetCodeFromTvcAsync(new ParamsOfGetCodeFromTvc() { Tvc = Startup.ctc._tvcZ });
                var t = await Startup.ctc.AddUser(Startup._client, Startup.ctc.CommandsContractAddress, new { own = own, bal = 10000000000 }.ToJson());
                var t1 = await Startup._client.Processing.SendMessageAsync(new ParamsOfSendMessage()
                {
                    Abi = Startup.ctc._tabi,
                    Message = msg.Message,
                    SendEvents = false
                });
                var r = await Startup._client.Processing.WaitForTransactionAsync(new ParamsOfWaitForTransaction()
                {
                    Abi = Startup.ctc._tabi,
                    Message = msg.Message,
                    ShardBlockId = t1.ShardBlockId,
                    SendEvents = false
                });
                await Startup.ctc.CreateClient(Startup._client, msg.Address, new { addr = msg.Address, likesNFT = "" }.ToJson(), keypair);
                return View(new IndexViewModel() { IsFinaleAuth = true, public_key = keypair.Public, secret_key = keypair.Secret, address = msg.Address });
            }
            catch (Exception)
            {
                return View(vm);
            }
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}
