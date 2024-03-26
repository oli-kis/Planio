namespace Planio.Services
{
    public class PasswordService
    {
        public string HashPassword(string password)
        {
            string salt = BCrypt.Net.BCrypt.GenerateSalt();


            string hashedPassword = BCrypt.Net.BCrypt.HashPassword(password, salt);

            return hashedPassword;
        }


        public bool VerifyPassword(string inputPassword, string password)
        {
            try
            {
                Console.WriteLine(inputPassword);
                Console.WriteLine(password);
                if (inputPassword == null)
                {
                    return false;
                }

                if (BCrypt.Net.BCrypt.Verify(password, inputPassword) == true)
                {
                    Console.WriteLine("Success");
                    return true;
                }

                Console.WriteLine("No Success");
                return false;


            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.ToString());
                return false;
            }
        }
    }
}
