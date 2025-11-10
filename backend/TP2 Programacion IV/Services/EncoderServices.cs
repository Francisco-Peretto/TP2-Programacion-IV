namespace TP2_Programming_IV.Services;

public class EncoderServices : IEncoderServices
{ 
    public string Hash(string password) => BCrypt.Net.BCrypt.HashPassword(password);
    public bool Verify(string password, string hashed) => BCrypt.Net.BCrypt.Verify(password, hashed);
}
