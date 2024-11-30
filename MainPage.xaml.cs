
using Microsoft.Maui.Controls.Shapes;
using Microsoft.Maui.Handlers;
using System.Threading;
using System.Threading.Tasks;
using static System.Runtime.InteropServices.JavaScript.JSType;

namespace clock
{
    public partial class MainPage : ContentPage
    {
        /*        3p
              --------     
             |       /|
           0p|    2p/ |7p
             |     /  |
              ---4p--- 
             |   /    |
           1p|  /5p   |8p
             | /      |
              --------
                 6р 
         
         */
        Dictionary<int, ushort> numbersMasks = new Dictionary<int, ushort>
        {
            { 0, 0b111101111},
            { 1, 0b110000000},
            { 2, 0b011011010},
            { 3, 0b111011000},
            { 4, 0b110010001},
            { 5, 0b101011001},
            { 6, 0b101011011},
            { 7, 0b000101100},
            { 8, 0b111011011},
            { 9, 0b111011001},
            

        };
        Dictionary<char, long> halfMask = new Dictionary<char, long>
        {
            {'A', 0b110011011},
            {'P', 0b010011011},
        };

        public enum Frames { hour_S, hour_J, minute_S, minute_J, second_S, second_J, half_S, half_J }
        TimeOnly currentTime = TimeOnly.FromDateTime(DateTime.Now);
        Dictionary<Frames, Grid> numbersGrids;

        private PeriodicTimer _timer;
        private CancellationTokenSource _cancellationTokenSource;
        public MainPage()
        {
            InitializeComponent();
            

            numbersGrids = new Dictionary<Frames, Grid> {
                {Frames.hour_S, HoursDisplayS },
                {Frames.hour_J, HoursDisplayJ },
                {Frames.minute_S, MinutesDisplayS },
                {Frames.minute_J, MinutesDisplayJ },
                {Frames.second_S, SecondsDisplayS },
                {Frames.second_J, SecondsDisplayJ },
                {Frames.half_S, HalfDisplayS },
                {Frames.half_J, HalfDisplayJ },
                };
            
            

            

            startClockAsync();
        }

        public Polygon dots()
        {
            Polygon polygon = new Polygon();
            polygon.Points.Add(new Point(6, 0));
            polygon.Points.Add(new Point(14, 0));
            polygon.Points.Add(new Point(20, 6));
            polygon.Points.Add(new Point(20, 14));
            polygon.Points.Add(new Point(14, 20));
            polygon.Points.Add(new Point(6, 20));
            polygon.Points.Add(new Point(0, 14));
            polygon.Points.Add(new Point(0, 6));
            polygon.Fill = Colors.Red;
            polygon.WidthRequest = 20;
            polygon.HeightRequest = 20;
            return polygon;
        }
        public void updateTime()
        {
            currentTime = TimeOnly.FromDateTime(DateTime.Now);
            write();
        }

/* Необъединенное слияние из проекта "clock (net8.0-maccatalyst)"
До:
        public void startClock()
        {
После:
        public void startClockAsync()
        {
*/
        public async Task startClockAsync()
        {
            

            currentTime = TimeOnly.FromDateTime(DateTime.Now);
            initializeTimeFrame(Frames.hour_S);
            initializeTimeFrame(Frames.hour_J);
            Grid grid = Dots1;
            grid.Add(dots(), column: 0, row: 0);
            grid.Add(dots(), column: 0, row: 2);
            initializeTimeFrame(Frames.minute_S);
            initializeTimeFrame(Frames.minute_J);
            grid = Dots2;
            grid.Add(dots(), column: 0, row: 0);
            grid.Add(dots(), column: 0, row: 2);
            initializeTimeFrame(Frames.second_S);
            initializeTimeFrame(Frames.second_J);
            initializeTimeFrame(Frames.half_S);
            InitializeM(Frames.half_J);

            _timer = new PeriodicTimer(TimeSpan.FromSeconds(1));
            _cancellationTokenSource = new CancellationTokenSource();

            try
            {
                while (await _timer.WaitForNextTickAsync(_cancellationTokenSource.Token))
                {
                    updateTime();
                }
            }
            catch (OperationCanceledException)
            {
                Console.WriteLine("Timer canceled.");
            }

        }

        public Polygon signM()
        {
            Polygon polygon = new Polygon();
            polygon.Points.Add(new Point(0,10));
            polygon.Points.Add(new Point(0,0));
            polygon.Points.Add(new Point(10,0));
            polygon.Points.Add(new Point(40,70));
            polygon.Points.Add(new Point(70,0));
            polygon.Points.Add(new Point(80,0));
            polygon.Points.Add(new Point(80,10));
            polygon.Points.Add(new Point(45,80));
            polygon.Points.Add(new Point(35,80));

            polygon.Fill = Colors.Red;
            polygon.WidthRequest = 80;
            polygon.HeightRequest = 80;
            return polygon;
        }

