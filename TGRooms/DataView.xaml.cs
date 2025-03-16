using Google.Protobuf.WellKnownTypes;
using Microsoft.EntityFrameworkCore.Internal;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel.DataAnnotations;
using System.DirectoryServices.ActiveDirectory;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;
using TGRooms.tgtables;
// Předem se omlouvám za katastrofický kód, který jsem zde napsal
// At the time of writing, only I and god knew what it did. Now, only god knows
namespace TGRooms
{
    public enum Choice { Mistnosti, Spravci };

    /// <summary>
    /// Interaction logic for DataView.xaml
    /// </summary>
    public partial class DataView : Window
    {
        public bool isRoomView = true;
        private MainWindow mainWindow;
        private Choice dataType = Choice.Mistnosti;
        private ObservableCollection<tgtables.Room> rooms = new ObservableCollection<tgtables.Room>();
        private ObservableCollection<tgtables.Spravce> spravci = new ObservableCollection<tgtables.Spravce>();
        private DateTime selTime = DateTime.Now;
        private bool isHist = false;
        private DateTime histVal;
        private double histBarVal;
        public DataView(MainWindow mw)
        {
            InitializeComponent();
            mainWindow = mw;
        }
        // https://stackoverflow.com/a/5709016
        public static bool DateBetween(DateTime input, DateTime date1, DateTime date2)
        {
            return (input > date1 && input < date2);
        }
        public void Set_Data_Type(Choice ch)
        {
            this.dataType = ch;
            object_name.Text = ch.ToString();
            switch (ch)
            {
                case Choice.Mistnosti:
                    secondary_object_name.Text = "Items in room";
                    break;
                case Choice.Spravci:
                    secondary_object_name.Text = "Rooms under Spravce";
                    break;
            }
            this.FetchData();
            this.Update_Combo_Box();
        }
        private void Back_To_Main(object sender, RoutedEventArgs e)
        {
            mainWindow.Visibility = Visibility.Visible;
            Visibility = Visibility.Hidden;
        }

        private void FetchData()
        {
            using (var context = new tgtables.TgtablesContext())
            {
                context.Database.EnsureCreated();
                switch (dataType)
                {
                    case Choice.Mistnosti:
                        rooms = new ObservableCollection<tgtables.Room>(context.Rooms.ToList());
                        foreach (tgtables.Room r in rooms)
                        {
                            Console.Out.WriteLine(r.Id + r.Name);
                        }
                        break;
                    case Choice.Spravci:
                        spravci = new ObservableCollection<tgtables.Spravce>(context.Spravces.ToList());
                        break;
                }
                Console.Out.WriteLine("Data fetched");
            }
        }

