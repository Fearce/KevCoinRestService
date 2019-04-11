using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Numerics;
using System.Threading.Tasks;

namespace KevCoinRestService.Models
{
    public class Key
    {
        public string PrivateKey { get; set; }
        public string PublicKey { get; set; }
    
        

        public Key(string privateKey)
        {
            PrivateKey = privateKey;
            PublicKey = GetPublicKeyFromPrivateKey(PrivateKey);
        }

        //public static string GetPublicKeyFromPrivateKeyEx(string privateKey)

        //{

        //    var curve = SecNamedCurves.GetByName("secp256k1");

        //    var domain = new ECDomainParameters(curve.Curve, curve.G, curve.N, curve.H);



        //    var d = new BC.Math.BigInteger(privateKey);

        //    var q = domain.G.Multiply(d);



        //    var publicKey = new BC.Crypto.Parameters.ECPublicKeyParameters(q, domain);

        //    return Base58Encoding.Encode(publicKey.Q.GetEncoded());

        //}

        public static string GetPublicKeyFromPrivateKey(string privateKey)

        {

            var p = BigInteger.Parse("0FFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFEFFFFFC2F", NumberStyles.HexNumber);

            var b = (BigInteger)7;

            var a = BigInteger.Zero;

            var Gx = BigInteger.Parse("79BE667EF9DCBBAC55A06295CE870B07029BFCDB2DCE28D959F2815B16F81798", NumberStyles.HexNumber);

            var Gy = BigInteger.Parse("483ADA7726A3C4655DA4FBFC0E1108A8FD17B448A68554199C47D08FFB10D4B8", NumberStyles.HexNumber);



            CurveFp curve256 = new CurveFp(p, a, b);

            Point generator256 = new Point(curve256, Gx, Gy);



            var secret = BigInteger.Parse(privateKey, NumberStyles.HexNumber);

            var pubkeyPoint = generator256 * secret;

            return pubkeyPoint.X.ToString("X") + pubkeyPoint.Y.ToString("X");


        }

        public override string ToString()
        {
            return $"{nameof(PrivateKey)}: {PrivateKey}, {nameof(PublicKey)}: {PublicKey}";
        }
    }


}
