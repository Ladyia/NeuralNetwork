using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;
using System.Drawing;

namespace NeuralImages
{
    class ErrorBackpropagation
    {
        /// <summary>
        /// Допущение ошибки.
        /// </summary>
        private double Assumption;

        /// <summary>
        /// Ошибки предыдущего слоя.
        /// </summary>
        private List<double> ErrorAfterLayer;

        /// <summary>
        /// Ошибки текущего слоя.
        /// </summary>
        private List<double> ErrorCurrentLayer;

        /// <summary>
        /// Текущий обучаемый слой.
        /// </summary>
        private Layer CurrentLayer;

        /// <summary>
        /// Учебные данные.
        /// </summary>
        private List<double> TeachersData;

        /// <summary>
        /// Текующая обучаемая сеть.
        /// </summary>
        private Network CurrentNet;

        /// <summary>
        /// Скорость обучения.
        /// </summary>
        private double SpeedOfTraining;

        public ErrorBackpropagation(double Assumption)
        {
            this.Assumption = Assumption;
        }

        /// <summary>
        /// Обнуляет ожидаемый результат.
        /// </summary>
        private void NullTeachData()
        {
            TeachersData = new List<double>();
            for (int i = 0; i < 33; i++) TeachersData.Add(0);
        }

        /// <summary>
        /// Корректирует веса выходного слоя.
        /// </summary>
        private void TeachOutLayer()
        {
            CurrentLayer = CurrentNet.Layers[CurrentNet.Layers.Count - 1]; //текущий слой
            List<Neuron> BeforeLayerNeurons = CurrentNet.Layers[CurrentNet.Layers.Count - 2].Neurons; //предыдущий слой
            ErrorCurrentLayer.Clear(); //очистка списка ошибок

            for (int ind = 0; ind < CurrentLayer.Neurons.Count; ind++)
            {
                ErrorCurrentLayer.Add(CurrentNet.Outputs[ind] * (1 - CurrentNet.Outputs[ind]) * (TeachersData[ind] - CurrentNet.Outputs[ind]));
                for (int j = 0; j < BeforeLayerNeurons.Count; j++) //теперь бегаем по нейронам предыдущего слоя и высчитываем веса. 
                {
                    double correctWeight = SpeedOfTraining * ErrorCurrentLayer[ind] * BeforeLayerNeurons[j].Signal;
                    CurrentLayer.Neurons[ind].CorrectWeight(j, correctWeight);
                }
            }
        }

        /// <summary>
        /// Обучает один скрытый слой.
        /// </summary>
        /// <param name="layerIndex">Индекс слоя, который нужно обучить.</param>
        public void TeachOneInnerLayer(int layerIndex)
        {
            ErrorCurrentLayer.Clear(); //очистка списка ошибок
            CurrentLayer = CurrentNet.Layers[layerIndex]; //текущий слой
            Layer beforeLayer = CurrentNet.Layers[layerIndex - 1]; //предыдущий слой
            List<Neuron> afterLayerNeurons = CurrentNet.Layers[layerIndex + 1].Neurons;
            double temp;

            for (int indCurrNeuron = 0; indCurrNeuron < CurrentLayer.Neurons.Count; indCurrNeuron++)
            {
                if (beforeLayer.Style != Layer.LayerStyle.input) //если предыдущий слой не входной,
                {
                    //то считаем с суммой 
                    temp = 0;
                    for (int k = 0; k < beforeLayer.Neurons.Count; k++)
                        temp += ErrorAfterLayer[k] * afterLayerNeurons[k].Weights[indCurrNeuron];
                    ErrorCurrentLayer.Add(CurrentLayer.Neurons[indCurrNeuron].Signal * (1 - CurrentLayer.Neurons[indCurrNeuron].Signal) * temp);
                }
                else
                    //иначе считаем без суммы, так как у входного слоя нет весов
                    ErrorCurrentLayer.Add(CurrentLayer.Neurons[indCurrNeuron].Signal * (1 - CurrentLayer.Neurons[indCurrNeuron].Signal));

                for (int indBN = 0; indBN < beforeLayer.Neurons.Count; indBN++) //теперь бегаем по нейронам предущего слоя и меняем веса
                    CurrentLayer.Neurons[indCurrNeuron].CorrectWeight(indBN, SpeedOfTraining * beforeLayer.Neurons[indBN].Signal
                                                                                                         * ErrorCurrentLayer[indCurrNeuron]);
            }
        }

