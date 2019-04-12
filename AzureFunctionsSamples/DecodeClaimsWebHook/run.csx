#r "Newtonsoft.Json"

using System;
using System.Net;
using System.Text;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Primitives;
using Newtonsoft.Json;

public static async Task<IActionResult> Run(HttpRequest req, ILogger log)
{
    string requestBody = await new StreamReader(req.Body).ReadToEndAsync();
    dynamic inputClaims = JsonConvert.DeserializeObject(requestBody);

    var decodedIdentity = string.Empty;
    var decodedAddress = string.Empty;

    // Decode the identity
    if(inputClaims?.encodedIdentity != null)
        decodedIdentity = ConvertFromSafeBase64String((string)inputClaims?.encodedIdentity);

    // Decode the address
    if(inputClaims?.encodedAddress != null)
        decodedAddress = ConvertFromSafeBase64String((string)inputClaims?.encodedAddress);

    // Return the result
    return new OkObjectResult(new {decodedIdentity, decodedAddress});
}

private static string ConvertFromSafeBase64String(string safeb64)
{
    // See https://tools.ietf.org/html/rfc3548#section-4
    var base64 = safeb64.Replace("-", "+").Replace("_", "/");
    return Encoding.UTF8.GetString(Convert.FromBase64String(base64));
}
