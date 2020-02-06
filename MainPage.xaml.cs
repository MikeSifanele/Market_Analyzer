using Syncfusion.UI.Xaml.Charts;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using System.Threading.Tasks;
using Windows.ApplicationModel.DataTransfer;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.Storage;
using Windows.UI;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Navigation;

// The Blank Page item template is documented at https://go.microsoft.com/fwlink/?LinkId=402352&clcid=0x409

namespace MarketAnalyzer
{
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class MainPage : Page
    {
        private int NumberOfCandles = 0;

        private string[] readRow;

        private int StartIndex = 0;

        private string DefaultSaveLocation = string.Empty;

        public List<CandleStick> CandleSticks { get; set; }

        public MainPage()
        {
            this.InitializeComponent();

            this.DataContext = CandleSticks;

            NumberOfCandles = 50;            

            InitializeChartSeries();
        }

        private void Page_Loaded(object sender, RoutedEventArgs e)
        {

        }

        private async void MainChart_Drop(object sender, DragEventArgs e)
        {
            if (e.DataView.Contains(StandardDataFormats.StorageItems))
            {
                var items = await e.DataView.GetStorageItemsAsync();
                if (items.Count > 0)
                {
                    await LoadCandleSticksFromFile(items[0] as StorageFile);
                }
            }
        }

        private async Task LoadCandleSticksFromFile(StorageFile storageFile)
        {
            try
            {
                CandleSticks = new List<CandleStick>();

                var readRows = await FileIO.ReadLinesAsync(storageFile);

                for (var rowIndex = 0; rowIndex < readRows.Count; rowIndex++)
                {
                    readRow = readRows[rowIndex].Split(",");

                    CandleSticks.Add(new CandleStick
                    {                                 // Year                                // Month                               // Day                                 //Hour                               // Minutes                           // Seconds
                        TimeSpan = new DateTime(int.Parse(readRow[0].Substring(0, 4)), int.Parse(readRow[0].Substring(4, 2)), int.Parse(readRow[0].Substring(6, 2)), int.Parse(readRow[1].Split(':')[0]), int.Parse(readRow[1].Split(':')[1]), int.Parse(readRow[1].Split(':')[2])),
                        Open = Convert.ToSingle(readRow[2]),
                        High = Convert.ToSingle(readRow[3]),
                        Low = Convert.ToSingle(readRow[4]),
                        Close = Convert.ToSingle(readRow[5])
                    });
                }

                StartIndex = CandleSticks.Count - NumberOfCandles;

                MainChart.Series["CandleStickSeries"].ItemsSource = CandleSticks.Skip(StartIndex).Take(NumberOfCandles);

                CalculateMovingAvarages();
            }
            catch
            {

            }
        }

        private void InitializeChartSeries()
        {
            CandleSeries series = new CandleSeries()
            {
                Name = "CandleStickSeries",

                ItemsSource = CandleSticks,

                XBindingPath = "TimeSpan",

                High = "High",
                Low = "Low",

                Open = "Open",
                Close = "Close",

                Background = new SolidColorBrush(Colors.White),

                ShowTooltip = true,

                BearFillColor = new SolidColorBrush(Colors.Red),

                BullFillColor = new SolidColorBrush(Colors.Lime)
            };

            MainChart.Series.Add(series);
        }

        private void MainChart_DragOver(object sender, DragEventArgs e)
        {
            e.AcceptedOperation = DataPackageOperation.Copy;
        }

        private void PreviousButton_Tapped(object sender, TappedRoutedEventArgs e)
        {
            try
            {
                if (StartIndex - NumberOfCandles > 0)
                {
                    MainChart.Series["CandleStickSeries"].ItemsSource = CandleSticks.Skip(StartIndex--).Take(NumberOfCandles);
                }
            }
            catch
            {

            }
        }

        private void NextButton_Tapped(object sender, TappedRoutedEventArgs e)
        {
            try
            {
                if (StartIndex + NumberOfCandles < CandleSticks.Count - 1)
                {
                    MainChart.Series["CandleStickSeries"].ItemsSource = CandleSticks.Skip(StartIndex++).Take(NumberOfCandles);
                }
            }
            catch
            {

            }
        }
    }

    public struct CandleStick
    {
        public DateTime TimeSpan { get; set; }
        public float Open { get; set; }
        public float High { get; set; }
        public float Low { get; set; }
        public float Close { get; set; }
    }
}
