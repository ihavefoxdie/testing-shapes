using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.IO;
using System.Text.Json;
using System.Threading;
using System.Windows;
using System.Windows.Controls;
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
        double _scale = 1;
        double Scale
        {
            get
            {
                return _scale;
            }
            set
            {
                if (value <= 0)
                {
                    return;
                }
                _scale = value;
            }
        }

        public MainWindow()
        {
            InitializeComponent();
            Enlarge.IsEnabled = false;
            Enlarge.Opacity = 0;
            Deflate.IsEnabled = false;
            Deflate.Opacity = 0;
        }

        private async void DrawRectangle()
        {
            bool fail = false;
            while (await periodicTimer.WaitForNextTickAsync())
            {
                MyScale.ScaleY = 10 * Scale;
                MyScale.ScaleX = 10 * Scale;

                if (canvas.Children.Count > 0)
                {
                    if (!fail)
                        canvas.Children.Clear();
                }

                SolidColorBrush myBrush = new SolidColorBrush(Colors.Blue);
                myBrush.Opacity = 0.5;

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

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            Run_Button.IsEnabled = false;
            Run_Button.Opacity = 0;
            DrawRectangle();
            Enlarge.IsEnabled = true;
            Enlarge.Opacity = 1;
            Deflate.IsEnabled = true;
            Deflate.Opacity = 1;
        }

        private void Deflate_Click(object sender, RoutedEventArgs e)
        {
            Scale -= 0.25;
        }

        private void Enlarge_Click(object sender, RoutedEventArgs e)
        {
            Scale += 0.25;
        }
    }
}
