using System;

namespace DaymsWPFBoiler.Data.Utilities
{
    /// <summary>
    /// Contains methods to operate over data such as Guids &amp; SQLiteHexStrings.
    /// </summary>
    public static class DataFunctions
    {
        public static string ToSQLiteDT(DateTime datetime) => string.Format("{0:yyyy-MM-dd HH:mm:ss.fff}", datetime);

        private static readonly long BaseDateTicks = new DateTime(1900, 1, 1).Ticks;

        /// <summary>
        /// Generate a new <see cref="Guid"/> using the comb algorithm
        /// <para>Creates a sequential unique identifier for better indexing and distributed systems use.</para>
        /// <para>Borrowed from NHibernate</para>
        /// </summary>
        /// <returns>A new identifier as a <see cref="Guid"/>.</returns>
        public static Guid NewGuidComb()
        {
            byte[] guidArray = Guid.NewGuid().ToByteArray();

            DateTime now = DateTime.UtcNow;

            TimeSpan span = new TimeSpan(now.Ticks - BaseDateTicks);
            TimeSpan time = now.TimeOfDay;

            // Convert to byte array
            byte[] spanArray = BitConverter.GetBytes(span.Days);
            // Note: SQL Server is accurate to 1/300th of a millisecond so we divide by 3.333333
            byte[] timeArray = BitConverter.GetBytes((long)(time.Milliseconds / 3.333333));

            // Reverse bytes to match SQL Server ordering
            Array.Reverse(spanArray);
            Array.Reverse(timeArray);

            // Copy bytes into Guid
            Array.Copy(spanArray, spanArray.Length - 2, guidArray, guidArray.Length - 6, 2);
            Array.Copy(timeArray, timeArray.Length - 4, guidArray, guidArray.Length - 4, 4);

            return new Guid(guidArray);
        }

        /// <summary>
        /// Determine if Guid is null or empty {00000000-0000-0000-0000-000000000000}
        /// </summary>
        /// <param name="guid">Guid to be evaluated</param>
        /// <returns>True, if null or empty; False, otherwise.</returns>
        public static bool IsNullOrEmptyGuid(Guid? guid)
        {
            return null == guid || guid == Guid.Empty;
        }

        /// <summary>
        /// Converts a Byte Array ({00-00-00-00-00-00-00-00-00-00-00-...-00})
        /// to a SQLite Hex String ("X'00000000000000000000...00'")
        /// </summary>
        /// <param name="bytes">Byte Array to convert</param>
        /// <returns>SQLite Hex String</returns>
        public static string BytesToSQLiteHexString(byte[] bytes) => $"X'{BitConverter.ToString(bytes).Replace("-", "")}'";

        /// <summary>
        /// Converts a Guid ({00000000-0000-0000-0000-000000000000})
        /// to a SQLite Hex String ("X'00000000000000000000000000000000'")
        /// </summary>
        /// <param name="guid">Guid to convert</param>
        /// <returns>SQLite Hex String</returns>
        public static string GuidToSQLiteHexString(Guid guid) => BytesToSQLiteHexString(guid.ToByteArray());

        /// <summary>
        /// Converts a Byte Array ({00-00-00-00-00-00-00-00-00-00-00-...-00})
        /// to a Guid ({00000000-0000-0000-0000-000000000000})
        /// </summary>
        /// <param name="byteArray">Byte Array to convert</param>
        /// <param name="guid">Guid to be output</param>
        /// <returns>true, if the conversion is successful; otherwise, false</returns>
        public static bool BytesToGuid(byte[] byteArray, out Guid guid) => SqliteHexStringToGuid(BytesToSQLiteHexString(byteArray), out guid);

        /// <summary>
        /// Converts a SQLite Hex String ("X'00000000000000000000000000000000'")
        /// to a Guid ({00000000-0000-0000-0000-000000000000})
        /// </summary>
        /// <param name="hexString">SQLite Hex String to convert</param>
        /// <param name="guid">Guid to be output</param>
        /// <returns>true, if the conversion is successful; otherwise, false</returns>
        public static bool SqliteHexStringToGuid(string hexString, out Guid guid)
        {
            string guidString = hexString.Replace("X", "").Replace("'", ""); // Accounting for sqlite hex markers "X'...guid...'"
            if (guidString.Length == 32)
            {
                string[] array1 = new string[]
                                            {
                                        guidString.Substring(0, 2),
                                        guidString.Substring(2, 2),
                                        guidString.Substring(4, 2),
                                        guidString.Substring(6, 2)
                                            };
                Array.Reverse(array1);

                string[] array2 = new string[]
                                        {
                                        guidString.Substring(8, 2),
                                        guidString.Substring(10, 2)
                                        };
                Array.Reverse(array2);

                string[] array3 = new string[]
                                        {
                                        guidString.Substring(12, 2),
                                        guidString.Substring(14, 2)
                                        };
                Array.Reverse(array3);

                return Guid.TryParse(string.Join("", array1) + "-" +
                                        string.Join("", array2) + "-" +
                                        string.Join("", array3) + "-" +
                                        guidString.Substring(16, 4) + "-" +
                                        guidString.Substring(20), out guid);
            }
            else
            {
                guid = Guid.Empty;
                return false;
            }
        }

    }
}
