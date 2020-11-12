using System;

namespace test
{
    class Program
    {
        static void Main(string[] args)
        {
            E_1();

            Console.ReadLine();
        }

        private static void E_1()
        {
            int num = 0;
            bool isError = false;
            while (num < 20)
            {
                if (!isError)
                {
                    try
                    {
                        E_2();
                    }
                    catch (Exception e)
                    {
                        Console.WriteLine("异常~~~~~" + e.Message);
                        isError = true;
                    }
                }
                num++;
                Console.WriteLine("mmmmmmmm" + num);
            }
        }

        private static int e_2_num = 0;
        private static void E_2()
        {
            e_2_num++;

            if (e_2_num == 10)
            {
                throw new Exception(">>>>>>>>>");
            }
            
        }
    }
}