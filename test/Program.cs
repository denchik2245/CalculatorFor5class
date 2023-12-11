using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.Drawing;

public class NumberSystemConverter
{
    private static string digits = "0123456789ABCDEFGHIJKLMNOPQRSTUVWXYZabcdefghijklmnopqrst";

    //Перевод из 10 системы
    public static string ConvertFromDecimal(long decimalNumber, int baseTo)
    {
        if (baseTo < 2 || baseTo > digits.Length)
            throw new ArgumentException("Основание должно быть от 2 до " + digits.Length);

        StringBuilder result = new StringBuilder();
        long currentNumber = decimalNumber;

        while (currentNumber > 0)
        {
            result.Insert(0, digits[(int)(currentNumber % baseTo)]);
            currentNumber /= baseTo;
        }

        return result.Length > 0 ? result.ToString() : "0";
    }

    //Перевод в 10 систему
    public static long ConvertToDecimal(string number, int baseFrom)
    {
        if (baseFrom < 2 || baseFrom > digits.Length)
            throw new ArgumentException("Основание должно быть от 2 до " + digits.Length);

        long result = 0;
        int exponent = 0;

        for (int i = number.Length - 1; i >= 0; i--)
        {
            char digit = number[i];
            int digitValue = digits.IndexOf(digit);

            if (digitValue == -1 || digitValue >= baseFrom)
                throw new ArgumentException("Номер недействителен в указанной базе.");

            result += digitValue * (long)Math.Pow(baseFrom, exponent);
            exponent++;
        }

        return result;
    }
    
    //Объяснение в виде деления столбиком
    public static string GenerateTreeStepsToBase(long decimalNumber, int baseTo)
    {
        StringBuilder steps = new StringBuilder();
        List<string> divisionsAndRemainders = new List<string>(); // Список для хранения строк деления и остатков

        // Переменные для хранения промежуточных результатов
        long quotient = decimalNumber;
        long remainder;

        // Цикл, выполняющий последовательное деление
        while (quotient != 0)
        {
            remainder = quotient % baseTo;
            char remainderSymbol = digits[(int)remainder]; // Преобразование остатка в символ

            // Проверяем, нужно ли добавлять числовое значение в скобки
            string remainderDisplay = remainder >= 10 ? $"{remainderSymbol} ({remainder})" : remainderSymbol.ToString();

            divisionsAndRemainders.Add($"{quotient} | {baseTo}");
            divisionsAndRemainders.Add($"--- {remainderDisplay}"); // Добавляем символ с числом в скобках при необходимости
            quotient /= baseTo;
        }

        // Строим строку с делением слева направо
        int indent = 0; // Начальный отступ
        for (int i = 0; i < divisionsAndRemainders.Count; i += 2)
        {
            steps.AppendLine(new string(' ', indent) + divisionsAndRemainders[i]);
            if (i + 1 < divisionsAndRemainders.Count)
            {
                steps.AppendLine(new string(' ', indent) + divisionsAndRemainders[i + 1]);
            }
            indent += 10; // Увеличиваем отступ для следующего шага
        }

        // Добавление направления взгляда и результата конвертации
        steps.AppendLine(new string(' ', indent) + "<-- Считаем справа налево");
        steps.Append(new string(' ', 1) + "Получилось: ");
        steps.Append(string.Join("", divisionsAndRemainders.Where((_, index) => index % 2 != 0).Reverse()));
        steps.Append($" в системе счисления {baseTo}");

        return steps.ToString();
    }
    
    //Линейное объяснение перевода СС
    public static string ConvertToDecimalWithExplanation(string originalNumber, int baseFrom)
    {
        StringBuilder explanation = new StringBuilder();
        explanation.AppendLine("Перевод числа " + originalNumber + " из системы с основанием " + baseFrom + " в десятичную систему:");

        long decimalValue = 0;
        for (int i = 0; i < originalNumber.Length; i++)
        {
            char digit = originalNumber[i];
            int digitValue = digits.IndexOf(digit);
            decimalValue += digitValue * (long)Math.Pow(baseFrom, originalNumber.Length - i - 1);
            explanation.AppendLine($"{digit} ({digitValue}) * {baseFrom}^{originalNumber.Length - i - 1} = {digitValue * (long)Math.Pow(baseFrom, originalNumber.Length - i - 1)}");
        }

        explanation.AppendLine($"Все складываем. Итого в десятичной системе: {decimalValue}");
        return explanation.ToString();
    }

    public static string GenerateStepsToBase(long decimalNumber, int baseTo)
    {
        StringBuilder steps = new StringBuilder();
        steps.AppendLine("Процесс конвертации:");
        steps.AppendLine("Переводим целое число " + decimalNumber + " в систему с основанием " + baseTo + " последовательным делением на " + baseTo + ":");

        List<string> remainders = new List<string>();
        while (decimalNumber > 0)
        {
            long remainder = decimalNumber % baseTo;
            long quotient = decimalNumber / baseTo;
            char remainderSymbol = digits[(int)remainder];
            string remainderDisplay = remainder >= 10 ? $"{remainderSymbol} ({remainder})" : remainderSymbol.ToString();
            remainders.Add(remainderDisplay);
            steps.AppendLine($"{decimalNumber} / {baseTo} = {quotient}, остаток: {remainderDisplay}");
            decimalNumber = quotient;
        }

        remainders.Reverse();
        steps.AppendLine("Записываем все остатки в обратном порядке (снизу вверх): " + string.Join("", remainders));
        return steps.ToString();
    }
    
