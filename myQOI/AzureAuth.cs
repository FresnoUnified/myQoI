using Microsoft.Azure.KeyVault;
using Microsoft.IdentityModel.Clients.ActiveDirectory;
using System;
using System.Threading.Tasks;
namespace SpeedtestNetCli
{
    public class AzureAuth
    {
        public static string GetVaultValue()

        {

            KeyVaultClient client = new KeyVaultClient(new KeyVaultClient.AuthenticationCallback(GetToken));

            var vaultAddress = "KeyVaultURL";

            var secretName = "SecretName";

            var secret = client.GetSecretAsync(vaultAddress, secretName).GetAwaiter().GetResult();

            return secret.Value;

        }
        static async Task<string> GetToken(string authority, string resource, string scope)

        {

            var clientId = "ClientID";

            var clientSecret = "ClientSecret";

            ClientCredential credential = new ClientCredential(clientId, clientSecret);

            var context = new AuthenticationContext(authority, TokenCache.DefaultShared);

            var result = await context.AcquireTokenAsync(resource, credential);

            return result.AccessToken;

        }
    }
   

}
