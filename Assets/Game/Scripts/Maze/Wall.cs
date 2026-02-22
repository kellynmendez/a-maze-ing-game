
public class Wall
{
    public bool exists;
    public bool isDoor;
    public string side;
    public Cell parent;

    public Wall(Cell parent, string side)
    {
        this.exists = true;
        this.parent = parent;
        this.side = side;
        this.isDoor = false;
    }
}
