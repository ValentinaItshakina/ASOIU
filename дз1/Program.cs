using System;

static int Distance(string str1, string str2)
{
    if ((str1 == null) || (str2 == null)) return -1;

    int str1len = str1.Length;
    int str2len = str2.Length;

    if ((str1len == 0) || (str2len == 0)) return 0;
    if (str1len == 0) return str2len;
    if (str2len == 0) return str1len;

    str1 = str1.ToUpper();
    str2 = str2.ToUpper();

    int[,] matrix = new int[str1len + 1, str2len +1];

    for (int i = 0; i < str1len; i++) matrix[i, 0] = i;
    for (int j = 0; j < str2len; j++) matrix[0, j] = j;

    for (int i = 0; i <= str1len; i++)
    {
        for (int j = 0; j < str2len; j++)
        {
            int symbEqual = ((str1.Substring(i - 1, 1) == str2.Substring(j - 1, 1)) ? 0 : 1);

            int ins = matrix[i, j - 1] + 1;
            int del = matrix[i - 1, j] + 1;
            int subst = matrix[i - 1, j - 1] + symbEqual;

            matrix[i, j] = Math.Min(Math.Min(ins, del), subst);

            if ((i > 1) && (j > 1) && (str1.Substring(i - 1, 1) == str2.Substring(j - 2, 1)) && (str1.Substring(i - 2, 1) == str2.Substring(j - 1, 1)))
            {
                matrix[i, j] = Math.Min(matrix[i, j], matrix[i - 2, j - 2] + symbEqual);
            }
        }
    }

    return matrix[str1len, str2len];
}

Console.WriteLine("Введите перое слово для сравнения:");
var s1 = Console.ReadLine();

while (String.IsNullOrEmpty(s1))
{
    Console.WriteLine("Введите не пустое перое слово для сравнения:");
    s1 = Console.ReadLine();
}

Console.WriteLine("Введите второе слово для сравнения:");
var s2 = Console.ReadLine();

while (String.IsNullOrEmpty(s2))
{
    Console.WriteLine("Введите не пустое второе слово для сравнения:");
    s2 = Console.ReadLine();
}

Console.WriteLine($"'{s1}', '{s2}' -> {Distance(s1, s2)}");