        private void Main_Box_Changed(object sender, SelectionChangedEventArgs? e)
        {
            secondary_box.Items.Clear();
            using (var context = new tgtables.TgtablesContext())
            {
                switch (this.dataType)
                {
                    case Choice.Mistnosti:
                        foreach (tgtables.Room r in rooms)
                        {
                            if (main_box.SelectedItem == null) continue;
                            if (r.Name == main_box.SelectedItem.ToString())
                            {
                                Console.WriteLine("Table " + r.Name);
                                var itemRooms = context.Itemrooms.Where(x => x.Room == r.Id && x.ValidTo == null).ToList();
                                itemRooms.ToList().ForEach((x) =>
                                {
                                    Console.WriteLine(x.ToString());
                                });
                                List<int>? itemIds = context.Itemrooms.Where(x => x.Room == r.Id).ToList().Select(i => i.Item).ToList(); 
                                itemIds.ForEach((x) =>
                                {
                                    Console.WriteLine(x.ToString());
                                });
                                var items = context.Items.Where(x => itemIds.Contains(x.Id)).ToList();
                                items.ForEach((x) =>
                                {
                                    Console.WriteLine(x.ToString());
                                });
                                itemRooms.Sort();

                                var itemRommsFOD = itemRooms.FirstOrDefault();
                                if (itemRommsFOD != null)
                                hist_begin.Text = itemRommsFOD.ValidFrom.ToString();
                                slider_hist.Focusable = true;
                                int count = 0;
                                Console.WriteLine("IsHist:" + isHist);
                                if (isHist)
                                {
                                    List<Itemroom>? itemrooms = context.Itemrooms.Where(x => x.Room == r.Id).ToList();
                                    itemRooms = new List<Itemroom>();
                                    foreach (var i in itemrooms)
                                    {
                                        bool isbet = false;
                                        if (!i.ValidTo.HasValue) { Console.WriteLine("Recent item" + i.Item); isbet = DateBetween(histVal, i.ValidFrom, DateTime.Now); }
                                        else { Console.WriteLine("Not so recent item" + i.Item); isbet = DateBetween(histVal, i.ValidFrom, i.ValidTo.Value); }
                                        if (isbet)
                                        {
                                            itemRooms.Add(i);
                                        }
                                    }
                                }
                                foreach (var i in itemRooms)
                                {
                                    var item = items.FirstOrDefault(x => x.Id == i.Item);
                                    if (item != null)
                                    {
                                        var itemRoomC = context.Itemrooms.Where(x => x.Item == item.Id).ToList();
                                        var itemRC = itemRoomC.FirstOrDefault();
                                        if (itemRC != null)
                                        {
                                            count++;
                                            secondary_box.Items.Add(item.Name);
                                        } else Console.WriteLine("Itemroom " + i.ToString() + " not valid");

                                    } else Console.WriteLine("Itemroom " + i.ToString() + " not valid");
                                }
                                if (count == 0) secondary_box.Items.Add("No items in this room");
                            }
                        }
                        break;

                    case Choice.Spravci:
                        foreach (tgtables.Spravce r in spravci)
                        {
                            slider_hist.Focusable = false;
                            if (main_box.SelectedItem == null) continue;
                            if (r.Name + " " + r.Surname != main_box.SelectedItem.ToString()) continue;
                            var rooma = context.Rooms.Where(x => x.Spravce == r.Id);
                            foreach (var room in rooma)
                            {
                                Console.WriteLine("Adding room " + room.Name + " with spravceid " + room.Spravce + " and actual spravce " + r.Id);
                                secondary_box.Items.Add(room.Name);
                            }
                            
                        }
                        break;
                }
            }
        }
        private void Secondary_Box_Changed(object sender, SelectionChangedEventArgs? e)
        {
            if (secondary_box.SelectedValue == null) { return; }
            Console.Out.WriteLine("Selection Changed to " + secondary_box.SelectedValue.ToString());
            using (var context = new tgtables.TgtablesContext())
            {
                switch (this.dataType)
                {
                    case Choice.Mistnosti:
                        var item = context.Items.Where(x => x.Name == secondary_box.SelectedValue.ToString()).ToList().FirstOrDefault();
                        if (item == null)
                        {
                            Console.WriteLine("Internal error (item is null)");

                            return;
                        }
                        hist_begin.Text = item.Acquired.ToString();
                        selected_id.Text = item.Id.ToString();
                        selected_name.Text = item.Name;
                        selected_acquired.Text = item.Acquired.ToString();
                        info_acq.Text = "Acquired:";
                        var soldtext = "Not sold yet";
                        if (item.Sold != null) soldtext = item.Sold.ToString();
                        selected_sold.Text = soldtext;
                        if (!isHist)
                        {
                            var val = context.Itemvalues.Where(x => x.ItemId == item.Id && x.ValidTo == null).ToList().FirstOrDefault();
                            if (val == null)
                            {
                                Console.WriteLine("Internal error (itemvalue is null)");
                                return;
                            }
                            selected_value.Text = val.Value.ToString();
                        } else
                        {
                            selected_value.Text = "";
                            context.Itemvalues.Where(x => x.ItemId == item.Id && x.ValidTo != null).ToList().ForEach(x =>
                            {
                               bool isBetween = DateBetween(histVal, x.ValidFrom, x.ValidTo.Value);
                                if (isBetween)
                                {
                                    selected_value.Text = x.Value.ToString();
                                    Console.WriteLine("Changed val to" + selected_value.Text);
                                    return;
                                }
                            });
                            if (selected_value.Text == "")selected_value.Text = context.Itemvalues.Where(x => x.ItemId == item.Id && x.ValidTo == null).ToList().FirstOrDefault().Value.ToString();
                            Console.WriteLine("Changed val to recent (" + selected_value.Text + ")");
                        }
                        break;
                    case Choice.Spravci:
                        var room = context.Rooms.Where(x => x.Name == secondary_box.SelectedValue.ToString()).ToList().FirstOrDefault();
                        if (room == null)
                        {
                            Console.WriteLine("Internal error (room is null)");
                            return;
                        }
                        selected_id.Text = room.Id.ToString();
                        selected_name.Text = room.Name;
                        var spravce = context.Spravces.Where(x => x.Id == room.Spravce).ToList().FirstOrDefault();
                        if (spravce == null)
                        {
                            Console.WriteLine("Internal error (spravce is null)");
                            return;
                        }
                        selected_acquired.Text = spravce.Name + " " + spravce.Surname;
                        info_acq.Text = "Spravce";
                        selected_sold.Text = "";
                        selected_value.Text = "";
                        break;
                }
            }
        }

        private void Update_Combo_Box()
        {
            main_box.Items.Clear();
            switch (this.dataType)
            {
                case Choice.Mistnosti:
                    foreach (tgtables.Room r in rooms)
                    {
                        main_box.Items.Add(r.Name);
                    }
                    break;
                case Choice.Spravci:
                    foreach (tgtables.Spravce r in spravci)
                    {
                        main_box.Items.Add(r.Name + " " + r.Surname);
                    }
                    break;
            }
            hist_end.Text = DateTime.Now.ToString();
        }
        // https://stackoverflow.com/a/1665843
        private bool dragStarted = false;

