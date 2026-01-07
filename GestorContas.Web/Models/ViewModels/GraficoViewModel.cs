using System.Collections.Generic;

namespace GestorContas.Web.Models.ViewModels
{
    public class GraficoViewModel
    {
        public List<string>? Labels { get; set; }
        public List<decimal>? Valores { get; set; }
        public List<string>? Cores { get; set; }
        public string? Titulo { get; set; }
        public List<DatasetViewModel>? Datasets { get; set; }
    }

    public class DatasetViewModel
    {
        public string Label { get; set; }
        public List<decimal> Data { get; set; }
        public string BackgroundColor { get; set; }
        public string BorderColor { get; set; }
        public bool Fill { get; set; } = false;
        public double Tension { get; set; } = 0.3;
    }
}
