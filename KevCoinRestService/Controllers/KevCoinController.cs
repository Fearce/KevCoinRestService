﻿using System;
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
                         "\n\nThe address of the test account is: 3C605877EE1269829B531EA5EAC374D1A78CE9B1B18930C33F88A4053ECA383E0909199B8D8AA537E2F92F09AA84F624D9A1181AB55C556AA1083930CAF9C1186 by visiting this page." +
                         "\nThe balance of the account is: " + KevCoin.GetBalanceOfAddress(TestWalletAddress) +
                         ". \n\nThe blockchain is valid: " + KevCoin.IsChainValid() +
                         "\n\nCheck your own balance with KevCoin/PublicKey, or try it with the above address" +
                         "\n\nGenerate a new wallet with KevCoin/GetKey, remember to save your keys somewhere." +
                         "\n\nEach new wallet is assigned with 50 free coins from the test account." +
                         "\n\nSend coins with KevCoin/SenderPublicKey/ReceiverPublicKey/Amount/SenderPrivateKey" +
                         "\n\nMine pending transactions with KevCoin/Mine/PublicKey" +
                         "\n\n\nRecent Transactions:";
            foreach (var block in KevCoin.Chain)
            {
                foreach (var tx in block.Transactions)
                {
                    msg += "\n" + tx.ToString();
                }
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
