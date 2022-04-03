namespace ProductService;

public static class FindPrimeNumber
{
    public static long Run(int n)
    {
        int count=0;
        long a = 2;
        while(count<n)
        {
            long b = 2;
            bool prime = true;
            while(b * b <= a)
            {
                if(a % b == 0)
                {
                    prime = false;
                    break;
                }
                b++;
            }
            if(prime)
            {
                count++;
            }
            a++;
        }
        return (--a);
    }
}
