using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Controls;

namespace MineSweeper.Services
{
    internal class GameTimerService
    {
        public async void StartTimer(TextBlock timer)
        {
            int time = 0;
            while (true)
            {
                await Task.Delay(1000);
                time++;
                timer.Text = time.ToString();
            }
        }
    }
}
