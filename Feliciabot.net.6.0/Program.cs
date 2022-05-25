// See https://aka.ms/new-console-template for more information
using Feliciabot.net._6._0;

try
{
    new Main().MainAsync().GetAwaiter().GetResult();
}
catch (FileNotFoundException e)
{
    Console.WriteLine(e.Message);
    Console.ReadLine();
}