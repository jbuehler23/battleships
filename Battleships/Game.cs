using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Battleships
{
    // Imagine a game of battleships.
    //   The player has to guess the location of the opponent's 'ships' on a 10x10 grid
    //   Ships are one unit wide and 2-4 units long, they may be placed vertically or horizontally
    //   The player asks if a given co-ordinate is a hit or a miss
    //   Once all cells representing a ship are hit - that ship is sunk.
    public class Game
    {
        // ships: each string represents a ship in the form first co-ordinate, last co-ordinate
        //   e.g. "3:2,3:5" is a 4 cell ship horizontally across the 4th row from the 3rd to the 6th column
        // guesses: each string represents the co-ordinate of a guess
        //   e.g. "7:0" - misses the ship above, "3:3" hits it.
        // returns: the number of ships sunk by the set of guesses
        //track the number of hits for testing purposes - removed for clarity

        public static int Play(string[] ships, string[] guesses)
        {
            ConcurrentDictionary<string, Ship> shipCoordinates = PopulateShips(ships);
            //store the number of ships sunk
            int shipsSunk = PlayGame(shipCoordinates, guesses);
            shipCoordinates.Clear(); //reset for next run
            return shipsSunk;
        }

        private static ConcurrentDictionary<string, Ship> PopulateShips(string[] ships)
        {
            //First, draw the matrix out for visual aid (o==guess, x==ship in above ex)
            // [0,1,2,3,4,5,6,7,8,9]
            //0[.,.,.,.,.,.,.,o,.,.]
            //1[.,.,.,.,.,.,.,.,.,.]
            //2[.,.,.,.,.,.,.,.,.,.]
            //3[.,.,x,o,x,x,.,.,.,.]
            //4[.,.,.,.,.,.,.,.,.,.]
            //5[.,.,.,.,.,.,.,.,.,.]
            //6[.,.,.,.,.,.,.,.,.,.]
            //7[.,.,.,.,.,.,.,.,.,.]
            //8[.,.,.,.,.,.,.,.,.,.]
            //9[.,.,.,.,.,.,.,.,.,.]
            //Keep a record of coordinates, and the "ship number"
            ConcurrentDictionary<string, Ship> shipCoordinatesDictionary = new ConcurrentDictionary<string, Ship>();

            //Parse the ship inputs first, and TryAdd to {ship coord, ship number}
            foreach (var shipString in ships)
            {
                //This should most likely be refactored into either a Factory/Builder, but pressed for time
                PopulateShip(shipCoordinatesDictionary, shipString);

            }

            return shipCoordinatesDictionary;
        }

        private static int PlayGame(ConcurrentDictionary<string, Ship> shipCoordinates, string[] guesses)
        {
            int shipsSunk = 0;
            foreach(var guess in guesses)
            {
                bool isHit = shipCoordinates.TryGetValue(guess, out var ship);
                if (isHit)
                {
                    Console.WriteLine("Hit: " + guess);
                    ship.HealthPoints--;
                    if (ship.IsSunk())
                    {
                        //we've sunk the ship!
                        Console.WriteLine("Sunk ship!");
                        shipsSunk++;
                    }
                    shipCoordinates.TryRemove(guess, out ship);
                }
            }

            return shipsSunk;
        }

        private static void PopulateShip(ConcurrentDictionary<string, Ship> shipCoordinates, string ship)
        {
            //split the "ships" into ranges - "3:2", "3:3", etc
            string[] coordinateRange = ship.Split(',');
            //first coord is 3:2, store this as a key in the map
            var firstCoordinate = coordinateRange[0];
            if (shipCoordinates.ContainsKey(firstCoordinate))
            {
                throw new ArgumentException("Cannot intersect ships!");
            }
            //second coor is 3:5, in this case, it's a range where y==2,3,4,5 (size=4)
            var secondCoordinate = coordinateRange[1];
            //now parse the coordinates to determine the size/orientation(though unneeded), and ensure it is within bounds of our 10x10 board
            string[] firstCoordinates = firstCoordinate.Split(':');
            var firstX = firstCoordinates[0];
            var firstY = firstCoordinates[1];

            string[] secondCoordinates = secondCoordinate.Split(':');
            var secondX = secondCoordinates[0];
            var secondY = secondCoordinates[1];
            //create shared Ship reference, this means you can have multiple references to the same Ship on different coordinates
            Ship shipObject = new Ship();
            //now compare our coordinates to work out the size, and do any error checking here (running out of time!)
            //This can be refactored, but pressed for time - apologies as I took longer than expected getting used to C# syntax!
            if (firstX == secondX)
            {
                //We know this ship is horizontal
                GenerateHorizontalShipData(shipCoordinates, firstX, firstY, secondY, shipObject);
            }
            else if (firstY == secondY)
            {
                //We know this ship is vertical
                GenerateVerticalShipData(shipCoordinates, firstX, firstY, secondX, shipObject);
            }
            else
            {
                //we have a ship of size 1 potentially
                throw new Exception("Please enter ships in format 'X:Y,X:Y' where X/X or Y/Y is a valid range of 2-4 spaces!");
            }
            //TryAdd the first coordinate with the populated ship data
            shipCoordinates.TryAdd(firstCoordinate, shipObject);
            Console.WriteLine("Ship Data populated: " + shipCoordinates.ToString());
        }

        private static void GenerateHorizontalShipData(ConcurrentDictionary<string, Ship> shipCoordinates, string firstX, string firstY, string secondY, Ship shipObject)
        {
            //we know this is a horizontal ship, so coordinates must be in format 3:2, 3:5
            var firstYint = int.Parse(firstY);
            var secondYint = int.Parse(secondY);
            var size = calculateSize(firstYint, secondYint); //needs to be inclusive
            shipObject.HealthPoints = size;
            //create coordinates to TryAdd to map for fast lookup during the "Play" phase
            //because we can assume ships will be of size 2-4, we can avoid another loop here, though it would look cleaner to do this in a loop
            string stringCoordinate = null;
            //always TryAdd the the base case
            stringCoordinate = firstX + ":" + secondYint;
            shipCoordinates.TryAdd(stringCoordinate, shipObject);
            switch (size)
            {
                case 2:
                    //decrementally store in the map
                    stringCoordinate = firstX + ":" + (secondYint - 1);
                    shipCoordinates.TryAdd(stringCoordinate, shipObject);
                    break;
                case 3:
                    //decrementally store in the map
                    stringCoordinate = firstX + ":" + (secondYint - 2);
                    shipCoordinates.TryAdd(stringCoordinate, shipObject);
                    stringCoordinate = firstX + ":" + (secondYint - 1);
                    shipCoordinates.TryAdd(stringCoordinate, shipObject);
                    break;
                case 4:
                    //decrementally store in the map
                    stringCoordinate = firstX + ":" + (secondYint - 3);
                    shipCoordinates.TryAdd(stringCoordinate, shipObject);
                    stringCoordinate = firstX + ":" + (secondYint - 2);
                    shipCoordinates.TryAdd(stringCoordinate, shipObject);
                    stringCoordinate = firstX + ":" + (secondYint - 1);
                    shipCoordinates.TryAdd(stringCoordinate, shipObject);
                    break;
            }
        }

        private static void GenerateVerticalShipData(ConcurrentDictionary<string, Ship> shipCoordinates, string firstX, string firstY, string secondX, Ship shipObject)
        {
            //we know this is a horizontal ship, so coordinates must be in format 3:3, 5:3
            var firstXint = int.Parse(firstX);
            var secondXint = int.Parse(secondX);
            var size = calculateSize(firstXint, secondXint);
            shipObject.HealthPoints = size;
            //create coordinates to TryAdd to map for fast lookup during the "Play" phase
            //because we can assume ships will be of size 2-4, we can avoid another loop here, though it would look cleaner to do this in a loop
            string stringCoordinate = null;
            //always TryAdd the base case
            stringCoordinate = secondXint + ":" + firstY;
            shipCoordinates.TryAdd(stringCoordinate, shipObject);
            switch (size)
            {
                case 2:
                    //decrementally store in the map
                    stringCoordinate = (secondXint - 1) + ":" + firstY;
                    shipCoordinates.TryAdd(stringCoordinate, shipObject);
                    break;
                case 3:
                    //decrementally store in the map
                    stringCoordinate = (secondXint - 2) + ":" + firstY;
                    shipCoordinates.TryAdd(stringCoordinate, shipObject);
                    stringCoordinate = (secondXint - 1) + ":" + firstY;
                    shipCoordinates.TryAdd(stringCoordinate, shipObject);
                    break;
                case 4:
                    //decrementally store in the map
                    stringCoordinate = (secondXint - 3) + ":" + firstY;
                    shipCoordinates.TryAdd(stringCoordinate, shipObject);
                    stringCoordinate = (secondXint - 2) + ":" + firstY;
                    shipCoordinates.TryAdd(stringCoordinate, shipObject);
                    stringCoordinate = (secondXint - 1) + ":" + firstY;
                    shipCoordinates.TryAdd(stringCoordinate, shipObject);
                    break;
            }
        }

        private static int calculateSize(int first, int second)
        {
            int size = (second - first) + 1; //has to be inclusive - so for coordinate 5,2 the size should be 4 not 3;
            //do some simple error checking on the ship size
            if (size < 0)
            {
                //we were given a range not in support format
                throw new ArgumentException("Ship Range must be in format X(highest):Y(highest), X(lowest):Y(lowest). Eg: 3:2,3:5");
            }
            else if (size == 0 || size == 1)
            {
                //we were given a ship with invalid size
                throw new ArgumentException("Ship size must be at least 2");
            }
            else if (size > 4)
            {
                //we were given a ship with invalid size
                throw new ArgumentException("Ship size must be less than 4");
            }
            return size;
        }
    }
}
