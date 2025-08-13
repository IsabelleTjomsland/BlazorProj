using System;
using BCrypt.Net;

class Program
{
    static void Main()
    {
        // Hash för admin
        string adminPassword = "admin123";
        string adminHash = BCrypt.Net.BCrypt.HashPassword(adminPassword);
        Console.WriteLine($"Hash för 'admin123':");
        Console.WriteLine(adminHash);

        // Hash för employee
        string isabellePassword = "isabelle123";
        string isabelleHash = BCrypt.Net.BCrypt.HashPassword(isabellePassword);
        Console.WriteLine($"Hash för 'isabelle123':");
        Console.WriteLine(isabelleHash);
    }
}
