using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;


namespace NeuralImages
{
    [Serializable()]
    class Network
    {
        List<Layer> layers;
        List<double> outputs;

        public Network()
        {
            this.layers = new List<Layer>();

            this.outputs = new List<double>();
        }

        /// <summary>
        /// Добавляет слой в сеть
        /// </summary>
        /// <param name="number">Порядковый номер слоя</param>
        /// <param name="size">Количество нейронов в слое</param>
        /// <param name="style">Вид слоя(входной, скрытый, выходной</param>
        public void AddLayer(int size)
        {
            if (this.Layers.Count == 0)
            {
                this.layers.Add(new Layer(this.Layers.Count + 1, size, Layer.LayerStyle.input));
            }
            else if(this.Layers.Count==1)
            {
                this.layers.Add(new Layer(this.Layers.Count + 1, size, Layer.LayerStyle.output));
            }
            else if(this.Layers.Count>1)
            {
                for(int i=1;i<this.Layers.Count;i++)
                {
                    this.Layers[i] = new Layer(i + 1, this.Layers[i].Neurons.Count, Layer.LayerStyle.associate);
                }
                this.Layers.Add(new Layer(this.Layers.Count + 1, size, Layer.LayerStyle.output));
            }

        }

        /// <summary>
        /// Задаёт входные данные в сеть
        /// </summary>
        /// <param name="input">Все данные целиком</param>
        public void Receive(List<double> input)
        {
            var iln = layers.Find(a => a.Style == Layer.LayerStyle.input).Neurons;
            for (int i = 0; i < iln.Count; i++)
            {
                iln[i].Receive(input[i]);
            }
        }

        public List<double> Outputs
        {
            get { return this.outputs; }
            private set { this.outputs=value;}
        }
        /// <summary>
        /// Соединяет друг с другом слои в сети
        /// </summary>
        public void ConnectLayers()
        {
            for(int i=1;i<this.layers.Count;i++)
            {
                this.layers[i].ConnectFrom(this.layers[i - 1]);
            }
        }   

        public List<Layer> Layers
        {
            get { return this.layers; }
        }
        /// <summary>
        /// Ваще нормальный метод такой;
        /// Запускает этого монстра(сеть туды-сюды),
        /// ничего не требует взамен,
        /// аще чёткий
        /// </summary>
        public void GoGoPower()
        {
            if (Layers.Count > 1)
            {
                for (int i = 0; i < this.layers.Count; i++)
                {
                    this.layers[i].GoGoPower();
                }
                if (this.Outputs.Count == 0)
                {
                    for (int i = 0; i < this.layers.Find(a => a.Style == Layer.LayerStyle.output).Neurons.Count; i++)
                    {
                        this.Outputs.Add(this.layers.Find(a => a.Style == Layer.LayerStyle.output).Neurons[i].Signal);
                    }
                }
                else
                {
                    for (int i = 0; i < this.layers.Find(a => a.Style == Layer.LayerStyle.output).Neurons.Count; i++)
                    {
                        this.Outputs[i] = (this.layers.Find(a => a.Style == Layer.LayerStyle.output).Neurons[i].Signal);
                    }
                }
            }
            else
            {
                if (this.Outputs.Count == 0)
                {
                    for (int i = 0; i < this.layers.Find(a => a.Style == Layer.LayerStyle.input).Neurons.Count; i++)
                    {
                        this.Outputs.Add(this.layers.Find(a => a.Style == Layer.LayerStyle.input).Neurons[i].Signal);
                    }
                }
                else
                {
                    for (int i = 0; i < this.layers.Find(a => a.Style == Layer.LayerStyle.input).Neurons.Count; i++)
                    {
                        this.Outputs[i]=(this.layers.Find(a => a.Style == Layer.LayerStyle.input).Neurons[i].Signal);
                    }
                }
            }
        }

        /// <summary>
        /// Очищает сеть от входных-выходных данных
        /// Метод используется при многократном использовании сети
        /// </summary>
        public void ClearNet()
        {
            foreach(Layer value in this.layers)
            {
                foreach(Neuron temp in value.Neurons)
                {
                    temp.Clear();
                }
            }
            this.outputs.Clear();
        }

        /// <summary>
        /// Возвращает нейрон выходного слоя с наибольшим значенем сигнала - "решение" сети.
        /// </summary>
        public Neuron Decision
        {
            get
            {
                var l = layers.Find(a => a.Style == Layer.LayerStyle.output);
                Neuron Res = l.Neurons[0];
                foreach (Neuron n in l.Neurons)
                    if (n.Signal > Res.Signal) Res = n;
                return Res;
            }
        }
    }
}
