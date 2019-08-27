using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Collections.Generic;
using TLib;
using TLib.Net.Udp;
namespace UnitTest
{
    [TestClass]
    public class CSharpTest
    {

        [TestMethod]
        public void RandomReservedTest()
        {
            var listA = new List<int>();
            var listB = new List<int>();
            for (int i = 0; i < 100; i++)
            {
                listA.Add(i);
                listB.Add(i);
            }
            CsharpHelper.RandomReserve(ref listA);
            bool equal = true;
            for (int i = 0; i < listA.Count; i++)
            {
                if (listA[i] != listB[i])
                {
                    equal = false;
                    break;
                }
            }
            Assert.IsFalse(equal);
        }



        [TestMethod]
        public void SwapTest()
        {
            int a = 2;
            int b = 3;
            CsharpHelper.Swap(ref a, ref b);
            Assert.AreEqual(a, 3);
            Assert.AreEqual(b, 2);
        }
    }
}
