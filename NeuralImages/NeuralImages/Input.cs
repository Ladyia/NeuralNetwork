using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NeuralImages
{
    [Serializable()]
    class Input
    {
        public Neuron Connected_neuron { get; set; }
        public double Weight { get; set; }
        public double WeightedSignal()
        {
            return Connected_neuron.Signal * Weight;
        }
        public Input(Neuron n, double w)
        {
            Connected_neuron = n;
            Weight = w;
        }
    }
}
