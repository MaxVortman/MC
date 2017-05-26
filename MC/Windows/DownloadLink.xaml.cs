using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;
using MahApps.Metro.Controls;
using Microsoft.Win32;
using System.Net;
using System.IO;
using System.Threading;

namespace MC.Windows
{
    /// <summary>
    /// Логика взаимодействия для DownloadLink.xaml
    /// </summary>
    public partial class DownloadLink : MetroWindow
    {
        public DownloadLink(string text, string path)
        {
            InitializeComponent();
            Title = $"{text} style downloading...";
            Path.Text = path;
            switch (text)
            {
                case "Parallel":
                    StartDownload.Click += StartDownloadParallel_Click;
                    break;
                case "Async":
                    StartDownload.Click += StartDownloadAsync_Click;
                    CB.Visibility = Visibility.Collapsed;
                    PB.Visibility = Visibility.Collapsed;
                    break;
                default:
                    break;
            }
        }

        private CancellationTokenSource _wcCTS;
        private static Int64 tempCount;
        private async void StartDownloadAsync_Click(object sender, RoutedEventArgs e)
        {
            if (Link.Text == default(string) || Path.Text == default(string))
            {
                MessageBox.Show("Please pick link, type and path.");
                return;
            }
            var link = Link.Text;
            var path = Path.Text;
            var fileRequest = WebRequest.Create(link);
            var fileResponse = fileRequest.GetResponse();
            var BufferSize = 16384;
            var byteBuffer = new byte[BufferSize];
            _wcCTS = new CancellationTokenSource();
            var wcCT = _wcCTS.Token;
            var ProgressLayout = new Windows.ProgressWindow("Download") { CTS = _wcCTS };
            var progress = new Progress<double>(ProgressLayout.ReportProgress);
            var realProgres = progress as IProgress<double>;
            ProgressLayout.Show();
            await Task.Run(() =>
            {
                using (var fromStream = fileResponse.GetResponseStream())
                {
                    var totalCount = fileResponse.ContentLength;
                    tempCount = 0;
                    using (var inStream = System.IO.File.Open(System.IO.Path.Combine(path, System.IO.Path.GetFileName(link)), FileMode.Create, FileAccess.Write, FileShare.Read))
                    {
                        try
                        {
                            var bytesRead = 0;
                            do
                            {
                                wcCT.ThrowIfCancellationRequested();
                                bytesRead = fromStream.Read(byteBuffer, 0, BufferSize);
                                inStream.Write(byteBuffer, 0, bytesRead);
                                tempCount += bytesRead;
                                realProgres.Report(tempCount * 100 / (double)totalCount);
                            } while (bytesRead > 0);
                        }
                        catch (OperationCanceledException)
                        {
                            MessageBox.Show("Download canceled.");
                        }
                    }
                }
            }, wcCT); 
        }

        private WebClient _wc = new WebClient();
        private CancellationTokenSource cancelToken;
        private void StartDownloadParallel_Click(object sender, RoutedEventArgs e)
        {
            if ((sender as Button).Content.ToString() == "Start")
            {
                if (Link.Text == default(string) || Path.Text == default(string) || CB.Text == default(string))
                {
                    MessageBox.Show("Please pick link, type and path.");
                    return;
                }
                CB.IsEnabled = false;
                _wc = new WebClient();
                _wc.DownloadProgressChanged += (s, eArgs) =>
                {
                    PB.Value = eArgs.ProgressPercentage;
                };
                _wc.DownloadFileCompleted += (s, eArgs) =>
                {
                    MessageBox.Show("Download completed!");                    
                    PB.Value = 0;
                    (sender as Button).Content = "Start";
                    CB.IsEnabled = true;
                };
                var optPar = new ParallelOptions();
                cancelToken = new CancellationTokenSource();
                optPar.CancellationToken = cancelToken.Token;
                Directory.CreateDirectory(Path.Text);
                if (CB.Text == "img")
                {

                    var link = Link.Text;
                    var path = Path.Text;
                    Task.Factory.StartNew(() =>
                     {
                         try
                         {
                             var result = Parallel.ForEach(GetAllImages(link, "img"), optPar, image =>
                             {
                                 optPar.CancellationToken.ThrowIfCancellationRequested();
                                 var _wc = new WebClient();
                                 if (image != null && image.StartsWith("http"))
                                     _wc.DownloadFile(new Uri(image), System.IO.Path.Combine(path, System.IO.Path.GetFileName(image)));                                 
                             });
                             if (result.IsCompleted)
                             {
                                 Dispatcher.Invoke(() =>
                                 {
                                     CB.IsEnabled = true;
                                     (sender as Button).Content = "Start";
                                 });
                             }
                         }
                         catch (OperationCanceledException)
                         {
                             MessageBox.Show("Download canceled.");
                             return;
                         }
                     });

                }
                else
                {
                    _wc.DownloadFileAsync(new Uri(Link.Text), System.IO.Path.Combine(Path.Text, System.IO.Path.GetFileName(Link.Text)));
                }


                (sender as Button).Content = "Cancel";
            }
            else
            {
                if (CB.Text == "img")
                {
                    cancelToken.Cancel();
                    PB.Value = 0;
                    (sender as Button).Content = "Start";
                }
                else
                {
                    _wc.CancelAsync();
                }
            }
        }

        private void View_Click(object sender, RoutedEventArgs e)
        {
            System.Windows.Forms.FolderBrowserDialog folderDialog = new System.Windows.Forms.FolderBrowserDialog();
            if (System.Windows.Forms.DialogResult.OK == folderDialog.ShowDialog())
            {
                Path.Text = folderDialog.SelectedPath;
            }
        }

        private void MetroWindow_Closing(object sender, System.ComponentModel.CancelEventArgs e)
        {
            if (_wc.IsBusy)
            {
                var result = MessageBox.Show("Do you want cancel downloading?", "??", MessageBoxButton.YesNo);
                if (result == MessageBoxResult.Yes)
                {
                    if (CB.Text == "img")
                    {
                        cancelToken.Cancel(); 
                    }
                    else
                    {
                        _wc.CancelAsync();
                    }
                }
            }
        }

        private List<string> GetAllImages(string uri, string type)
        {
            var ImageList = new List<string>();
            // Declaring 'x' as a new WebClient() method
            WebClient x = new WebClient();
            // Setting the URL, then downloading the data from the URL.
            string source = x.DownloadString(uri);
            // Declaring 'document' as new HtmlAgilityPack() method
            HtmlAgilityPack.HtmlDocument document = new HtmlAgilityPack.HtmlDocument();
            // Loading document's source via HtmlAgilityPack
            document.LoadHtml(source);
            // For every tag in the HTML containing the node img.
            foreach (var link in document.DocumentNode
                                 .Descendants(type)
                                 .Select(l => l.Attributes["src"]))
            {
                // Storing all links found in an array. You can declare this however you want.
                ImageList.Add(link?.Value.ToString());
            }
            return ImageList;
        }
    }
}
