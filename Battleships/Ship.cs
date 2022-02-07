namespace Battleships
{
    public class Ship
    {
        private int healthPoints;
        private int size;
        private bool isSunk;        

        public int HealthPoints { get => healthPoints; set => healthPoints = value; }
        public int Size { get => size; set => size = value; }
        public bool IsSunk()
        {
            if (healthPoints == 0)
            {
                return true;
            }
            else 
            { 
                return false; 
            }
        }
    }
}