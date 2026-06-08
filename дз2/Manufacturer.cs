using System;

namespace ElectronicsStore
{
    public class Manufacturer
    {

        public int Id { get; set; }


        public string Name { get; set; }


        public Manufacturer(int id, string name)
        {
            Id = id;
            Name = name;
        }


        public Manufacturer() : this(0, "") { }


        public override string ToString() => $"[ID: {Id}] {Name}";
    }
}