    //Перевод в Римскую
    public static string ConvertToRoman(int number)
    {
        if (number < 1 || number > 5000)
            throw new ArgumentOutOfRangeException("Число должно быть в диапазоне 1-5000.");

        string[] M = { "", "M", "MM", "MMM", "MMMM", "MMMMM" };
        string[] C = { "", "C", "CC", "CCC", "CD", "D", "DC", "DCC", "DCCC", "CM" };
        string[] X = { "", "X", "XX", "XXX", "XL", "L", "LX", "LXX", "LXXX", "XC" };
        string[] I = { "", "I", "II", "III", "IV", "V", "VI", "VII", "VIII", "IX" };

        return M[number / 1000] + C[(number % 1000) / 100] + X[(number % 100) / 10] + I[number % 10];
    }

    //Перевод из Римской
    public static int ConvertFromRoman(string roman)
    {
        roman = roman.ToUpper();
        Dictionary<char, int> romanMap = new Dictionary<char, int>
        {
            { 'I', 1 },
            { 'V', 5 },
            { 'X', 10 },
            { 'L', 50 },
            { 'C', 100 },
            { 'D', 500 },
            { 'M', 1000 }
        };

        int number = 0;
        for (int i = 0; i < roman.Length; i++)
        {
            if (i + 1 < roman.Length && romanMap[roman[i]] < romanMap[roman[i + 1]])
            {
                number -= romanMap[roman[i]];
            }
            else
            {
                number += romanMap[roman[i]];
            }
        }

        return number;
    }
    
    //Сложение
    public static string Summa(string num1, string num2, int baseOfNumbers)
    {
        if (baseOfNumbers > digits.Length)
        {
            throw new ArgumentException($"Основание системы счисления {baseOfNumbers} превышает количество доступных символов.");
        }

        StringBuilder result = new StringBuilder();
        int carry = 0;

        // Удаляем все пробелы для корректных вычислений
        num1 = num1.Replace(" ", "");
        num2 = num2.Replace(" ", "");

        int maxLength = Math.Max(num1.Length, num2.Length);
        num1 = num1.PadLeft(maxLength, '0');
        num2 = num2.PadLeft(maxLength, '0');

        for (int i = maxLength - 1; i >= 0; i--)
        {
            int digitValue1 = digits.IndexOf(num1[i]);
            int digitValue2 = digits.IndexOf(num2[i]);

            if (digitValue1 >= baseOfNumbers || digitValue2 >= baseOfNumbers)
            {
                throw new ArgumentException("Одна из цифр не соответствует заданной системе счисления.");
            }

            int sum = digitValue1 + digitValue2 + carry;
            carry = sum / baseOfNumbers;
            sum %= baseOfNumbers;

            result.Insert(0, digits[sum]);
        }

        if (carry > 0)
        {
            result.Insert(0, digits[carry]);
        }

        // Возвращаем результат без лидирующих нулей
        return result.ToString().TrimStart('0');
    }
    
    //Вычитание
    public static string Subtract(string num1, string num2, int baseOfNumbers)
    {
        // Конвертация чисел из исходной системы счисления в десятичную
        long decimalNum1 = ConvertToDecimal(num1, baseOfNumbers);
        long decimalNum2 = ConvertToDecimal(num2, baseOfNumbers);

        // Вычитание чисел в десятичной системе счисления
        long decimalResult = decimalNum1 - decimalNum2;
        
        if (decimalResult < 0)
        {
            throw new ArgumentException("Результат вычитания отрицательный.");
        }

        // Конвертация результата обратно в исходную систему счисления
        string result = ConvertFromDecimal(decimalResult, baseOfNumbers);

        // Возвращаем результат
        return result;
    }
    
    // Умножение
    public static string Multiply(string num1, string num2, int baseOfNumbers)
    {
        // Конвертация чисел из исходной системы счисления в десятичную
        long decimalNum1 = ConvertToDecimal(num1, baseOfNumbers);
        long decimalNum2 = ConvertToDecimal(num2, baseOfNumbers);

        // Умножение чисел в десятичной системе счисления
        long decimalResult = decimalNum1 * decimalNum2;

        // Конвертация результата обратно в исходную систему счисления
        string result = ConvertFromDecimal(decimalResult, baseOfNumbers);

        // Возвращаем результат
        return result;
    }
}

public class ConverterForm : Form
{
    //Создание обычного текста
    private Label inputLabel;
    private Label baseFromLabel;
    private Label baseToLabel;
    private Label resultLabel;
    private Label linearStepsLabel;
    private Label LabelforSumma;
    private Label LabelforSummaNum1;
    private Label LabelforSummaNum2;
    private Label resultForRoman;
    private Label resultForSumma;
    private Label StepsForSumma;
    private Label LabelforMulti;
    private Label LabelForSubstract;
    
