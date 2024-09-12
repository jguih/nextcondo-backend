
namespace IntegrationTests.Utils;

public static class HttpClientExtension
{
    public static async Task<HttpResponseMessage> LoginAsync(
        this HttpClient httpClient, 
        string email, 
        string password)
    {
        using MultipartFormDataContent credentials = new()
        {
            { new StringContent(email), "email" },
            { new StringContent(password), "password" }
        };

        return await httpClient.PostAsync("/Auth/login", credentials);
    }
}
