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
using System.Runtime.Serialization;
using System.Runtime.Serialization.Formatters.Binary;
using System.IO;
using Microsoft.Win32;
using System.Drawing;

namespace NeuralImages
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        Network net;
        List<double> teacherData;
        ErrorBackpropagation teacher = new ErrorBackpropagation(0.5);
        Converter converter = new Converter(20, 20);
        List<double> data;
        //string filename = "кек.bmp";

        public MainWindow()
        {
            InitializeComponent();
            teacherData = new List<double>();                  
        }

        private void startButton_Click(object sender, RoutedEventArgs e)
        {          
            outputText.Text = "Вывод:";
            net.Receive(data);
            net.GoGoPower();
            
            this.outputText.Text += Environment.NewLine + net.Decision.Name + ": вывод=" + net.Decision.Signal;
        }

        private void teachButton_Click(object sender, RoutedEventArgs e)
        {
            teacher.Learn33Letters(@"Resources\Arial\", net, 1.8);
        }

        private void Create(object sender, RoutedEventArgs e)
        {
            net = new Network();
            this.createGrid.Visibility = Visibility.Visible;
            this.createGrid.IsEnabled = true;
            this.netGrid.Visibility = Visibility.Collapsed;
            this.netGrid.IsEnabled = false;
        }

        private void Load(object sender, RoutedEventArgs e)
        {
            BinaryFormatter bin = new BinaryFormatter();
            OpenFileDialog open = new OpenFileDialog();
            open.Filter = "network files (*.net)|*.net";
            Nullable<bool> op=open.ShowDialog();
            if (op == true)
            {
                this.netName.Text = open.FileName.TrimEnd(".net".ToCharArray());
                Stream stream = File.Open(this.netName.Text+".net", FileMode.Open);
                this.net = (Network)bin.Deserialize(stream);
                this.createGrid.Visibility = Visibility.Collapsed;
                this.createGrid.IsEnabled = true;
                this.netGrid.Visibility = Visibility.Visible;
                this.netGrid.IsEnabled = true;
                stream.Close();
            }
            if(this.teacherData!=null)
            {
                this.teacherData.Clear();
            }
            else
            {
                this.teacherData = new List<double>();
            }
            for (int i = 0; i < net.Layers.Find(a => a.Style == Layer.LayerStyle.output).Neurons.Count; i++)
            {
                this.teacherData.Add(0);
            }
        }

        private void Save(object sender, RoutedEventArgs e)
        {
            if (this.net != null)
            {
                BinaryFormatter bin = new BinaryFormatter();
                if (File.Exists(System.IO.Path.Combine(this.netName.Text)))
                {

                    File.Delete(System.IO.Path.Combine(this.netName.Text));
                    Stream stream = File.Open(System.IO.Path.Combine(this.netName.Text), FileMode.Create);
                    bin.Serialize(stream, this.net);
                    stream.Close();
                }
                else
                {
                    Stream stream = File.Open(System.IO.Path.Combine(@"Resources\", this.netName.Text.ToString() + ".net"), FileMode.Create);
                    bin.Serialize(stream, this.net);
                    stream.Close();                   
                }
            }

        }

        private void addButton_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                net.AddLayer(Convert.ToInt32(this.neuroText.Text));
                this.neuroText.Text = "";
                this.netText.Text += Environment.NewLine + "Добавлен слой " + this.net.Layers.Count.ToString() +
                    " нейронов:" + this.net.Layers[this.net.Layers.Count - 1].Neurons.Count.ToString();
            }
            catch (FormatException)
            {
                this.neuroText.Text = "Введите целое число";
            }
        }

        private void createButton_Click(object sender, RoutedEventArgs e)
        {
            if (netName.Text!="")
            {
                net.ConnectLayers();
            }
            else
            {
                netName.Text = "Введите имя сети";
            }
            this.netText.Text = "Сеть:";
            this.createGrid.Visibility = Visibility.Collapsed;
            this.createGrid.IsEnabled = false;
            this.netGrid.Visibility = Visibility.Visible;
            this.netGrid.IsEnabled = true;
            for(int i=0;i<net.Layers.Find(a=>a.Style==Layer.LayerStyle.output).Neurons.Count;i++)
            {
                this.teacherData.Add(0);
            }
        }


        private void LoadedWin(object sender, RoutedEventArgs e)
        {
            createGrid.Visibility = Visibility.Collapsed;
            createGrid.IsEnabled = false;
            netGrid.Visibility = Visibility.Collapsed;
            netGrid.IsEnabled = false;
            string[] dirs=Directory.GetFiles(@"Resources\Arial\","*.bmp");
            for(int i=0;i<dirs.Length;i++)
            {
                this.imageComboBox.Items.Add(dirs[i].TrimStart(@"Resources\Arial\".ToCharArray()).TrimEnd(".bmp".ToCharArray()));
            }
          
        }

        private void imageComboBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            BitmapImage temp = new BitmapImage();
            temp.BeginInit();
            temp.UriSource=new Uri(@"Resources\Arial\" + this.imageComboBox.SelectedValue.ToString(), UriKind.RelativeOrAbsolute);
            temp.EndInit();
            for (int i = 0; i < net.Layers.Find(a => a.Style == Layer.LayerStyle.output).Neurons.Count; i++)
            {
                this.teacherData[i]=0;
            }
            this.teacherData[this.imageComboBox.SelectedIndex] = 1;
            this.imageLoad.BeginInit();
            this.imageLoad.Source = temp;
            this.imageLoad.EndInit();
            data = converter.Convert(@"Resources\Arial\" + this.imageComboBox.SelectedValue.ToString()+".bmp");         
        }

        private void OneImageTeachClick(object sender, RoutedEventArgs e)
        {

        }
    }
}