    private RichTextBox stepsRichTextBox;
    private RichTextBox treeStepsRichTextBox;
    private DataGridView TableForSum;
    
    //Создание кнопок
    private Button convertButton;
    private Button sumButton;
    private Button convertButForRoman;
    private Button MultiButton;
    private Button SubstractButton;
    
    //Создание инпутов
    private TextBox inputTextBox;
    private TextBox processTextBox;
    private TextBox number1TextBox;
    private TextBox number2TextBox;
    
    //Создание стрелочек вверх вниз
    private NumericUpDown baseFromUpDown;
    private NumericUpDown baseToUpDown;
    private NumericUpDown UpDownFopSumma;
    
    //Создание радиокнопок
    private RadioButton rbToNumberSystem;
    private RadioButton rbToRoman;
    private RadioButton rbAddition;
    private RadioButton rbSubtraction;
    private RadioButton rbMultiplication;
    
    //Конструктор
    public ConverterForm()
    {
        InitializeComponent();
        UpdateUI();
    }
    
    private void InitializeComponent()
    {
        //Инициализация раздела Сложение
        LabelforSumma = new Label();
        LabelforSumma.Location = new Point(10, 50);
        LabelforSumma.Size = new Size(280, 20);
        LabelforSumma.Text = "Введите систему счисления складываемых чисел:";
        
        LabelforSummaNum1 = new Label();
        LabelforSummaNum1.Location = new Point(10, 90);
        LabelforSummaNum1.Size = new Size(133, 20);
        LabelforSummaNum1.Text = "Введите первое число:";
        
        LabelforSummaNum2 = new Label();
        LabelforSummaNum2.Location = new Point(10, 130);
        LabelforSummaNum2.Size = new Size(133, 20);
        LabelforSummaNum2.Text = "Введите второе число:";
        
        number1TextBox = new TextBox();
        number1TextBox.Location = new Point(145, 88);
        number1TextBox.Size = new Size(100, 20);
        
        number2TextBox = new TextBox();
        number2TextBox.Location = new Point(145, 128);
        number2TextBox.Size = new Size(100, 20);
        
        UpDownFopSumma = new NumericUpDown();
        UpDownFopSumma.Location = new Point(290, 48);
        UpDownFopSumma.Size = new Size(80, 20);
        UpDownFopSumma.Maximum = new decimal(new [] { 50, 0, 0, 0 });
        UpDownFopSumma.Minimum = new decimal(new [] { 2, 0, 0, 0 });
        UpDownFopSumma.Value = new decimal(new [] { 10, 0, 0, 0 });
        
        sumButton = new Button();
        sumButton.Location = new Point(10, 170);
        sumButton.Size = new Size(75, 23);
        sumButton.Text = "Сложить";
        sumButton.Click += (SumButton_Click);
        
        resultForSumma = new Label();
        resultForSumma.Location = new Point(90, 174);
        resultForSumma.Size = new Size(1050, 20);
        resultForSumma.Text = "Результат: ";
        
        /* StepsForSumma = new Label();
        StepsForSumma.Location = new Point(10, 220);
        StepsForSumma.Size = new Size(500, 300);
        StepsForSumma.Text = "";
        StepsForSumma.BorderStyle = BorderStyle.FixedSingle; */
        
        
        TableForSum = new DataGridView();
        TableForSum.Location = new Point(10, 220);
        TableForSum.Size = new Size(310, 190);
        
        int columnCount = 12; // или любое другое число столбцов, которое вам нужно
        for (int i = 0; i < columnCount; i++)
        {
            TableForSum.Columns.Add(new DataGridViewTextBoxColumn());
        }
        
        int desiredColumnWidth = 25;
        foreach (DataGridViewColumn col in TableForSum.Columns)
        {
            col.Width = desiredColumnWidth;
        }
        
        int rowCount = 13; // для 4 строк
        TableForSum.Rows.Add(rowCount);

        TableForSum.AllowUserToAddRows = false;
        TableForSum.AllowUserToDeleteRows = false;
        TableForSum.AllowUserToOrderColumns = false;
        TableForSum.AllowUserToResizeColumns = false;
        TableForSum.AllowUserToResizeRows = false;
        TableForSum.ColumnHeadersVisible = false;
        TableForSum.RowHeadersVisible = false;
        TableForSum.ReadOnly = true; // делаем ячейки только для чтения
        TableForSum.CellClick += (sender, e) => TableForSum.ClearSelection();
        
        
        //Инициализация раздела Перевести в Римскую
        inputLabel = new Label();
        inputLabel.Location = new Point(10, 50);
        inputLabel.Size = new Size(100, 20);
        inputLabel.Text = "Введите число:";
        
        inputTextBox = new TextBox();
        inputTextBox.Location = new Point(120, 48);
        inputTextBox.Size = new Size(150, 20);
        
        convertButForRoman = new Button();
        convertButForRoman.Location = new Point(12, 80);
        convertButForRoman.Size = new Size(260, 30);
        convertButForRoman.Text = "Конвертировать в Римскую/из Римской";
        convertButForRoman.Click += (ConvertButForRoman_Click);
        
        resultForRoman = new Label();
        resultForRoman.Location = new Point(10, 120);
        resultForRoman.Size = new Size(250, 20);
        resultForRoman.Text = "Римское: ";
        
        //Инициализация раздела Перевод в другую СС
        baseFromLabel = new Label();
        baseFromLabel.Location = new Point(10, 81);
        baseFromLabel.Size = new Size(180, 20);
        baseFromLabel.Text = "Из системы счисления (2-50):";
        
        baseToLabel = new Label();
        baseToLabel.Location = new Point(10, 110);
        baseToLabel.Size = new Size(180, 20);
        baseToLabel.Text = "В систему счисления (2-50):";
        
        baseFromUpDown = new NumericUpDown();
        baseFromUpDown.Location = new Point(190, 78);
        baseFromUpDown.Size = new Size(80, 20);
        baseFromUpDown.Maximum = new decimal(new [] { 50, 0, 0, 0 });
        baseFromUpDown.Minimum = new decimal(new [] { 2, 0, 0, 0 });
        baseFromUpDown.Value = new decimal(new [] { 10, 0, 0, 0 });
        
        baseToUpDown = new NumericUpDown();
        baseToUpDown.Location = new Point(190, 108);
        baseToUpDown.Size = new Size(80, 20);
        baseToUpDown.Maximum = new decimal(new [] { 50, 0, 0, 0 });
        baseToUpDown.Minimum = new decimal(new [] { 2, 0, 0, 0 });
        baseToUpDown.Value = new decimal(new [] { 2, 0, 0, 0 });
        
        convertButton = new Button();
        convertButton.Location = new Point(12, 138);
        convertButton.Size = new Size(260, 30);
        convertButton.Text = "Конвертировать";
        convertButton.Click += (ConvertButton_Click);
        
        resultLabel = new Label();
        resultLabel.Location = new Point(10, 180);
        resultLabel.Size = new Size(250, 20);
        resultLabel.Text = "Результат: ";
        
        linearStepsLabel = new Label();
        linearStepsLabel.Location = new Point(10, 220);
        linearStepsLabel.Size = new Size(500, 300);
        linearStepsLabel.Text = "";
        linearStepsLabel.BorderStyle = BorderStyle.FixedSingle;
        
        treeStepsRichTextBox = new RichTextBox();
        treeStepsRichTextBox.Location = new Point(530, 220);
        treeStepsRichTextBox.Size = new Size(600, 300);
        treeStepsRichTextBox.Text = "";
        
        //Умножить
        LabelforMulti = new Label();
        LabelforMulti.Location = new Point(10, 50);
        LabelforMulti.Size = new Size(280, 20);
        LabelforMulti.Text = "Введите систему счисления умножаемых чисел:";
        
        MultiButton = new Button();
        MultiButton.Location = new Point(10, 170);
        MultiButton.Size = new Size(75, 23);
        MultiButton.Text = "Умножить";
        MultiButton.Click += (MultiplyButton_Click);
        
        //Вычитание
        SubstractButton = new Button();
        SubstractButton.Location = new Point(10, 170);
        SubstractButton.Size = new Size(75, 23);
        SubstractButton.Text = "Вычесть";
        SubstractButton.Click += (SubtractButton_Click);
        
        LabelForSubstract = new Label();
        LabelForSubstract.Location = new Point(10, 50);
        LabelForSubstract.Size = new Size(280, 20);
        LabelForSubstract.Text = "Введите систему счисления вычитаемых чисел:";
        
        
        //Инициализация радиокнопок
        rbToNumberSystem = new RadioButton();
        rbToNumberSystem.Checked = true;
        rbToNumberSystem.Location = new Point(10, 10);
        rbToNumberSystem.Size = new Size(230, 20);
        rbToNumberSystem.TabStop = true;
        rbToNumberSystem.Text = "Перевести в другую систему счисления";
        rbToNumberSystem.CheckedChanged += new EventHandler(rbToNumberSystem_CheckedChanged);
        
        rbToRoman = new RadioButton();
        rbToRoman.Location = new Point(250, 10);
        rbToRoman.Size = new Size(140, 20);
        rbToRoman.Text = "Перевести в Римскую систему";
        rbToRoman.CheckedChanged += new EventHandler(rbToRoman_CheckedChanged);
        
        rbAddition = new RadioButton();
        rbAddition.Text = "Сложение";
        rbAddition.Location = new Point(400, 10); 
        rbAddition.Size = new Size(80, 20);
        rbAddition.CheckedChanged += new EventHandler(rbAddition_CheckedChanged);
        
        rbSubtraction = new RadioButton();
        rbSubtraction.Text = "Вычитание";
        rbSubtraction.Location = new Point(485, 10);
        rbSubtraction.Size = new Size(85, 20);
        rbSubtraction.CheckedChanged += new EventHandler(rbSubstract_CheckedChanged);

        rbMultiplication = new RadioButton();
        rbMultiplication.Text = "Умножение";
        rbMultiplication.Location = new Point(575, 10);
        rbMultiplication.Size = new Size(100, 20);
        rbMultiplication.CheckedChanged += new EventHandler(rbMulti_CheckedChanged);
        
        //Добавление элементов на форму
        Text = "Калькулятор для 5 классника";
        ClientSize = new Size(1100, 700);
        Controls.Add(inputLabel);
        Controls.Add(baseFromLabel);
        Controls.Add(baseToLabel);
        Controls.Add(resultLabel);
        Controls.Add(LabelforSumma);
        Controls.Add(LabelforSummaNum1);
        Controls.Add(LabelforSummaNum2);
        Controls.Add(resultForRoman);
        Controls.Add(resultForSumma);
        Controls.Add(StepsForSumma);
        Controls.Add(LabelforMulti);
        Controls.Add(LabelForSubstract);
        
        Controls.Add(TableForSum);
        
        Controls.Add(convertButton);
        Controls.Add(sumButton);
        Controls.Add(convertButForRoman);
        Controls.Add(MultiButton);
        Controls.Add(SubstractButton);
        
        Controls.Add(inputTextBox);
        Controls.Add(number1TextBox);
        Controls.Add(number2TextBox);
        
        Controls.Add(baseFromUpDown);
        Controls.Add(baseToUpDown);
        Controls.Add(UpDownFopSumma);
        
        Controls.Add(rbToNumberSystem);
        Controls.Add(rbToRoman);
        Controls.Add(rbAddition);
        Controls.Add(rbSubtraction);
        Controls.Add(rbMultiplication);
        
        Controls.Add(linearStepsLabel);
        Controls.Add(treeStepsRichTextBox);
        
        ((System.ComponentModel.ISupportInitialize)(baseFromUpDown)).BeginInit();
        ((System.ComponentModel.ISupportInitialize)(baseToUpDown)).BeginInit();
        SuspendLayout();
        ResumeLayout(false);
        PerformLayout();
    }
    