        /// <summary>
        /// Обучает все скрытые слои.
        /// </summary>
        private void TeachOtherLayers()
        {
            int indLayer = CurrentNet.Layers.Count - 2; // Слой перед выходным
            while (indLayer != 0) //так как индекс у входного слоя = 0, то выполняем обучение, пока не дойдем до него
            {
                TeachOneInnerLayer(indLayer);
                indLayer--;
            }
        }

        /// <summary>
        /// Проверяет насколько выходные значения сети отклоняются от допустимого.
        /// </summary>
        /// <returns>Допустимо или нет.</returns>
        private bool IsRight()
        {
            //P.S этот метод должен вызываться после каждой итерации обучения
            CurrentNet.GoGoPower(); //получаем результат после 1 итерации обученеия
            for (int i = 0; i < CurrentNet.Outputs.Count; i++) //если хоть 1 нейрон не обучился, то все, конец
                if (Math.Abs(CurrentNet.Outputs[i] - TeachersData[i]) > Assumption)
                    return false;
            return true;
        }

        /// <summary>
        /// Обучить все слои по всем буквам.
        /// </summary>
        /// <param name="Path">Путь к файлам с буквами.</param>
        /// <param name="NeuralNet">Обучаемая сеть.</param>
        /// <param name="SpeedOfTraining">Скорость тренировки.</param>
        public void Learn33Letters(String Path, Network NeuralNet, double SpeedOfTraining)
        {
            Converter converter = new Converter(); 
            this.CurrentNet = NeuralNet;
            this.SpeedOfTraining = SpeedOfTraining;
            //string alph = "АБВГДЕЁЖЗИКЛМНОПРСТУФХЦЧШЩЪЫЬЭЮЯ";
            string alph = "АБВГД"; //для того, чтобы бы бегать по изображениям, так удобней по именам бегать

            for (int i = 0; i < 5; i++)
            {
                
                List<double> data = converter.Convert(@"Resources\Arial\" + alph[i] + ".bmp");
                CurrentNet.Receive(data);
                NeuralNet.GoGoPower();

                //все очищаем на всякий случай
                ErrorAfterLayer = new List<double>();
                ErrorCurrentLayer = new List<double>();

                NullTeachData(); // обнуление ожидаемых данных                
                TeachersData[i] = 1; //меняем ожидаемые значения

                while (!IsRight()) //пока результат не будет отклоняться от ожидаемого в пределах допустимого значения 
                {
                    TeachOutLayer(); 
                    TeachOtherLayers();
                }
            }
        }

        /// <summary>
        /// Полностью обучить сеть только для 1 буквы.
        /// </summary>
        /// <param name="NeuralNet">Обучемая сеть.</param>
        /// <param name="TeachersData">Ожидаемый результат.</param>
        /// <param name="SpeedOfTraining">Скорость обучения.</param>
        public void FullTeachingLetter(Network NeuralNet, List<double> TeachersData, double SpeedOfTraining)
        {
            this.CurrentNet = NeuralNet;
            this.TeachersData = TeachersData;
            this.SpeedOfTraining = SpeedOfTraining;

            if (NeuralNet.Outputs != TeachersData)
            {
                CurrentNet.Receive(TeachersData);

                //все очищаем на всякий случай
                ErrorAfterLayer = new List<double>();
                ErrorCurrentLayer = new List<double>();
                NeuralNet.GoGoPower();

                while (!IsRight()) //пока результат не будет отклоняться от ожидаемого в пределах допустимого значения 
                {
                    TeachOutLayer();
                    TeachOtherLayers();
                }
            }
        }

        /// <summary>
        /// Одна итерация для всей сети целиком.
        /// </summary>
        public void GiveALesson(Network NeuralNet, List<double> TeachersData, double SpeedOfTraining)
        {
            this.CurrentNet = NeuralNet;
            CurrentNet.Receive(TeachersData);

            //все очищаем на всякий случай
            ErrorAfterLayer = new List<double>();
            ErrorCurrentLayer = new List<double>();
          
            TeachOutLayer();
            TeachOtherLayers();
        }
    }
}
