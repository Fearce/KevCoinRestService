using System;
using System.Collections.Generic;
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
        Blockchain KevCoin = new Blockchain();

        private static string myKey ="34ce346728813839fbfa307ce520006c56044714aa548929850f9d8a784133c1";
        private string myWalletAddress = Key.GetPublicKeyFromPrivateKey(myKey);

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
            return new string[] { myKey, myWalletAddress };
        }

        // GET: api/KevCoin/5
        [HttpGet("{id}", Name = "Get")]
        public string Get(int id)
        {
            return "value";
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
