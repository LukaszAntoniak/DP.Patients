using IdentityModel.Client;
using Microsoft.Identity.Client;
using System;
using System.Net.Http;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;

namespace DP.Client
{
    class Program
    {
        static async Task Main(string[] args)
        {
            var client = new HttpClient();

            // Method 1 - Secred Id
            // Required Microsoft.AspNetCore.Authentication.JwtBearer 3.1.9 - newer is not compatibile with .Net Core 3.0
            // OAuth 2.0 - best method for server - server authentication as secretId shouldn't be exposed to users
            string token1 = await GetToken();

            /*
            // Method 2 - Interactive method
            // Required microsoft.identity.client package
            // login: student2@marciniwanowski.onmicrosoft.com  password: 9s6ecX8ADYfj
            // Browser-based process of authorization - best for users who can run console app locally as it requires browser to get access.
            var app = PublicClientApplicationBuilder.Create("fce95216-40e5-4a34-b041-f287e46532be")
                .WithAuthority("https://login.microsoftonline.com/146ab906-a33d-47df-ae47-fb16c039ef96/v2.0")
                .WithDefaultRedirectUri()
                .Build();
            // mehtod requires to log in as a user configured properly in the Azure Active DIrectory Example: login: password:
            var token2 = await app.AcquireTokenInteractive(new[] { "api://fce95216-40e5-4a34-b041-f287e46532be/.default" }).ExecuteAsync();


            // Method 3 - device code
            // Required microsoft.identity.client package
            // Process of authorization when application can't open browser - best for users who CANNOT run console app locally as it requires browser to get access.
            var result = await app.AcquireTokenWithDeviceCode(
                new[] { "api://fce95216-40e5-4a34-b041-f287e46532be/.default" },
                async deviceCpdeResult =>
                {
                    Console.WriteLine(deviceCpdeResult.Message);
                }
            )
            .ExecuteAsync();
            var token3 = result.AccessToken;
            */

            client.DefaultRequestHeaders.Authorization = BasicAuthenticationHeaderValue.Parse("Bearer " + token1);


            /* Module 2,3
            Console.WriteLine("Dane nowego pacjenta");
            Console.WriteLine("Podaj imie:");
            string imie = Console.ReadLine();
            Console.WriteLine("Podaj nazwisko:");
            string nazwisko = Console.ReadLine();
            Console.WriteLine("Podaj wiek:");
            int wiek = int.Parse(Console.ReadLine());
            Console.WriteLine("Podaj @:");
            string email = Console.ReadLine();

            
            Patient newPatient = new Patient()
            {
                FirstName = imie,
                LastName = nazwisko,
                Age = wiek,
                CovidTestDate = DateTime.Now,
                Email = email
            };

            string patientJson = JsonSerializer.Serialize(newPatient);

            
            var message = await client.PostAsync(
                "https://localhost:44327/api/patients/new",
                new StringContent(patientJson, Encoding.UTF8, "application/json")
            );
            */

            // Module 4
            var exceptionRunner = await client.PutAsync(
                "https://localhost:5001/api/patients",
                new StringContent("", Encoding.UTF8, "application/json")
            );
        }

        private static async Task<string> GetToken()
        {
            using var client = new HttpClient();

            DiscoveryDocumentResponse disco = await client.GetDiscoveryDocumentAsync(new DiscoveryDocumentRequest()
            {
                Address = "https://login.microsoftonline.com/146ab906-a33d-47df-ae47-fb16c039ef96/v2.0/",
                Policy =
                {
                    ValidateEndpoints = false
                }
            });

            if (disco.IsError)
                throw new InvalidOperationException(
                $"No discovery document. Details: {disco.Error}");

            var tokenRequest = new ClientCredentialsTokenRequest
            {
                Address = disco.TokenEndpoint,
                ClientId = "fce95216-40e5-4a34-b041-f287e46532be",
                ClientSecret = "XWGsyzt9uM-Ia2SA8WE7~gvUJ~4og-U_1p",
                Scope = "api://fce95216-40e5-4a34-b041-f287e46532be/.default"
            };
            var token = await client.RequestClientCredentialsTokenAsync(tokenRequest);

            if (token.IsError)
                throw new InvalidOperationException($"Couldn't gather token. Details: {token.Error}");

            return token.AccessToken;
        }
    }

    public class Patient
    {
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public int Age { get; set; }
        public DateTime CovidTestDate { get; set; }
        public string Email { get; set; }
    }
}
