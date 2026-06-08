using System;

namespace ElectronicsStore
{

    public class Smartphone
    {
        
        public int Id { get; set; }

        
        public int ManufacturerId { get; set; }


        public string Name { get; set; }

        private int _price;

        public int Price
        {
            get => _price;
            set
            {
                if (value < 0)
                    throw new ArgumentException("Критическая ошибка: Стоимость смартфона не может быть меньше нуля!");
                _price = value;
            }
        }


        public Smartphone(int id, int manufacturerId, string name, int price)
        {
            Id = id;
            ManufacturerId = manufacturerId;
            Name = name;
            Price = price; 
        }

        public Smartphone() : this(0, 0, "", 0) { }

        public override string ToString() => $"[ID: {Id}] {Name} | Код бренда: #{ManufacturerId} | Цена: {Price} руб.";
    }
}