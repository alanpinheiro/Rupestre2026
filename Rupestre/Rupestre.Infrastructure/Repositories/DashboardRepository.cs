using Dapper;
using Rupestre.Domain.Interfaces;
using Rupestre.Infrastructure.Data;

namespace Rupestre.Infrastructure.Repositories;

public class DashboardRepository : IDashboardRepository
{
    private readonly ConnectionManager _connectionManager;

    public DashboardRepository(ConnectionManager connectionManager)
    {
        _connectionManager = connectionManager;
    }

    public async Task<DashboardData> GetDashboardDataAsync()
    {
        const string sql = @"
-- Cards
SELECT
    ISNULL((
        SELECT SUM(vp.SubTotal)
        FROM VendaProduto vp
        INNER JOIN Venda v ON v.Id = vp.Venda_Id
        WHERE CAST(v.DataVenda AS DATE) = CAST(GETDATE() AS DATE)
          AND v.StatusVenda <> 0
    ), 0) AS VendasHoje,
    (
        SELECT COUNT(*)
        FROM Venda
        WHERE StatusEntrega IN (2, 3)
          AND StatusVenda <> 0
    ) AS MoveisAEntregar,
    (
        SELECT COUNT(*)
        FROM Venda
        WHERE CAST(DataEntrega AS DATE) = CAST(GETDATE() AS DATE)
          AND StatusEntrega <> 1
          AND StatusVenda <> 0
    ) AS MontagensParaHoje;

-- Faturamento vs Recebimento (últimos 6 meses)
WITH Meses AS (
    SELECT 0 AS n UNION ALL SELECT 1 UNION ALL SELECT 2
    UNION ALL SELECT 3 UNION ALL SELECT 4 UNION ALL SELECT 5
),
MesesBase AS (
    SELECT
        YEAR(DATEADD(MONTH, -n, GETDATE())) * 100 + MONTH(DATEADD(MONTH, -n, GETDATE())) AS MesOrdem,
        FORMAT(DATEADD(MONTH, -n, GETDATE()), 'MM/yyyy') AS MesLabel
    FROM Meses
),
Fat AS (
    SELECT YEAR(v.DataVenda) * 100 + MONTH(v.DataVenda) AS MesOrdem,
           SUM(vp.SubTotal) AS Faturamento
    FROM Venda v
    INNER JOIN VendaProduto vp ON vp.Venda_Id = v.Id
    WHERE v.DataVenda >= DATEFROMPARTS(YEAR(DATEADD(MONTH,-5,GETDATE())), MONTH(DATEADD(MONTH,-5,GETDATE())), 1)
      AND v.StatusVenda <> 0
    GROUP BY YEAR(v.DataVenda) * 100 + MONTH(v.DataVenda)
),
Rec AS (
    SELECT YEAR(v.DataVenda) * 100 + MONTH(v.DataVenda) AS MesOrdem,
           SUM(pg.ValorLiquidoPagamento) AS Recebimento
    FROM Venda v
    INNER JOIN VendaPagamento pg ON pg.Venda_Id = v.Id
    WHERE v.DataVenda >= DATEFROMPARTS(YEAR(DATEADD(MONTH,-5,GETDATE())), MONTH(DATEADD(MONTH,-5,GETDATE())), 1)
      AND v.StatusVenda <> 0
    GROUP BY YEAR(v.DataVenda) * 100 + MONTH(v.DataVenda)
)
SELECT m.MesLabel AS Mes,
       ISNULL(f.Faturamento, 0) AS Faturamento,
       ISNULL(r.Recebimento, 0) AS Recebimento
FROM MesesBase m
LEFT JOIN Fat f ON f.MesOrdem = m.MesOrdem
LEFT JOIN Rec r ON r.MesOrdem = m.MesOrdem
ORDER BY m.MesOrdem;

-- Top 10 produtos
SELECT TOP 10 p.Nome, SUM(vp.Quantidade) AS QuantidadeVendida, p.Estoque
FROM VendaProduto vp
INNER JOIN Produto p ON p.Id = vp.Produto_Id
INNER JOIN Venda v ON v.Id = vp.Venda_Id
WHERE v.StatusVenda <> 0
GROUP BY p.Nome, p.Estoque
ORDER BY QuantidadeVendida DESC;";

        using var conn = _connectionManager.CreateConnection();
        using var multi = await conn.QueryMultipleAsync(sql);

        var cards = await multi.ReadFirstAsync<(decimal VendasHoje, int MoveisAEntregar, int MontagensParaHoje)>();
        var faturamento = await multi.ReadAsync<MesFaturamento>();
        var abc = await multi.ReadAsync<AbcItem>();

        return new DashboardData
        {
            VendasHoje = cards.VendasHoje,
            MoveisAEntregar = cards.MoveisAEntregar,
            MontagensParaHoje = cards.MontagensParaHoje,
            Faturamento = faturamento,
            Abc = abc
        };
    }
}
