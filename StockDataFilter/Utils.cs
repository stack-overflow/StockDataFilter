using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace StockDataFilter
{
    static class Utils
    {
        public static string[] LongestCommonSubsequence(string[] A, string[] B)
        {
            List<string> seq = new List<string>();

            var w = A.Length;
            var h = B.Length;
            int[,] C = new int[w + 1, h + 1];
            int i, j;

            for (i = 1; i < w + 1; ++i)
            {
                for (j = 1; j < h + 1; ++j)
                {
                    if (A[i - 1] == B[j - 1])
                    {
                        C[i, j] = C[i - 1, j - 1] + 1;
                    }
                    else if (C[i - 1, j] > C[i, j - 1])
                    {
                        C[i, j] = C[i - 1, j];
                    }
                    else
                    {
                        C[i, j] = C[i, j - 1];
                    }
                }
            }

            i = 0;
            j = 0;
            while (i < w && j < h)
            {
                if (A[i] == B[j])
                {
                    seq.Add(A[i]);
                    ++i; ++j;
                }
                else if (C[i + 1, j] > C[i, j + 1])
                {
                    ++i;
                }
                else
                {
                    ++j;
                }
            }

            return seq.ToArray();
        }

    }
}
