using System;
using System.Collections.Generic;

namespace GestorContas.Web.Services.Diagnostico.Dtos
{
    public class DiagnosticoFinanceiroDto
    {
        public ResumoDiagnosticoGeralDto ResumoGeral { get; set; } = new();
        public List<DiagnosticoContaDto> DiagnosticoContas { get; set; } = new();
        public List<DiagnosticoDuplicadoDto> DuplicadosSuspeitos { get; set; } = new();
        public List<DiagnosticoTransferenciaAtipicaDto> TransferenciasAtipicas { get; set; } = new();
        public List<EvolucaoMensalDiagnosticoDto> EvolucaoMensal { get; set; } = new();
        public List<PossivelErroSinalDto> ErrosSinalOuTipo { get; set; } = new();
    }

    public class ResumoDiagnosticoGeralDto
    {
        public decimal SaldoAtualTotalSistema { get; set; }
        public decimal SaldoInicialTotalContas { get; set; }
        public decimal TotalEntradasSemTransferencia { get; set; }
        public decimal TotalSaidasSemTransferencia { get; set; }
        public decimal ResultadoRealAcumulado { get; set; }
        public decimal TotalTransferenciasSaida { get; set; }
        public decimal TotalTransferenciasEntrada { get; set; }
        public decimal DesbalancoTransferencias { get; set; }
        public int TotalDuplicadosIdentificados { get; set; }
        public decimal ValorTotalDuplicados { get; set; }
    }

    public class DiagnosticoContaDto
    {
        public int ContaId { get; set; }
        public string NomeConta { get; set; } = string.Empty;
        public decimal SaldoInicial { get; set; }
        public decimal TotalEntradas { get; set; }
        public decimal TotalSaidas { get; set; }
        public decimal SaldoAtualCalculado { get; set; }
        public decimal TotalTransferenciasEnviadas { get; set; }
        public decimal TotalTransferenciasRecebidas { get; set; }
        public int TotalLancamentos { get; set; }
        public DateTime? DataPrimeiroLancamento { get; set; }
        public DateTime? DataUltimoLancamento { get; set; }
    }

    public class DiagnosticoDuplicadoDto
    {
        public DateTime Data { get; set; }
        public decimal Valor { get; set; }
        public string Tipo { get; set; } = string.Empty;
        public string ContaNome { get; set; } = string.Empty;
        public int QuantidadeDuplicados { get; set; }
        public decimal ImpactoFinanceiro { get; set; }
        public List<int> LancamentosIds { get; set; } = new();
        public List<string> Descricoes { get; set; } = new();
    }

    public class DiagnosticoTransferenciaAtipicaDto
    {
        public int Id { get; set; }
        public DateTime Data { get; set; }
        public string Descricao { get; set; } = string.Empty;
        public decimal Valor { get; set; }
        public string Tipo { get; set; } = string.Empty;
        public string ContaNome { get; set; } = string.Empty;
        public string CategoriaNome { get; set; } = string.Empty;
        public string MotivoAlert { get; set; } = string.Empty;
    }

    public class EvolucaoMensalDiagnosticoDto
    {
        public int Ano { get; set; }
        public int Mes { get; set; }
        public string MesAnoLabel { get; set; } = string.Empty;
        public decimal EntradasOperacionais { get; set; }
        public decimal SaidasOperacionais { get; set; }
        public decimal ResultadoOperacionalMes { get; set; }
        public decimal SaldoAcumuladoResultadoMes { get; set; }
        public decimal TransferenciasMes { get; set; }
        public decimal SaldoAcumuladoFimMes { get; set; }
        public decimal DivergenciaSaldo { get; set; }
        public List<LancamentoDetalheMesDto> LancamentosMes { get; set; } = new();
    }

    public class LancamentoDetalheMesDto
    {
        public int Id { get; set; }
        public DateTime Data { get; set; }
        public string Descricao { get; set; } = string.Empty;
        public decimal Valor { get; set; }
        public string Tipo { get; set; } = string.Empty;
        public string ContaNome { get; set; } = string.Empty;
        public string CategoriaNome { get; set; } = string.Empty;
        public bool IsTransferencia { get; set; }
    }

    public class PossivelErroSinalDto
    {
        public int Id { get; set; }
        public DateTime Data { get; set; }
        public string Descricao { get; set; } = string.Empty;
        public decimal Valor { get; set; }
        public string Tipo { get; set; } = string.Empty;
        public string CategoriaNome { get; set; } = string.Empty;
        public string Suspeita { get; set; } = string.Empty;
    }
}
