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
using System.Windows.Navigation;
using System.Windows.Shapes;
using MC.Source.Archivers;
using MC.Source.Entries;
using MC.Source.Searchers;
using MC.Source.Visitors;
using MC.Source.Visitors.ThreadVisitors;

namespace MC.Windows
{
    /// <summary>
    /// Логика взаимодействия для DialogPatternPage.xaml
    /// </summary>
    public partial class DialogPatternPage : Page
    {
        private readonly Action action;
        private readonly Entity entity;
        private readonly string typeOfThread;

        public DialogPatternPage(Action action, Entity entity, string typeOfThread)
        {
            InitializeComponent();
            this.action = action;
            this.entity = entity;
            this.typeOfThread = typeOfThread;
        }

        private void TypeButton_Click(object sender, RoutedEventArgs e)
        {
            Button btn = sender as Button;
            switch (btn.Content)
            {
                case "Strategy":
                    UseStrategy();
                    break;
                case "Visitor":
                    UseVisitor();
                    break;
                default:
                    break;
            }
        }

        private void UseVisitor()
        {
            switch (action)
            {
                case Action.Archive:
                    entity.AcceptArchive(ThreadsVisitorFactory.CreateVisitor(typeOfThread));
                    break;
                case Action.Search:
                    entity.AcceptSearch(ThreadsVisitorFactory.CreateVisitor(typeOfThread));
                    break;
                default:
                    break;
            }
        }

        private void UseStrategy()
        {
            switch (action)
            {
                case Action.Archive:
                    new ArchiveFactory().CreateObject(typeOfThread, entity.Path).DoThread();
                    break;
                case Action.Search:
                    new SearchFactory().CreateObject(typeOfThread, entity.Path).DoThread();
                    break;
                default:
                    break;
            }
        }
    }
}
