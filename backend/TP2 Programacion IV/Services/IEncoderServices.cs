namespace TP2_Programming_IV.Services;

public interface IEncoderServices
{
    string Hash(string password);
    bool Verify(string password, string hashed);
}
