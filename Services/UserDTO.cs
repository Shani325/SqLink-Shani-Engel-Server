namespace Services
{
    public class UserDTO
    {
        
        public string fullName { get; set; }
        public int yearOfBirth { get; set; }

        public string tz { get; set; }  

        public bool Check()
        {
            return (tz.Length == 9 && tz.All(char.IsDigit) && yearOfBirth <= DateTime.Now.Year);
        }
    }
}