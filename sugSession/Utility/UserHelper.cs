using Sitecore.Security.Accounts;
using SugSession.Model;
using System;
using System.Web.Security;
using System.Text;
using System.IO;
using System.Security.Cryptography;
using Sitecore.Services.Infrastructure.Web.Http.Security;
using System.Collections.Generic;
using System.Linq;

namespace SugSession.Utility
{
    public class UserHelper
    {
        public static MembershipUser FindUserByEmailAndDomain(string emailToMatch, string sitecoreDomain)
        {
            var userCollection = Membership.FindUsersByEmail(emailToMatch);
            foreach (MembershipUser user in userCollection)
            {
                if (!user.Email.Equals(emailToMatch, StringComparison.OrdinalIgnoreCase)) continue;
                User sitecoreUser = User.FromName(user.UserName, false);
                var domain = sitecoreUser.Domain;
                if (domain == null) return null;
                var domainName = domain.Name;
                if (domainName == sitecoreDomain)
                    return user;
            }
            return null;
        }

        public static string EncryptDecryptPassword(AuthenticateRequest authenticateRequest)
        {
            var keybytes = Encoding.UTF8.GetBytes("wOOMgwO4MDgA4MDDDAADA4==");
            var iv = Encoding.UTF8.GetBytes("8080808080808080");
            string decryptedPassword;
            if (authenticateRequest.Encrypted.Equals("true", StringComparison.InvariantCultureIgnoreCase))
                decryptedPassword = DecryptStringFromBytes(System.Convert.FromBase64String(authenticateRequest.Password), keybytes, iv);
            else
                decryptedPassword = authenticateRequest.Password;
            return decryptedPassword;
        }

        private static string DecryptStringFromBytes(byte[] cipherText, byte[] key, byte[] iv)
        {
            // Check arguments.  
            if (cipherText == null || cipherText.Length <= 0)
            {
                throw new ArgumentNullException("cipherText");
            }
            if (key == null || key.Length <= 0)
            {
                throw new ArgumentNullException("key");
            }
            if (iv == null || iv.Length <= 0)
            {
                throw new ArgumentNullException("key");
            }

            // Declare the string used to hold  
            // the decrypted text.  
            string plaintext = null;

            // Create an RijndaelManaged object  
            // with the specified key and IV.  
            using (var rijAlg = new RijndaelManaged())
            {
                //Settings  
                rijAlg.Mode = CipherMode.CBC;
                rijAlg.Padding = PaddingMode.PKCS7;
                rijAlg.FeedbackSize = 128;

                rijAlg.Key = key;
                rijAlg.IV = iv;

                // Create a decrytor to perform the stream transform.  
                var decryptor = rijAlg.CreateDecryptor(rijAlg.Key, rijAlg.IV);

                try
                {
                    // Create the streams used for decryption.  
                    using (var msDecrypt = new MemoryStream(cipherText))
                    {
                        using (var csDecrypt = new CryptoStream(msDecrypt, decryptor, CryptoStreamMode.Read))
                        {

                            using (var srDecrypt = new StreamReader(csDecrypt))
                            {
                                // Read the decrypted bytes from the decrypting stream  
                                // and place them in a string.  
                                plaintext = srDecrypt.ReadToEnd();

                            }

                        }
                    }
                }
                catch
                {
                    plaintext = "keyError";
                }
            }

            return plaintext;
        }

        public static bool ValidateToken(ITokenProvider Provider, System.Net.Http.HttpRequestMessage Request)
        {
            var isValid = false;
            if (Request.Headers.Contains("token"))
            {
                IEnumerable<string> headerValues = Request.Headers.GetValues("token");
                var token = headerValues.FirstOrDefault();
                isValid = Provider.ValidateToken(token).IsValid;
            }
            return isValid;
        }

        public static UserClaims GetUserClaims(ITokenProvider Provider, System.Net.Http.HttpRequestMessage Request)
        {
            var UserClaims = new UserClaims();
            if (Request.Headers.Contains("token"))
            {
                IEnumerable<string> headerValues = Request.Headers.GetValues("token");
                var token = headerValues.FirstOrDefault();
                var claims = Provider.ValidateToken(token).Claims;

                foreach (var _claim in claims)
                {
                    if (_claim.Type == "userid")
                    {
                        UserClaims.UserId = _claim.Value;
                    }
                    if (_claim.Type == "useremail")
                    {
                        UserClaims.EmailId = _claim.Value;
                    }
                }

            }
            return UserClaims;
        }
    }
}
