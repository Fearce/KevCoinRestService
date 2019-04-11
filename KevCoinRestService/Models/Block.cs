using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;

namespace KevCoinRestService.Models
{
    public class Block
    {
        public string Timestamp { get; set; }
        public List<Transaction> Transactions { get; set; }
        public string PreviousHash { get; set; }
        public int Nonce { get; set; }
        public string Hash { get; set; }


        public Block(string timestamp, List<Transaction> transactions, string previousHash)
        {
            Timestamp = timestamp;
            Transactions = transactions;
            PreviousHash = previousHash;
            Nonce = 0;
            Hash = CalculateHash();
        }

        public string CalculateHash()
        {
            return HashGenerator.CalculateHash(Timestamp + Transactions.ToString() + PreviousHash + Nonce);
        }
        

        public void MineBlock(int difficulty)
        {
            string zeros = "";
            for (int i = 0; i <= difficulty; i++)
            {
                zeros += "0";
            }
            while (Hash.Substring(0, difficulty) != zeros)
            {
                Nonce++;
                Hash = CalculateHash();
            }

            Debug.WriteLine("Block mined: " + Hash);
        }

        public bool HasValidTransactions()
        {
            foreach (Transaction tx in Transactions)
            {
                if (!tx.IsValid()) return false;
            }

            return true;
        }
    }
}
