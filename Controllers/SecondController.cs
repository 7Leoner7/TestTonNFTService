using Microsoft.AspNetCore.Mvc;
using NewNFTGel.Models;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TestTonClientQueries;
using TonSdk;
using TonSdk.Modules;
using static NewNFTGel.Controllers.MainController;

namespace NewNFTGel.Controllers
{
    public class SecondController : Controller
    {
        [HttpPost()]
        public async Task<IActionResult> TransactionAsync(TransactModel vm)
        {
            try
            {
                vm.isSuccess = false;
                if (Startup.ctc == null) return View(vm);
                vm.IsFinaleAuth = await authorize(vm, true);
                vm.buyer = (await Startup.ctc.GetClient(Startup._client, vm.address, new { }.ToJson(), new KeyPair() { Public = vm.public_key, Secret = vm.secret_key })).Decoded.Output.Value<OwnerNFT>("client");
                var noj = JObject.Parse(vm.nft).ToObject<NFTofJSON>();
                NFT _nft = new NFT()
                {
                    pay_able = noj.isSelling,
                    PathNFT = noj.url,
                    addrNFT = "",
                    description = noj.descript,
                    cost = noj.cost,
                    det = new determinant() { ColorSum = 0, decomposition = "", deltaColor = 0},
                    Creater = vm.buyer,
                    Owner = vm.buyer,
                    IdColl = noj.IdColl,
                    idNFT = noj.idNFT,
                    IsOriginal = true
                };
                if(_nft.pay_able == true)
                {
                    var temp = await Startup.ctc.TransactionNFT(Startup._client, Startup.ctc.CommandsContractAddress, new { nft = _nft, buyer = vm.buyer}.ToJson());
                    vm.isSuccess = temp.Decoded.Output.Value<bool>("value");
                }
                
                return View(vm);
            }
            catch (Exception)
            {
                return View(vm);
            }
        }

        [HttpPost()]
        public async Task<IActionResult> GetBalanceAsync(BalanceModel vm)
        {
            try
            {
                vm.bal = 0;
                if (Startup.ctc == null) return View(vm);
                vm.IsFinaleAuth = await authorize(vm, true);
                if((vm.IsFinaleAuth == true)&&(vm.address!=Startup.ctc.CommandsContractAddress)) 
                    vm.bal = (await Startup.ctc.GetBalance(Startup._client, vm.address, new { }.ToJson(), new KeyPair() { Secret = vm.secret_key, Public = vm.public_key })).Decoded.Output.Value<float>("bal")/1000000000;
                return View(vm);
            }
            catch (Exception)
            {
                vm.IsFinaleAuth = false;
                return View(vm);
            }
        }

        [HttpPost()]
        public IActionResult GetMessagesAddress(ListClientsMessagesModel vm)
        {
            try
            {
                using (StreamReader sr = new StreamReader(Startup._hostEnvironment.WebRootPath + "/lib/Messages.json"))
                {
                    var json = JObject.Parse(sr.ReadToEnd());
                    List<string> temp = new List<string>();
                    foreach (var e in json)
                    temp.Add(e.Key);
                    vm.addressArray = temp.ToArray();
                }
                return View(vm);
            }
            catch (Exception)
            {
                return View(vm);
            }
        }

        [HttpPost()]
        public IActionResult GetClientsDialog(ContactModel vm)
        {
            try
            {
                using (StreamReader sr = new StreamReader(Startup._hostEnvironment.WebRootPath + "/lib/Messages.json"))
                {
                    var json = JObject.Parse(sr.ReadToEnd());
                    foreach (var e in json)
                    {
                        if (e.Key == vm.address)
                        {
                            var b = e.Value.First.First;
                            vm.messages = b.ToObject<string[]>();
                            break;
                        }
                    }
                }
                return View(vm);
            }
            catch (Exception)
            {
                return View(vm);
            }
        }
        
        public static void SendMessage(ref ContactModel vm)
        {
            JObject json = new();
            using (StreamReader sr = new StreamReader(Startup._hostEnvironment.WebRootPath + "/lib/Messages.json"))
            {
                try
                {
                    json = JObject.Parse(sr.ReadToEnd());
                    foreach (var e in json)
                    {
                        if (e.Key == vm.address)
                        {
                            var temp = e.Value.First.First.ToObject<string[]>();
                            vm.messages = temp.Concat(vm.messages).ToArray();
                            JToken j;
                            json.Remove<string, JToken>(e.Key, out j);
                            break;
                        }
                    }
                }
                catch { }
                json.Add(vm.address, new { vm.messages }.ToJson());
            }
            using (FileStream sw = new FileStream(Startup._hostEnvironment.WebRootPath + "/lib/Messages.json", FileMode.Create, FileAccess.Write))
            {
                sw.Position = 0;
                sw.Write(Encoding.Default.GetBytes(json.ToString().ToCharArray()));
            }
        }

        [HttpPost()]
        public IActionResult SendClientMessage(ContactModel vm)
        {
            try
            {
                SendMessage(ref vm);
                return View(vm);
            }
            catch (Exception)
            {
                return View(vm);
            }
        }
    }
}
