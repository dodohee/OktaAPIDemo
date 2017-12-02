using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using OktaAPIShared.Models;

namespace OktaCustomerUI.Helpers
{
    public static class PermutateHelper
    {
        
        public static void GetPermutations(PermutationModel pm)
        {
            if (string.IsNullOrEmpty(pm.Permutate)) return;
            var arr = pm.Permutate.ToCharArray();
            StartPermutation(arr, pm);
        }

        private static void StartPermutation(char[] list, PermutationModel pm)
        {
            var iCharCount = list.Length - 1;
            GetEachPer(list, 0, iCharCount, pm);
        }

        private static void GetEachPer(char[] list, int iCharCurrent, int iCharCount, PermutationModel pm)
        {
            if (iCharCurrent == iCharCount)
            {
                var test = new string(list);
                if (!pm.Permutations.Contains(test))
                {
                    //only add unique
                    pm.Permutations.Add(new string(list));
                }
            }
            else
                for (int i = iCharCurrent; i <= iCharCount; i++)
                {
                    Swap(ref list[iCharCurrent], ref list[i]);
                    GetEachPer(list, iCharCurrent + 1, iCharCount, pm);
                    Swap(ref list[iCharCurrent], ref list[i]);
                }
        }

        private static void Swap(ref char a, ref char b)
        {
            var tmp = a;
            a = b;
            b = tmp;
        }
        
    }
}