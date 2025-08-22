using System.Text;
using Microsoft.AspNetCore.Mvc;
using DilbertServiceReference;

namespace Demo.SOAP.Azure.AppInsights.Controllers;

[ApiController]
[Route("[controller]")]
public class MemController : ControllerBase
{
    private readonly ILogger<MemController> _logger;
    private readonly Random _random;
    private readonly byte[] _alphabet;

    public MemController(ILogger<MemController> logger)
    {
        _logger = logger;
        _random = new Random();
        _alphabet = Encoding.ASCII.GetBytes("ABCDEFGHIJKLMNOPQRSTUVWXYZabcdefghijklmnopqrstuvwxyz0123456789");
    }

    [HttpPost(Name = "Alloc")]
    public async Task<string> Alloc(int size)
    {
        var bytes = new byte[size];

        for (int i = 0; i < bytes.Length; i++)
        {
            bytes[i] = _alphabet[_random.Next(_alphabet.Length)];
        }

        return await Task.FromResult(Encoding.ASCII.GetString(bytes));
    }
}
