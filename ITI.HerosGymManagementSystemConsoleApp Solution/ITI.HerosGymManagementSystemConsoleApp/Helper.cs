using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace ITI.HerosGymManagementSystemConsoleApp
{
    internal static class Helper
    {
        public static string GetHiddenInput()
        {

            Console.Write("Enter the User Password: ");
            string password = "";
            ConsoleKeyInfo key;

            do
            {
                key = Console.ReadKey(true);

                if (char.IsLetterOrDigit(key.KeyChar) || char.IsSymbol(key.KeyChar) || char.IsPunctuation(key.KeyChar))
                {
                    password += key.KeyChar;
                    Console.Write("*");
                }
                else if (key.Key == ConsoleKey.Backspace && password.Length > 0)
                {
                    password = password.Substring(0, password.Length - 1);
                    Console.Write("\b \b");
                }
            } while (key.Key != ConsoleKey.Enter);

            return password;
        }
        public static void PrintDataTable(DataTable dataTable)
        {

            foreach (DataColumn column in dataTable.Columns)
            {
                Console.Write($"{column.ColumnName,-20}");
            }
            Console.WriteLine("\n");


            foreach (DataRow row in dataTable.Rows)
            {
                foreach (var item in row.ItemArray)
                {
                    Console.Write($"{item,-20}");
                }
                Console.WriteLine();
            }
        }
        public static void GetUserTravelOnApp(SqlConnection connection, int UserId)
        {
            int option;
            bool Flag;
            string Msg = "[1] Members\n[2] Coaches\n[3] Memberships\n[4] Programs\n[5] Payments\n[6] Users\n[7] Exist..";
            Console.WriteLine(Msg);

            do
            {
                Console.WriteLine("Choose One Option:");
                Flag = int.TryParse(Console.ReadLine(), out option);
                Console.Clear();
                if (!Flag)
                    Console.WriteLine(Msg);
            } while (!Flag);

            

            switch (option)
            {
                case 1:
                    {
                        Members members = new Members(connection, UserId);
                        members.Options();
                    }
                    break;
                case 2:
                    {
                        Coaches coaches = new Coaches(connection, UserId);
                        coaches.ShowUserMenu();
                    }
                    break;
                case 3:
                    MemberShip.ExcutingMemberShipModelOptions(connection, UserId);
                    break;
                case 4:
                    GymProgram.ExcutingProgramModelOptions(connection, UserId);
                    break;
                case 5:
                    {
                        Payment payment =new Payment(connection, UserId);
                        payment.ShowUserMenu();
                    }
                    break;
                case 6:
                    User.ExcutingUserModelOptions(connection, UserId);
                    break;
                case 7:
                    Environment.Exit(0);
                    break;
                default:
                    Console.Clear();
                    GetUserTravelOnApp(connection, UserId);
                    break;

            }
        }
        public static int DisplayMemberShipOptionsToUser()
        {
            int option;

            Console.WriteLine("Choose One Option..");
            Console.WriteLine("[1] Create a new Membership.");
            Console.WriteLine("[2] Update a specific Membership.");
            Console.WriteLine("[3] Read All Memberships.");
            Console.WriteLine("[4] Delete a specific Membership.");
            Console.WriteLine("[5] Read All Deleted Memberships.");
            Console.WriteLine("[6] Return..");

            int.TryParse(Console.ReadLine(), out option);

            Console.Clear();

            return option;

        }
        public static int DisplayProgramsOptionsToUser()
        {
            int option;

            Console.WriteLine("Choose One Option..");
            Console.WriteLine("[1] Create a new Program.");
            Console.WriteLine("[2] Update a specific program.");
            Console.WriteLine("[3] Read all programs.");
            Console.WriteLine("[4] Delete a specific program.");
            Console.WriteLine("[5] Read All Deleted programs.");
            Console.WriteLine("[6] Return..");

            int.TryParse(Console.ReadLine(), out option);

            Console.Clear();

            return option;

        }
        public static int DisplayUsersOptionsToUser()
        {
            int option;

            Console.WriteLine("Choose One Option..");
            Console.WriteLine("[1] Show all users.");
            Console.WriteLine("[2] Edit your info.");
            Console.WriteLine("[3] Return..");

            int.TryParse(Console.ReadLine(), out option);

            Console.Clear();
            return option;
        }
        public static bool IsValidEmail(string email)
        {


            string pattern = @"^[a-zA-Z0-9._%+-]+@[a-zA-Z0-9.-]+\.[a-zA-Z]{2,}$";
            Regex regex = new Regex(pattern);

            return regex.IsMatch(email);
        }
        public static bool IsValidName(string name)
        {


            string pattern = @"^[a-zA-Z\s'-]+$";
            Regex regex = new Regex(pattern);

            return regex.IsMatch(name);
        }
        public static string? GetUserName(SqlConnection connection, int UserId)
        {
            SqlCommand command = new SqlCommand($"select Name from Users where Id = {UserId}", connection);
            return command.ExecuteScalar().ToString();
        }
        public static bool IsValidPhoneNumber(string phoneNumber)
        {
            // Validate the phone number format based on your specific requirements
            // For example, you might want to check if it contains only digits and has a specific length
            if (phoneNumber.Length != 11)
            {
                return false;
            }
            string[] validPrefixes = { "010", "011", "012", "015" };
            bool isValidPrefix = false;

            foreach (string prefix in validPrefixes)
            {
                if (phoneNumber.StartsWith(prefix))
                {
                    isValidPrefix = true;
                    break;
                }
            }
            bool isValidFormat = Regex.IsMatch(phoneNumber, "^[0-9]+$");
            return isValidPrefix && isValidFormat;
        }
        public static bool ValidateDates(DateTime startDate, DateTime endDate)
        {

            if (endDate <= startDate)
            {
                Console.WriteLine("End date must be later than the start date.");
                return false;
            }

            if ((endDate - startDate).TotalDays <= 30)
            {
                Console.WriteLine("The difference between start date and end date must be more than one month.");
                return false;
            }

            return true;
        }
        public static bool ProgramIdExists(SqlConnection connection, int ProgramID)
        {
            string sqlQuery = $"SELECT COUNT(*) FROM Programs WHERE Id = {ProgramID}";
            SqlCommand command = new SqlCommand(sqlQuery, connection);
            int count = (int)command.ExecuteScalar();
            return count > 0;

        }
        public static bool CoachIdExists(SqlConnection connection, int coachId)
        {
            string sqlQuery = $"SELECT COUNT(*) FROM coaches WHERE Id = {coachId}";
            SqlCommand command = new SqlCommand(sqlQuery, connection);
            int count = (int)command.ExecuteScalar();
            return count > 0;

        }
        public static bool CoachNameExists(SqlConnection connection, string name)
        {
            string sqlQuery = $"SELECT COUNT(*) FROM coaches WHERE Name = '{name}'";
            SqlCommand command = new SqlCommand(sqlQuery, connection);
            int count = (int)command.ExecuteScalar();
            return count > 0;

        }
    }
}
