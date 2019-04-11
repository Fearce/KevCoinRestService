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

        private Blockchain KevCoin = Program.KevCoin;

        



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
            Key key = new Key(HashGenerator.CalculateHash(Convert.ToBase64String(parameters.D)));
            Transaction tx = new Transaction(TestWalletAddress, key.PublicKey, 50);
            tx.SignTransaction(new Key(TestKey));
            KevCoin.AddTransaction(tx);
            KevCoin.MinePendingTransactions(TestWalletAddress);
            return key.ToString();
        }

        /// <summary>
        /// Welcome screen
        /// </summary>
        /// <returns></returns>
        // GET: api/KevCoin
        [HttpGet]
        public string Get()
        {
            //Debug.WriteLine("Is chain valid? " + KevCoin.IsChainValid());
            //Transaction tx1 = new Transaction(TestWalletAddress, "test", 10);
            //Debug.WriteLine("Wallet address: " + TestWalletAddress);
            //tx1.SignTransaction(new Key(TestKey));
            //KevCoin.AddTransaction(tx1);

            //Debug.WriteLine("Starting the miner");
            //KevCoin.MinePendingTransactions(TestWalletAddress);

            //Debug.WriteLine("Balance of wallet is : " + KevCoin.GetBalanceOfAddress(TestWalletAddress));

            // KevCoin.Chain[1].Transactions[0].Amount = 1;

            //Debug.WriteLine("Is chain valid? " + KevCoin.IsChainValid());

            string msg = "Welcome to KevCoin. " +
                         "\n\nThe address of the test account is: 3C605877EE1269829B531EA5EAC374D1A78CE9B1B18930C33F88A4053ECA383E0909199B8D8AA537E2F92F09AA84F624D9A1181AB55C556AA1083930CAF9C1186 by visiting this page." +
                         "\nThe balance of the account is: " + KevCoin.GetBalanceOfAddress(TestWalletAddress) +
                         ". \n\nThe blockchain is valid: " + KevCoin.IsChainValid() +
                         "\n\nCheck your own balance with KevCoin/YourWallet, or try it with the above address" +
                         "\n\nGenerate a new wallet with KevCoin/GetKey" +
                         "\n\nEach new wallet is assigned with 50 free tokens from the test account." +
                         "\n\nSend tokens with KevCoin/FromAddress/ToAddress/Amount/PrivateKey" +
                         "\n\nMine pending transactions with KevCoin/Mine/YourWallet" +
                         "\n\n\nRecent Transactions: \n";


            return msg;
        }

        /// <summary>
        /// Get your balance
        /// </summary>
        /// <param name="key"></param>
        /// <returns></returns>
        // GET: api/KevCoin/Address
        [HttpGet("{key}", Name = "Balance")]
        public decimal GetBalance(string key)
        {
            return KevCoin.GetBalanceOfAddress(key);
        }

        /// <summary>
        /// Send a transaction
        /// </summary>
        /// <param name="fromAddress"></param>
        /// <param name="toAddress"></param>
        /// <param name="amount"></param>
        /// <param name="privateKey"></param>
        /// <returns></returns>
        // GET: api/KevCoin/FromAddress/ToAddress/Amount/PrivateKey
        [Route("{FromAddress}/{ToAddress}/{Amount}/{PrivateKey}")]
        [HttpGet]
        public string SendTransaction(string fromAddress, string toAddress, decimal amount, string privateKey)
        {
            string msg = $"";
            Transaction tx = new Transaction(fromAddress,toAddress,amount);
            tx.SignTransaction(new Key(privateKey));
            KevCoin.AddTransaction(tx);
            msg += $"Transaction added to pending transactions and successfully signed." +
                   $"\nTransaction details: \nFrom Address: {fromAddress}.\nTo Address: {toAddress}.\n Amount: {amount}.";
            return msg;
        }

        /// <summary>
        /// Mine pending transactions
        /// </summary>
        /// <param name="walletAddress"></param>
        /// <returns></returns>
        // GET: api/KevCoin/FromAddress/ToAddress/Amount/PrivateKey
        [Route("Mine/{WalletAddress}")]
        [HttpGet]
        public string MineTransactions(string walletAddress)
        {
            string msg = $"Pending transactions successfully mined.";
            KevCoin.MinePendingTransactions(walletAddress);
            return msg;
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
