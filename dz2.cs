using System;
using System.Text;
using System.IO;

namespace ElectronicsStore
{
    class Program
    {
        static void Main(string[] args)
        {

            Console.OutputEncoding = Encoding.UTF8;
            Console.InputEncoding = Encoding.UTF8;

            string dbFile = "smartphones.db";
            string mfgCsvPath = Path.Combine(AppContext.BaseDirectory, "manufacturer.csv");
            string phnCsvPath = Path.Combine(AppContext.BaseDirectory, "smartphone.csv");

            var db = new DatabaseManager(dbFile);
            db.InitializeDatabase(mfgCsvPath, phnCsvPath);

            string userChoice;
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
                userChoice = Console.ReadLine()?.Trim() ?? "";

                switch (userChoice)
                {
                    case "1": ShowAllBrands(db); break;
                    case "2": ShowAllPhones(db); break;
                    case "3": CreateNewPhone(db); break;
                    case "4": ModifyExistingPhone(db); break;
                    case "5": RemovePhoneFromDb(db); break;
                    case "6": ViewReportsMenu(db); break;
                    case "0": Console.WriteLine("\n[Инфо] Сессия закрыта. Программа успешно завершена."); break;
                    default: Console.WriteLine("\n[Предупреждение] Ошибка ввода: Такого пункта меню нет."); break;
                }
            } while (userChoice != "0");
        }

        static void ShowAllBrands(DatabaseManager db)
        {
            Console.WriteLine("\n--- Производители в системе ---");
            var brands = db.GetAllManufacturers();
            foreach (var b in brands) Console.WriteLine($" {b}");
        }

        static void ShowAllPhones(DatabaseManager db)
        {
            Console.WriteLine("\n--- Модели смартфонов в наличии ---");
            var phones = db.GetAllSmartphones();
            foreach (var p in phones) Console.WriteLine($" {p}");
            Console.WriteLine($"Всего на складе: {phones.Count} позиций.");
        }

