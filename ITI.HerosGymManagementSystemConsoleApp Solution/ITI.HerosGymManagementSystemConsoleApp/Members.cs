using System.Data.SqlClient;
namespace ITI.HerosGymManagementSystemConsoleApp
{
    public class Members
    {

        SqlConnection connection;
        int UserId;

        #region methods
        public Members(SqlConnection _connection, int _UserId)
        {
            this.connection = _connection;
            UserId = _UserId;
        }
        public bool IsMemberExistsByPhoneNumber(string phoneNumber)
        {


            string query = "SELECT 1 FROM Members m " +
                           "INNER JOIN Member_Phones mp ON m.Id = mp.Id " +
                           "WHERE mp.Phone = @PhoneNumber";

            SqlCommand cmd = new SqlCommand(query, connection);
            cmd.Parameters.AddWithValue("@PhoneNumber", phoneNumber);

            return cmd.ExecuteScalar() != null;
        }
        public void Options()
        {
            while (true)
            {
                Console.WriteLine("Choose an operation:");
                Console.WriteLine("1. Add New Member");
                Console.WriteLine("2. Search Member");
                Console.WriteLine("3. Update Member");
                Console.WriteLine("4. Add New Member Program");
                Console.WriteLine("5. Return..");
                Console.Write("Enter your choice: ");
                string? choice = Console.ReadLine();

                switch (choice)
                {
                    case "1":
                        InsertMemberFromUserInput(1);
                        break;
                    case "2":
                        SearchMember();
                        break;
                    case "3":
                        InsertMemberFromUserInput(2);
                        break;
                    case "4":
                        UserInputsForProgram();
                        break;
                    case "5":
                        Console.Clear();
                        Helper.GetUserTravelOnApp(connection, UserId);
                        break;
                    default:
                        Console.WriteLine("Invalid choice!!");
                        Console.Clear();
                        break;
                }
            }
        }
        public void SearchMember()
        {
            Console.Clear();
            Console.WriteLine("Enter the phone number:");
            string? phone = Console.ReadLine();
            while (!Helper.IsValidPhoneNumber(phone))
            {
                Console.WriteLine("Enter the vaild phone number !");
                phone = Console.ReadLine();
            }
            if (IsMemberExistsByPhoneNumber(phone))
            {
                Console.WriteLine("this member is exists !!");
            }
            else
            {
                Console.WriteLine("this member is not exists ");
            }
            
        }
        public void InsertMemberFromUserInput(int Flag)
        {
            Console.Clear();
            Console.WriteLine("Enter the phone number:");
            string? phone = Console.ReadLine();
            while (!Helper.IsValidPhoneNumber(phone))
            {
                Console.WriteLine("Enter the valid phone number:");
                phone = Console.ReadLine();
            }
            bool IsMemberFound = IsMemberExistsByPhoneNumber(phone);
            if (IsMemberFound && Flag == 1)
            {
                Console.WriteLine("this member is exists before!!");
                return;
            }
            if (!IsMemberFound && Flag == 2)
            {
                Console.WriteLine("this member is not exists before");
                return;
            }
            string? name;
            string? email;
            Console.WriteLine("Enter Member Details:");

            while (true)
            {
                Console.Write("Enter Member Name: ");
                name = Console.ReadLine();
                if (Helper.IsValidName(name)) break;

            }
            while (true)
            {
                Console.Write("Enter Member Email: ");
                email = Console.ReadLine();
                if (Helper.IsValidEmail(email)) break;
            }

            Console.Write("Enter Member Age: ");
            int age;
            while (!int.TryParse(Console.ReadLine(), out age) || age <= 0)
            {
                Console.WriteLine("Invalid age!! Enter a valid age..");
            }

            Console.Write("Gender (M/F): ");
            char gender;
            while (!char.TryParse(Console.ReadLine(), out gender) || (char.ToLower(gender) != 'm' && char.ToLower(gender) != 'f'))
            {
                Console.WriteLine("Invalid gender!!");
            }
            MemberShip.ReturnAllMembershipSNotCompleted(connection, UserId);
            Console.Write("Enter The Membership Id: ");
            int membershipId;
            while (!int.TryParse(Console.ReadLine(), out membershipId) || membershipId <= 0)
            {
                Console.WriteLine("Invalid Membership Id!! Please enter a valid one..");
            }

            GymProgram.GetAllPrograms(connection, UserId);
            int programId;
            Console.Write("Enter The Program Id: ");
            while (!int.TryParse(Console.ReadLine(), out programId) || programId <= 0)
            {
                Console.WriteLine("Invalid Program Id!! Please enter a valid one..");
            }
            DateTime startDate; DateTime endDate;
            while (true)
            {
                Console.Write("Enter  start Date (yyyy-MM-dd): ");
                while (!DateTime.TryParseExact(Console.ReadLine(), "yyyy-MM-dd", null, System.Globalization.DateTimeStyles.None, out startDate))
                {
                    Console.WriteLine("Invalid input. Enter a valid date in the format yyyy-MM-dd.");
                    Console.Write("Enter start Date (yyyy-MM-dd): ");
                }

                Console.Write("Enter end Date (yyyy-MM-dd): ");
                while (!DateTime.TryParseExact(Console.ReadLine(), "yyyy-MM-dd", null, System.Globalization.DateTimeStyles.None, out endDate))
                {
                    Console.WriteLine("Invalid input. Enter a valid date in the format yyyy-MM-dd.");
                    Console.Write("Enter end Date (yyyy-MM-dd): ");
                }
                if (Helper.ValidateDates(startDate, endDate))
                    break;
            }
            InsertMember(name, email, age, gender, UserId, membershipId, programId, phone, startDate, endDate, Flag);
        }
        public void InsertMember(string name, string email, int age, char gender, int userId, int membershipId, int programId, string phone, DateTime startDate, DateTime endDate, int flag)
        {
            
            try
            {
                string insertQuery = "INSERT INTO Members (Name, Email, Age, Gender, User_Id, Membership_Id) " +
                                     "VALUES (@Name, @Email, @Age, @Gender, @User_Id, @Membership_Id); " +
                                     "SELECT SCOPE_IDENTITY();";

                SqlCommand cmd = new SqlCommand(insertQuery, connection);

                cmd.Parameters.AddWithValue("@Name", name);
                cmd.Parameters.AddWithValue("@Email", email);
                cmd.Parameters.AddWithValue("@Age", age);
                cmd.Parameters.AddWithValue("@Gender", gender);
                cmd.Parameters.AddWithValue("@User_Id", userId);
                cmd.Parameters.AddWithValue("@Membership_Id", membershipId);

                int memberId = Convert.ToInt32(cmd.ExecuteScalar());


                int res1 = InsertIntoMemberPhones(memberId, phone);
                int res2 = InsertIntoMemberPrograms(memberId, programId, startDate, endDate);

                if (flag == 1)
                {
                    if (res1 > 0 && res2 > 0)
                    {
                        Console.WriteLine("Member added successfully");
                        CalcPayment(startDate, endDate, programId, memberId);

                    }
                    else
                        Console.WriteLine("Member not  added ");
                }
                else
                {
                    if (res1 > 0 && res2 > 0)
                        Console.WriteLine("Member updated successfully");
                    else
                        Console.WriteLine("Member not updated ");
                }
            }
            catch (Exception e)
            {
                Console.WriteLine(e.Message);
            }


        }
        public int InsertIntoMemberPhones(int memberId, string phone)
        {
            string insertQuery = "INSERT INTO Member_Phones (Id, Phone) VALUES (@Member_Id, @Phone);";

            using (SqlCommand cmd = new SqlCommand(insertQuery, connection))
            {
                cmd.Parameters.AddWithValue("@Member_Id", memberId);
                cmd.Parameters.AddWithValue("@Phone", phone);

                return cmd.ExecuteNonQuery();
            }

        }
        public void CalcPayment(DateTime startDate, DateTime endDate, int programId, int memberId)
        {

            int numberOfMonths = ((endDate.Year - startDate.Year) * 12) + endDate.Month - startDate.Month;


            string query = "SELECT Salary FROM Programs WHERE Id = @ProgramId";
            decimal salary = 0;
            using (SqlCommand command = new SqlCommand(query, connection))
            {
                command.Parameters.AddWithValue("@ProgramId", programId);
                object result = command.ExecuteScalar();
                salary = (decimal)result;
                decimal totalAmount = numberOfMonths * salary;
                Payment payment = new Payment(connection, UserId);
                payment.Time = DateTime.Now.TimeOfDay;
                payment.Date = DateTime.Now.Date;
                payment.Amount = totalAmount;
                payment.Member_Id = memberId;
                payment.User_Id = UserId;
                payment.CreatePayment(payment);
            }



        }
        public void UserInputsForProgram()
        {
            Console.Clear();
            int memberId;
            Console.Write("Enter The Member Id: ");
            while (!int.TryParse(Console.ReadLine(), out memberId) || memberId <= 0)
            {
                Console.WriteLine("Invalid Member Id!!");
            }
            int programId;
            Console.Write("Enter The Program Id: ");
            while (!int.TryParse(Console.ReadLine(), out programId) || programId <= 0)
            {
                Console.WriteLine("Invalid Program Id!!");
            }
            DateTime startDate;
            Console.Write("Enter start Date (yyyy-MM-dd): ");
            while (!DateTime.TryParseExact(Console.ReadLine(), "yyyy-MM-dd", null, System.Globalization.DateTimeStyles.None, out startDate))
            {
                Console.WriteLine("Invalid input!! Please enter a valid date in the format yyyy-MM-dd.");
                Console.Write("Enter start Date (yyyy-MM-dd): ");
            }
            DateTime endDate;
            Console.Write("Enter end Date (yyyy-MM-dd): ");
            while (!DateTime.TryParseExact(Console.ReadLine(), "yyyy-MM-dd", null, System.Globalization.DateTimeStyles.None, out endDate))
            {
                Console.WriteLine("Invalid input!! Please enter a valid date in the format yyyy-MM-dd.");
                Console.Write("Enter end Date (yyyy-MM-dd): ");
            }
            int res = InsertIntoMemberPrograms(memberId, programId, startDate, endDate);
            if (res > 0)
            {
                Console.WriteLine("The program added successfully");
            }
            else
            {
                Console.WriteLine("The program not added ");
            }
        }
        public int InsertIntoMemberPrograms(int memberId, int programId, DateTime startDate, DateTime endDate)
        {
            try
            {

                string insertQuery = "INSERT INTO Member_Programs (Member_Id, Program_Id, StartDate, EndDate) " +
                                     "VALUES (@Member_Id, @Program_Id, @StartDate, @EndDate);";

                using (SqlCommand cmd = new SqlCommand(insertQuery, connection))
                {
                    cmd.Parameters.AddWithValue("@Member_Id", memberId);
                    cmd.Parameters.AddWithValue("@Program_Id", programId);
                    cmd.Parameters.AddWithValue("@StartDate", startDate);
                    cmd.Parameters.AddWithValue("@EndDate", endDate);

                    return cmd.ExecuteNonQuery();
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine("Enter vaild id ");
                return -1;
            }

        }
        #endregion
    }

}
