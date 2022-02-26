using System;
using System.Threading.Tasks;
using CloudAcademy.BitcoinCoverter.Code;

namespace BitcoinConverter.Client
{
    class Program
    {
        static async Task Main(string[] args)
        {
            var converter = new ConverterSvc();

            while(true)
            {
                try
                {
                    Console.Write("Coins: ");
                    var coins = Double.Parse(Console.ReadLine());
                    Console.Write("Currency (USD, GBP, or EUR): ");
                    var currency = (CloudAcademy.BitcoinCoverter.Code.Currency)Enum.Parse(typeof(CloudAcademy.BitcoinCoverter.Code.Currency), Console.ReadLine(), true);

                    Console.Write(currency);

                    var amount = await converter.ConvertBitcoins(currency, coins);

                    Console.WriteLine(" {0} Bitcoins = {1} {2}", coins, amount, currency);
                }
                catch 
                {
                    //swallow
                }
            }
        }
    }
}
