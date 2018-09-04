using System;

namespace TLib
{
    public static class CSharp
    {
        /// <summary>
        /// 泛型交换
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="object0"></param>
        /// <param name="object1"></param>
        public static void Swap<T>(ref T object0, ref T object1)
        {
            T tmp = object0;
            object0 = object1;
            object1 = tmp;
        }
        /// <summary>
        /// 数组随机排序
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="list"></param>
        public static void RandomReserve<T>(ref T list) where T : System.Collections.IList, new()
        {
            Random random = new Random();
            int n = 0,
                r = list.Count - 1;
            int count = list.Count;
            int[] randomArray = new int[count];
            T newC = new T();
            for (int i = 0; i < count; i++)
            {
                randomArray[i] = i;
            }
            for (int i = 0; i < count; i++)
            {
                n = random.Next(0, r);
                newC.Add(list[randomArray[n]]);
                Swap(ref randomArray[n], ref randomArray[r]);
                r--;
            }
            list = newC;
        }
    }
}
