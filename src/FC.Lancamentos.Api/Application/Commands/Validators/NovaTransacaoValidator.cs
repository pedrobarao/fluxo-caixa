using FluentValidation;

namespace FC.Lancamentos.Api.Application.Commands.Validators;

public class NovaTransacaoValidator : AbstractValidator<NovaTransacaoCommand>
{
    public NovaTransacaoValidator()
    {
        RuleFor(x => x.Valor)
            .GreaterThan(0).WithMessage("O valor deve ser maior que zero.");

        RuleFor(x => x.Descricao)
            .NotEmpty().WithMessage("A descrição é obrigatória.")
            .MaximumLength(250).WithMessage("A descrição não pode ter mais que 250 caracteres.");

        RuleFor(x => x.Tipo)
            .NotEmpty().WithMessage("O tipo é obrigatório.");
    }
}