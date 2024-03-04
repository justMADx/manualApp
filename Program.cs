using manualApp.Entities;

public class Program
{
    public static void Main(string[] args)
    {
        EmployeeDatabaseManager databaseManager = new EmployeeDatabaseManager();
        
        databaseManager.CreateTable();
        
        Employee newEmployee = new Employee
        {
            FullName = "Иванов Иван Иванович",
            BirthDate = new DateTime(1990, 5, 15),
            Gender = 'М'
        };
        databaseManager.InsertEmployee(newEmployee);
        
        List<Employee> allEmployees = databaseManager.GetEmployees();
        foreach (var employee in allEmployees)
        {
            Console.WriteLine($"ФИО: {employee.FullName}, Дата рождения: {employee.BirthDate.ToShortDateString()}, Пол: {employee.Gender}");
            employee.CalculateAge();
        }

        databaseManager.FillEmployeesAutomatically(1000000);
        
        List<Employee> additionalEmployees = new List<Employee>();
        Random random = new Random();
        for (int i = 0; i < 100; i++)
        {
            string fullName = $"F{(char)('A' + random.Next(26))} LastName";
            DateTime birthDate = new DateTime(random.Next(1950, 2000), random.Next(1, 13), random.Next(1, 29));

            Employee employee = new Employee
            {
                FullName = fullName,
                BirthDate = birthDate,
                Gender = 'M'
            };

            additionalEmployees.Add(employee);
        }

        databaseManager.InsertEmployeesBatch(additionalEmployees);

        Console.WriteLine("Данные успешно добавлены в базу данных.");
        
        databaseManager.MeasureExecutionTimeForQuery();
    }
}