    //Обновление интерфейса в зависимости от выбранной операции
    private void UpdateUI()
    {
        // Проверка, какая радиокнопка выбрана
        bool isConversionSelected = rbToNumberSystem.Checked;
        bool isRomanSelected = rbToRoman.Checked;
        bool isAdditionSelected = rbAddition.Checked;
        bool isMultiplicationSelected = rbMultiplication.Checked;
        bool isSubstractSelected = rbSubtraction.Checked;

        // Включение/выключение элементов управления для конвертации
        baseFromLabel.Visible = isConversionSelected;
        baseFromUpDown.Visible = isConversionSelected;
        baseToLabel.Visible = isConversionSelected;
        baseToUpDown.Visible = isConversionSelected;
        
        // Кнопка "Конвертировать" и поле "Результат" доступны для конвертации и римских чисел
        convertButton.Visible = isConversionSelected;
        resultLabel.Visible = isConversionSelected;
        
        // Всегда показываем поле ввода числа
        inputTextBox.Visible = isConversionSelected || isRomanSelected;
        inputLabel.Visible = isConversionSelected || isRomanSelected;
        LabelforSumma.Visible = isAdditionSelected;

        //Перевести в Римскую
        convertButForRoman.Visible = isRomanSelected;
        resultForRoman.Visible = isRomanSelected;
        
        // Сложения
        LabelforSumma.Visible = isAdditionSelected;
        number1TextBox.Visible = isAdditionSelected || isMultiplicationSelected || isSubstractSelected;
        number2TextBox.Visible = isAdditionSelected || isMultiplicationSelected || isSubstractSelected;
        sumButton.Visible = isAdditionSelected;
        UpDownFopSumma.Visible = isAdditionSelected || isMultiplicationSelected || isSubstractSelected;
        LabelforSummaNum1.Visible = isAdditionSelected || isMultiplicationSelected || isSubstractSelected;
        LabelforSummaNum2.Visible = isAdditionSelected || isMultiplicationSelected || isSubstractSelected;
        resultForSumma.Visible = isAdditionSelected || isMultiplicationSelected || isSubstractSelected;
        TableForSum.Visible = isAdditionSelected || isMultiplicationSelected || isSubstractSelected;
        
        //Вычитание
        SubstractButton.Visible = isSubstractSelected;
        LabelForSubstract.Visible = isSubstractSelected;
        
        //Умножение
        LabelforMulti.Visible = isMultiplicationSelected;
        MultiButton.Visible = isMultiplicationSelected;
        
        // Включение/выключение шагов конвертации в зависимости от выбора операции
        linearStepsLabel.Visible = isConversionSelected;
        treeStepsRichTextBox.Visible = isConversionSelected;
    }
    
