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
using MC.Source.Entries;

namespace MC.Windows
{
    /// <summary>
    /// Логика взаимодействия для DialogThreadPage.xaml
    /// </summary>
    public partial class DialogThreadPage : Page
    {
        private readonly myAction action;
        private readonly Entity entity;

        public DialogThreadPage(myAction action, Entity entity)
        {
            InitializeComponent();
            this.action = action;
            this.entity = entity;
        }

        private void TypeButton_Click(object sender, RoutedEventArgs e)
        {
            var btn = sender as Button;
            var patternDialog = new DialogPatternPage(action, entity, btn.Content as string);
            NavigationService.Navigate(patternDialog);
        }
    }
}
