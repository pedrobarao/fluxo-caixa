using FC.Consolidado.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace FC.Consolidado.Infra.Data.Mappings;

public class TransacaoMapping : IEntityTypeConfiguration<Transacao>
{
    public void Configure(EntityTypeBuilder<Transacao> builder)
    {
        builder.ToTable("Transacoes");

        builder.HasKey(c => c.Id);

        builder.Property(c => c.DataHora)
            .IsRequired()
            .HasColumnType("datetime");

        builder.Property(c => c.Descricao)
            .IsRequired()
            .HasColumnType("nvarchar(250)");

        builder.Property(c => c.Tipo)
            .IsRequired()
            .HasColumnType("varchar(20)");

        builder.Property(c => c.Valor)
            .IsRequired()
            .HasColumnType("number(21,2)");
    }
}