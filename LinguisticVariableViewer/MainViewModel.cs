/*MIT License

Copyright (c) 2020 - 2024 Sergiy Tolkachov

Permission is hereby granted, free of charge, to any person obtaining a copy
of this software and associated documentation files (the "Software"), to deal
in the Software without restriction, including without limitation the rights
to use, copy, modify, merge, publish, distribute, sublicense, and/or sell
copies of the Software, and to permit persons to whom the Software is
furnished to do so, subject to the following conditions:

The above copyright notice and this permission notice shall be included in all
copies or substantial portions of the Software.

THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR
IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY,
FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE
AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER
LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM,
OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN THE
SOFTWARE.*/

using NLog;
using OxyPlot;
using OxyPlot.Series;
using SymOntoClay.Core.Internal.StandardLibrary.FuzzyLogic;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LinguisticVariableViewer
{
    public class MainViewModel
    {
        private static readonly Logger _logger = LogManager.GetCurrentClassLogger();

        public MainViewModel()
        {
            _logger.Info("MainViewModel()");

            MyModel = new PlotModel();





            CreateCase2();


        }

        private void CreateCase2()
        {
            var xList = Range(0, 20, 0.1);

            var minimalSeries = DefineLFunction(0, 1, xList);
            MyModel.Series.Add(minimalSeries);

            var lowSeries = DefineTrapezoid(0, 0.5, 5, 7, xList);
            MyModel.Series.Add(lowSeries);

            var middleSeries = DefineTrapezoid(4, 5, 16, 20, xList);
            MyModel.Series.Add(middleSeries);


            var maximalSeries = DefineSFunction(10, 15, 20, xList);
            MyModel.Series.Add(maximalSeries);
        }

        private void CreateLogicValueCase()
        {
            var xList = Range(0, 1.01, 0.05);





            var maximalSeries = DefineSFunction(0.8, 0.9, 1, xList);
            MyModel.Series.Add(maximalSeries);
        }

        private void CreateCase1()
        {
            var xList = Range(0, 30, 0.1);



            var customSeries = DefineSFunction(12, 22, xList);
            MyModel.Series.Add(customSeries);
        }

        private IEnumerable<double> Range(double start, double stop, double step)
        {
            var result = new List<double>();

            for (var x = start; x <= stop; x += step)
            {
                result.Add(x);
            }

            return result;
        }

        private LineSeries DefineTrapezoid(double a, double b, double c, double d, IEnumerable<double> xList)
        {
            var customSeries = new LineSeries();

            var pointsList = customSeries.Points;

            foreach (var x in xList)
            {
                pointsList.Add(new DataPoint(x, SystemMemberFunctions.Trapezoid(x, a, b, c, d)));
            }

            return customSeries;
        }

        private LineSeries DefineSFunction(double a, double b, IEnumerable<double> xList)
        {
            return DefineSFunction(a, (a + b) / 2, b, xList);
        }

        private LineSeries DefineSFunction(double a, double m, double b, IEnumerable<double> xList)
        {
            var customSeries = new LineSeries();

            var pointsList = customSeries.Points;

            foreach (var x in xList)
            {
                pointsList.Add(new DataPoint(x, SystemMemberFunctions.SFunction(x, a, m, b)));
            }

            return customSeries;
        }

        private LineSeries DefineLFunction(double a, double b, IEnumerable<double> xList)
        {
            var customSeries = new LineSeries();

            var pointsList = customSeries.Points;

            foreach (var x in xList)
            {
                pointsList.Add(new DataPoint(x, SystemMemberFunctions.LFunction(x, a, b)));
            }

            return customSeries;
        }

        public PlotModel MyModel { get; private set; }
    }
}
