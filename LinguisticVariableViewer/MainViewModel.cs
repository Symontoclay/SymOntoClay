using NLog;
using OxyPlot;
using OxyPlot.Series;
using SymOntoClay.Core.Internal.StandardLibrary.FuzzyLogic;
using System;
using System.Collections.Generic;
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

            MyModel = new PlotModel { Title = "Example 1" };

            //var seriesItem = new FunctionSeries(Math.Cos, 0, 10, 0.1, "cos(x)");
            //seriesItem.Color = OxyColors.Black;

            //MyModel.Series.Add(seriesItem);
            //MyModel.Series.Add(new FunctionSeries(Math.Sin, 0, 10, 0.1, "sin(x)"));

            //var customSeries = new LineSeries();
            //customSeries.Points.Add(new DataPoint(1, 0.5));
            //customSeries.Points.Add(new DataPoint(2, 0.5));
            //customSeries.Points.Add(new DataPoint(3, 0));

            //MyModel.Series.Add(customSeries);

            var xList = Range(0, 30, 0.1);

            var customSeries = DefineLFunction(5, 10, xList);
            MyModel.Series.Add(customSeries);

            customSeries = DefineTrapezoid(5, 10, 15, 20, xList);
            MyModel.Series.Add(customSeries);

            customSeries = DefineSFunction(12, 22, xList);
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
