using FC.Core.Communication;
using FC.Core.Mediator;
using FC.MessageBus;

namespace FC.Lancamentos.Api.Application.Commands;

public class NovaTransacaoCommandHandler(IMessageBus bus) : IRequestHandler<NovaTransacaoCommand, Result>
{
    public async Task<Result> Handle(NovaTransacaoCommand request, CancellationToken cancellationToken)
    {
        // var operacao = new Operacao(request.Valor, request.Descricao, request.Tipo);
        //
        // await bus.Publish(LancamentoRealizadoEvent.Create(operacao), cancellationToken);

        return Result.Success();
    }
}