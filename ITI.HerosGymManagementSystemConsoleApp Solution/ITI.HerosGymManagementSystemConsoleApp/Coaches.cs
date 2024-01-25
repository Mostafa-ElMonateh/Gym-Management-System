﻿using System.Data.SqlClient;
using System.Data;
namespace ITI.HerosGymManagementSystemConsoleApp
{
    class Coaches
    {
        #region Fields
        private readonly SqlConnection connection;
        int UserId;
        #endregion

        #region Methods
        #region CTORs
        public Coaches(SqlConnection connection, int _UserId)
        {
            this.connection = connection;
            UserId = _UserId;
        }
        #endregion
        public void ShowUserMenu()
        {
            while (true)
            {
                Console.WriteLine("Choose an operation:");
                Console.WriteLine("1. Add Coach");
                Console.WriteLine("2. Search Coach");
                Console.WriteLine("3. Update Coach");
                Console.WriteLine("4. Delete Coach");
                Console.WriteLine("5. Get All Coachs");
                Console.WriteLine("6. Return");
                Console.Write("Enter  your choice: ");
                string? choice = Console.ReadLine();
                Console.Clear();
                switch (choice)
                {
                    case "1":
                        VaildUserInput(1);
                        break;
                    case "2":
                        SearchCoach();
                        break;
                    case "3":
                        VaildUserInput(2);
                        break;
                    case "4":
                        DeleteCoach();
                        break;
                    case "5":
                        GetAll();
                        break;
                    case "6":
                        Helper.GetUserTravelOnApp(connection, UserId);
                        break;
                    default:
                        Console.WriteLine("Invalid choice. Please enter a number between 1 and 5.");
                        break;
                }
            }
        }
        public void VaildUserInput(int Flag)
        {
            /* Ask coach for his data 
             * [ Name - Email - Program id - Phone - Id-->(update) - Address ] 
             */
            string? name, phone;
            string? email;
            int programId, userId = UserId, coachId = 0;
            if (Flag == 2)
            {

                Console.Write("Enter Coach ID: ");
                while (!int.TryParse(Console.ReadLine(), out coachId) || !(Helper.CoachIdExists(connection, coachId)))
                {
                    Console.WriteLine("Invalid Coach ID. Enter a valid integer.");
                }

            }
            while (true)
            {
                Console.Write("Enter Coach Name: ");
                name = Console.ReadLine();
                bool check = Helper.CoachNameExists(connection, name);
                if (check) Console.WriteLine("this Coach  name is existed");
                if (Helper.IsValidName(name) && !check)
                    break;

            }
            while (true)
            {
                Console.Write("Enter Coach Email: ");
                email = Console.ReadLine();
                if (Helper.IsValidEmail(email)) break;
            }



            while (true)
            {
                GymProgram.GetAllPrograms(connection, userId);
                Console.Write("Enter Program ID: ");
                bool ok = int.TryParse(Console.ReadLine(), out programId);
                bool exist = Helper.ProgramIdExists(connection, programId);
                if (ok && exist)
                    break;

            }


            while (true)
            {
                Console.Write("Enter Coach Phone: ");
                phone = Console.ReadLine();
                bool check = Helper.IsValidPhoneNumber(phone);
                if (check) break;
            }

            Console.Write("Enter Coach Address: ");
            string? address = Console.ReadLine();
            if (Flag == 1)
            {
                AddCoach(name, email, programId, userId, phone, address);

            }
            else
            {

                UpdateCoach(coachId, name, email, programId, userId, phone, address);
            }

        }
        public void AddCoach(string name, string email, int programId, int userId, string phone, string address)
        {


            try
            {


                // Insert into Coaches table
                string insertCoachQuery = "INSERT INTO Coaches (Name, Email, Program_Id, User_Id, IsDeleted) VALUES (@Name, @Email, @ProgramId, @UserId, 'f'); SELECT SCOPE_IDENTITY();";
                SqlCommand cmd = new SqlCommand(insertCoachQuery, connection);
                cmd.Parameters.AddWithValue("@Name", name);
                cmd.Parameters.AddWithValue("@Email", email);
                cmd.Parameters.AddWithValue("@ProgramId", programId);
                cmd.Parameters.AddWithValue("@UserId", userId);
                int coachId = Convert.ToInt32(cmd.ExecuteScalar());

                // Insert into Coach_Phones table
                string insertPhoneQuery = "INSERT INTO Coach_Phones (Id, Phone) VALUES (@CoachId, @Phone);";
                SqlCommand phoneCmd = new SqlCommand(insertPhoneQuery, connection);

                phoneCmd.Parameters.AddWithValue("@CoachId", coachId);
                phoneCmd.Parameters.AddWithValue("@Phone", phone);
                int result2 = phoneCmd.ExecuteNonQuery();


                // Insert into Coach_Addresses table
                string insertAddressQuery = "INSERT INTO Coach_Addresses (Id, Address) VALUES (@CoachId, @Address);";
                SqlCommand addressCmd = new SqlCommand(insertAddressQuery, connection);

                addressCmd.Parameters.AddWithValue("@CoachId", coachId);
                addressCmd.Parameters.AddWithValue("@Address", address);
                int result3 = addressCmd.ExecuteNonQuery();
                if (result2 > 0 && result3 > 0)
                    Console.WriteLine($"The coach inserted successfully and its id is {coachId}");
                else
                    Console.WriteLine($"The coach not inserted");

            }
            catch (Exception e)
            {
                Console.WriteLine($"Enter vaild id ");
            }

            ShowUserMenu();
        }
        public void DeleteCoach()
        {
            // Update IsDeleted in Coaches table
            try
            {
                int coachId;

                while (true)
                {
                    Console.Write("Enter Coach ID: ");
                    bool check = int.TryParse(Console.ReadLine(), out coachId);
                    bool ok = Helper.CoachIdExists(connection, coachId);
                    if (ok && check) break;


                }


                string updateQuery = "UPDATE Coaches SET IsDeleted = 'f' WHERE Id = @CoachId;";
                SqlCommand cmd = new SqlCommand(updateQuery, connection);
                cmd.Parameters.AddWithValue("@CoachId", coachId);
                int result2 = cmd.ExecuteNonQuery();
                if (result2 > 0)
                    Console.WriteLine($"The coach is deleted successfully ");
                else
                    Console.WriteLine($"The coach not deleted");
            }
            catch (Exception e)
            {
                Console.WriteLine($"Error: {e.Message}");
            }

            ShowUserMenu();


        }
        public void GetAll()
        {
            string sqlQuery = $"select * from GetAllCoachesData";

            DataTable dataTable = new DataTable();

            using (SqlDataAdapter adapter = new SqlDataAdapter(sqlQuery, connection))
            {
                adapter.Fill(dataTable);
            }

            Helper.PrintDataTable(dataTable);
        }
        public void SearchCoach()
        {
            Console.Clear();
            //search for coach by its id 
            int coachId;
            Console.Write("Enter Coach ID: ");
            while (!int.TryParse(Console.ReadLine(), out coachId))
            {
                Console.WriteLine("Invalid Coach ID. Enter a valid integer.");
            }
            try
            {
                string selectQuery = "SELECT C.*, CP.Phone, CA.Address " +
                                     "FROM Coaches C " +
                                     "LEFT JOIN Coach_Phones CP ON C.Id = CP.Id " +
                                     "LEFT JOIN Coach_Addresses CA ON C.Id = CA.Id " +
                                     "WHERE C.Id = @CoachId AND C.IsDeleted = 'f';";

                SqlCommand cmd = new SqlCommand(selectQuery, connection);

                cmd.Parameters.AddWithValue("@CoachId", coachId);

                SqlDataAdapter adapter = new SqlDataAdapter(cmd);

                DataTable resultTable = new DataTable();
                adapter.Fill(resultTable);
                if (resultTable.Rows.Count > 0)
                {
                    Console.WriteLine($"\nCoach found... \nName: {resultTable.Rows[0]["Name"]}\n" +
                        $"Email: {resultTable.Rows[0]["Email"]}\nPhone: {resultTable.Rows[0]["Phone"]}\n" +
                        $"Address: {resultTable.Rows[0]["Address"]}\n");
                }
                else
                {
                    Console.WriteLine("Coach not found.");
                }


            }
            catch (Exception e)
            {
                Console.WriteLine($"\nEnter vaild id \n");
            }
            ShowUserMenu();


        }
        public void UpdateCoach(int coachId, string newName, string newEmail, int newProgramId, int newUserId, string newPhone, string newAddress)
        {
            // Update coaches data by know its id  
            try
            {


                string updateQuery = "UPDATE Coaches SET Name = @NewName, Email = @NewEmail, Program_Id = @NewProgramId, User_Id = @NewUserId " +
                                     "WHERE Id = @CoachId AND IsDeleted = 'f';" +
                                     "UPDATE Coach_Phones SET Phone = @NewPhone WHERE Id = @CoachId;" +
                                     "UPDATE Coach_Addresses SET Address = @NewAddress WHERE Id = @CoachId;";
                SqlCommand cmd = new SqlCommand(updateQuery, connection);
                cmd.Parameters.AddWithValue("@CoachId", coachId);
                cmd.Parameters.AddWithValue("@NewName", newName);
                cmd.Parameters.AddWithValue("@NewEmail", newEmail);
                cmd.Parameters.AddWithValue("@NewProgramId", newProgramId);
                cmd.Parameters.AddWithValue("@NewUserId", newUserId);
                cmd.Parameters.AddWithValue("@NewPhone", newPhone);
                cmd.Parameters.AddWithValue("@NewAddress", newAddress);
                int result2 = cmd.ExecuteNonQuery();
                if (result2 > 0)
                    Console.WriteLine($"The coach is updated successfully ");
                else
                    Console.WriteLine($"The coach not updated");
            }
            catch (Exception e)
            {
                Console.WriteLine($"Error: {e.Message}");
            }
            ShowUserMenu();


        }
        #endregion
    }
}
