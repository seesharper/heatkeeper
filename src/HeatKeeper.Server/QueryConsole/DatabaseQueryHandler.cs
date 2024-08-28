namespace HeatKeeper.Server.QueryConsole;

[RequireAdminRole]
[Post("api/queryconsole")]
public record DatabaseQuery(string Sql) : IQuery<Table>;

public class DatabaseQueryHandler(IDbConnection dbConnection) : IQueryHandler<DatabaseQuery, Table>
{
    public async Task<Table> HandleAsync(DatabaseQuery query, CancellationToken cancellationToken = default)
    {
        var dataReader = await dbConnection.ExecuteReaderAsync(query.Sql);
        var fieldCount = dataReader.FieldCount;
        Column[] columns = new Column[fieldCount];
        for (int i = 0; i < fieldCount; i++)
        {
            columns[i] = new Column(dataReader.GetName(i));
        }
        List<Row> rows = [];
        while (dataReader.Read())
        {
            Cell[] cells = new Cell[fieldCount];
            for (int i = 0; i < fieldCount; i++)
            {
                if (dataReader.IsDBNull(i))
                {
                    cells[i] = new Cell(null);
                    continue;
                }

                cells[i] = new Cell(dataReader.GetValue(i));
            }

            rows.Add(new Row(cells.ToArray()));
        }
        return new Table(columns, rows.ToArray());
    }
}

public record Column(string Name);

public record Cell(object Value);

public record Row(params Cell[] Cells);

public record Table(Column[] Columns, Row[] Rows);