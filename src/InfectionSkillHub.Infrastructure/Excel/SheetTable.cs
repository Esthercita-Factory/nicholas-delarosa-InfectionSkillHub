using ClosedXML.Excel;

namespace InfectionSkillHub.Infrastructure.Excel;

/// <summary>Worksheet read as a table: the first used row holds the column names.</summary>
internal sealed class SheetTable
{
    private readonly Dictionary<string, int> _columns;
    private readonly IXLWorksheet _sheet;
    private readonly int _headerRow;

    private SheetTable(IXLWorksheet sheet, int headerRow, Dictionary<string, int> columns)
    {
        _sheet = sheet;
        _headerRow = headerRow;
        _columns = columns;
    }

    public static SheetTable From(IXLWorksheet sheet)
    {
        var headerRow = sheet.FirstRowUsed();
        var columns = new Dictionary<string, int>(StringComparer.OrdinalIgnoreCase);

        if (headerRow is not null)
        {
            foreach (var cell in headerRow.CellsUsed())
            {
                columns.TryAdd(cell.GetFormattedString().Trim(), cell.Address.ColumnNumber);
            }
        }

        return new SheetTable(sheet, headerRow?.RowNumber() ?? 1, columns);
    }

    public IReadOnlyList<string> MissingColumns(params string[] required)
        => required.Where(name => !_columns.ContainsKey(name)).ToList();

    /// <summary>Data rows after the header, skipping the ones that are completely empty.</summary>
    public IEnumerable<SheetRow> Rows()
    {
        var lastRow = _sheet.LastRowUsed()?.RowNumber() ?? _headerRow;

        for (var number = _headerRow + 1; number <= lastRow; number++)
        {
            var row = _sheet.Row(number);
            if (!row.IsEmpty())
            {
                yield return new SheetRow(row, _columns);
            }
        }
    }
}

internal sealed class SheetRow
{
    private readonly IXLRow _row;
    private readonly IReadOnlyDictionary<string, int> _columns;

    public SheetRow(IXLRow row, IReadOnlyDictionary<string, int> columns)
    {
        _row = row;
        _columns = columns;
    }

    public int Number => _row.RowNumber();

    public IXLCell Cell(string column) => _row.Cell(_columns[column]);

    public string Text(string column) => Cell(column).GetFormattedString().Trim();
}