        public void InitializeM(Frames gridId)
        {
            Grid grid = numbersGrids[gridId];
            grid.Add(vertical(), column: 0, row: 1);
            grid.Add(vertical(), column: 0, row: 3);
            grid.Add(signM(), column: 1, row: 1);
            grid.Add(vertical(), column: 2, row: 1);
            grid.Add(vertical(), column: 2, row: 3);

        }

        public Polygon horizontal(bool on = true)
        {
            int x = 80, y = 10;
            Polygon polygon = new Polygon();
            polygon.Points.Add(new Point(x-70, 0));
            polygon.Points.Add(new Point(0,y/2));
            polygon.Points.Add(new Point(x-70, y));
            polygon.Points.Add(new Point(x-10,y));
            polygon.Points.Add(new Point(x, y/2));
            polygon.Points.Add(new Point(x-10, 0));
            polygon.Fill = Colors.Red;
            polygon.WidthRequest = x;
            polygon.HeightRequest = y;
            return polygon; 
        }

        public Polygon vertical(bool on = true)
        {
            int x = 10; int y = 80;
            Polygon polygon = new Polygon();
            polygon.Points.Add(new Point(0, y-70));
            polygon.Points.Add(new Point(x/2, 0));
            polygon.Points.Add(new Point(x, y-70));
            polygon.Points.Add(new Point(x, y-10));
            polygon.Points.Add(new Point(x/2, y));
            polygon.Points.Add(new Point(0, y-10));
            polygon.Fill = Colors.Red;
            polygon.WidthRequest = x;
            polygon.HeightRequest = y;
            return polygon;
        }

        public Polygon diagonal1(bool on = true) 
        {
            Polygon polygon = new Polygon();
            polygon.Points.Add(new Point(70,0));
            polygon.Points.Add(new Point(80,0));
            polygon.Points.Add(new Point(80,10));
            polygon.Points.Add(new Point(80/2+10,80));
            polygon.Points.Add(new Point(80/2,80));
            polygon.Points.Add(new Point(80/2,70));
            polygon.Fill = Colors.Red;
            polygon.WidthRequest = 80;
            polygon.HeightRequest = 80;
            return polygon;
        }       
        public Polygon diagonal2(bool on = true) 
        {
            Polygon polygon = new Polygon();
            polygon.Points.Add(new Point(40,0));
            polygon.Points.Add(new Point(40,10));
            polygon.Points.Add(new Point(10,80));
            polygon.Points.Add(new Point(0,80));
            polygon.Points.Add(new Point(0,70));
            polygon.Points.Add(new Point(30,0));
            polygon.Fill = Colors.Red;
            polygon.WidthRequest = 80;
            polygon.HeightRequest = 80;
            return polygon;
        }
        public void initializeTimeFrame(Frames gridId)
        {
            Grid grid = numbersGrids[gridId];
            grid.Add(vertical(), column:0, row:1);
            grid.Add(vertical(), column:0, row:3);
            grid.Add(diagonal1(), column:1, row:1);
            grid.Add(horizontal(), column:1, row:0);
            grid.Add(horizontal(), column:1, row:2);
            grid.Add(diagonal2(), column: 1, row: 3);
            grid.Add(horizontal(), column:1, row:4);
            grid.Add(vertical(), column: 2, row: 1);
            grid.Add(vertical(), column: 2, row: 3);
        }

        public void checkHalf(bool past, Frames gridId)
        {
            var binDigit = halfMask['A'];
            if (past) { binDigit = halfMask['P']; }
            var mask = 0b000000001;
            Grid grid = numbersGrids[gridId];

            for (int i = 0; i < 9; i++)
            {
                ((Polygon)grid.Children[i]).IsVisible = Convert.ToBoolean(mask & binDigit);
                mask = (mask <<= 1);
            }
        }
        public void write()
        {
            bool past = false;
            int half = currentTime.Hour;
            if (currentTime.Hour / 12 == 1)
            {
                half = currentTime.Hour % 12;
                past = true;
            }
            
            writeTime(half / 10, Frames.hour_S);
            writeTime(half % 10, Frames.hour_J);
            writeTime(currentTime.Minute / 10, Frames.minute_S);
            writeTime(currentTime.Minute % 10, Frames.minute_J);
            writeTime(currentTime.Second / 10, Frames.second_S);
            writeTime(currentTime.Second % 10, Frames.second_J);
            checkHalf(past, Frames.half_S);
        }
        public void writeTime(int digit, Frames gridId)
        {
            var binDigit = numbersMasks[digit];
            var mask = 0b000000001;
            Grid grid = numbersGrids[gridId];
            
            for (int i = 0; i < 9; i++)
            {
                ((Polygon)grid.Children[i]).IsVisible = Convert.ToBoolean(mask & binDigit);
                mask = (mask <<= 1);
            }
            

        }
    }

}
