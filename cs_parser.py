import os
import chardet


def detect_encoding(file_path):
    """
    Автоматически определяет кодировку файла
    """
    with open(file_path, 'rb') as file:
        raw_data = file.read()
        result = chardet.detect(raw_data)
        encoding = result['encoding']
        confidence = result['confidence']

        # Если определение неуверенное, пробуем распространенные кодировки
        if confidence < 0.7:
            for enc in ['utf-8', 'windows-1251', 'cp1251', 'iso-8859-1']:
                try:
                    raw_data.decode(enc)
                    return enc
                except UnicodeDecodeError:
                    continue
            return 'utf-8'  # fallback
        return encoding if encoding else 'utf-8'


def simple_cs_parser(root_dir, output_file):
    """
    Простой парсер C# файлов с обработкой разных кодировок,
    вкл й          ючающий в начало файла инструкцию для ИИ.
    """

    # --- ИНСТРУКЦИЯ ДЛЯ ИИ (ПРОМПТ) ---
    ai_instruction_prompt = """
--- ИНСТРУКЦИЯ ДЛЯ ИИ ---

РОЛЬ ИИ: Опытный C# разработчик игр на Unity.
  - В совершенстве владеет принципами SOLID, ООП и паттернами проектирования (например, ECS, паттерн Компонент).
  - Специализация: физика, контроллеры персонажей (особенно сложные, основанные на узлах/физике), оптимизация и архитектура кода.
  - Цель: Предоставлять точный, чистый и оптимальный C# код, объяснять сложные концепции, давать советы по архитектуре и помогать в реализации геймдизайнерских идей.

РОЛЬ ПОЛЬЗОВАТЕЛЯ: Начинающий геймдизайнер.
  - Заинтересован в понимании, почему код написан именно так, и как мои дизайнерские решения влияют на архитектуру.
  - Цель: Получать понятные объяснения, конкретные рекомендации по коду и реализовывать новые функции в соответствии с лучшими практиками.

ПРАВИЛА ОТВЕТА ИИ:
1. Всегда отвечай с точки зрения опытного C# разработчика, адаптируя сложные объяснения для начинающего геймдизайнера.
2. При предложении изменений в коде, ссылайся на конкретные принципы (например, "Разделение логики в SlimeCharacterController нарушает SRP, лучше использовать паттерн Компонент").
3. Указывай, в каком файле проекта следует внести изменения.
4. При внесении изменений в скрипт, пиши изменный скрипт полностью, а не отдельную часть.

--- КОНЕЦ ИНСТРУКЦИИ ДЛЯ ИИ ---\n
--- START OF FILE all_cs_files.txt ---
"""
    # ---------------------------------

    cs_files = []

    # Сначала соберем все файлы
    for root, dirs, files in os.walk(root_dir):
        for file in files:
            if file.endswith('.cs'):
                full_path = os.path.join(root, file)
                cs_files.append(full_path)

    if not cs_files:
        print("Не найдено .cs файлов!")
        return

    print(f"Найдено {len(cs_files)} .cs файлов")

    with open(output_file, 'w', encoding='utf-8') as out_f:

        # 1. Встраиваем промпт в самое начало файла
        out_f.write(ai_instruction_prompt)

        for i, cs_file in enumerate(cs_files, 1):
            print(f"Обработка {i}/{len(cs_files)}: {os.path.basename(cs_file)}")

            # Добавляем разделитель между файлами
            out_f.write(f"\n{'=' * 80}\n")
            out_f.write(f"ФАЙЛ: {cs_file}\n")
            out_f.write(f"{'=' * 80}\n\n")

            try:
                # Определяем кодировку
                encoding = detect_encoding(cs_file)
                print(f"  Кодировка: {encoding}")

                # Читаем файл в правильной кодировке
                with open(cs_file, 'r', encoding=encoding) as cs_f:
                    content = cs_f.read()
                    out_f.write(content)
                    out_f.write('\n')

            except Exception as e:
                print(f"  ОШИБКА: {e}")
                out_f.write(f"ОШИБКА ЧТЕНИЯ ФАЙЛА: {e}\n")
                # Пробуем прочитать как бинарный и сохранить сырые данные
                try:
                    with open(cs_file, 'rb') as bin_f:
                        raw_content = bin_f.read()
                        out_f.write("СОДЕРЖИМОЕ ФАЙЛА (сырые байты):\n")
                        out_f.write(str(raw_content))
                except Exception as e2:
                    out_f.write(f"Не удалось прочитать даже как бинарный файл: {e2}\n")

    print(f"\nВсе файлы объединены в: {output_file}")


# Использование
if __name__ == "__main__":
    root_directory = "C:\\Users\\mexanik01\\Documents\\GameDev\\Safe Rabbit\\Assets\\Sctipts"  # Ваш оригинальный путь
    output_filename = "all_cs_files.txt"

    simple_cs_parser(root_directory, output_filename)