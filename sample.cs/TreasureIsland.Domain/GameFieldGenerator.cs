using System;
using System.Collections.Generic;

namespace Glow.Sample.TreasureIsland.Domain;

public static class GameFieldGenerator
{
    private static List<Tile> tiles;

    // blue: (10) ['#e7f5ff', '#d0ebff', '#a5d8ff', '#74c0fc', '#4dabf7', '#339af0', '#228be6', '#1c7ed6', '#1971c2', '#1864ab']
    // cyan: (10) ['#e3fafc', '#c5f6fa', '#99e9f2', '#66d9e8', '#3bc9db', '#22b8cf', '#15aabf', '#1098ad', '#0c8599', '#0b7285']
    // dark: (10) ['#C1C2C5', '#A6A7AB', '#909296', '#5c5f66', '#373A40', '#2C2E33', '#25262b', '#1A1B1E', '#141517', '#101113']
    // grape: (10) ['#f8f0fc', '#f3d9fa', '#eebefa', '#e599f7', '#da77f2', '#cc5de8', '#be4bdb', '#ae3ec9', '#9c36b5', '#862e9c']
    // gray: (10) ['#f8f9fa', '#f1f3f5', '#e9ecef', '#dee2e6', '#ced4da', '#adb5bd', '#868e96', '#495057', '#343a40', '#212529']
    // green: (10) ['#ebfbee', '#d3f9d8', '#b2f2bb', '#8ce99a', '#69db7c', '#51cf66', '#40c057', '#37b24d', '#2f9e44', '#2b8a3e']
    // indigo: (10) ['#edf2ff', '#dbe4ff', '#bac8ff', '#91a7ff', '#748ffc', '#5c7cfa', '#4c6ef5', '#4263eb', '#3b5bdb', '#364fc7']
    // lime: (10) ['#f4fce3', '#e9fac8', '#d8f5a2', '#c0eb75', '#a9e34b', '#94d82d', '#82c91e', '#74b816', '#66a80f', '#5c940d']
    // orange: (10) ['#fff4e6', '#ffe8cc', '#ffd8a8', '#ffc078', '#ffa94d', '#ff922b', '#fd7e14', '#f76707', '#e8590c', '#d9480f']
    // pink: (10) ['#fff0f6', '#ffdeeb', '#fcc2d7', '#faa2c1', '#f783ac', '#f06595', '#e64980', '#d6336c', '#c2255c', '#a61e4d']
    // red: (10) ['#fff5f5', '#ffe3e3', '#ffc9c9', '#ffa8a8', '#ff8787', '#ff6b6b', '#fa5252', '#f03e3e', '#e03131', '#c92a2a']
    // teal: (10) ['#e6fcf5', '#c3fae8', '#96f2d7', '#63e6be', '#38d9a9', '#20c997', '#12b886', '#0ca678', '#099268', '#087f5b']
    // violet: (10) ['#f3f0ff', '#e5dbff', '#d0bfff', '#b197fc', '#9775fa', '#845ef7', '#7950f2', '#7048e8', '#6741d9', '#5f3dc4']
    // yellow:

    static GameFieldGenerator()
    {
        // var grass = new Tile("dark", TileName.Grass, true);
        // var water = new Tile("gray", TileName.Water, false);
        // var mountain = new Tile("grape", TileName.Mountain, false);
        // tiles = new List<Tile>() { grass, water, mountain, };
        var grass = new Tile("lime", TileName.Grass, true);
        var water = new Tile("blue", TileName.Water, false);
        var mountain = new Tile("dark", TileName.Mountain, false);
        var wood = new Tile("green", TileName.Wood, true);
        var corn = new Tile("yellow", TileName.Corn, true);
        var stone = new Tile("red", TileName.Corn, true);
        var mud = new Tile("orange", TileName.Corn, true);
        tiles = new List<Tile>()
        {
            grass,
            grass,
            grass,
            grass,
            grass,

            wood,
            wood,
            wood,

            corn,
            corn,

            stone,
            stone,

            water,
            water,

            mud,
            mud,

            mountain,
        };
    }

    public static GameField OrientedRectangle(int height, int width)
    {
        var random = new Random();
        var hexas = new List<Field>();


        for (var q = 0; q < width; q++)
        {
            var offset = (int) (q / 2.0); // or q>>1
            for (var r = -offset; r < height - offset; r++)
            {
                var material = tiles[random.Next(tiles.Count)];

                hexas.Add(Field.New(new Position(q, r, -q - r), material));
            }
        }

        return new GameField(hexas);
    }

    public static GameField Hexagon(int mapRadius)
    {
        var random = new Random();
        var hexas = new List<Field>();

        var pickRandomMaterial = true;

        for (var q = -mapRadius; q <= mapRadius; q++)
        {
            var r1 = Math.Max(-mapRadius, -q - mapRadius);
            var r2 = Math.Min(mapRadius, -q + mapRadius);
            for (var r = r1; r <= r2; r++)
            {
                var material = pickRandomMaterial ? tiles[random.Next(tiles.Count)] : tiles[(q + r) % tiles.Count];

                hexas.Add(Field.New(new Position(q, r, -q - r), material));
            }
        }

        return new GameField(hexas);

        // return hexas
        //
        // for (var q = 0; q < width; q++)
        // {
        //     var offset = (int) (q / 2.0); // or q>>1
        //     for (var r = -offset; r < height - offset; r++)
        //     {
        //         var material = (Material) (random.Next() % 5 + 1);
        //         hexas.Add(new Field(new Position(q, r, -q - r), material));
        //     }
        // }

        return new GameField(hexas);
    }
}