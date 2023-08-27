using System.Collections.Generic;
using System.IO;
using System.Text.Json;
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
        public MainWindow()
        {
            InitializeComponent();
            DrawRectangle();
        }

        private async void DrawRectangle()
        {
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
            /*canvas.Children.Insert(0, exampleRectangle);
            canvas.Children.Insert(1, newRect);*/
            string serializedPolygon = File.ReadAllText("rectangles.json");
            var deserialized = JsonSerializer.Deserialize<List<PolygonForJson>>(serializedPolygon);
            Polygon polygon = new Polygon
            {
                Fill = myBrush,
                Points = {
                new Point((double)deserialized[0].JaggedVertices[0][0], (double)deserialized[0].JaggedVertices[0][1]),
                new Point((double)deserialized[0].JaggedVertices[1][0], (double)deserialized[0].JaggedVertices[1][1]),
                new Point((double)deserialized[0].JaggedVertices[2][0], (double)deserialized[0].JaggedVertices[2][1]),
                new Point((double)deserialized[0].JaggedVertices[3][0], (double)deserialized[0].JaggedVertices[3][1]),
                }
            };
            Polygon polygon3 = new Polygon
            {
                Fill = myBrush,
                Points = {
                new Point((double)deserialized[0].JaggedVertices[0][0] +1, (double)deserialized[0].JaggedVertices[0][1]),
                new Point((double)deserialized[0].JaggedVertices[1][0] + 1, (double)deserialized[0].JaggedVertices[1][1]),
                new Point((double)deserialized[0].JaggedVertices[2][0] + 1, (double)deserialized[0].JaggedVertices[2][1]),
                new Point((double)deserialized[0].JaggedVertices[3][0] + 1, (double)deserialized[0].JaggedVertices[3][1]),
                }
            };

            Polygon polygon2 = new Polygon
            {
                Fill = myBrush,
                Points = {
                new Point(200, 200),
                new Point(300, 200.5),
                new Point(300, 300),
                new Point(200, 300),
                }
            };

            MyScale.ScaleY = 30;
            MyScale.ScaleX = 30;
            canvas.Children.Insert(0, polygon);

            TextBlock text = new();
            text.Text = deserialized[0].Name;
            text.FontSize = 1;
            text.RenderTransform = new TranslateTransform
            {
                X = polygon.Points[0].X,
                Y = polygon.Points[0].Y,

            };
            TextBlock text2 = new();
            text2.Text = deserialized[0].Name;
            text2.FontSize = 1;
            text2.RenderTransform = new TranslateTransform
            {
                X = polygon3.Points[0].X,
                Y = polygon3.Points[0].Y,

            };
            canvas.Children.Insert(1, text);
            canvas.Children.Insert(2, polygon3);
            canvas.Children.Insert(3, text2);
        }

        private void Rectangle_SourceUpdated(object sender, DataTransferEventArgs e)
        {
        }
    }
}
