using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Text;
using System.Text.RegularExpressions;
using System.Windows;
using MC.Source.Threading;
using Microsoft.Win32;

namespace MC.Source.Searchers
{
    public class SearchByPattern : IThreder
    {
        private Queue<string>[] filesQueue;

        private FileQueueCreator fileQueueCreator;

        public SearchByPattern(string sourceFilePath, ISearcher searcher)
        {
            this.sourceFilePath = sourceFilePath;
            this.searcher = searcher;
            fileQueueCreator = new FileQueueCreator(sourceFilePath);
            filesQueue = fileQueueCreator.GetFilledQueueOfFilesPath();
        }

        public void DoThread()
        {
            SearchInThread(searcher.Search(filesQueue, SearchAndSaveIn));
        }

        private void SearchInThread(ThreadProcess StartThread)
        {
            var fileDialog = new SaveFileDialog
            {
                Filter = "All Files | *.* ",
                AddExtension = true,
                DefaultExt = "txt"
            };
            //getting full file name, where we'll save the txt
            if (fileDialog.ShowDialog() == true)
            {
                _passport = new List<Group>();
                _number = new List<Group>();
                _tin = new List<Group>();
                _pin = new List<Group>();
                _email = new List<Group>();
                _ftp = new List<Group>();
                _vk = new List<Group>();
                _exeptions = new StringBuilder();
                var messageBoxResult = System.Windows.MessageBox.Show("Would you like to open the file after the search is complete?",
                        "Are you sure?", System.Windows.MessageBoxButton.YesNo);
                var fileName = fileDialog.FileName;
                StartThread(() =>
                {
                    using (var writer = new StreamWriter(fileName))
                    {
                        writer.Write(WriteInStream().ToString());
                    }
                    MessageBox.Show(_exeptions.ToString(),
                            "Attention!", MessageBoxButton.OK);
                    if (messageBoxResult == MessageBoxResult.Yes)
                    {
                        try
                        {
                            Process.Start(fileName);
                        }
                        catch (Exception e)
                        {
                            MessageBox.Show(e.Message, "Error!", MessageBoxButton.OK, MessageBoxImage.Error);
                        }
                    }
                });
            }
        }

        private StringBuilder WriteInStream()
        {
            var sb = new StringBuilder();
            sb.Append("Passport \r\n \r\n");
            for (int i = 0; i < _passport.Count; i++)
                sb.Append($"{_passport[i]} \r\n");
            sb.Append("Number \r\n \r\n");
            for (int i = 0; i < _number.Count; i++)
                sb.Append($"{_number[i]} \r\n");
            sb.Append("TIN \r\n \r\n");
            for (int i = 0; i < _tin.Count; i++)
                sb.Append($"{_tin[i]} \r\n");
            sb.Append("PIN \r\n \r\n");
            for (int i = 0; i < _pin.Count; i++)
                sb.Append($"{_pin[i]} \r\n");
            sb.Append("Email \r\n \r\n");
            for (int i = 0; i < _email.Count; i++)
                sb.Append($"{_email[i]} \r\n");
            sb.Append("FTP \r\n \r\n");
            for (int i = 0; i < _ftp.Count; i++)
                sb.Append($"{_ftp[i]} \r\n");
            sb.Append("VK \r\n \r\n");
            for (int i = 0; i < _vk.Count; i++)
                sb.Append($"{_vk[i]} \r\n");
            return sb;
        }

        private const string Filter = @"(?x)(?i)(?m)(?'passport'\b\d{2}\s\d{2}\s\d{4}\b)|
                                    (?'number'(?<=\s)\+?[78]\s ?\d{3}\s?\d{3}[\s-]?\d{2}[\s-]?\d{2}\b)|
                                    (?'TIN'\b\d{12}\b)|
                                    (?'PIN'\b\d{3}\-\d{3}\-\d{3}\s\d{2}\b)|
                                    (?'email'\b(\w|\p{P})+(?=@)(\w|\p{P})\w+\.\w+\b)|
                                    (?'ftp'\bftps?:\/\/?(\w|\p{P})*\b)|
                                    (?'vk'\b(https?:\/\/)?vk\.com\/?(\w|\p{P})*\b)";
        private readonly Regex Regex = new Regex(Filter);
        private List<Group> _passport = new List<Group>();
        private List<Group> _number = new List<Group>();
        private List<Group> _tin = new List<Group>();
        private List<Group> _pin = new List<Group>();
        private List<Group> _email = new List<Group>();
        private List<Group> _ftp = new List<Group>();
        private List<Group> _vk = new List<Group>();
        private StringBuilder _exeptions = new StringBuilder();
        protected readonly string sourceFilePath;
        private readonly ISearcher searcher;

        private void SearchAndSaveIn(string filePath)
        {
            try
            {
                var text = "";
                try
                {
                    using (var reader = new StreamReader(filePath))
                    {
                        text = reader.ReadToEnd();
                    }
                }
                catch (Exception e)
                {
                    _exeptions.Append($"{filePath} is not read, because: {e.Message}\n");
                }
                foreach (Match match in Regex.Matches(text))
                {
                    var group = match.Groups["passport"];
                    if (@group.ToString() != "")
                    {
                        _passport.Add(@group);
                    }
                    @group = match.Groups["number"];
                    if (@group.ToString() != "")
                    {
                        _number.Add(@group);
                    }
                    @group = match.Groups["TIN"];
                    if (@group.ToString() != "")
                    {
                        _tin.Add(@group);
                    }
                    @group = match.Groups["PIN"];
                    if (@group.ToString() != "")
                    {
                        _pin.Add(@group);
                    }
                    @group = match.Groups["email"];
                    if (@group.ToString() != "")
                    {
                        _email.Add(@group);
                    }
                    @group = match.Groups["ftp"];
                    if (@group.ToString() != "")
                    {
                        _ftp.Add(@group);
                    }
                    @group = match.Groups["vk"];
                    if (@group.ToString() != "")
                    {
                        _vk.Add(@group);
                    }
                }
            }

            catch (UnauthorizedAccessException e)
            {
                _exeptions.Append($"{filePath} is not read, because: {e.Message}\n");
            }
        }
    }
}
