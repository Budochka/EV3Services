using System.Security.Cryptography;
using NLog;

namespace v380stream;

class ConnectionHanler
{
    private readonly Logger _logs;

    public ConnectionHanler(Logger log)
    {
        _logs = log;
    }
    private string GenerateRandomPrintable(int len)
    {
        var init = "ABCDEFGHIJKLMNOPQRSTUVWXYZabcdefghijklmnopqrstuvwxyz1234567890!@#$%^&*()_+-=";
        var set = init.ToCharArray();
        Random rnd = new Random();

        var result = "";

        for (int i = 0; i < len; i++)
        {
            result += set[rnd.Next(set.Length)];
        }

        return result;
    }

    private string GeneratePassword(string password)
    {
        const int nBlockLen = 16;
        string randomKey = GenerateRandomPrintable(nBlockLen);
        const string staticKey = "macrovideo+*#!^@";
        var buf = new char[nBlockLen - password.Length % nBlockLen];


        using (var aes1 = Aes.Create())
        {
            staticKey.Select(c => (byte)c).ToArray();
            aes1.Mode = CipherMode.CBC;

            using (var encryptor = aes1.CreateEncryptor())
            {

            }
        }
    }
}