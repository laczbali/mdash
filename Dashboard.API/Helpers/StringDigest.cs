using System;
using System.Text.RegularExpressions;

namespace Dashboard.API.Helpers
{
    public static class StringDigest
    {
        /// <summary>
        /// Parses a CSV string into a 2D string array.
        /// Number of columns gets determined by the Start Row!
        /// </summary>
        /// <param name="csvText">The string to parse</param>
        /// <param name="columnDelimiter">Delimiter between columns in a row (;)</param>
        /// <param name="rowDelimiter">Delimiter between rows (\r\n)</param>
        /// <param name="removeEmptyRows">(true)</param>
        /// <param name="startRow">In case there are header rows that don't need to be included (0)</param>
        /// <returns></returns>
        public static string[,] ParseCSVTo2DArray(string csvText, string columnDelimiter = ";", string rowDelimiter = "\r\n",
            bool removeEmptyRows = true, int startRow = 0)
        {
            // Split string to rows
            string[] rows;
            if (removeEmptyRows) { rows = csvText.Split(rowDelimiter, StringSplitOptions.RemoveEmptyEntries); }
            else { rows = csvText.Split(rowDelimiter); }

            // Set up output array
            int rowCount = rows.Length;
            int colCount = rows[startRow].Split(columnDelimiter).Length;
            string[,] output = new string[rowCount - startRow, colCount];

            // Fill out output array
            for (int i = startRow; i < rowCount; i++)
            {
                string[] fields = rows[i].Split(columnDelimiter);
                
                for (int j = 0; j < colCount; j++)
                {
                    try
                    {
                        output[i - startRow, j] = fields[j];
                    }
                    catch (IndexOutOfRangeException)
                    {
                        // In case of staggered input array
                        output[i - startRow, j] = "";
                    }
                }
            }

            return output;
        }
    }
}