using System;
using System.Text;

namespace ElectronicsStore
{
    class Program
    {
        static void Main(string[] args)
        {
            Console.OutputEncoding = Encoding.UTF8;
            Console.InputEncoding = Encoding.UTF8;

            var db = new DatabaseManager();
            db.Init();

            string choice;
            do
            {
                Console.WriteLine("\n=========================================");
                Console.WriteLine("    МЕНЮ УПРАВЛЕНИЯ МАГАЗИНОМ СМАРТФОНОВ  ");
                Console.WriteLine("=========================================");
                Console.WriteLine("1 - Вывести справочник производителей");
                Console.WriteLine("2 - Показать полный список смартфонов");
                Console.WriteLine("3 - Внести новый смартфон в базу");
                Console.WriteLine("4 - Модифицировать (изменить) параметры смартфона");
                Console.WriteLine("5 - Безвозвратно удалить смартфон");
                Console.WriteLine("6 - Перейти в блок аналитических Отчётов");
                Console.WriteLine("0 - Завершить работу");
                Console.Write("Введите номер действия: ");
                choice = Console.ReadLine()?.Trim() ?? "";

                switch (choice)
                {
                    case "1": ShowBrands(db); break;
                    case "2": ShowPhones(db); break;
                    case "3": CreatePhone(db); break;
                    case "4": ModifyPhone(db); break;
                    case "5": RemovePhone(db); break;
                    case "6": ViewReports(db); break;
                    case "0": Console.WriteLine("\nРабота завершена."); break;
                    default: Console.WriteLine("\nОшибка: такого пункта нет."); break;
                }
            } while (choice != "0");
        }

        static void ShowBrands(DatabaseManager db)
        {
            Console.WriteLine("\n--- Производители в системе ---");
            foreach (var b in db.GetBrands()) Console.WriteLine($" {b}");
        }

        static void ShowPhones(DatabaseManager db)
        {
            Console.WriteLine("\n--- Модели смартфонов в наличии ---");
            var list = db.GetPhones();
            foreach (var p in list) Console.WriteLine($" {p}");
            Console.WriteLine($"Всего на складе: {list.Count} позиций.");
        }

        static void CreatePhone(DatabaseManager db)
        {
            Console.WriteLine("\n--- Регистрация нового смартфона ---");
            ShowBrands(db);
            Console.Write("Укажите числовой код производителя (ID): ");
            if (!int.TryParse(Console.ReadLine(), out int mfgId)) return;

            Console.Write("Введите название модели: ");
            string name = Console.ReadLine()?.Trim() ?? "";
            if (string.IsNullOrEmpty(name)) return;

            Console.Write("Укажите розничную цену в рублях: ");
            if (!int.TryParse(Console.ReadLine(), out int price)) return;

            try
            {
                var testPhone = new Smartphone(0, mfgId, name, price);
                db.AddPhone(testPhone.ManufacturerId, testPhone.Name, testPhone.Price);
                Console.WriteLine("[Успех] Смартфон добавлен.");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"[Ошибка]: {ex.Message}");
            }
        }

        static void ModifyPhone(DatabaseManager db)
        {
            Console.WriteLine("\n--- Изменение параметров смартфона ---");
            Console.Write("Введите ID изменяемого смартфона: ");
            if (!int.TryParse(Console.ReadLine(), out int id)) return;

            var phone = db.GetPhoneById(id);
            if (phone == null)
            {
                Console.WriteLine("[Ошибка] Смартфон не найден.");
                return;
            }

            Console.WriteLine($"Текущие данные: {phone}");
            Console.Write($"Новое наименование [{phone.Name}]: ");
            string nInput = Console.ReadLine()?.Trim() ?? "";
            if (nInput.Length > 0) phone.Name = nInput;

            Console.Write($"Новая цена [{phone.Price}]: ");
            string pInput = Console.ReadLine()?.Trim() ?? "";
            if (pInput.Length > 0 && int.TryParse(pInput, out int newPrice))
            {
                try { phone.Price = newPrice; }
                catch (Exception ex) { Console.WriteLine($"[Ошибка]: {ex.Message}"); return; }
            }

            db.UpdatePhone(phone);
            Console.WriteLine("[Успех] Информация обновлена.");
        }

        static void RemovePhone(DatabaseManager db)
        {
            Console.WriteLine("\n--- Удаление позиции ---");
            Console.Write("Введите ID смартфона, который нужно удалить: ");
            if (!int.TryParse(Console.ReadLine(), out int id)) return;

            db.DeletePhone(id);
            Console.WriteLine("[Успех] Запись ликвидирована.");
        }

        static void ViewReports(DatabaseManager db)
        {
            string repChoice;
            do
            {
                Console.WriteLine("\n-----------------------------------------");
                Console.WriteLine("          АНАЛИТИЧЕСКИЕ ОТЧЕТЫ           ");
                Console.WriteLine("-----------------------------------------");
                Console.WriteLine("1 - Прайс-лист смартфонов (JOIN)");
                Console.WriteLine("2 - Подсчет количества моделей (GROUP BY + COUNT)");
                Console.WriteLine("3 - Расчет средней стоимости (GROUP BY + AVG)");
                Console.WriteLine("0 - Вернуться в основное меню");
                Console.Write("Выберите отчет: ");
                repChoice = Console.ReadLine()?.Trim() ?? "";

                switch (repChoice)
                {
                    case "1":
                        new ReportBuilder(db)
                            .Query("SELECT p.phn_name, m.mfg_name, p.phn_price FROM smartphone p JOIN manufacturer m ON p.mfg_id = m.mfg_id ORDER BY p.phn_name ASC")
                            .Title("ПОЛНЫЙ ПРАЙС-ЛИСТ СМАРТФОНОВ")
                            .Header("Модель смартфона", "Бренд", "Цена (руб.)")
                            .ColumnWidths(25, 20, 15)
                            .Footer("Всего наименований в прайсе")
                            .Print();
                        break;
                    case "2":
                        new ReportBuilder(db)
                            .Query("SELECT m.mfg_name, COUNT(*) FROM smartphone p JOIN manufacturer m ON p.mfg_id = m.mfg_id GROUP BY m.mfg_name")
                            .Title("ОБЪЕМ МОДЕЛЬНОГО РЯДА ПО БРЕНДАМ")
                            .Header("Бренд производитель", "Количество моделей")
                            .ColumnWidths(25, 25)
                            .Footer("Проанализировано торговых марок")
                            .Print();
                        break;
                    case "3":
                        new ReportBuilder(db)
                            .Query("SELECT m.mfg_name, ROUND(AVG(p.phn_price), 2) FROM smartphone p JOIN manufacturer m ON p.mfg_id = m.mfg_id GROUP BY m.mfg_name")
                            .Title("СРЕДНЯЯ ЦЕНА СМАРТФОНОВ ПО БРЕНДАМ")
                            .Header("Бренд производитель", "Средняя стоимость")
                            .ColumnWidths(25, 25)
                            .Footer("Количество исследованных сегментов")
                            .Print();
                        break;
                }
            } while (repChoice != "0");
        }
    }
}