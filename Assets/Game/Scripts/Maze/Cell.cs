
public class Cell
{
    public int row;
    public int col;
    public bool available;
    public Wall left;
    public Wall bottom;
    public Room room;

    public Cell(int row, int col)
    {
        this.row = row;
        this.col = col;
        left = new Wall(this, "L");
        bottom = new Wall(this, "B");
        available = true;
        room = null;
    }

}
