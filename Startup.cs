using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using NewNFTGel.DopComp;
using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using TestTonClientQueries;
using TonSdk;
using TonSdk.Modules;
using static TestTonClientQueries.CommandsTonContract;

namespace NewNFTGel
{
    public class Startup
    {
        static public CommandsTonContract ctc;
        static public ITonClient _client;
        static public IWebHostEnvironment _hostEnvironment;
        static public Thread checker;

        async private Task RegMainContract()
        {
            _client = TonClient.Create(new ClientConfig
            {
                Network = new NetworkConfig
                {
                    ServerAddress = "http://localhost:80",
                    MessageRetriesCount = 10,
                    OutOfSyncThreshold = 1250, 
                    QueryTimeout = 1000,
                    MessageProcessingTimeout = 1000
                },
                Abi = new AbiConfig
                {
                    MessageExpirationTimeout = 2500
                }
            });
            TonClientNodeSeExtensions.GiverAbi = TonUtil.LoadAbi(_hostEnvironment.WebRootPath + "/lib/ABI_Contracts/Giver.abi.json");
            TonClientNodeSeExtensions._GiverAddr = "0:841288ed3b55d9cdafa806807f02a0ae0c169aa5edfe88a789a6482429756a94";
            TonClientNodeSeExtensions.GiveAmount = 99999999999999ul;
            TonClientNodeSeExtensions.GiveCommand = "sendGrams";
            TonClientNodeSeExtensions.GiverForMainContract = true;

            var keypair = await _client.Crypto.GenerateRandomSignKeysAsync();
            var abi = TonUtil.LoadAbi(_hostEnvironment.WebRootPath + "/lib/ABI_Contracts/ArtsOfWoodNFT.abi.json");
            var tvc = TonUtil.LoadTvc(_hostEnvironment.WebRootPath + "/lib/TVC_Contracts/ArtsOfWoodNFT.tvc");

            var abi0 = TonUtil.LoadAbi(_hostEnvironment.WebRootPath + "/lib/ABI_Contracts/ClientNFT.abi.json");
            var tvc0 = TonUtil.LoadTvc(_hostEnvironment.WebRootPath + "/lib/TVC_Contracts/ClientNFT.tvc");

            var clientAddr = await _client.DeployWithGiverAsync(new ParamsOfEncodeMessage() //Запуск контракта и регистрация
            {
                Abi = abi,
                DeploySet = new DeploySet
                {
                    Tvc = tvc
                },
                Signer = new Signer.Keys
                {
                    KeysProperty = keypair
                },
                CallSet = new CallSet
                {
                    FunctionName = Enum.GetValues(typeof(CommandsContract)).GetValue(0).ToString()
                }
            });
            Console.WriteLine(clientAddr.Address + "\nПубличный ключ: " + keypair.Public + "\nСекретный ключ: " + keypair.Secret + "\nDataToSign: " + clientAddr.DataToSign);
            ctc = new CommandsTonContract(abi, tvc, abi0, tvc0, keypair, clientAddr.Address);

            await Startup.ctc.AddUser(Startup._client, Startup.ctc.CommandsContractAddress, new { own = new OwnerNFT() { OwnAddrWallet = clientAddr.Address, bal = 1, LikesNFTbyID = ""}, bal = 1 }.ToJson());
        }

        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        public IConfiguration Configuration { get; }

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {
            services.AddControllersWithViews();
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        async public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            Controllers.MainController.NCSteck = new List<Tuple<Newtonsoft.Json.Linq.JObject, OwnersCollectionNFT>>();
            checker = new Thread(CheckAndConfirm.CheckerNFTStart);
            checker.Start();
            _hostEnvironment = env;
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }
            else
            {
                app.UseExceptionHandler("/Main/Error");
                // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
                app.UseHsts();
            }
            app.UseHttpsRedirection();
            app.UseStaticFiles();

            app.UseRouting();

            app.UseAuthorization();

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllerRoute(
                    name: "default",
                    pattern: "{controller=Main}/{action=Index}/{id?}");
            });

            await RegMainContract();
        }
    }
}
