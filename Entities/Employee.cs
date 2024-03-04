namespace manualApp.Entities;

public class Employee
{
    public Employee()
    {
        
    }
    
    public string FullName { get; set; }
    public DateTime BirthDate { get; set; }
    public char Gender { get; set; }

    public void CalculateAge()
    {
        int age = DateTime.Today.Year - BirthDate.Year;
        if (BirthDate > DateTime.Today.AddYears(-age))
        {
            age--;
        }
        Console.WriteLine($"Возраст: {age} лет");
    }
}