        private void Slider_DragCompleted(object sender, DragCompletedEventArgs e)
        {
            HandleHistChanged(((Slider)sender).Value);
            this.dragStarted = false;
        }

        private void Slider_DragStarted(object sender, DragStartedEventArgs e)
        {
            this.dragStarted = true;
        }

        private void Slider_ValueChanged(
            object sender,
            RoutedPropertyChangedEventArgs<double> e)
        {
            if (!dragStarted)
                HandleHistChanged(e.NewValue);
        }

        private void HandleHistChanged(double val)
        {
            Console.WriteLine("Handling with val" + val);
            // TODO: Logic
            if (val == 10)
            {
                // Default
                isHist = false;
                Main_Box_Changed(main_box, null);
                Secondary_Box_Changed(secondary_box, null);
                try {
                    if (current_date != null)
                        current_date.Text = DateTime.Now.ToString();
                } catch (Exception e)
                {
                    Console.WriteLine("error cause loading first");
                }
                return;
                }
            isHist = true;
            using (var context = new tgtables.TgtablesContext())
            {
                DateTime? past = null;
                int? id = null;
                Item? sel_item = null;
                Spravce? sel_spravce = null;
                switch (dataType)
                {
                    case Choice.Mistnosti:
                        // TODO: Fix crash
                        Item? item = null;
                        try
                        {
                            item = context.Items.Where(x => x.Name == secondary_box.SelectedValue.ToString()).ToList().FirstOrDefault();
                        } catch (Exception e)
                        {
                            var selval = main_box.SelectedValue;
                            if (selval == null) return;
                            var room = context.Rooms.Where(x => x.Name == selval.ToString()).ToList().FirstOrDefault();
                            var items = context.Itemrooms.Where(x => x.Room == room.Id).ToList();
                            if (items.Count == 0)
                            {
                                Console.WriteLine("No items in room");
                                return;
                            }
                            items.Sort();
                            item = context.Items.Where(x => x.Id == items.FirstOrDefault().Item).FirstOrDefault();
                        }
                        if (item == null)
                        {
                            Console.WriteLine("Internal error (item is null)");
                            return;
                        }
                        past = item.Acquired;
                        id = item.Id;
                        sel_item = item;
                        break;
                    case Choice.Spravci:
                        Console.WriteLine("Internal Error: no history for Spravci/Room");
                        break;
                }

                if (past == null)
                {
                    Console.WriteLine("Internal error (past is null)");
                    return;
                }
                // A warcrime
                long most_past_ts = ((DateTimeOffset)past.Value).ToUnixTimeSeconds();// past.ToTimestamp();
                long rn = ((DateTimeOffset)DateTime.Now).ToUnixTimeSeconds();
                long sub = rn - most_past_ts;
                DateTime Date = DateTimeOffset.FromUnixTimeSeconds(Convert.ToInt64(sub*(val/10)) + most_past_ts).DateTime;
                histVal = Date;
                current_date.Text = Date.ToString();
                hist_begin.Text = past.Value.ToString();
                Console.WriteLine("Selected Date = " + Date.ToString());

            }
                
            //END TODO
            Main_Box_Changed(main_box, null);
            Secondary_Box_Changed(secondary_box, null);
        }

        private void btn_pridat_Click(object sender, RoutedEventArgs e)
        {
            PridatItem piWindow = new PridatItem();
            piWindow.Visibility = Visibility.Visible;
        }

        private void btn_vyradit_Click(object sender, RoutedEventArgs e)
        {
            if (secondary_box.SelectedValue == null) return;
            if (dataType == Choice.Spravci) return;
            using (var context = new tgtables.TgtablesContext())
            {
                switch (this.dataType)
                {
                    case Choice.Mistnosti:
                        var item = context.Items.Where(x => x.Name == secondary_box.SelectedValue.ToString()).ToList().FirstOrDefault();
                        if (item == null)
                        {
                            Console.WriteLine("Internal error (item is null)");
                            return;
                        }
                        item.Sold = DateTime.Now;
                        context.SaveChanges();
                        var itemroom = context.Itemrooms.Where(x => x.Item == item.Id && x.ValidTo == null).ToList().FirstOrDefault();
                        itemroom.ValidTo = DateTime.Now;
                        context.SaveChanges();
                        break;
                    case Choice.Spravci:
                        Console.WriteLine("Internal Error: nejde vyradit spravce");
                        break;
                }
            }
        }

        private void btn_presunout_Click(object sender, RoutedEventArgs e)
        {
            if (dataType == Choice.Spravci) return;
            if (secondary_box.SelectedValue == null) return;
            using (var context = new tgtables.TgtablesContext())
            {

                ChangeItem changeItem = new ChangeItem(context.Items.Where(x => x.Name == secondary_box.SelectedValue.ToString()).ToList().FirstOrDefault());
                changeItem.Visibility = Visibility.Visible;

            }

        }
    }
}
