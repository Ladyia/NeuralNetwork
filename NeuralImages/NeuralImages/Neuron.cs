using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NeuralImages
{
    [Serializable()]
    class Neuron
    {
        List<Input> inputs;
        List<Neuron> outputs;

        /// <summary>
        /// Идентификатор нейрона - не давайте одинаковые имена!
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        /// Здесь хранится выходное значение нейрона после последнего вызова Calculate. 
        /// Если после этого был вызван метод Clear, то Signal равно нулю.
        /// </summary>
        public double Signal { get; private set; }

        /// <summary>
        /// Здесь хранятся входные значения входных нейронов.
        /// Эти значения представляют собой непосредственно вводимые в сеть данные
        /// Эти значения не взвешиваются при расчёте выходного сигнала нейрона и 
        /// просто находится их среднее арифметическое (и, конечно, потом оно 
        /// проходит через функцию активации).
        /// Данный список значений используется в расчёте,
        /// только если данный нейрон - входной (задаётся флагом IsInputNeuron).
        /// </summary>
        public List<double> DirectInputs { get; private set; }

        /// <summary>
        /// Ставится в конструкторе, если мы хотим, чтобы данный нейрон был входным 
        /// (в расчёте использовал входные данные, не взвешивая их).
        /// </summary>
        public bool IsInputNeuron { get; private set; }

        /// <summary>
        /// Указывает, является ли данный нейрон нейроном смещения.
        /// Такой нейрон будет всегда выдавать 1 при вызове функции Calculate
        /// </summary>
        public bool IsBias { get; private set; }

        /// <summary>
        /// Множитель при Х в формуле логистической функции.
        /// Большие значения ( > 1) приближают функцию к хэвисайдовской,
        /// малые ( меньше 0) приближают функцию к прямой y = 0,5.
        /// </summary>
        static double SigmoidSharpness = 0.6;

        /// <summary>
        /// По умолчанию нейрон не входной.
        /// </summary>
        public Neuron()
        {
            inputs = new List<Input>();
            outputs = new List<Neuron>();
            DirectInputs = new List<double>();
            IsInputNeuron = false;
            Activate = LogisticalActivation;
        }

        /// <summary>
        /// Конструктор класса
        /// </summary>
        /// <param name="n">Имя нейрона</param>
        /// <param name="Direct">Входной или нет</param>
        public Neuron(string n, bool Direct = false, bool isBias = false)
            : this()
        {
            Name = n;
            IsInputNeuron = Direct;
            IsBias = isBias;
        }

        /// <summary>
        /// Функция суммации. Если нейрон не входной, считает скалярное произведение векторов 
        /// сигналов подсоединённых ко входам нейронов и весов этих входов.
        /// Если входной, то просто суммирует все поданные сигналы.
        /// </summary>
        /// <returns></returns>
        double Sum()
        {
            if (IsInputNeuron) return DirectInputs.Average();
            double sum = inputs.Sum(a => a.Connected_neuron.Signal * a.Weight);
            return sum;
        }

        /// <summary>
        /// Тип функций, подходящих в качестве активационных.
        /// </summary>
        /// <param name="X">Сигнал с сумматора</param>
        /// <returns></returns>
        public delegate double activation_func(double X);

        /// <summary>
        /// Производная логистической функции
        /// </summary>
        /// <returns></returns>
        public double LogDifFunctionOfSum()
        {
            return LogisticalDifferentiatedActivation(Sum());
        }
        
        /// <summary>
        /// Функция активации нейрона
        /// </summary>
        activation_func Activate;

        /// <summary>
        /// Логистическая функция.
        /// y = 1/(1+exp(-ss*x));
        /// где ss - SigmoidSharpness
        /// </summary>
        /// <param name="X"></param>
        /// <returns></returns>
        public static double LogisticalActivation(double X)
        {
            return 1.0 / (1 + Math.Pow(Math.E, -X * SigmoidSharpness));
        }

        /// <summary>
        /// Производная лонистической функции
        /// </summary>
        /// <param name="X"></param>
        /// <returns></returns>
        public static double LogisticalDifferentiatedActivation(double X)
        {
            double la = LogisticalActivation(X);
            return la * (1 - la);
        }

        /// <summary>
        /// Функция Хэвисайда, нифига себе, охереть!
        /// </summary>
        /// <param name="X"></param>
        /// <returns></returns>
        public static double HeavysideActivation(double X)
        {
            return X > 0.5 ? 1 : 0;
        }

        /// <summary>
        /// Устанавливает функцию активации. Вот сюрприз-то!
        /// </summary>
        /// <param name="F"></param>
        public void SetActivationFunction(activation_func F)
        {
            Activate = F;
        }

        /// <summary>
        /// Функция вычисляет выходное значение нейрона - Signal 
        /// в соответствии с типом нейрона.
        /// </summary>
        public void Calculate()
        {
            Signal = IsBias ? 1 : Activate(Sum());
        }

        /// <summary>
        /// Соединяет выход данного нейрона ко входу нейрона to
        /// При этом у нейрона to добавляется вход с весом w.
        /// </summary>
        /// <param name="to"></param>
        /// <param name="w"></param>
        public void ConnectToForward(Neuron to, double w = 1)
        {
            outputs.Add(to);
            to.inputs.Add(new Input(this, w));
        }

        /// <summary>
        /// Добавляет данному нейрону новый вход с весом w, 
        /// подсоединённый к нейрону from.
        /// </summary>
        /// <param name="from"></param>
        /// <param name="w"></param>
        public void ConnectToBackward(Neuron from, double w = 1)
        {
            inputs.Add(new Input(from, w));
            from.outputs.Add(this);
        }

        /// <summary>
        /// Функция добавляет значение signal в список прямых
        /// входных значений. Этот список используется только
        /// у входных нейронов. Эти значения не взвешиваются при расчёте выхода нейрона.
        /// Очистить список можно можно вызовом функции Clear.
        /// </summary>
        /// <param name="signal"></param>
        public void Receive(double signal)
        {
            DirectInputs.Add(signal);
        }

        public override string ToString()
        {
            return "Имя: " + Name + "; выход - " + Signal.ToString();
        }

        /// <summary>
        /// Функция очищает список прямых входов и обнуляет сигнал
        /// </summary>
        public void Clear()
        {
            DirectInputs.Clear();
            Signal = 0;
        }

        /// <summary>
        /// получает копию списка всех входов нейрона
        /// </summary>
        /// <returns></returns>
        public List<Input> GetInputs()
        {
            return new List<Input>(inputs);
        }

        /// <summary>
        /// Прибавляет к весу входа под индексом N (в GetInputs())
        /// величину d.
        /// </summary>
        /// <param name="N"></param>
        /// <param name="d"></param>
        public void CorrectWeight(int N, double d)
        {
            inputs[N].Weight += d;
        }

        /// <summary>
        /// получает копию списка нейронов, которые подсоединены к выходу
        /// данного нейрона
        /// </summary>
        public List<Neuron> Outputs
        {
            get
            {
                return outputs;
            }
        }

        /// <summary>
        /// Получает копию списка весов
        /// </summary>
        public List<double> Weights
        {
            get
            {
                return new List<double>(inputs.Select(a => a.Weight));
            }
        }
    }
}
