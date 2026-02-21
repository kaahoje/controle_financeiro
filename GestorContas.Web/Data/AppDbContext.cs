using Microsoft.EntityFrameworkCore;
using GestorContas.Web.Models;

namespace GestorContas.Web.Data
{
    public class AppDbContext : DbContext
    {
        public AppDbContext(DbContextOptions<AppDbContext> options) : base(options)
        {
        }


        public DbSet<Lancamento> Lancamentos { get; set; } = null!;
        public DbSet<Categoria> Categorias { get; set; } = null!;
        public DbSet<Conta> Contas { get; set; } = null!;
        public DbSet<Prefeitura> Prefeituras { get; set; } = null!;

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            // Configure the decimal precision for the Valor property
            modelBuilder.Entity<Lancamento>()
                .Property(l => l.Valor)
                .HasPrecision(18, 2);

            // Configure the decimal precision for SaldoInicial in Conta
            modelBuilder.Entity<Conta>()
                .Property(c => c.SaldoInicial)
                .HasPrecision(18, 2);

            // Seed initial data for Categorias
            modelBuilder.Entity<Categoria>().HasData(
                new Categoria { Id = 1, Nome = "Salário" },
                new Categoria { Id = 2, Nome = "Aluguel" },
                new Categoria { Id = 3, Nome = "Energia" },
                new Categoria { Id = 4, Nome = "Água" },
                new Categoria { Id = 5, Nome = "Internet" },
                new Categoria { Id = 6, Nome = "Mercado" }
            );

            // Seed initial data for Contas
            modelBuilder.Entity<Conta>().HasData(
                new Conta { Id = 1, Nome = "Dinheiro", Descricao = "Dinheiro em espécie", SaldoInicial = 0, Ativa = true },
                new Conta { Id = 2, Nome = "Cartão de Crédito", Descricao = "Cartão de crédito principal", SaldoInicial = 0, Ativa = true },
                new Conta { Id = 3, Nome = "Cartão de Débito", Descricao = "Cartão de débito", SaldoInicial = 0, Ativa = true },
                new Conta { Id = 4, Nome = "PIX", Descricao = "Transferências via PIX", SaldoInicial = 0, Ativa = true },
                new Conta { Id = 5, Nome = "Conta Corrente", Descricao = "Conta corrente bancária", SaldoInicial = 0, Ativa = true },
                new Conta { Id = 6, Nome = "Poupança", Descricao = "Conta poupança", SaldoInicial = 0, Ativa = true }
            );

            base.OnModelCreating(modelBuilder);
        }
    }
}
