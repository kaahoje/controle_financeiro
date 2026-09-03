using GestorContas.Web.Models;
using GestorContas.Web.Services.Conciliacao.Dtos;
using System.Collections.Generic;
using System.Linq;

namespace GestorContas.Web.Services.Conciliacao
{
    /// <summary>
    /// Estratégia de correspondência 1-para-1 exata baseada em Data, Valor e Tipo.
    /// </summary>
    public class MatchExatoConciliacaoStrategy : IConciliacaoExtratoStrategy
    {
        public ResultadoConciliacaoDto Conciliar(List<ItemExtratoDto> itensExtrato, List<Lancamento> lancamentosDb)
        {
            var resultado = new ResultadoConciliacaoDto();
            var matchedDbIds = new HashSet<int>();

            foreach (var item in itensExtrato)
            {
                var match = lancamentosDb.FirstOrDefault(e =>
                    !matchedDbIds.Contains(e.Id) &&
                    e.Data.Date == item.Data.Date &&
                    e.Valor == item.Valor &&
                    e.Tipo == item.Tipo);

                if (match != null)
                {
                    matchedDbIds.Add(match.Id);
                    resultado.Conciliados.Add(new ItemConciliadoDto
                    {
                        Extrato = item,
                        Sistema = new ItemSistemaDto
                        {
                            Id = match.Id,
                            Data = match.Data,
                            Descricao = match.Descricao,
                            Valor = match.Valor,
                            Tipo = match.Tipo,
                            CategoriaNome = match.Categoria?.Nome ?? "Sem Categoria"
                        }
                    });
                }
                else
                {
                    resultado.ApenasExtrato.Add(item);
                }
            }

            resultado.ApenasSistema = lancamentosDb
                .Where(e => !matchedDbIds.Contains(e.Id))
                .Select(e => new ItemSistemaDto
                {
                    Id = e.Id,
                    Data = e.Data,
                    Descricao = e.Descricao,
                    Valor = e.Valor,
                    Tipo = e.Tipo,
                    CategoriaNome = e.Categoria?.Nome ?? "Sem Categoria"
                })
                .ToList();

            return resultado;
        }

        public List<ResultadoVerificacaoDuplicadoDto> VerificarDuplicados(List<ItemExtratoDto> itensExtrato, List<Lancamento> lancamentosDb, List<Lancamento>? historicoCompletoDb = null)
        {
            var matchedIds = new HashSet<int>();
            var resultados = new List<ResultadoVerificacaoDuplicadoDto>();
            var baseHistorico = historicoCompletoDb ?? lancamentosDb;

            foreach (var item in itensExtrato)
            {
                var match = lancamentosDb.FirstOrDefault(e =>
                    !matchedIds.Contains(e.Id) &&
                    e.Data.Date == item.Data.Date &&
                    e.Valor == item.Valor &&
                    e.Tipo == item.Tipo);

                if (match != null)
                {
                    matchedIds.Add(match.Id);
                    resultados.Add(new ResultadoVerificacaoDuplicadoDto
                    {
                        Status = "duplicado",
                        DbId = match.Id,
                        DescricaoDb = match.Descricao,
                        CategoriaIdSugerida = match.CategoriaId,
                        CategoriaNomeSugerida = match.Categoria?.Nome
                    });
                }
                else
                {
                    // Buscar por aproximação:
                    // 1. Preferência por Descricao exata/contida
                    // 2. Segunda preferência por DescricaoNoExtrato exata/contida
                    Lancamento? melhorMatch = EncontrarMelhorMatchDescricao(item.Descricao, baseHistorico);

                    if (melhorMatch != null)
                    {
                        // Atualizar o Tipo do item importado para o tipo do lançamento localizado por aproximação
                        item.Tipo = melhorMatch.Tipo;

                        resultados.Add(new ResultadoVerificacaoDuplicadoDto
                        {
                            Status = "novo",
                            CategoriaIdSugerida = melhorMatch.CategoriaId,
                            CategoriaNomeSugerida = melhorMatch.Categoria?.Nome,
                            TipoSugerido = melhorMatch.Tipo
                        });
                    }
                    else
                    {
                        resultados.Add(new ResultadoVerificacaoDuplicadoDto
                        {
                            Status = "novo"
                        });
                    }
                }
            }

            return resultados;
        }

        private Lancamento? EncontrarMelhorMatchDescricao(string descricaoExtrato, List<Lancamento> lancamentos)
        {
            if (string.IsNullOrWhiteSpace(descricaoExtrato) || lancamentos == null || !lancamentos.Any())
                return null;

            var descNorm = NormalizarTexto(descricaoExtrato);
            if (string.IsNullOrEmpty(descNorm))
                return null;

            // 1ª Preferência: Descricao exata (ignorando caixa e acentos)
            var match1 = lancamentos.FirstOrDefault(l => NormalizarTexto(l.Descricao) == descNorm);
            if (match1 != null) return match1;

            // 2ª Preferência: DescricaoNoExtrato exata
            var match2 = lancamentos.FirstOrDefault(l => !string.IsNullOrEmpty(l.DescricaoNoExtrato) && NormalizarTexto(l.DescricaoNoExtrato) == descNorm);
            if (match2 != null) return match2;

            // 3ª Preferência: Descricao contida ou que contém (aproximação por sub-string)
            var match3 = lancamentos.FirstOrDefault(l => NormalizarTexto(l.Descricao).Contains(descNorm) || descNorm.Contains(NormalizarTexto(l.Descricao)));
            if (match3 != null) return match3;

            // 4ª Preferência: DescricaoNoExtrato contida ou que contém
            var match4 = lancamentos.FirstOrDefault(l => !string.IsNullOrEmpty(l.DescricaoNoExtrato) && (NormalizarTexto(l.DescricaoNoExtrato).Contains(descNorm) || descNorm.Contains(NormalizarTexto(l.DescricaoNoExtrato))));
            if (match4 != null) return match4;

            return null;
        }

        private string NormalizarTexto(string texto)
        {
            if (string.IsNullOrWhiteSpace(texto)) return string.Empty;
            return texto.Trim().ToLowerInvariant();
        }
    }
}
