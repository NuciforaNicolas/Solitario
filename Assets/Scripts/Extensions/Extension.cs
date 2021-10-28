using System.Collections;
using System.Collections.Generic;
using System;

public static class Extension
{
    private static Random random = new Random();
    public static void Shuffle<T>(this IList<T> list){
        for(int i = list.Count - 1; i > 1; i--){
            int index = random.Next(i + 1);
            var tmp = list[i];
            list[i] = list[index];
            list[index] = tmp;
        }
    }
}
