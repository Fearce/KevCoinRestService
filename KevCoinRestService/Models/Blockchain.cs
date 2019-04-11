using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace KevCoinRestService.Models
{
    public class Blockchain
    {
        public List<Block> Chain { get; set; }
        public int Difficulty { get; set; }
        public List<Transaction> PendingTransactions { get; set; }
        public decimal MiningReward { get; set; }


        public Blockchain()
        {
            Chain = new List<Block>()
            {
                CreateGenesisBlock(),
            };
            Difficulty = 1;
            PendingTransactions = new List<Transaction>();
            MiningReward = 100;
        }

        private Block CreateGenesisBlock()
        {
            return new Block(DateTime.UtcNow.ToString("d"),new List<Transaction>(),"0" );
        }

        public Block GetLatestBlock()
        {
            return Chain.Last();
        }

        private bool isMining = false;

        public void MinePendingTransactions(string miningRewardAddress)
        {
            if (!isMining)
            {
                isMining = true;
                Transaction rewardTx = new Transaction(null, miningRewardAddress, MiningReward);
                PendingTransactions.Add(rewardTx);

                // TO-DO limit amount of transactions   
                Block block = new Block(DateTime.UtcNow.ToString("d"), PendingTransactions, GetLatestBlock().Hash);
                block.MineBlock(Difficulty);

                Chain.Add(block);

                PendingTransactions = new List<Transaction>();
                isMining = false;
            }

        }

        public void AddTransaction(Transaction transaction)
        {
            if (string.IsNullOrEmpty(transaction.FromAddress) || string.IsNullOrEmpty(transaction.ToAddress))
            {
                throw new Exception("Transaction must include from and to address");
            }

            if (!transaction.IsValid())
            {
                throw new Exception("Cannot add invalid transaction to chain");
            }

            PendingTransactions.Add(transaction);
        }

       public decimal GetBalanceOfAddress(string address)
        {
            decimal balance = 0;

            foreach (var block in Chain)
            {
                foreach (var trans in block.Transactions)
                {
                    // if the address has sent detract from balance
                    if (trans.FromAddress == address)
                    {
                        balance -= trans.Amount;
                    }
                    // if receiver add to balance
                    if (trans.ToAddress == address)
                    {
                        balance += trans.Amount;
                    }
                }
            }

            return balance;
        }



       public bool IsChainValid()
        {
            for (int i = 1; i < Chain.Count; i++)
            {
                var currentBlock = Chain[i];
                var previousBlock = Chain[i - 1];

                if (!currentBlock.HasValidTransactions())
                {
                    return false;
                }

                if (currentBlock.Hash != currentBlock.CalculateHash())
                {
                    return false;
                }

                if (currentBlock.PreviousHash != previousBlock.Hash)
                {
                    return false;
                }
            }

            return true;
        }

    }
}
