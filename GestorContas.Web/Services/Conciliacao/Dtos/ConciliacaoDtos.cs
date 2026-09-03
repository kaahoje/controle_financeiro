using GestorContas.Web.Models;
using GestorContas.Web.Models.Enums;
using System;
using System.Collections.Generic;

namespace GestorContas.Web.Services.Conciliacao.Dtos
{
    public class ItemExtratoDto
    {
        public DateTime Data { get; set; }
        public decimal Valor { get; set; }
        public TipoLancamento Tipo { get; set; }
        public string Descricao { get; set; } = string.Empty;
    }

    public class ItemConciliadoDto
    {
        public ItemExtratoDto Extrato { get; set; } = new();
        public ItemSistemaDto Sistema { get; set; } = new();
    }

    public class ItemSistemaDto
    {
        public int Id { get; set; }
        public DateTime Data { get; set; }
        public string Descricao { get; set; } = string.Empty;
        public decimal Valor { get; set; }
        public TipoLancamento Tipo { get; set; }
        public string CategoriaNome { get; set; } = string.Empty;
    }

    public class ResultadoConciliacaoDto
    {
        public List<ItemConciliadoDto> Conciliados { get; set; } = new();
        public List<ItemExtratoDto> ApenasExtrato { get; set; } = new();
        public List<ItemSistemaDto> ApenasSistema { get; set; } = new();
        public int TotalExtrato => Conciliados.Count + ApenasExtrato.Count;
        public int TotalSistemaNoPeriodo => Conciliados.Count + ApenasSistema.Count;
    }

    public class ResultadoVerificacaoDuplicadoDto
    {
        public string Status { get; set; } = "novo"; // "duplicado" ou "novo"
        public int? DbId { get; set; }
        public string? DescricaoDb { get; set; }
        public int? CategoriaIdSugerida { get; set; }
        public string? CategoriaNomeSugerida { get; set; }
        public TipoLancamento? TipoSugerido { get; set; }
    }

    public class SugestaoDescricaoDto
    {
        public int CategoriaId { get; set; }
        public string CategoriaNome { get; set; } = string.Empty;
        public TipoLancamento Tipo { get; set; }
    }
}
