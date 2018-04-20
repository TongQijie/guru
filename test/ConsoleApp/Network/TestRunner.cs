using System;
using System.Collections.Generic;
using System.IO;
using System.Security.Cryptography;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using Guru.DependencyInjection;
using Guru.Network.Abstractions;

namespace ConsoleApp.Network
{
    public class TestRunner
    {
        public async Task Run()
        {
            var browser = DependencyContainer.Resolve<IWebBrowser>();
            await browser.Browse("https://pan.baidu.com/", null);
            var gid = GenerateGid();
            var token = "";
            await browser.Browse("https://passport.baidu.com/v2/api/", new Dictionary<string, string>()
            {
                { "getapi", "" },
                { "tpl", "netdisk" },
                { "subpro", "netdisk_web" },
                { "apiver", "v3" },
                { "tt", GenerateTimestamp().ToString() },
                { "class", "login" },
                { "gid", gid },
                { "loginversion", "v4" },
                { "logintype", "basicLogin" },
                { "traceid", "" },
                { "callback", "bd__cbs__mx2p3r" },
            }, async (response) => {
                var content = await response.GetStringAsync();
                var match = Regex.Match(content, "\"token\"\\s*:\\s*\"(?<token>.+?)\"");
                if (match.Success)
                {
                    token = match.Groups["token"].Captures[0].Value;
                }
            });
            Console.WriteLine($"Token:{token}");
            var pubkey = "";
            var rsakey = "";
            await browser.Browse("https://passport.baidu.com/v2/getpublickey", new Dictionary<string, string>()
            {
                { "token", token },
                { "tpl", "netdisk" },
                { "subpro", "netdisk_web" },
                { "apiver", "v3" },
                { "tt", GenerateTimestamp().ToString() },
                { "gid", gid },
                { "traceid", "" },
                { "callback", "bd__cbs__sqvtqi" },
            }, async (response) => {
                var content = await response.GetStringAsync();
                var match1 = Regex.Match(content, "\"pubkey\"\\s*:\\s*'(?<pubkey>.+?)'");
                if (match1.Success)
                {
                    pubkey = match1.Groups["pubkey"].Captures[0].Value
                        .Replace("\\n", "")
                        .Replace("-----BEGIN PUBLIC KEY-----", "")
                        .Replace("-----END PUBLIC KEY-----", "");
                }
                var match2 = Regex.Match(content, "\"key\"\\s*:\\s*'(?<rsakey>.+?)'");
                if (match2.Success)
                {
                    rsakey = match2.Groups["rsakey"].Captures[0].Value;
                }
            });
            Console.WriteLine($"pubkey:{pubkey}");
            Console.WriteLine($"rsakey:{rsakey}");

            var password = "";
            using (RSA rsa = CreateRsaFromPublicKey("MIGfMA0GCSqGSIb3DQEBAQUAA4GNADCBiQKBgQDU5ASF+Vtbhuct3XiFkczwWFijU0xhm1R7cPqC+ix/Ut+9zBbYfvifel3a8gRAX4ETvASh9rtwEeR3iFin0WAZa6DxcpyljXgd6ip6WG3ZJDIdx/TiqkGuu7lPbEADHJODfdTSIJYP6IYZ0XsV7B/xPP5i/ZP9EJfMaauJFdboVQIDAQAB"))
            {
                var cipherBytes = rsa.Encrypt(Encoding.UTF8.GetBytes("baiduzhidao"), RSAEncryptionPadding.Pkcs1);
                password = Convert.ToBase64String(cipherBytes);
            }
            Console.WriteLine($"password:{password}");
        }
        
        private long GenerateTimestamp()
        {
            return DateTimeOffset.Now.ToUnixTimeMilliseconds();
        }

        private string GenerateGid()
        {
            // "xxxxxxx-xxxx-4xxx-yxxx-xxxxxxxxxxxx".replace(/[xy]/g, function(e) {
            //     var t = 16 * Math.random() | 0
            //       , n = "x" == e ? t : 3 & t | 8;
            //     return n.toString(16)
            // }).toUpperCase()
            var source = "xxxxxxx-xxxx-4xxx-yxxx-xxxxxxxxxxxx";
            var target = new StringBuilder();
            foreach (var c in source)
            {
                if (c == 'x')
                {
                    target.Append((new Random().Next(0, 15)).ToString("X1"));
                }
                else if (c == 'y')
                {
                    target.Append((new Random().Next(8, 11)).ToString("X1"));
                }
                else
                {
                    target.Append(c);
                }
            }
            return target.ToString();
        }

        private RSA CreateRsaFromPublicKey(string publicKeyString)
        {
            byte[] SeqOID = { 0x30, 0x0D, 0x06, 0x09, 0x2A, 0x86, 0x48, 0x86, 0xF7, 0x0D, 0x01, 0x01, 0x01, 0x05, 0x00 };
            byte[] x509key;
            byte[] seq = new byte[15];
            int x509size;

            x509key = Convert.FromBase64String(publicKeyString);
            x509size = x509key.Length;

            using (var mem = new MemoryStream(x509key))
            {
                using (var binr = new BinaryReader(mem))
                {
                    byte bt = 0;
                    ushort twobytes = 0;

                    twobytes = binr.ReadUInt16();
                    if (twobytes == 0x8130)
                        binr.ReadByte();
                    else if (twobytes == 0x8230)
                        binr.ReadInt16();
                    else
                        return null;

                    seq = binr.ReadBytes(15);
                    if (!CompareBytearrays(seq, SeqOID))
                        return null;

                    twobytes = binr.ReadUInt16();
                    if (twobytes == 0x8103)
                        binr.ReadByte();
                    else if (twobytes == 0x8203)
                        binr.ReadInt16();
                    else
                        return null;

                    bt = binr.ReadByte();
                    if (bt != 0x00)
                        return null;

                    twobytes = binr.ReadUInt16();
                    if (twobytes == 0x8130)
                        binr.ReadByte();
                    else if (twobytes == 0x8230)
                        binr.ReadInt16();
                    else
                        return null;

                    twobytes = binr.ReadUInt16();
                    byte lowbyte = 0x00;
                    byte highbyte = 0x00;

                    if (twobytes == 0x8102)
                        lowbyte = binr.ReadByte();
                    else if (twobytes == 0x8202)
                    {
                        highbyte = binr.ReadByte();
                        lowbyte = binr.ReadByte();
                    }
                    else
                        return null;
                    byte[] modint = { lowbyte, highbyte, 0x00, 0x00 };
                    int modsize = BitConverter.ToInt32(modint, 0);

                    int firstbyte = binr.PeekChar();
                    if (firstbyte == 0x00)
                    {
                        binr.ReadByte();
                        modsize -= 1;
                    }

                    byte[] modulus = binr.ReadBytes(modsize);

                    if (binr.ReadByte() != 0x02)
                        return null;
                    int expbytes = (int)binr.ReadByte();
                    byte[] exponent = binr.ReadBytes(expbytes);

                    var rsa = RSA.Create();
                    var rsaKeyInfo = new RSAParameters
                    {
                        Modulus = modulus,
                        Exponent = exponent
                    };
                    rsa.ImportParameters(rsaKeyInfo);
                    return rsa;
                }
            }
        }

        private bool CompareBytearrays(byte[] a, byte[] b)
        {
            if (a.Length != b.Length)
                return false;
            int i = 0;
            foreach (byte c in a)
            {
                if (c != b[i])
                    return false;
                i++;
            }
            return true;
        }
    }
}