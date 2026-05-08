using System.ComponentModel;

namespace Rupestre.Domain.Enums;

public enum StatusDespesa
{
    [Description("Pago")]
    Pago = 1,

    [Description("Pendente")]
    Pendente = 2,

    [Description("Cancelada")]
    Cancelada = 3
}
