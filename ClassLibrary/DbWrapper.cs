using MySql.Data.MySqlClient;
using System.Data;

public class DbWrapper
{
    private static DbWrapper? Instance = null;
    private MySqlConnection connection = new("");
    private string connString = "";

    public static DbWrapper Wrapper
    {
        get
        {
            if (Instance == null)
                Instance = new DbWrapper();
            return Instance;
        }
    }

    public bool IsOpen => connection.State == ConnectionState.Open;

    private DbWrapper()
    {
        string[] db_args = File.ReadAllLines(
            Environment.GetFolderPath(Environment.SpecialFolder.DesktopDirectory) +
            "/db_args.txt");

        string server = db_args[1];
        string db = db_args[2];
        string userId = db_args[3];
        string password = db_args[4];

        connString = $"Server={server};Database={db};User ID={userId};Password={password};";
        connection = new MySqlConnection(connString);
    }

    public void Open() => connection.Open();
    public void Close() => connection.Close();

    public DataTable RunQuery(string sqlString)
    {
        MySqlDataAdapter adapter = new MySqlDataAdapter(sqlString, connection);
        DataTable table = new DataTable();
        adapter.Fill(table);
        return table;
    }

    public int RunNonQuery(string sqlString)
    {
        int numRecords = 0;
        MySqlCommand command = new(sqlString, connection);
        try
        {
            if (!IsOpen) Open();
            numRecords = command.ExecuteNonQuery();
            Close();
        }
        catch
        {
            Close();
            throw;
        }
        return numRecords;
    }

    public object? RunQueryScalar(string sqlString)
    {
        object? obj = null;
        MySqlCommand command = new(sqlString, connection);

        try
        {
            if (!IsOpen) Open();
            obj = command.ExecuteScalar();
            Close();
        }
        catch
        {
            Close();
            throw;
        }

        return obj;
    }
}
