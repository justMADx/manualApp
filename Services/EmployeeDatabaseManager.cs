using System.Data.SQLite;
using System.Globalization;

namespace manualApp.Entities;

public class EmployeeDatabaseManager
{
     private string connectionString = "Data Source=D:\\\\GitHub\\\\manualApp\\\\manualApp\\\\temp_db.db;Version=3;";

     public void CreateTable()
     {
         using (SQLiteConnection connection = new SQLiteConnection(connectionString))
         {
             connection.Open();
             
             string checkTableQuery = "SELECT name FROM sqlite_master WHERE type='table' AND name='Employees'";
             using (SQLiteCommand checkTableCommand = new SQLiteCommand(checkTableQuery, connection))
             {
                 object result = checkTableCommand.ExecuteScalar();
                 if (result != null && result.ToString() == "Employees")
                 {
                     Console.WriteLine("Таблица Employees уже существует.");
                     return;
                 }
             }
             
             string createTableQuery = "CREATE TABLE Employees (Id INTEGER PRIMARY KEY AUTOINCREMENT, FullName TEXT, BirthDate TEXT, Gender CHAR)";
             using (SQLiteCommand command = new SQLiteCommand(createTableQuery, connection))
             {
                 command.ExecuteNonQuery();
                 Console.WriteLine("Таблица Employees создана успешно.");
             }
         }
     }

    public void InsertEmployee(Employee employee)
    {
        using (SQLiteConnection connection = new SQLiteConnection(connectionString))
        {
            connection.Open();
            Console.WriteLine("Соединение с базой данных установлено.");

            string query = "INSERT INTO Employees (FullName, BirthDate, Gender) VALUES (@FullName, @BirthDate, @Gender)";
            using (SQLiteCommand command = new SQLiteCommand(query, connection))
            {
                command.Parameters.AddWithValue("@FullName", employee.FullName);
                command.Parameters.AddWithValue("@BirthDate", employee.BirthDate.Date.ToString("d"));
                command.Parameters.AddWithValue("@Gender", employee.Gender.ToString());

                int rowsAffected = command.ExecuteNonQuery();
                Console.WriteLine($"Добавлена запись: {rowsAffected}");
            }
        }
    }

    public List<Employee> GetEmployees()
    {
        List<Employee> employees = new List<Employee>();

        using (SQLiteConnection connection = new SQLiteConnection(connectionString))
        {
            connection.Open();
            Console.WriteLine("Соединение с базой данных установлено.");

            string query = "SELECT FullName, BirthDate, Gender FROM Employees ORDER BY FullName";
            using (SQLiteCommand command = new SQLiteCommand(query, connection))
            {
                using (SQLiteDataReader reader = command.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        string dateStr = reader["BirthDate"].ToString();
                        DateTime birthDate;
                        if (DateTime.TryParseExact(dateStr, "dd.MM.yyyy", CultureInfo.InvariantCulture, DateTimeStyles.None, out birthDate))
                        {
                            Employee employee = new Employee
                            {
                                FullName = reader["FullName"].ToString(),
                                BirthDate = birthDate,
                                Gender = Convert.ToChar(reader["Gender"])
                            };
                            employees.Add(employee);
                        }
                        else
                        {
                            Console.WriteLine($"Ошибка преобразования даты: {dateStr}");
                        }
                    }
                }
            }

            return employees;
        }
    }

    public void FillEmployeesAutomatically(int count)
    {
        List<Employee> employees = GenerateRandomEmployees(count);
        InsertEmployeesBatch(employees);
    }

    private List<Employee> GenerateRandomEmployees(int count)
    {
        var employees = new List<Employee>();
        Random random = new Random();

        for (int i = 0; i < count; i++)
        {
            string fullName = $"{(char)('A' + random.Next(26))}FirstName LastName";
            DateTime birthDate = new DateTime(random.Next(1950, 2000), random.Next(1, 13), random.Next(1, 29));
            char gender = random.Next(2) == 0 ? 'M' : 'F';

            Employee employee = new Employee
            {
                FullName = fullName,
                BirthDate = birthDate,
                Gender = gender
            };

            employees.Add(employee);
        }

        return employees;
    }

    public void InsertEmployeesBatch(List<Employee> employees)
    {
        using (SQLiteConnection connection = new SQLiteConnection(connectionString))
        {
            connection.Open();
            Console.WriteLine("Соединение с базой данных установлено.");

            string query = "INSERT INTO Employees (FullName, BirthDate, Gender) VALUES (@FullName, @BirthDate, @Gender)";
            using (SQLiteCommand command = new SQLiteCommand(query, connection))
            {
                foreach (Employee employee in employees)
                {
                    command.Parameters.AddWithValue("@FullName", employee.FullName);
                    command.Parameters.AddWithValue("@BirthDate", employee.BirthDate);
                    command.Parameters.AddWithValue("@Gender", employee.Gender);

                    command.ExecuteNonQuery();
                    command.Parameters.Clear();
                }

                Console.WriteLine($"Добавлено записей: {employees.Count}");
            }
        }
    }
    
    
    public void MeasureExecutionTimeForQuery()
    {
        DateTime startTime = DateTime.Now;

        using (SQLiteConnection connection = new SQLiteConnection(connectionString))
        {
            connection.Open();
            SQLiteCommand command = connection.CreateCommand();

            command.CommandText = "SELECT FullName, BirthDate, Gender FROM Employees WHERE Gender = 'M' AND FullName LIKE 'F%'";
            using (SQLiteDataReader reader = command.ExecuteReader())
            {
                while (reader.Read())
                {
                    string fullName = reader["FullName"].ToString();
                    DateTime birthDate = Convert.ToDateTime(reader["BirthDate"]);
                    string genderStr = reader["Gender"].ToString();
                    char gender = genderStr[0];
                    
                    Console.WriteLine($"Full Name: {fullName}, Birth Date: {birthDate}, Gender: {gender}");
                }
            }
        }

        DateTime endTime = DateTime.Now;
        TimeSpan executionTime = endTime - startTime;
        Console.WriteLine($"Время выполнения запроса: {executionTime.TotalMilliseconds} мс");
    }

}
