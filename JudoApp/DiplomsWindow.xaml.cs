using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Markup;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;
using System.Xml.Linq;

namespace JudoApp
{
    /// <summary>
    /// Логика взаимодействия для DiplomsWindow.xaml
    /// </summary>
    public partial class DiplomsWindow : Window
    {
        public DiplomsWindow(int tatami)
        {
            InitializeComponent();

            var pageTemplate = (DataTemplate)FindResource("PageTemplate");

            // Создаем данные
            using (var db = new JudoDBEntities())
            {
                var dataList = db.Fights.Where(x => x.Participant.Groups.Any(y => y.Tatamy.Id == tatami));
                var winners = dataList
                .Where(fight => fight.WhitePoints.HasValue && fight.RedPoints.HasValue &&
                               fight.WhitePoints != fight.RedPoints)
                .Select(fight => fight.WhitePoints > fight.RedPoints ?
                                fight.Participant :
                                fight.Participant1);

                var winners1 = winners
                       .Include(p => p.Sportsclubs)
                       .Include(p => p.Towns)
                       .Include(p => p.Groups)
                       .ToList()
                       .Select(p => new ParticipantDisplay
                       {
                           Id = p.Id,
                           FIO = p.FIO,
                           Gender = p.Gender,
                           Weight = p.Weight,
                           BirthDate = p.BirthDate,
                           Street = p.Street,
                           Sportsclubs = p.Sportsclubs,
                           Towns = p.Towns,
                           Groups = p.Groups
                       })
                       .ToList();
                var document = XamlDocumentGenerator.CreateDocument(winners1, pageTemplate);
                documentViewer.Document = document;
            }
        }
    }
    public class XamlDocumentGenerator
    {
        public static FixedDocument CreateDocument<T>(IEnumerable<T> items, DataTemplate pageTemplate)
        {
            var document = new FixedDocument();

            foreach (var item in items)
            {
                var pageContent = new PageContent();
                var fixedPage = new FixedPage
                {
                    Width = 793.7,  
                    Height = 1122.5 
                };

                var contentPresenter = new ContentPresenter
                {
                    Content = item,
                    ContentTemplate = pageTemplate
                };

                fixedPage.Children.Add(contentPresenter);

                ((IAddChild)pageContent).AddChild(fixedPage);
                document.Pages.Add(pageContent);
            }

            return document;
        }
    }
}
