var client = new HttpClient();
var request = new HttpRequestMessage(HttpMethod.Post, "endpoint API");
request.Headers.Add("Authorization", "Bearer "API KEY");
var content = new StringContent("{\r\n    \"question\": \"hello ai\"\r\n}\r\n", null, "application/json");
request.Content = content;
var response = await client.SendAsync(request);
response.EnsureSuccessStatusCode();
Console.WriteLine(await response.Content.ReadAsStringAsync());