    // Функция для заполнения таблицы результатами сложения с переносами
    public static void FillSummationTable(DataGridView dataGridView, string num1, string num2, int baseOfNumbers)
{
    // Сначала вычисляем сумму и переносы
    int maxLength = Math.Max(num1.Length, num2.Length);
    num1 = num1.PadLeft(maxLength, '0');
    num2 = num2.PadLeft(maxLength, '0');

    // Переменные для хранения суммы и переноса
    string sumResult = "";
    int carryOver = 0;
    int[] carries = new int[maxLength]; // Массив для переносов

    // Расчет суммы и переносов
    for (int i = maxLength - 1; i >= 0; i--)
    {
        int sum = (num1[i] - '0') + (num2[i] - '0') + carryOver;
        carryOver = sum / baseOfNumbers;
        sumResult = (sum % baseOfNumbers) + sumResult;
        if (i > 0) // Сохраняем перенос над следующим разрядом
        {
            carries[i - 1] = carryOver;
        }
    }
    if (carryOver > 0)
    {
        sumResult = carryOver + sumResult;
    }

    // Заполнение таблицы
    dataGridView.Rows.Clear();
    dataGridView.Rows.Add(5); // Добавляем 5 строк для переноса, чисел и результата

    // Выводим переносы
    for (int i = 0; i < carries.Length; i++)
    {
        dataGridView.Rows[0].Cells[i + (dataGridView.ColumnCount - maxLength)].Value = carries[i] > 0 ? "1" : "";
    }

    // Выводим цифры первого числа
    for (int i = 0; i < num1.Length; i++)
    {
        dataGridView.Rows[1].Cells[i + (dataGridView.ColumnCount - maxLength)].Value = num1[i].ToString();
    }

    // Выводим цифры второго числа
    for (int i = 0; i < num2.Length; i++)
    {
        dataGridView.Rows[2].Cells[i + (dataGridView.ColumnCount - maxLength)].Value = num2[i].ToString();
    }

    // Выводим тире
    for (int i = 0; i < maxLength; i++)
    {
        dataGridView.Rows[3].Cells[i + (dataGridView.ColumnCount - maxLength)].Value = "-";
    }

    // Выводим результат сложения
    for (int i = 0; i < sumResult.Length; i++)
    {
        dataGridView.Rows[4].Cells[i + (dataGridView.ColumnCount - sumResult.Length)].Value = sumResult[i].ToString();
    }
}
    
