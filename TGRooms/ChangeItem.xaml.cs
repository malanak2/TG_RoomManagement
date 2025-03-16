using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices.Marshalling;
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
using TGRooms.tgtables;

namespace TGRooms
{
    /// <summary>
    /// Interaction logic for ChangeItem.xaml
    /// </summary>
    public partial class ChangeItem : Window
    {
        tgtables.Item item;
        public ChangeItem(tgtables.Item? i)
        {
            InitializeComponent();
            if (i == null) return;
            item = i;
            using (var connection = new tgtables.TgtablesContext())
            {
                var query = from spravce in connection.Rooms
                            select spravce;
                int s = connection.Itemrooms.Where(ir => ir.Id == item.Id && ir.ValidTo == null).ToList().FirstOrDefault().Room;
                int selindex = 0;
                int c = -1;
                foreach (var item in query)
                {
                    c++;
                    if (item.Id == s) selindex = c;
                    room_combo.Items.Add(item.Name);
                }
                room_combo.SelectedIndex = selindex;
                value_box.Text = connection.Itemvalues.First(iv => iv.ItemId == item.Id && iv.ValidTo == null).Value.ToString();
            }
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            using (var connection = new tgtables.TgtablesContext())
            {
                var item = connection.Items.First(i => i.Id == this.item.Id);
                var oldItemRoom = connection.Itemrooms.First(ir => ir.Item == item.Id);
                oldItemRoom.ValidTo = DateTime.Now;
                var itemroom = new Itemroom();//
                itemroom.Item = item.Id;
                itemroom.ValidFrom = DateTime.Now;
                itemroom.ValidTo = null;
                itemroom.Room = connection.Rooms.First(r => r.Name == room_combo.SelectedValue.ToString()).Id;
                connection.Itemrooms.Add(itemroom);
                connection.SaveChanges();
                var itemval = connection.Itemvalues.First(iv => iv.ItemId == item.Id && iv.ValidTo == null);
                itemval.ValidTo = DateTime.Now;
                connection.SaveChanges();
                var Itemval = new tgtables.Itemvalue
                {
                    ItemId = item.Id,
                    Value = int.Parse(value_box.Text),
                    ValidFrom = DateTime.Now
                };
                connection.Itemvalues.Add(Itemval);
                connection.SaveChanges();
            }
        }
    }
}
