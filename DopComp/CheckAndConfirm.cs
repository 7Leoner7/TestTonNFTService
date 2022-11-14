using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using TestTonClientQueries;
using Newtonsoft.Json.Linq;
using static NewNFTGel.Controllers.MainController;
using static NewNFTGel.Controllers.SecondController;
using TonSdk;
using AntiPlagueImg;
using System.IO;
using System.Text;

namespace NewNFTGel.DopComp
{
    static public class CheckAndConfirm
    {
        async static public void CheckerNFTStart()
        {
            try
            { 
                Console.WriteLine("Checker enable");
                while (true)
                {
                    await Task.Delay(100);
                    if (NCSteck.Count != 0)
                    {
                        await CheckAndDeployNFT(NCSteck[0].Item1, NCSteck[0].Item2);
                        NCSteck.RemoveAt(0);
                        Console.WriteLine("Стек пройден");
                    }
                }
            }
            catch (Exception e)
            {
                Console.WriteLine(e.Message);
                Console.WriteLine("checker restart");
                Startup.checker.Start();
            }
           
        }

        public static void ConsoleMessage(string message, string address)
        {
            string[] str = new string[1];
            str[0] = message;
            var vm = new Models.ContactModel() { address = address, messages = str };
            SendMessage(ref vm);
        }

        //расчёты детерминанты!!!
        public static async Task CheckAndDeployNFT(JObject json, OwnersCollectionNFT OwnersCollect)
        {
            try
            {
                Console.WriteLine("Стек начат");
                List<NFT> tempList = new List<NFT>(); 
                FileStream fs = new FileStream(Startup._hostEnvironment.WebRootPath + "/lib/CollectsNFT/PicturesComposition.json", FileMode.OpenOrCreate, FileAccess.ReadWrite); 
                JArray jsonArr = new();
                ulong id = 0; 
                byte[] buffer = new byte[fs.Length];
                await fs.ReadAsync(buffer, 0, buffer.Length);
                string textFromFile = Encoding.Default.GetString(buffer);
                 try
                 { 
                    jsonArr = JArray.Parse(textFromFile);
                 }
                 catch
                 {
                    jsonArr = null;
                 }   
                   
                foreach (var e in json)
                {
                    var temp = e.Value.ToObject<NFTofJSON>();
                    Console.WriteLine("Анализ NFT номер " + id);
                    PictureComposition pc = new PictureComposition(temp.url, false, 256, 256);
                    NFT nft = new NFT
                    {
                        addrNFT = OwnersCollect.Creator.OwnAddrWallet,
                        pay_able = temp.isSelling,
                        cost = temp.cost,
                        description = temp.descript,
                        Creater = OwnersCollect.Creator,
                        IdColl = OwnersCollect.IdColl,
                        idNFT = id,
                        IsOriginal = false,
                        Owner = OwnersCollect.Creator,
                        PathNFT = temp.url,
                        det = new determinant { ColorSum = 0, decomposition = pc.GetCodeComposition, deltaColor = 0 }
                    };
                    double PlaguePoint = 0;
                    if(jsonArr!=null)
                    foreach(var e0 in jsonArr)
                    {
                        var temp0 = e0.ToObject<OwnersCollectionNFT>();
                        for(int i = 0; i < temp0.CollNFT.Length; i++)
                        {
                           PictureComposition pc0 = new PictureComposition(temp0.CollNFT[i].det.decomposition, new Tuple<byte, byte, byte, byte>(0, 0, 0, 0), 256, 256);
                           PlaguePoint = pc.EqualsF(pc0);
                            if (PlaguePoint >= 75) break;
                        }
                        if (PlaguePoint >= 75) 
                        { 
                            Console.WriteLine("NFT под номером " + id + " не прошёл проверку на оригинальность.");
                            ConsoleMessage("Console: NFT под номером " + id + " не прошёл проверку на оригинальность.", OwnersCollect.Creator.OwnAddrWallet);
                            break;
                        }
                    }
                    if (PlaguePoint < 75) 
                    {
                        nft.IsOriginal = true;
                        tempList.Add(nft);
                        Console.WriteLine("NFT под номером " + id + " прошёл проверку на оригинальность УСПЕШНО!!!");
                        ConsoleMessage("Console: NFT под номером " + id + " прошёл проверку на оригинальность УСПЕШНО!!!", OwnersCollect.Creator.OwnAddrWallet);
                        id++;
                    }
                }
                OwnersCollect.CollNFT = tempList.ToArray();
                string[] decomposition = new string[OwnersCollect.CollNFT.Length];
                if (tempList.Count != 0)
                {
                    for (int i = 0; i < OwnersCollect.CollNFT.Length; i++) 
                    {
                        decomposition[i] = OwnersCollect.CollNFT[i].det.decomposition;
                        OwnersCollect.CollNFT[i].det.decomposition = ""; 
                    }
                    var t = await Startup.ctc.CreateNFT(Startup._client, Startup.ctc.CommandsContractAddress, new { ListNFT = OwnersCollect }.ToJson());
                    if (t == null) { 
                        Console.WriteLine("Деплой провален");
                        ConsoleMessage("Console: Деплой коллекции провален", OwnersCollect.Creator.OwnAddrWallet);
                       
                    }
                    else { 
                        Console.WriteLine("Деплой пройден успешно");
                        ConsoleMessage("Console: Деплой коллекции успешно пройден", OwnersCollect.Creator.OwnAddrWallet);
                        for (int i = 0; i < OwnersCollect.CollNFT.Length; i++)
                           OwnersCollect.CollNFT[i].det.decomposition = decomposition[i];
                        if (jsonArr == null)
                        {
                            OwnersCollectionNFT[] temp = { OwnersCollect };
                            var t0 = JArray.FromObject(temp);
                            var text = Encoding.Default.GetBytes(t0.ToString().ToCharArray());
                            fs.Position = 0;
                            await fs.WriteAsync(text, 0, text.Length);
                        }
                        else if (tempList.Count != 0)
                        {
                            jsonArr.Add(OwnersCollect.ToJson());
                            var text = Encoding.Default.GetBytes(jsonArr.ToString().ToCharArray());
                            fs.Position = 0;
                            await fs.WriteAsync(text, 0, text.Length);
                        }
                    }
                }
               
                fs.Close();
            }
            catch (Exception e)
            {
                Console.WriteLine(e.Message);
            }
        }
    }
}
