using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;
using Org.BouncyCastle.Asn1.Sec;
using Org.BouncyCastle.Crypto;
using Org.BouncyCastle.Crypto.Parameters;
using Org.BouncyCastle.Security;

namespace KevCoinRestService.Models
{
    public class Transaction
    {

        public string FromAddress { get; set; }
        public string ToAddress { get; set; }
        public decimal Amount { get; set; }
        public string Signature { get; set; }

        public Transaction(string fromAddress, string address, decimal amount)
        {
            FromAddress = fromAddress;
            ToAddress = address;
            Amount = amount;
        }



        public void SignTransaction(Key signingKey)
        {
            if (signingKey.PublicKey != FromAddress)
            {
                throw new Exception("You cannot sign transactions for other wallets!");
            }

            var hashTx = HashGenerator.CalculateHash(FromAddress + ToAddress + Amount);

            var curve = SecNamedCurves.GetByName("secp256k1");
            var domain = new ECDomainParameters(curve.Curve, curve.G, curve.N, curve.H);

            var keyParameters = new
                ECPrivateKeyParameters(new Org.BouncyCastle.Math.BigInteger(signingKey.PrivateKey),
                    domain);

            ISigner signer = SignerUtilities.GetSigner("SHA-256withECDSA");

            signer.Init(true, keyParameters);
            signer.BlockUpdate(Encoding.ASCII.GetBytes(hashTx), 0, hashTx.Length);
            var signature = signer.GenerateSignature();
            Signature = Base58Encoding.Encode(signature);
        }

        public bool IsValid()
        {
            if (string.IsNullOrEmpty(FromAddress)) return true;

            if (string.IsNullOrEmpty(Signature) || Signature.Length == 0)
                throw new Exception("No signature in this transaction");

            var curve = SecNamedCurves.GetByName("secp256k1");
            var domain = new ECDomainParameters(curve.Curve, curve.G, curve.N, curve.H);

            var publicKeyBytes = Base58Encoding.Decode(FromAddress);

            var q = curve.Curve.DecodePoint(publicKeyBytes);

            var keyParameters = new
                Org.BouncyCastle.Crypto.Parameters.ECPublicKeyParameters(q,
                    domain);

            ISigner signer = SignerUtilities.GetSigner("SHA-256withECDSA");
            var hashTx = HashGenerator.CalculateHash(FromAddress + ToAddress + Amount);
            signer.Init(false, keyParameters);
            signer.BlockUpdate(Encoding.ASCII.GetBytes(hashTx), 0, hashTx.Length);

            var signatureBytes = Base58Encoding.Decode(Signature);

            return signer.VerifySignature(signatureBytes);
        }

        public override string ToString()
        {
            return $"{this.Amount}:{this.FromAddress}:{this.ToAddress}";
        }

        //public bool IsValid()
        //{
        //    if (this.fromAddress === null) return true;

        //    if (!this.signature || this.signature.length === 0)
        //    {
        //        throw new Error('No signature in this transaction');
        //    }

        //    const publicKey = ec.keyFromPublic(this.fromAddress, 'hex');
        //    return publicKey.verify(this.calculateHash(), this.signature);
        //}

    }
}
