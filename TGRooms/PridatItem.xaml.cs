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

namespace TGRooms
{
    /// <summary>
    /// Interaction logic for PridatItem.xaml
    /// </summary>
    public partial class PridatItem : Window
    {
        public PridatItem()
        {
            InitializeComponent();
            using (var connection = new tgtables.TgtablesContext())
            {
                var query = from spravce in connection.Rooms
                            select spravce;
                foreach (var item in query)
                {
                    spravce_combo.Items.Add(item.Name);
                }
            }
        }
        private void Pridat_Item(object? e, RoutedEventArgs? args)
        {
            using (var connection = new tgtables.TgtablesContext())
            {
                var item = new tgtables.Item
                {
                    Name = name_box.Text,
                    Acquired = DateTime.Now,
                    Sold = null
                };
                connection.Items.Add(item);
                connection.SaveChanges();
                var Itemroom = new tgtables.Itemroom
                {
                    Item = item.Id,
                    Room = connection.Rooms.First(r => r.Name == spravce_combo.SelectedValue.ToString()).Id
                };
                connection.Itemrooms.Add(Itemroom);
                var Itemval = new tgtables.Itemvalue
                {
                    ItemId = item.Id,
                    Value = int.Parse(value_box.Text)
                };
                connection.Itemvalues.Add(Itemval);
                connection.SaveChanges();
            }
            Close();
        }

        
    }
}
