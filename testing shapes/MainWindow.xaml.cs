using System;
using System.Collections.Generic;
using System.IO;
using System.Text.Json;
using System.Threading;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Media;
using System.Windows.Shapes;

namespace testing_shapes
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        PeriodicTimer periodicTimer = new PeriodicTimer(TimeSpan.FromMilliseconds(100));

    public MainWindow()
        {
            InitializeComponent();
            
        }

        private async void DrawRectangle()
        {
            bool fail = false;
            while (await periodicTimer.WaitForNextTickAsync())
            {
                if (canvas.Children.Count > 0)
                {
                    if (!fail)
                        canvas.Children.Clear();
                }
                Rectangle exampleRectangle = new Rectangle();
                exampleRectangle.Width = 150;
                exampleRectangle.Height = 150;
                // Create a SolidColorBrush and use it to
                // paint the rectangle.
                SolidColorBrush myBrush = new SolidColorBrush(Colors.Blue);
                myBrush.Opacity = 0.5;
                exampleRectangle.Stroke = Brushes.Red;
                exampleRectangle.StrokeThickness = 4;
                exampleRectangle.Fill = myBrush;
                Rectangle newRect = new Rectangle();
                newRect.Width = 150;
                newRect.Height = 150;
                newRect.Fill = myBrush;
                newRect.Stroke = Brushes.Red;
                newRect.StrokeThickness = 4;
                string serializedPolygon;

                try
                {
                    serializedPolygon = File.ReadAllText("rectangles.json");
                }
                catch
                {
                    fail = true;
                    continue;
                }
                fail = false;
                var deserialized = JsonSerializer.Deserialize<List<PolygonForJson>>(serializedPolygon);
                
                List<Polygon> items = new List<Polygon>();
                for (int i = 0; i < deserialized.Count; i++)
                {
                    Polygon polygon = new Polygon
                    {
                        Fill = myBrush,
                        Points = {
                        new Point((double)deserialized[i].JaggedVertices[0][0], (double)deserialized[i].JaggedVertices[0][1]),
                        new Point((double)deserialized[i].JaggedVertices[1][0], (double)deserialized[i].JaggedVertices[1][1]),
                        new Point((double)deserialized[i].JaggedVertices[2][0], (double)deserialized[i].JaggedVertices[2][1]),
                        new Point((double)deserialized[i].JaggedVertices[3][0], (double)deserialized[i].JaggedVertices[3][1]),
                    }
                    };
                    items.Add(polygon);
                }


                MyScale.ScaleY = 10;
                MyScale.ScaleX = 10;
                int n = 0;
                for (int i = 0; i < items.Count; i++)
                {
                    canvas.Children.Insert(n++, items[i]);
                    TextBlock text = new();
                    text.Text = deserialized[i].Name;
                    text.FontSize = 1;
                    text.RenderTransform = new TranslateTransform
                    {
                        X = items[i].Points[0].X,
                        Y = items[i].Points[0].Y,

                    };
                    canvas.Children.Insert(n++, text);
                }
            }

        }

        private void Rectangle_SourceUpdated(object sender, DataTransferEventArgs e)
        {
            DrawRectangle();
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            DrawRectangle();
        }
    }
}
