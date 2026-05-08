using System.ComponentModel;

namespace Rupestre.Domain.Enums;

public enum StatusVenda
{
    [Description("Cancelada")]
    Cancelada = 0,

    [Description("Concluída")]
    Concluida = 1,

    [Description("Pendente")]
    Pendente = 2
}