    //Кнопка Сложить
    private void SumButton_Click(object sender, EventArgs e)
    {
        string num1 = number1TextBox.Text;
        string num2 = number2TextBox.Text;
        int baseOfNumbers = (int)UpDownFopSumma.Value;

        try
        {
            // Предполагается, что метод Summa корректно суммирует числа в указанной системе счисления
            string result = NumberSystemConverter.Summa(num1, num2, baseOfNumbers);
            resultForSumma.Text = "Результат сложения: " + result;

            // Очищаем таблицу перед заполнением новыми данными
            foreach (DataGridViewRow row in TableForSum.Rows)
            {
                foreach (DataGridViewCell cell in row.Cells)
                {
                    cell.Value = "";
                }
            }

            // Заполняем таблицу согласно результатам сложения
            FillSummationTable(TableForSum, num1, num2, baseOfNumbers);

            // Если вам нужно отобразить шаги сложения в текстовом виде
            /* string summationSteps = NumberSystemConverter.GenerateSummationStepsWithCarry(num1, num2, baseOfNumbers);
            StepsForSumma.Text = summationSteps; */
        }
        catch (Exception ex)
        {
            MessageBox.Show("Ошибка: " + ex.Message);
        }
    }
    
    public static void FillMultiplicationTable(DataGridView dataGridView, string num1, string num2, int baseOfNumbers)
    {
        // Очищаем таблицу перед заполнением
        dataGridView.Rows.Clear();
        int maxLength = num1.Length + num2.Length; // Максимально возможная длина результата
        dataGridView.ColumnCount = maxLength; // Устанавливаем количество столбцов

        // Устанавливаем ширину столбцов
        foreach (DataGridViewColumn col in dataGridView.Columns)
        {
            col.Width = 25;
        }

        // Добавляем строки для исходных чисел, разделителей, промежуточных результатов и итогового результата
        int totalRows = 2 + 1 + num2.Length + 1 + 1; // 2 исходных числа, 2 разделителя и итоговый результат
        dataGridView.Rows.Add(totalRows);

        // Заполнение строк исходными числами
        for (int i = 0; i < num1.Length; i++)
        {
            dataGridView.Rows[0].Cells[maxLength - num1.Length + i].Value = num1[i].ToString();
        }
        for (int i = 0; i < num2.Length; i++)
        {
            dataGridView.Rows[1].Cells[maxLength - num2.Length + i].Value = num2[i].ToString();
        }

        // Заполнение первого разделителя тире
        for (int i = 0; i < maxLength; i++)
        {
            dataGridView.Rows[2].Cells[i].Value = "-";
        }

        // Расчет и отображение промежуточных результатов умножения
        for (int i = 0; i < num2.Length; i++)
        {
            int digit2 = Convert.ToInt32(num2[num2.Length - 1 - i].ToString(), baseOfNumbers);
            int carry = 0;
            // Подготовка строки для промежуточного результата
            List<string> tempResult = Enumerable.Repeat(" ", maxLength).ToList();

            for (int j = 0; j < num1.Length; j++)
            {
                int digit1 = Convert.ToInt32(num1[num1.Length - 1 - j].ToString(), baseOfNumbers);
                int product = digit1 * digit2 + carry;
                carry = product / baseOfNumbers;
                product %= baseOfNumbers;

                // Заполняем строку с промежуточным результатом начиная справа
                tempResult[maxLength - 1 - (i + j)] = product.ToString();
            }

            // Если после умножения остался перенос, записываем его в следующий разряд
            if (carry > 0)
            {
                tempResult[maxLength - 1 - (i + num1.Length)] = carry.ToString();
            }

            // Добавляем промежуточный результат в DataGridView
            for (int k = 0; k < maxLength; k++)
            {
                dataGridView.Rows[i + 3].Cells[k].Value = tempResult[k]; // +3 учитывая исходные числа и строку с тире
            }
        }
        
        // Заполнение второго разделителя тире
        for (int i = 0; i < maxLength; i++)
        {
            dataGridView.Rows[3 + num2.Length].Cells[i].Value = "-";
        }

        // Вычисление итогового результата
        string result = NumberSystemConverter.Multiply(num1, num2, baseOfNumbers);

        // Отображение итогового результата
        for (int i = 0; i < result.Length; i++)
        {
            dataGridView.Rows[3 + num2.Length + 1].Cells[i].Value = result[i].ToString();
        }
}
    
