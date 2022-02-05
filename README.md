# Imagine a game of battleships...
The player has to guess the location of the opponent's 'ships' on a 10x10 grid. Ships are one unit wide and 2-4 units long, they may be placed vertically or horizontally. The player asks if a given co-ordinate is a hit or a miss. Once all cells representing a ship are hit - that ship is sunk.

## Ships
Each string represents a ship in the form first co-ordinate, last co-ordinate e.g. `"3:2,3:5"` is a 4 cell ship horizontally across the 4th row from the 3rd to the 6th column
## Guesses 
Each string represents the co-ordinate of a guess e.g. `"7:0"` - misses the ship above, "3:3" hits it.
## Returns
The number of ships sunk by the set of guesses
