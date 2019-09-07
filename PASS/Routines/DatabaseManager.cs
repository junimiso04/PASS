using PASS.Entities;
using System;
using System.Collections.Generic;
using System.Data.SQLite;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PASS.Routines
{
    internal static class DatabaseManager
    {
        private static SQLiteConnection conn = new SQLiteConnection();
        private static List<Problem> probList = new List<Problem>();
        private static List<Answer> answerList = new List<Answer>();

        internal static bool CreateDatabase(string path)
        {
            try
            {
                if (!File.Exists(path))
                    SQLiteConnection.CreateFile(path);
                return true;
            }
            catch
            {
                return false;
            }
        }

        internal static bool OpenDatabase(string path)
        {
            if (!File.Exists(path))
                throw new Exception("The file does not exist.");

            conn = new SQLiteConnection($"Data source={path}; Version=3;");
            conn.Open();
            return true;
        }

        internal static bool CloseDatabase()
        {
            conn.Close();

            if (conn.State == System.Data.ConnectionState.Closed) // checked.
                return true;
            else
                return false;
        }

        internal static bool RestartDatabase(string path)
        {
            if (!IsOpen())
                return false;

            if (CloseDatabase())
                OpenDatabase(path);
            return true;
        }

        internal static bool IsOpen()
        {
            if (conn.State == System.Data.ConnectionState.Open) // checked.
                return true;
            else
                return false;
        }

        internal static bool IsTableExists(string table)
        {
            string sql = $"SELECT COUNT(*) FROM sqlite_master WHERE name='{table}'";

            SQLiteCommand cmd = new SQLiteCommand(sql, conn);
            if (Convert.ToInt32(cmd.ExecuteScalar().ToString()) == 0) // checked.
                return false;
            else
                return true;
        }

        internal static bool IsRowExists(string table, int code)
        {
            string sql = $"SELECT COUNT(*) FROM {table} WHERE code='{code}'";
            SQLiteCommand cmd = new SQLiteCommand(sql, conn);
            if (Convert.ToInt32(cmd.ExecuteScalar().ToString()) == 0) // checked.
                return false;
            else
                return true;
        }

        internal static bool CreateSet(string setTitle)
        {
            string sql = $"CREATE TABLE IF NOT EXISTS {setTitle} (code int, title varchar(100), description text, memory int, timelimit int)";
            setTitle = setTitle.ToUpper();
            setTitle = setTitle.Replace(" ", "_");

            SQLiteCommand cmd = new SQLiteCommand(sql ,conn);

            int result = cmd.ExecuteNonQuery();
            cmd.Dispose();

            if (result != 0)  // checked.
                return false;
            else
                return true;
        }

        internal static bool AddProblem(Problem prob)
        {
            string sql = $"INSERT INTO {prob.Set} (code, title, description, memory, timelimit) VALUES ({prob.Code}, '{prob.Title}', '{prob.Description}', {prob.Memory}, {prob.TimeLimit})";

            SQLiteCommand cmd = new SQLiteCommand(sql, conn);

            int result = cmd.ExecuteNonQuery();
            cmd.Dispose();

            if (result == 0) // checked.
                return false;
            else
                return true;
        }

        private static bool LoadProblemList(string set)
        {
            string sql = $"SELECT * FROM {set}";

            SQLiteCommand cmd = new SQLiteCommand(sql, conn);
            SQLiteDataReader reader = cmd.ExecuteReader();

            probList.Clear();

            while (reader.Read())
            {
                Problem prob = new Problem();
                prob.Code = Convert.ToInt32(reader["code"]);
                prob.Title = Convert.ToString(reader["title"]);
                prob.Description = Convert.ToString(reader["description"]);
                prob.Memory = Convert.ToInt32(reader["memory"]);
                prob.TimeLimit = Convert.ToInt32(reader["timelimit"]);
                probList.Add(prob);
            }
            cmd.Dispose();
            return true;
        }

        internal static Problem GetProblem(string set, int code)
        {
            LoadProblemList(set);
            foreach(Problem prob in probList)
            {
                if (prob.Code == code) // checked.
                    return prob;
                else
                    return null;
            }
            return null;
        }

        internal static List<Problem> GetProblemList(string set)
        {
            if (LoadProblemList(set))
                return probList;
            else
                return null;
        }


        internal static bool CreateAnswerTable(string setCode)
        {
            string sql = $"CREATE TABLE IF NOT EXISTS '{setCode}' (input text, output text);";

            SQLiteCommand cmd = new SQLiteCommand(sql, conn);

            int result = cmd.ExecuteNonQuery();
            cmd.Dispose();

            if (result == 0) // checked.
                return false;
            else
                return true;
        }

        internal static bool AddAnswer(string setCode, string input, string output)
        {

            string sql = $@"INSERT INTO '{setCode}' (input, output) VALUES ('{input}', '{output}')";

            SQLiteCommand cmd = new SQLiteCommand(sql, conn);

            int result = cmd.ExecuteNonQuery();
            cmd.Dispose();

            if (result == 0) // checked.
                return false;
            else
                return true;
        }

        private static bool LoadAnswerList(string setCode)
        {
            string sql = $"SELECT * FROM '{setCode}'";

            SQLiteCommand cmd = new SQLiteCommand(sql, conn);
            SQLiteDataReader reader = cmd.ExecuteReader();

            answerList.Clear();

            while (reader.Read())
            {
                Answer answer = new Answer();
                answer.Input = Convert.ToString(reader["input"]);
                answer.Output = Convert.ToString(reader["output"]);

                answerList.Add(answer);
            }
            cmd.Dispose();
            return true;
        }

        internal static List<Answer> GetAnswerList(string setCode)
        {
            if (LoadAnswerList(setCode))
                return answerList;
            else
                return null;
        }
    }
}