    //Кнопка умножение
    private void MultiplyButton_Click(object sender, EventArgs e)
    {
        try
        {
            string num1 = number1TextBox.Text;
            string num2 = number2TextBox.Text;
            int baseOfNumbers = (int)UpDownFopSumma.Value;

            string result = NumberSystemConverter.Multiply(num1, num2, baseOfNumbers);
            resultForSumma.Text = "Результат умножения: " + result;

            ClearMultiplicationTable(TableForSum);
            FillMultiplicationTable(TableForSum, num1, num2, baseOfNumbers);
        }
        catch (Exception ex)
        {
            MessageBox.Show("Ошибка: " + ex.Message);
        }
    }
    
    private void ClearMultiplicationTable(DataGridView dataGridView)
    {
        dataGridView.Rows.Clear();
        foreach (DataGridViewColumn col in dataGridView.Columns)
        {
            col.Width = 25; // или любая другая ширина, соответствующая вашему дизайну
        }
    }

    public static void FillSubtractionTable(DataGridView dataGridView, string num1, string num2, int baseOfNumbers)
{
    // Очищаем таблицу перед заполнением
    dataGridView.Rows.Clear();
    int maxLength = Math.Max(num1.Length, num2.Length);
    dataGridView.ColumnCount = maxLength; // Устанавливаем количество столбцов

    // Устанавливаем ширину столбцов
    foreach (DataGridViewColumn col in dataGridView.Columns)
    {
        col.Width = 25;
    }

    // Добавляем строки для заимствований, чисел, разделителя и результата
    dataGridView.Rows.Add(5); // 1 для заимствований, 2 для чисел, 1 для разделителя, 1 для результата

    // Подготовка чисел, дополненных нулями слева
    num1 = num1.PadLeft(maxLength, '0');
    num2 = num2.PadLeft(maxLength, '0');

    // Массив для хранения заимствований
    string[] borrowings = new string[maxLength];
    string result = "";

    // Вычитание с заимствованиями
    int borrow = 0;
    for (int i = maxLength - 1; i >= 0; i--)
    {
        int digit1 = Convert.ToInt32(num1[i].ToString(), baseOfNumbers);
        int digit2 = Convert.ToInt32(num2[i].ToString(), baseOfNumbers) + borrow;
        borrow = 0;

        if (digit1 < digit2)
        {
            borrow = 1;
            digit1 += baseOfNumbers; // Заимствуем из следующего старшего разряда
            if (i > 0) // Убеждаемся, что не выходим за пределы массива
                borrowings[i - 1] = "*"; // Отмечаем заимствование
        }

        int difference = digit1 - digit2;
        result = Convert.ToString(difference, baseOfNumbers) + result;
    }

    // Выводим числа
    for (int i = 0; i < maxLength; i++)
    {
        dataGridView.Rows[1].Cells[i].Value = num1[i].ToString();
        dataGridView.Rows[2].Cells[i].Value = num2[i].ToString();
    }

    // Выводим заимствования
    for (int i = 0; i < maxLength; i++)
    {
        dataGridView.Rows[0].Cells[i].Value = borrowings[i];
    }

    // Выводим разделитель тире
    for (int i = 0; i < maxLength; i++)
    {
        dataGridView.Rows[3].Cells[i].Value = "-";
    }

    // Выводим результат вычитания
    for (int i = 0; i < result.Length; i++)
    {
        dataGridView.Rows[4].Cells[maxLength - result.Length + i].Value = result[i].ToString();
    }
}
    
