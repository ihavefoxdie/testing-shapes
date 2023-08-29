class PolygonForJson
{
    public int ID { get; set; }

    public int Width { get; set; }

    public int Height { get; set; }

    public decimal[] Center { get; set; }

    public decimal[][] JaggedVertices { get; set; }

    public string Name { get; set; }

    public PolygonForJson(int ID, int Width, int Height, decimal[] Center, decimal [][] JaggedVertices, string Name)
    {
        this.ID = ID;
        this.Width = Width;
        this.Height = Height;
        this.Center = Center;
        this.Name = Name;
        this.JaggedVertices = JaggedVertices;
    }

    public PolygonForJson()
    {
        
    }
}
