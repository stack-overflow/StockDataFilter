using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace StockDataFilter
{
    static class Utils
    {
//def longest_common_sequence(A, B):
//    w, h = len(A), len(B)
//    C = [[0 for x in range(w + 1)] for y in range(h + 1)]

//    for i in range(1, w + 1):
//        for j in range(1, h + 1):
//            if A[i - 1] == B[j - 1]:
//                C[i][j] = C[i - 1][j - 1] + 1
//            elif C[i - 1][j] > C[i][j - 1]:
//                C[i][j] = C[i - 1][j]
//            else:
//                C[i][j] = C[i][j - 1]

//    seq = []
//    i, j = 0, 0
//    while i<w and j<h:
//        if A[i] == B[j]:
//            seq.append(A[i])
//            print(A[i])
//            i += 1
//            j += 1
//        elif C[i + 1][j] > C[i][j + 1]:
//            i += 1
//        else:
//            j += 1
//    return seq
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
