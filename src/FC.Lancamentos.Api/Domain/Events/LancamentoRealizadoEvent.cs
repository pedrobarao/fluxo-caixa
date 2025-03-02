using FC.Core.Messages;
using FC.Lancamentos.Api.Domain.Entities;

namespace FC.Lancamentos.Api.Domain.Events;

public class LancamentoRealizadoEvent : Event
{
    public static LancamentoRealizadoEvent Create(Transacao payload)
    {
        return new LancamentoRealizadoEvent
        {
            Payload = payload
        };
    }
}