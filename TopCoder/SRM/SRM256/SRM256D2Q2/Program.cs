using System;
using System.Collections;
using System.Text;

public class MagicCube
{
    private int subScore(int[] nums)
    {
        int minSum = int.MaxValue;
        int maxSum = int.MinValue;

        for (int i = 0; i < 3; i++)
        {
            for (int j = 0; j < 3; j++)
            {
                int[] sums = new int[3];

                for (int k = 0; k < 3; k++)
                {
                    sums[0] += nums[i*9+j*3+ k];
                    sums[1] += nums[i * 9 + k * 3 + j];
                    sums[2] += nums[k * 9 + j * 3 + i];
                }
                for (int k = 0; k < 3; k++)
                {
                    if (sums[k] < minSum)
                        minSum = sums[k];
                    if (sums[k] > maxSum)
                        maxSum = sums[k];
                }

            }
        }

        return maxSum - minSum;
    }

    public int getScore(int[] nums)
    {
        int score = subScore(nums);

        for (int i = 0; i < nums.Length-1; i++)
        {
            for (int j = i + 1; j < nums.Length; j++)
            {
                int temp = nums[i];
                nums[i] = nums[j];
                nums[j] = temp;
                int newScore = subScore(nums);
                if (newScore < score)
                {
                    score = newScore;
                }
                temp = nums[i];
                nums[i] = nums[j];
                nums[j] = temp;
            }
        }
        return score;
    }
}

namespace SRM256D2Q2
{
    class Program
    {
        static void Main(string[] args)
        {
        }
    }
}
