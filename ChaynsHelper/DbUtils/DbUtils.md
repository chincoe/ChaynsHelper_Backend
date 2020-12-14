## DbUtils
A static helper for repositories with the following features:
* Simple one-to-many mapping with dapper
* Convert IEnumerables to DataTables to pass Lists to a stored procedure *by Eike Wolff*

### OneToManyMapper
```c#
// Model

// Add attributes to tell the mapper what the key and list of your parent object is
public class ParentModel {
    // [...]

    [OneToManyMapping(MappingAttributeType.Key)]
    public int Id { get; set; }

    [OneToManyMapping(MappingAttributeType.List)]
    public IEnumerable<ListModel> { get; set; }
}

// In Repository
const string query = @"
SELECT  p.Id            // parentId
        p.Column,       // parent columns
        l.ListModelId,  // list id (CANNOT be named Id)
        l.Data          // list columns
FROM Parent p
JOIN List l on p.ListId = l.ListModelId";

using var con = await _dbContext.GetDBContext()
var result = await con.QueryAsync(
    query,
    DbUtils.CreateOneToManyMapper<ParentModel, ListModel>(), 
    param,
    splitOn: "ListModelId"
);
return result.Distinct();
```

The code takes a query that produces a result with redundant ParentModels for each ListModel and returns a List of ParentModels each with a list of their respective ListModels.

### ToDataTable(this IEnumerable<T> iList)
 * Create a user defined type in your database
```tsql
CREATE TYPE [dbo].[udtMyUserDefinedType] AS TABLE
(
    [Id]   INT,
    [Data] NVARCHAR(128)
)
```
 * Declare your stored procedure
 
```tsql
CREATE PROCEDURE [dbo].[spMyGreatProcedure](
    @list [udtMyUserDefinedType] READONLY
)
AS
DECLARE @count INT = (
        SELECT Count([Id])
        FROM @list
        );
DECLARE @iteration INT = 0;
DECLARE @Id INT;
    WHILE (@iteration < @count)
        BEGIN
            SET @Id = (
                SELECT [Id]
                FROM @list
                ORDER BY [Id]
                OFFSET @iteration ROWS FETCH NEXT 1 ROW ONLY
                )
            SELECT * FROM @list WHERE [Id] = @Id
            -- ....
        END
GO;
```
 * Define the columns in the model using the ColumnAttribute
```c#
public class MyModel 
{
    [Column]
    public int Id { get; set; }
    [Column]
    public string Data { get; set; }
    
    public string ExcludedData { get; set; }
}
```
 * Use this helper to pass a DataTable
```c#
public async Task<MyModel> AccessDatabaseWithList(IEnumerable<MyModel> list)
{
    const string cmd = "spMyGreatProcedure";
    var param = new DynamicParameters();
    var dataTable = list.ToDataTable();
    param.Add("list", dataTable.AsTableValuedParameter("udtMyUserDefinedType"));
    /* ... */
}
```