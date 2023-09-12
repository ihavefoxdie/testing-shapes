using System;
using System.Collections.Generic;
using System.IO;
using System.Text.Json;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Shapes;

namespace testing_shapes
{
    /// <summary>
    /// Interaction logic for Polygons.xaml
    /// </summary>
    public partial class Polygons : Page
    {
        double _scale = 1.25;
        bool KeepUp = false;
        Task? _task;
        CancellationTokenSource cancelTokenSource;
        CancellationToken token;
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

        string PausePlayTitle
        {
            get
            {
                if (KeepUp)
                {
                    return "Pause";
                }
                return "Play";
            }
        }

        public Polygons()
        {
            InitializeComponent();
            cancelTokenSource = new CancellationTokenSource();
            token = cancelTokenSource.Token;
            Pause_Play.Content = PausePlayTitle;
        }

        private async Task AsyncDrawRectangle()
        {
            Polygon polygon = new();
            List<Polygon> items = new List<Polygon>();
            bool fail = true;
            SolidColorBrush myBrush = new SolidColorBrush(Colors.MediumTurquoise);
            myBrush.Opacity = 0.5;
            SolidColorBrush backroundBrush = new SolidColorBrush(Colors.LightGray);
            backroundBrush.Opacity = 0.2;
            SolidColorBrush polyColor = myBrush;

            _task = Task.Run(() =>
            {
                string oldSerializedPolygon = "";
                double olderScale = Scale;
                while (KeepUp)
                {
                    Thread.Sleep(10);
                    string serializedPolygon;
                    fail = false;

                    try
                    {
                        serializedPolygon = File.ReadAllText("rectangles.json");
                    }
                    catch
                    {
                        fail = true;
                        continue;
                    }

                    if (serializedPolygon == oldSerializedPolygon && olderScale == Scale)
                    {
                        fail = true;
                        continue;
                    }
                    olderScale = Scale;
                    oldSerializedPolygon = serializedPolygon;

                    fail = false;
                    var deserialized = JsonSerializer.Deserialize<List<PolygonForJson>>(serializedPolygon);
                    if (deserialized == null)
                    {
                        fail = true;
                        continue;
                    }

                    items = new List<Polygon>();

                    for (int i = 0; i < deserialized.Count; i++)
                    {
                        if (deserialized[i] == null)
                            continue;

                        if (deserialized[i].JaggedVertices == null)
                            continue;

                        if (deserialized[i].JaggedVertices.GetLength(0) != 4)
                            break;

                        if (deserialized[i].Name == "")
                        {
                            polyColor = backroundBrush;
                        }
                        else
                            polyColor = myBrush;
                        this.Dispatcher.Invoke(() =>
                        {
                            polygon = new Polygon
                            {
                                Fill = polyColor,
                                Points = {
                                    new Point((double)deserialized[i].JaggedVertices![0][0], (double)deserialized[i].JaggedVertices![0][1]),
                                    new Point((double)deserialized[i].JaggedVertices![1][0], (double)deserialized[i].JaggedVertices![1][1]),
                                    new Point((double)deserialized[i].JaggedVertices![2][0], (double)deserialized[i].JaggedVertices![2][1]),
                                    new Point((double)deserialized[i].JaggedVertices![3][0], (double)deserialized[i].JaggedVertices![3][1]),
                                }
                            };
                            items.Add(polygon);
                        });
                    }

                    this.Dispatcher.Invoke(() =>
                    {
                        MyScale.ScaleY = 10 * Scale;
                        MyScale.ScaleX = 10 * Scale;

                        if (canvas.Children.Count > 0)
                        {
                            if (!fail)
                                canvas.Children.Clear();
                        }
                    });

                    for (int i = 0; i < items.Count; i++)
                    {
                        this.Dispatcher.Invoke(() =>
                        {
                            TextBlock text = new();
                            text.Opacity = 0.7;
                            text.FontFamily = HomeButton.FontFamily;
                            text.Text = deserialized[i].Name;
                            text.FontSize = 1;
                            text.RenderTransform = new TranslateTransform
                            {
                                X = items[i].Points[0].X,
                                Y = items[i].Points[0].Y,
                            };
                            canvas.Children.Insert(canvas.Children.Count, items[i]);
                            canvas.Children.Insert(canvas.Children.Count, text);
                        });
                    }
                    fail = false;
                }
            }, token);

            await _task;
        }


        private void Deflate_Click(object sender, RoutedEventArgs e)
        {
            Scale -= 0.25;
        }

        private void Enlarge_Click(object sender, RoutedEventArgs e)
        {
            Scale += 0.25;
        }

        private void Reload_Click(object sender, RoutedEventArgs e)
        {
            StopRendering();
        }

        private void StopRendering()
        {
            Cancel();
            canvas.Children.Clear();
            Pause_Play.Content = PausePlayTitle;
        }

        private void Cancel()
        {
            if (!cancelTokenSource.IsCancellationRequested)
            {
                KeepUp = false;
                Pause_Play.Content = PausePlayTitle;
                cancelTokenSource.Cancel();
                cancelTokenSource.Dispose();
                if (_task != null && (_task.IsCanceled || _task.IsCompleted || _task.IsCompletedSuccessfully || _task.IsFaulted))
                    _task.Dispose();
            }
        }

        private async void Pause_Play_Click(object sender, RoutedEventArgs e)
        {
            if (!KeepUp)
            {
                KeepUp = true;
                cancelTokenSource = new CancellationTokenSource();
                token = cancelTokenSource.Token;
                Pause_Play.Content = PausePlayTitle;
                await AsyncDrawRectangle();
            }
            else
                Cancel();
        }

        private void HomeButton_Click(object sender, RoutedEventArgs e)
        {
            StopRendering();
            this.NavigationService.Navigate(new Uri("Menu.xaml", UriKind.Relative));
        }
    }
}
