using System.Collections.Generic;
using Utility.Core;

namespace DataProcessors.Civilization
{
    [Launchable("Civilization")]
    public class CivControl : ToBeImplemented {
        private readonly ICollection<Tile> _tiles = new List<Tile>();
        /*
         * Choices:
         * Build monuments to yourself.  This creates a lasting cultural legacy, for which you will receive points.
         * Focus on gaining wealth and riches through plunder.
         * Focus on expanding your continuous tax base.
         * Focus on the arts which will create innovation.
         * You can choose a policy that allows royalty to grow and allows more people the chance to innovate so it happens more quickly.
         *      What are the pros and cons?
         * You can choose specific religious prescriptions:
         *      Are you a God, or merely God-chosen?
         *      How much power will shamans be allowed in society?
         * How much control will you exert over agricultural planning?
         * 
         * 
         * How about I choose two points in time...
         * Imagine the kinds of choices leaders were making at those times...
         *  and imagine the kinds of choices other people were making at those times...
         *  And imagine the kinds of changes that take place in between.
         *  
         * The choices that will work out the best are those that match the conditions...
         * For instance, it makes sense to go for raiding and barbarism if you don't have natural resources
         *      suitable for early neolithic agriculture.
         */

        public CivControl() : base("Go with the tile approach.  Everything happens locally in a tile.\n"
                                                                           + "Players can take the part of rulers\n\tthat set policies that affect those tiles.")
        {
            //todo: This will be a good way to stage the part of the game where two nearby cities are competing culturally
            //I will be able to add complexity to this and see people moving back and forth
            //Ultimately I'll be able to add armies to these tiles, and have them fight etc....
            //I will be able to stage everything with juts these tiles, then I can put geo-spatial data on top of it and add graphic representation
            
            //Cities will be structures that sit on top of groups of tiles that push POLICIES to them.

            var tile1 = new Tile();
            var water = new WaterResource();
            tile1.Resources.Add(water);
            var population = new Population(100);
            tile1.Populations.Add(population);
            _tiles.Add(tile1);

            var tile2 = new Tile();
            water = new WaterResource();
            tile2.Resources.Add(water);
            population = new Population(100);
            tile2.Populations.Add(population);
            _tiles.Add(tile2);

            tile1.ConnectedTiles.Add(tile2);
            tile2.ConnectedTiles.Add(tile1);

            tile1.Profile.WaterAvailabilityRatio = 1.1;
            tile2.Profile.WaterAvailabilityRatio = 1.2;

            tile1.Iterate();
            tile2.Iterate();
        }
    }

    public class Tile
    {
        public ICollection<Population> Populations { get; set; }
        public ICollection<Tile> ConnectedTiles { get; set; }
        public ICollection<Resource> Resources { get; set; }
        public TileProfile Profile { get; set; }

        public Tile()
        {
            Profile = new TileProfile {WaterAvailabilityRatio = 1.1};
            Resources = new List<Resource>();
            ConnectedTiles = new List<Tile>();
            Populations = new List<Population>();
        }

        public void Iterate()
        {
            //create a chart of what happens to different segments of the population
            /*
             * | Move to Tile 0 | 9%
             * | Move to Tile 1 | 3%
             * | Die            | 5%
             * 
             * The chance to move to a tile is based on current happiness vs. percieved happiness in another tile vs. threshold to move
             */

            Populations.Do(population => population.Happiness(Profile));
//            Populations.Do(population => population.Happiness(Resources));
        }
    }

    public class TileProfile
    {
        public double WaterAvailabilityRatio { get; set; }
    }

    public class GameDate
    {}

    public class DistributionCurve
    {}

    public class Population
    {
        private PopulationClass _class = new PopulationClass();
        private readonly int _people;

        public Population(int people)
        {
            _people = people;
        }

        public int Happiness(TileProfile profile)
        {
            return profile.WaterAvailabilityRatio >= 1.1 ? 100 : 50;
        }
    }

    public class PopulationClass
    {
        public ICollection<Doctrine> Doctrines { get; set; }
        public DistributionCurve Age { get; set; }

        public PopulationClass()
        {
            Doctrines = new List<Doctrine>();
        }
    }

    public class Doctrine
    {}

    public class Resource
    {}

    public class WaterResource : Resource
    {
        public double FlowRate { get; set; }
        public double Width { get; set; }
        public double Height { get; set; }
        public double Salinity { get; set; }
    }

    public class PerennialWaterResource : WaterResource
    {}

    public class SeasonalWaterResource : WaterResource
    {}

    public class Technology
    {}

    public class IrrigationBasin : Technology
    {
        public double MaxFlowRate { get; set; }
        public double MaxWidth { get; set; }
        public double MaxHeight { get; set; }
    }

    public class DryConstructionDam : Technology //for seasonal or divertable water flows
    {}

    public class Shaduf : Technology //for moving water from lower canals to higher canals
                                    //presumably to protect crops from flooding
    {}

    //your agricultural output causes fewer people to resettle, fewer people to die, and causes more people to have babies
}