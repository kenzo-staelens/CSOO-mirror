using Logica;
using System.Reflection;

internal class Program
{
    private static void Main(string[] args)
    {
        Console.WriteLine("Hello, World!");

        Console.WriteLine(Logica.EnumExtender.getDescriptionOf(commands.Inc));
    }
}