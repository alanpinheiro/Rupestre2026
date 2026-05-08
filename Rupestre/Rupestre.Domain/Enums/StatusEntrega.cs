using System.ComponentModel;

namespace Rupestre.Domain.Enums;

public enum StatusEntrega
{
    [Description("Entregue")]
    Entregue = 1,

    [Description("Agendada")]
    Agendada = 2,

    [Description("Atrasada")]
    Atrasada = 3
}
