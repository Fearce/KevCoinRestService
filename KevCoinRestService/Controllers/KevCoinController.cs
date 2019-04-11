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

        // GET: api/KevCoin/GetCoin
        [HttpGet("GetKey")]
        public string GetKey()
        {
            var rsa = new RSACryptoServiceProvider(1024);

            return ToXmlString(rsa);

        }

        public static string ToXmlString(RSACryptoServiceProvider rsa)
        {
            RSAParameters parameters = rsa.ExportParameters(true);

            //string keys = string.Format("<RSAKeyValue><Modulus>{0}</Modulus><Exponent>{1}</Exponent><P>{2}</P><Q>{3}</Q><DP>{4}</DP><DQ>{5}</DQ><InverseQ>{6}</InverseQ><D>{7}</D></RSAKeyValue>",
            //    Convert.ToBase64String(parameters.Modulus),
            //    Convert.ToBase64String(parameters.Exponent),
            //    Convert.ToBase64String(parameters.P),
            //    Convert.ToBase64String(parameters.Q),
            //    Convert.ToBase64String(parameters.DP),
            //    Convert.ToBase64String(parameters.DQ),
            //    Convert.ToBase64String(parameters.InverseQ),
            //    Convert.ToBase64String(parameters.D));
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
