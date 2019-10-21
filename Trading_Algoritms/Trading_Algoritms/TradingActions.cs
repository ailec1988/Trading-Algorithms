using System;

namespace Trading_Algorithms
{
    public interface ITradingActions {
        void Buy();
        void Sell();
    }
    public class TradingActions:ITradingActions
    {
        public void Buy()
        {
            Console.WriteLine("Buying");
        }

        public void Sell()
        {
            Console.WriteLine("Selling");
        }
    }
}