    //Кнопка вычитание
    private void SubtractButton_Click(object sender, EventArgs e)
    {
        string num1 = number1TextBox.Text;
        string num2 = number2TextBox.Text;
        int baseOfNumbers = (int)UpDownFopSumma.Value;

        try
        {
            string result = NumberSystemConverter.Subtract(num1, num2, baseOfNumbers);
            resultForSumma.Text = "Результат вычитания: " + result;

            // Очищаем таблицу перед заполнением новыми данными
            foreach (DataGridViewRow row in TableForSum.Rows)
            {
                foreach (DataGridViewCell cell in row.Cells)
                {
                    cell.Value = "";
                }
            }

            // Заполняем таблицу согласно результатам вычитания
            FillSubtractionTable(TableForSum, num1, num2, baseOfNumbers);
        }
        catch (Exception ex)
        {
            MessageBox.Show("Ошибка: " + ex.Message);
        }
    }
    
    //Кнопка конвертировать для Римских
    private void ConvertButForRoman_Click(object sender, EventArgs e)
    {
        string conversionSteps = "";

        try
        {
            string inputNumber = inputTextBox.Text;

            // Попытка интерпретировать ввод как десятичное число
            if (int.TryParse(inputNumber, out int arabicNumber))
            {
                // Проверка на допустимый диапазон для римских чисел
                if (arabicNumber >= 1 && arabicNumber <= 3999)
                {
                    string romanNumber = NumberSystemConverter.ConvertToRoman(arabicNumber);
                    resultForRoman.Text = "Римское: " + romanNumber;
                    conversionSteps = "Переводим десятичное число " + arabicNumber + " в римское число:\n" + arabicNumber + " -> " + romanNumber + "\n";
                }
                else
                {
                    MessageBox.Show("Число для перевода в римскую систему должно быть в диапазоне от 1 до 3999.");
                }
            }
            else // В противном случае, предполагаем, что это римское число
            {
                try
                {
                    long decimalNumber = NumberSystemConverter.ConvertFromRoman(inputNumber);
                    resultForRoman.Text = "Десятичное: " + decimalNumber;
                    conversionSteps = "Переводим римское число " + inputNumber + " в десятичное число:\n" + inputNumber + " -> " + decimalNumber + "\n";
                }
                catch (Exception)
                {
                    MessageBox.Show("Некорректный ввод римского числа.");
                }
            }

            // Установка текста с шагами конвертации
            linearStepsLabel.Text = conversionSteps;
        }
        catch (Exception ex)
        {
            MessageBox.Show("Ошибка: " + ex.Message);
        }
    }
    
    //Кнопка конвертировать
    private void ConvertButton_Click(object sender, EventArgs e)
    {
        string conversionSteps = "";
    
        try
        {
            int baseFrom = (int)baseFromUpDown.Value;
            int baseTo = (int)baseToUpDown.Value;
            string inputNumber = inputTextBox.Text;
            long decimalNumber = 0;
            string linearConversionSteps = "";
            string treeConversionSteps = "";
        
            if (rbToNumberSystem.Checked)
            {
                // Переводим в десятичную систему, если исходная система не десятичная
                if (baseFrom != 10)
                {
                    decimalNumber = NumberSystemConverter.ConvertToDecimal(inputNumber, baseFrom);

                    // Генерация объяснения перевода в десятичную систему
                    linearConversionSteps += NumberSystemConverter.ConvertToDecimalWithExplanation(inputNumber, baseFrom) + "\n";
                }
                else
                {
                    decimalNumber = long.Parse(inputNumber);
                }

                // Переводим из десятичной в целевую систему счисления
                string convertedNumber = NumberSystemConverter.ConvertFromDecimal(decimalNumber, baseTo);
                resultLabel.Text = "Результат: " + convertedNumber;

                // Добавление линейного объяснения перевода из десятичной в целевую систему к общим шагам
                linearConversionSteps += NumberSystemConverter.GenerateStepsToBase(decimalNumber, baseTo);
            
                // Генерация древовидного объяснения
                treeConversionSteps = NumberSystemConverter.GenerateTreeStepsToBase(decimalNumber, baseTo);
            }

            linearStepsLabel.Text = linearConversionSteps;
            treeStepsRichTextBox.Text = treeConversionSteps;
        }
        catch (Exception ex)
        {
            MessageBox.Show("Ошибка: " + ex.Message);
        }
    }
    
    private void rbAddition_CheckedChanged(object sender, EventArgs e)
    {
        UpdateUI();
    }
    
    private void rbSubstract_CheckedChanged(object sender, EventArgs e)
    {
        UpdateUI();
    }
    
    private void rbMulti_CheckedChanged(object sender, EventArgs e)
    {
        UpdateUI();
    }
    
    private void rbToNumberSystem_CheckedChanged(object sender, EventArgs e)
    {
        UpdateUI();
    }
    
    private void rbToRoman_CheckedChanged(object sender, EventArgs e)
    {
        UpdateUI();
    }
    
  [STAThread]
  static void Main()
  {
      Application.EnableVisualStyles();
      Application.SetCompatibleTextRenderingDefault(false);
      
      Application.Run(new ConverterForm());
  }
}