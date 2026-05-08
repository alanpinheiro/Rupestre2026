using System.ComponentModel;

namespace Rupestre.Domain.Enums;

public enum TipoPagamento
{
    [Description("Produto")]
    Produto = 1,

    [Description("Frete")]
    Frete = 2
}
