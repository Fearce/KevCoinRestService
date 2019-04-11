using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Security.Cryptography;
using System.Threading.Tasks;
using KevCoinRestService.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace KevCoinRestService.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class KevCoinController : ControllerBase
    {
        private static string TestKey = "34ce346728813839fbfa307ce520006c56044714aa548929850f9d8a784133c1";
        private static string TestWalletAddress = Key.GetPublicKeyFromPrivateKey(TestKey);

        Blockchain KevCoin = new Blockchain();

        



        /// <summary>
        /// Generates a random private & public key for you to use as a wallet.
        /// </summary>
        /// <returns></returns>
        // GET: api/KevCoin/GetCoin
        [HttpGet("GetKey")]
        public string GetKey()
        {
            var rsa = new RSACryptoServiceProvider(1024);
            RSAParameters parameters = rsa.ExportParameters(true);
            return new Key(HashGenerator.CalculateHash(Convert.ToBase64String(parameters.D))).ToString();
        }

        // GET: api/KevCoin
        [HttpGet]
        public IEnumerable<string> Get()
        {
            Debug.WriteLine("Is chain valid? " + KevCoin.IsChainValid());
            Transaction tx1 = new Transaction(TestWalletAddress, "test", 10);
            Debug.WriteLine("Wallet address: " + TestWalletAddress);
            tx1.SignTransaction(new Key(TestKey));
            KevCoin.AddTransaction(tx1);

            Debug.WriteLine("Starting the miner");
            KevCoin.MinePendingTransactions(TestWalletAddress);

            Debug.WriteLine("Balance of wallet is : " + KevCoin.GetBalanceOfAddress(TestWalletAddress));

            Debug.WriteLine("Is chain valid? " + KevCoin.IsChainValid());

            return new string[] { "Get test wip" };
        }

        // GET: api/KevCoin/Address
        [HttpGet("{key}", Name = "Balance")]
        public string GetBalance(string key)
        {
            return KevCoin.GetBalanceOfAddress(key).ToString("C");
        }

        // POST: api/KevCoin
        [HttpPost]
        public void Post([FromBody] string value)
        {
        }

        // PUT: api/KevCoin/5
        [HttpPut("{id}")]
        public void Put(int id, [FromBody] string value)
        {
        }

        // DELETE: api/ApiWithActions/5
        [HttpDelete("{id}")]
        public void Delete(int id)
        {
        }
    }
}