        static void CreateNewPhone(DatabaseManager db)
        {
            Console.WriteLine("\n--- Регистрация нового смартфона ---");
            ShowAllBrands(db);

            Console.Write("Укажите числовой код производителя (ID): ");
            if (!int.TryParse(Console.ReadLine(), out int mfgId)) return;

            Console.Write("Введите название модели (например, Redmi Note 13): ");
            string name = Console.ReadLine()?.Trim() ?? "";
            if (string.IsNullOrEmpty(name)) return;

            Console.Write("Укажите розничную цену в рублях: ");
            if (!int.TryParse(Console.ReadLine(), out int price)) return;

            try
            {
                var phone = new Smartphone(0, mfgId, name, price);
                db.AddSmartphone(phone);
                Console.WriteLine("[Успех] Смартфон успешно добавлен в базу данных.");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"[Провал операции]: {ex.Message}");
            }
        }

        static void ModifyExistingPhone(DatabaseManager db)
        {
            Console.WriteLine("\n--- Изменение параметров существующей модели ---");
            Console.Write("Введите ID изменяемого смартфона: ");
            if (!int.TryParse(Console.ReadLine(), out int id)) return;

            var phone = db.GetSmartphoneById(id);
            if (phone == null)
            {
                Console.WriteLine("[Ошибка] Запись с таким ID не обнаружена в системе.");
                return;
            }

            Console.WriteLine($"Текущие данные: {phone}");
            Console.WriteLine("(Если поле не нужно менять, просто нажмите Enter)");

            Console.Write($"Новое наименование [{phone.Name}]: ");
            string nInput = Console.ReadLine()?.Trim() ?? "";
            if (nInput.Length > 0) phone.Name = nInput;

            Console.Write($"Новый ID бренда [{phone.ManufacturerId}]: ");
            string bInput = Console.ReadLine()?.Trim() ?? "";
            if (bInput.Length > 0 && int.TryParse(bInput, out int newMfgId)) phone.ManufacturerId = newMfgId;

            Console.Write($"Новая цена [{phone.Price}]: ");
            string pInput = Console.ReadLine()?.Trim() ?? "";
            if (pInput.Length > 0 && int.TryParse(pInput, out int newPrice))
            {
                try { phone.Price = newPrice; }
                catch (Exception ex) { Console.WriteLine($"[Ошибка]: {ex.Message}"); return; }
            }

            db.UpdateSmartphone(phone);
            Console.WriteLine("[Успех] Информация обновлена.");
        }

        static void RemovePhoneFromDb(DatabaseManager db)
        {
            Console.WriteLine("\n--- Удаление позиции ---");
            Console.Write("Введите ID смартфона, который нужно стереть: ");
            if (!int.TryParse(Console.ReadLine(), out int id)) return;

            var phone = db.GetSmartphoneById(id);
            if (phone == null) return;

            Console.Write($"Подтвердите действие: Стереть {phone.Name}? (да/нет): ");
            if (Console.ReadLine()?.Trim().ToLower() == "да")
            {
                db.DeleteSmartphone(id);
                Console.WriteLine("[Успех] Запись ликвидирована.");
            }
        }

        static void ViewReportsMenu(DatabaseManager db)
        {
            string repChoice;
            do
            {
                Console.WriteLine("\n-----------------------------------------");
                Console.WriteLine("          АНАЛИТИЧЕСКИЕ ОТЧЕТЫ           ");
                Console.WriteLine("-----------------------------------------");
                Console.WriteLine("1 - Прайс-лист смартфонов с привязкой бренда (JOIN)");
                Console.WriteLine("2 - Подсчет количества моделей по брендам (GROUP BY + COUNT)");
                Console.WriteLine("3 - Расчет средней стоимости техники бренда (GROUP BY + AVG)");
                Console.WriteLine("0 - Вернуться в основное меню");
                Console.Write("Выберите отчет: ");
                repChoice = Console.ReadLine()?.Trim() ?? "";

                switch (repChoice)
                {
                    case "1": ExecuteReport1(db); break;
                    case "2": ExecuteReport2(db); break;
                    case "3": ExecuteReport3(db); break;
                }
            } while (repChoice != "0");
        }


        static void ExecuteReport1(DatabaseManager db)
        {
            new ReportBuilder(db)
                .Query(@"SELECT p.phn_name, m.mfg_name, p.phn_price 
                         FROM smartphone p 
                         JOIN manufacturer m ON p.mfg_id = m.mfg_id 
                         ORDER BY p.phn_name ASC")
                .Title("ПОЛНЫЙ ПРАЙС-ЛИСТ СМАРТФОНОВ С УКАЗАНИЕМ БРЕНДА")
                .Header("Модель смартфона", "Бренд производитель", "Цена (руб.)")
                .ColumnWidths(25, 25, 15)
                .Footer("Всего наименований электроники в прайсе") // Использование функционала Группы В
                .Print();
        }


        static void ExecuteReport2(DatabaseManager db)
        {
            new ReportBuilder(db)
                .Query(@"SELECT m.mfg_name, COUNT(*) AS total_models 
                         FROM smartphone p 
                         JOIN manufacturer m ON p.mfg_id = m.mfg_id 
                         GROUP BY m.mfg_name 
                         ORDER BY total_models DESC")
                .Title("ОБЪЕМ МОДЕЛЬНОГО РЯДА ПО БРЕНДАМ")
                .Header("Бренд производитель", "Количество доступных моделей")
                .ColumnWidths(25, 30)
                .Footer("Проанализировано торговых марок") // Использование функционала Группы В
                .Print();
        }


        static void ExecuteReport3(DatabaseManager db)
        {
            new ReportBuilder(db)
                .Query(@"SELECT m.mfg_name, ROUND(AVG(p.phn_price), 2) AS average_cost 
                         FROM smartphone p 
                         JOIN manufacturer m ON p.mfg_id = m.mfg_id 
                         GROUP BY m.mfg_name 
                         ORDER BY average_cost DESC")
                .Title("СРЕДНЯЯ ЦЕНОВАЯ КАТЕГОРИЯ СМАРТФОНОВ")
                .Header("Бренд производитель", "Средняя стоимость (руб.)")
                .ColumnWidths(25, 25)
                .Footer("Количество исследованных сегментов") // Использование функционала Группы В
                .Print();
        }
    }
}