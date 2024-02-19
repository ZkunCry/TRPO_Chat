using System.Text;
using System.Security.Cryptography;
namespace WebApplication1
{
    public class Hash
    {
        private static string salt = "salt1235";
        public static string ComputeSHA256Hash(string input)
        {
            string saltedPassword = input + salt;

            // Преобразуем пароль и соль в байты
            byte[] saltedPasswordBytes = Encoding.UTF8.GetBytes(saltedPassword);

            // Создаем объект для хэширования пароля с помощью SHA256
            using (SHA256 sha256 = SHA256.Create())
            {
                // Вычисляем хэш
                byte[] hashBytes = sha256.ComputeHash(saltedPasswordBytes);

                // Преобразуем байты хэша в строку шестнадцатеричного представления
                StringBuilder stringBuilder = new StringBuilder();
                for (int i = 0; i < hashBytes.Length; i++)
                {
                    stringBuilder.Append(hashBytes[i].ToString("x2"));
                }

                return stringBuilder.ToString();
            }
        }
        public static bool ComparePasswords(string inputPassword, string hashedPassword)
        {
            // Хэшируем введенный пароль с использованием той же соли
            string hashedInputPassword = ComputeSHA256Hash(inputPassword);

            // Сравниваем полученный хэш с сохраненным хэшем пароля
            return hashedInputPassword == hashedPassword;
        }


    }
}
