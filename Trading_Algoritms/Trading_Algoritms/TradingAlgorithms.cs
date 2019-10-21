using System;
using System.Collections.Generic;

namespace Trading_Algorithms
{
  
    public interface ITrading_Algorithms
    {
        void Average(decimal closePrice);
        void Stop();
        void Pause();
        void Resume();
        void Config_Algorithm();
        void BackTest(decimal[] data);
       
    }

    public class SImple_Moving_Average_Algorithm : ITrading_Algorithms
    {
        
        Queue <decimal> _closingPrices;
        int _period;
        decimal closePriceTotal;
        bool buying, selling, paused, stopped;
        ITradingActions _tradingAction;
        
        public SImple_Moving_Average_Algorithm(int period, ITradingActions tradingAction)
        {
            closePriceTotal = 0;// darle el valor de la suma de los datos q estan en la cola
            _period = period;
           // _closingPrices leerlo de la base de datos y hacerlo de tama;o period
            _tradingAction = tradingAction;
            buying = false;
            selling = false;
            paused = false;
        }

        public void Average(decimal closePrice)
        {
            if (_closingPrices.Count < (_period - 1))
                throw new Exception("No hay suficientes datos para calcular SMA");
            closePriceTotal += closePrice;
            _closingPrices.Enqueue(closePrice);
            if (_closingPrices.Count > _period)
            {
                closePriceTotal -= _closingPrices.Dequeue();
            }
            var simpleMovingAverage = closePriceTotal / _closingPrices.Count;
            
            //Round to 2 decimal point
            simpleMovingAverage = decimal.Round(simpleMovingAverage, 2, MidpointRounding.AwayFromZero);
            //habria que guardar algo en base de datos antes de comprar o vender 
            if (!paused)
            {
                if (closePrice > simpleMovingAverage & !buying)
                {
                    _tradingAction.Buy();
                    buying = true;
                    selling = false;

                }
                else if (closePrice < simpleMovingAverage & !selling)
                {
                    _tradingAction.Sell();
                    selling = true;
                    buying = false;

                }
            }
        }
        public void Pause()
        {
            paused = true;
        }

        public void Resume()
        {
            paused = false;
        }

        public void Stop()
        {
            //resetear todo;
        }

        public void BackTest(decimal[] testData)
        {
           decimal closePriceTotalintern = 0;
            for (int i = 0; i < testData.Length; i++)
            {
                closePriceTotalintern += testData[i];
            }
            var simpleMovingAverage = closePriceTotalintern / (testData.Length + 1);
            //Round to 2 decimal point
            simpleMovingAverage = decimal.Round(simpleMovingAverage, 2, MidpointRounding.AwayFromZero);

        }

        public void Config_Algorithm()
        {
            throw new NotImplementedException();
        }

       

       
    }
    public class Exponential_Moving_Average_Algorithm : ITrading_Algorithms
    {

        Queue<decimal> _closingPrices;
        int _period;
        decimal previousEma;
        decimal alpha;
        bool buying, selling, paused;
        ITradingActions _tradingAction;

        public Exponential_Moving_Average_Algorithm(int period, ITradingActions tradingAction)
        {
            previousEma = 0;// darle el valor de la suma de los datos q estan en la cola
            Reset(period);
            // _closingPrices leerlo de la base de datos y hacerlo de tama;o period
            _tradingAction = tradingAction;
            buying = false;
            selling = false;
            paused = false;

        }

        public void Average(decimal closePrice)
        {
            if (_closingPrices.Count < (_period - 1))
                throw new Exception("No hay suficientes datos para calcular SMA");

            _closingPrices.Enqueue(closePrice);
            if (_closingPrices.Count > _period)
            {
                _closingPrices.Dequeue();
            }
            if (previousEma == 0) { CalcularPreviousEmaBySMA(); }
            previousEma = ((decimal)1 - alpha) * previousEma + alpha * closePrice;


            //Round to 2 decimal point
            previousEma = decimal.Round(previousEma, 2, MidpointRounding.AwayFromZero);
            //habria que guardar algo en base de datos antes de comprar o vender 
            if (!paused)
            {
                if (closePrice > previousEma & !buying)
                {
                    _tradingAction.Buy();
                    buying = true;
                    selling = false;

                }
                else if (closePrice < previousEma & !selling)
                {
                    _tradingAction.Sell();
                    selling = true;
                    buying = false;

                }
            }
        }

        private void CalcularPreviousEmaBySMA()
        {
            decimal closePriceTotal = 0;
            foreach (var v in _closingPrices)
            {
                closePriceTotal += v;

            }
            previousEma = closePriceTotal / _closingPrices.Count;

            //Round to 2 decimal point
            previousEma = decimal.Round(previousEma, 2, MidpointRounding.AwayFromZero);
            //habria que guardar algo en base de datos antes de comprar o vender 
        }

        public void Pause()
        {
            paused = true;
        }

        public void Resume()
        {
            paused = false;
        }

        public void Stop()
        {
            //resetear todo;
        }
        public void BackTest(decimal[] testData)
        {
            throw new NotImplementedException();
        }

        public void Config_Algorithm()
        {
            throw new NotImplementedException();
        }

        public void Reset(int period = 10)
        {
            if (period <= 0)
                throw new ArgumentException("The period must be a positive integer", "period");
            _period = period;
            alpha = (decimal)2 / ((decimal)_period + (decimal)1);
           
        }
    }

   
}
