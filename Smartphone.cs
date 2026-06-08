using System;

namespace ElectronicsStore
{
    /// <summary>
    /// Смартфон (Основная таблица, сторона «много»)
    /// </summary>
    public class Smartphone
    {
        /// <summary>Идентификатор смартфона (Первичный ключ)</summary>
        public int Id { get; set; }

        /// <summary>Идентификатор производителя (Внешний ключ -> manufacturer.mfg_id)</summary>
        public int ManufacturerId { get; set; }

        /// <summary>Коммерческое название модели</summary>
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