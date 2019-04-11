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
        private static string TestKey = "3577c85d2e422f61d9c9d26f2a5b836c9ac9b0b1e77857e1ef9ae062fe3247db";
        private static string TestWalletAddress = "2565e62e2255157fbc54dcd3087bd02fffd8bc98ad950ec67d3f916b51648cef";

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
            key.PublicKey =
                HashGenerator.CalculateHash(key.PrivateKey + parameters.D + parameters.DP + parameters.DQ +
                                            parameters.Exponent);
            Transaction tx = new Transaction(TestWalletAddress, key.PublicKey, 50);
            tx.SignTransaction(new Key(TestKey));
            KevCoin.AddTransaction(tx);
            // KevCoin.MinePendingTransactions(TestWalletAddress);
            return $"Private key: {key.PrivateKey}" +
                   $"\nPublic key: {key.PublicKey}" +
                   $"\nYour mining reward of 50 coins is added to pending transactions.";

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
                         "\n\nThe address of the test account is: 2565e62e2255157fbc54dcd3087bd02fffd8bc98ad950ec67d3f916b51648cef." +
                         "\nThe balance of the test account is: " + KevCoin.GetBalanceOfAddress(TestWalletAddress) +
                         ". \n\nThe blockchain is valid: " + KevCoin.IsChainValid() +
                         "\n\nCheck your own balance with KevCoin/PublicKey, or try it with the above address" +
                         "\n\nGenerate a new wallet with KevCoin/GetKey, remember to save your keys somewhere." +
                         "\n\nEach new wallet is assigned with 50 free coins from the test account." +
                         "\n\nSend coins with KevCoin/SenderPublicKey/ReceiverPublicKey/Amount/SenderPrivateKey" +
                         "\n\nMine pending transactions with KevCoin/Mine/PublicKey" +
                         "\n\n\nPast 50 Transactions:";
            Dictionary<Transaction,string> transactions = new Dictionary<Transaction,string>();
            foreach (var block in KevCoin.Chain)
            {
                foreach (var tx in block.Transactions)
                {
                    if (transactions.Count < 50)
                    {
                        transactions.Add(tx,block.Timestamp);
                    }
                    else
                    {
                        transactions.Remove(transactions.Last().Key);
                        transactions.Add(tx, block.Timestamp);
                    }
                }
            }

            foreach (var tx in transactions)
            {
                msg += "\n" + tx.Value + " " + tx.Key.ToString();
            }

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
        /// Testing method to reset the blockchain if it is invalid
        /// </summary>
        /// <returns></returns>
        [Route("Resetchain")]
        [HttpGet]
        public string ResetBlockchain()
        {
            Program.KevCoin = new Blockchain();
            return "Blockchain reset.";
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
            if (KevCoin.GetBalanceOfAddress(fromAddress) >= amount)
            {
                Transaction tx = new Transaction(fromAddress, toAddress, amount);
                tx.SignTransaction(new Key(){PrivateKey = privateKey,PublicKey = fromAddress});
                KevCoin.AddTransaction(tx);
                msg += $"Transaction added to pending transactions and successfully signed." +
                       $"\nTransaction details: \nFrom Address: {fromAddress}.\nTo Address: {toAddress}.\n Amount: {amount}.";
            }
            else
            {
                msg += $"Transaction declined due to lack of funds." +
                       $"\nAddress: {fromAddress}.\nBalance: {KevCoin.GetBalanceOfAddress(fromAddress)}.\nRequested amount to be transferred: {amount}";
            }
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
            string msg = "";
            if (KevCoin.PendingTransactions.Count < 1)
            {
                msg = "No transactions to mine.";
            }
            else
            {
                msg = $"{KevCoin.PendingTransactions.Count} pending transactions successfully mined." +
                      $"\nWallet {walletAddress}\n rewarded with {KevCoin.MiningReward} coins.";
                KevCoin.MinePendingTransactions(walletAddress);
            }
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
