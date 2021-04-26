public class WorldGenerator
{

    static float waterWidth = 5f;
    static float beachWidth = 10f;
    static float noJungleWidth = 12f;

    // public static IntegerMap GenerateWorld(int w, int h)
    // {
    //     IntegerMap n_map = new IntegerMap(w, h);

    //     for (var x = 0; x < n_map.width; x++)
    //         for (var y = 0; y < n_map.width; y++)
    //         {
    //             if (x < n_map.width / 100f * waterWidth)
    //             {
    //                 n_map.SetValue(x, y, -1);
    //                 continue;
    //             }
    //             if (x < n_map.width / 100f * beachWidth)
    //             {
    //                 n_map.SetValue(x, y, 0);
    //                 continue;
    //             }
    //             if (x < n_map.width / 100f * noJungleWidth)
    //             {
    //                 n_map.SetValue(x, y, 1);
    //                 continue;
    //             }
    //             n_map.SetValue(x, y, 3);
    //         }

    //     return n_map;
    // }

    public static IntegerMap GenerateBananas(int w, int h)
    {
        IntegerMap n_map = new IntegerMap(w, h);

        for (var x = 0; x < n_map.width; x++)
            for (var y = 0; y < n_map.width; y++)
            {
                n_map.SetValue((x, y), 1);
            }

        return n_map;
    }



}