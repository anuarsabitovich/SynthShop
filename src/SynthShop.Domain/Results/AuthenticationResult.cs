﻿namespace SynthShop.Domain.Results;

public class AuthenticationResult
{
    public string Token { get; set; }
    public string RefreshToken { get; set; }
    public bool Success { get; set; }
    public IEnumerable<string> Errors { get; set; } = Enumerable.Empty<string>();
}