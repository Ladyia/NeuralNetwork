using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NeuralImages
{
    [Serializable()]
    class Layer
    {
        List<Neuron> neurons;//список нейронов данного слоя
        LayerStyle style;//вид слоя
        int number; //номер слоя в сети

        double init_weights_width = 0.25;

        /// <summary>
        /// стиль слоя поределяет его вид
        /// входной, выходной и скрытый
        /// </summary>
        public enum LayerStyle
        {
            input=1,
            associate,
            output
        }
        public Layer()
        {
            this.neurons = new List<Neuron>();
            this.style = Layer.LayerStyle.input;
            this.number = 1;
        }
        /// <summary>
        /// Создаёт объект класса Layer
        /// </summary>
        /// <param name="number">Порядковый номер</param>
        /// <param name="size">Количество нейронов в слое</param>
        /// <param name="style">Вид слоя:1 слой-входной; последний-выходной; остальные-скрытые</param>
        public Layer(int number, int size, LayerStyle style):this()
        {
            for(int i=0;i<size;i++)
            {
                this.neurons.Add(new Neuron(Convert.ToString("слой " + number + " номер " + (i + 1)), style == LayerStyle.input ? true : false, style != LayerStyle.output && i == 1 ? true : false));
            }
            this.Style = style;
            this.Number = number;
        }
        /// <summary>
        /// Соединяет этот слой со следующим
        /// </summary>
        /// <param name="n">Слой для соединения</param>
        public void ConnectTo(Layer n)
        {
            Random r=new Random();
            for(int i=0;i<n.neurons.Count;i++)
            {
                for(int j=0;j<this.neurons.Count;j++)
                {
                    this.neurons[j].ConnectToForward(n.neurons[i],r.NextDouble()*init_weights_width - init_weights_width / 2 );
                }
            }
        }
        /// <summary>
        /// Соединяет слой с предыдущим
        /// </summary>
        /// <param name="n">Слой для соединения</param>
        public void ConnectFrom(Layer n)
        {
            Random r = new Random();
            for (int i = 0; i < this.neurons.Count; i++)
            {
                for (int j = 0; j < n.neurons.Count; j++)
                {
                    this.neurons[i].ConnectToBackward(n.neurons[j], r.NextDouble() * init_weights_width - init_weights_width / 2);                }
                }
        }
        /// <summary>
        /// Запускает все нейроны в слое
        /// </summary>
        public void GoGoPower()
        {
            for(int i=0;i<this.neurons.Count;i++)
            {
                this.neurons[i].Calculate();
            }
        }

        public LayerStyle Style
        {
            get { return this.style; }
             set { this.style=value;}
        }

        public int Number
        {
            get { return this.number; }
            private set { this.number=value;}
        }

        public List<Neuron> Neurons
        {
            get { return this.neurons; }
            private set { this.neurons = value; }
        }
    }

}
