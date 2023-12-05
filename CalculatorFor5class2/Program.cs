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
            divisionsAndRemainders.Add($"{quotient} | {baseTo}");
            divisionsAndRemainders.Add($"--- {remainder}");
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
    
    //Умножение
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

    //Вычитание
    public static string Subtract(string num1, string num2, int baseOfNumbers)
    {
        // Конвертация чисел из исходной системы счисления в десятичную
        long decimalNum1 = ConvertToDecimal(num1, baseOfNumbers);
        long decimalNum2 = ConvertToDecimal(num2, baseOfNumbers);

        // Вычитание чисел в десятичной системе счисления
        long decimalResult = decimalNum1 - decimalNum2;

        // Если результат отрицательный, мы можем обрабатывать это по-разному,
        // например, возвращать результат с минусом или генерировать исключение.
        if (decimalResult < 0)
        {
            throw new ArgumentException("Результат вычитания отрицательный.");
        }

        // Конвертация результата обратно в исходную систему счисления
        string result = ConvertFromDecimal(decimalResult, baseOfNumbers);

        // Возвращаем результат
        return result;
    }

    //Сложение
    public static string Summa(string num1, string num2, int baseOfNumbers)
    {
        StringBuilder steps = new StringBuilder();
        steps.AppendLine($"Сложение чисел {num1} и {num2} в системе с основанием {baseOfNumbers}:\n");

        // Переводим числа в десятичную систему для упрощения процесса
        long decimalNum1 = ConvertToDecimal(num1, baseOfNumbers);
        long decimalNum2 = ConvertToDecimal(num2, baseOfNumbers);

        // Переменная для хранения переноса, если он есть
        long carry = 0;
        StringBuilder result = new StringBuilder();
        int position = 1; // Позиция разряда, начиная с единиц

        // Переводим в обратную строку, чтобы начать сложение с младших разрядов
        char[] num1Array = num1.Reverse().ToArray();
        char[] num2Array = num2.Reverse().ToArray();

        int maxLength = Math.Max(num1Array.Length, num2Array.Length);
        for (int i = 0; i < maxLength; i++)
        {
            long value1 = i < num1Array.Length ? digits.IndexOf(num1Array[i]) : 0;
            long value2 = i < num2Array.Length ? digits.IndexOf(num2Array[i]) : 0;
            long sum = value1 + value2 + carry;
            carry = sum / baseOfNumbers;
            sum = sum % baseOfNumbers;
            result.Insert(0, digits[(int)sum]);
        
            steps.AppendLine($"Шаг {position}: Складываем {value1} (из {num1}) и {value2} (из {num2}) " +
                             $"+ перенос {carry}. Результат = {sum} и перенос = {carry}.");

            position++;
        }

        // Если остался перенос после последнего сложения
        if (carry > 0)
        {
            result.Insert(0, digits[(int)carry]);
            steps.AppendLine($"Добавляем оставшийся перенос {carry} в старший разряд.");
        }

        steps.AppendLine($"Итоговый результат сложения: {result}");
        return steps.ToString();
    }
    
}

public class ConverterForm : Form
{
    //Создание обычного текста
    private Label inputLabel;
    private Label baseFromLabel;
    private Label baseToLabel;
    private Label resultLabel;
    private Label stepsLabel;
    private Label linearStepsLabel;
    
    private RichTextBox stepsRichTextBox;
    private RichTextBox treeStepsRichTextBox;
    
    //Создание кнопок
    private Button convertButton;
    private Button sumButton;
    
    //Создание инпутов
    private TextBox inputTextBox;
    private TextBox processTextBox;
    private TextBox number1TextBox;
    private TextBox number2TextBox;
    
    //Создание стрелочек вверх вниз
    private NumericUpDown baseFromUpDown;
    private NumericUpDown baseToUpDown;
    
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
        //Инициализация обычного текста
        inputLabel = new Label();
        inputLabel.Location = new Point(10, 50);
        inputLabel.Size = new Size(100, 20);
        inputLabel.Text = "Введите число:";
        
        baseFromLabel = new Label();
        baseFromLabel.Location = new Point(10, 81);
        baseFromLabel.Size = new Size(180, 20);
        baseFromLabel.Text = "Из системы счисления (2-50):";
        
        baseToLabel = new Label();
        baseToLabel.Location = new Point(10, 110);
        baseToLabel.Size = new Size(180, 20);
        baseToLabel.Text = "В систему счисления (2-50):";
        
        resultLabel = new Label();
        resultLabel.Location = new Point(10, 178);
        resultLabel.Size = new Size(250, 20);
        resultLabel.Text = "Результат: ";
        
        linearStepsLabel = new Label();
        linearStepsLabel.Location = new Point(10, 220);
        linearStepsLabel.Size = new Size(500, 300);
        linearStepsLabel.Text = "";
        linearStepsLabel.BorderStyle = BorderStyle.FixedSingle;
        
        
        //Деревовидное
        treeStepsRichTextBox = new RichTextBox();
        treeStepsRichTextBox.Location = new Point(530, 220);
        treeStepsRichTextBox.Size = new Size(600, 300);
        treeStepsRichTextBox.Text = "";
        
        //Инициализация кнопок
        convertButton = new Button();
        convertButton.Location = new Point(12, 138);
        convertButton.Size = new Size(260, 30);
        convertButton.Text = "Конвертировать";
        convertButton.Click += (ConvertButton_Click);
        
        sumButton = new Button();
        sumButton.Location = new Point(230, 100);
        sumButton.Size = new Size(75, 23);
        sumButton.Text = "Сложить";
        sumButton.Click += (SumButton_Click);
        sumButton.Click += new EventHandler(SumButton_Click);
        
        //Инициализация инпутов
        inputTextBox = new TextBox();
        inputTextBox.Location = new Point(120, 48);
        inputTextBox.Size = new Size(150, 20);
        
        number1TextBox = new TextBox();
        number1TextBox.Location = new Point(10, 100);
        number1TextBox.Size = new Size(100, 20);
        
        number2TextBox = new TextBox();
        number2TextBox.Location = new Point(120, 100);
        number2TextBox.Size = new Size(100, 20);
        
        //Инициализация стрелочек вверх вниз
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
        
        //Инициализация радиокнопок
        rbToNumberSystem = new RadioButton();
        rbToNumberSystem.Checked = true;
        rbToNumberSystem.Location = new Point(10, 10);
        rbToNumberSystem.Size = new Size(230, 20);
        rbToNumberSystem.TabStop = true;
        rbToNumberSystem.Text = "Перевести в другую систему счисления";
        
        rbToRoman = new RadioButton();
        rbToRoman.Location = new Point(250, 10);
        rbToRoman.Size = new Size(140, 20);
        rbToRoman.Text = "Перевести в Римскую систему";
        rbToRoman.CheckedChanged += (sender, e) => UpdateUI();
        
        rbAddition = new RadioButton();
        rbAddition.Text = "Сложение";
        rbAddition.Location = new Point(400, 10); 
        rbAddition.Size = new Size(80, 20);
        rbAddition.CheckedChanged += new EventHandler(rbAddition_CheckedChanged);
        
        rbSubtraction = new RadioButton();
        rbSubtraction.Text = "Вычитание";
        rbSubtraction.Location = new Point(485, 10);
        rbSubtraction.Size = new Size(85, 20);

        rbMultiplication = new RadioButton();
        rbMultiplication.Text = "Умножение";
        rbMultiplication.Location = new Point(575, 10);
        rbMultiplication.Size = new Size(100, 20);
        
        
        //Добавление элементов на форму
        Text = "Калькулятор для 5 классника";
        ClientSize = new Size(1100, 700);
        Controls.Add(inputLabel);
        Controls.Add(baseFromLabel);
        Controls.Add(baseToLabel);
        Controls.Add(resultLabel);
        
        Controls.Add(convertButton);
        Controls.Add(sumButton);
        
        Controls.Add(inputTextBox);
        Controls.Add(number1TextBox);
        Controls.Add(number2TextBox);
        
        Controls.Add(baseFromUpDown);
        Controls.Add(baseToUpDown);
        
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

        // Включение/выключение элементов управления для конвертации
        baseFromLabel.Visible = isConversionSelected;
        baseFromUpDown.Visible = isConversionSelected;
        baseToLabel.Visible = isConversionSelected;
        baseToUpDown.Visible = isConversionSelected;
        
        // Кнопка "Конвертировать" и поле "Результат" доступны для конвертации и римских чисел
        convertButton.Visible = isConversionSelected || isRomanSelected;
        resultLabel.Visible = isConversionSelected || isRomanSelected;
        
        // Всегда показываем поле ввода числа
        inputTextBox.Visible = true;
        
        // Включение/выключение элементов управления для сложения
        number1TextBox.Visible = isAdditionSelected;
        number2TextBox.Visible = isAdditionSelected;
        sumButton.Visible = isAdditionSelected;
        
        // Включение/выключение шагов конвертации в зависимости от выбора операции
        linearStepsLabel.Visible = isConversionSelected || isAdditionSelected;
        treeStepsRichTextBox.Visible = isConversionSelected;
    }
    
    //Кнопка Сложить
    // Обработчик события нажатия на кнопку "Сложение"
    private void SumButton_Click(object sender, EventArgs e)
    {
        // Очистка поля для вывода пошагового процесса
        processTextBox.Clear();

        // Сброс поля результата перед новым вычислением
        resultLabel.Text = "Результат: ";

        // Чтение чисел и системы счисления из текстовых полей
        string num1 = number1TextBox.Text;
        string num2 = number2TextBox.Text;
        int baseOfNumbers = (int)baseFromUpDown.Value;

        try
        {
            // Выполнение сложения и получение детального описания процесса
            string additionSteps = NumberSystemConverter.Summa(num1, num2, baseOfNumbers);
            processTextBox.AppendText(additionSteps);
        }
        catch (Exception ex)
        {
            // Вывод сообщения об ошибке
            MessageBox.Show("Ошибка: " + ex.Message);
        }
    }
    
    //Кнопка умножение
    private void MultiplyButton_Click(object sender, EventArgs e)
    {
        try
        {
            // Получаем числа и систему счисления
            string num1 = number1TextBox.Text;
            string num2 = number2TextBox.Text;
            int baseOfNumbers = (int)baseFromUpDown.Value;

            // Выполняем умножение и отображаем результат
            string result = NumberSystemConverter.Multiply(num1, num2, baseOfNumbers);
            resultLabel.Text = "Результат умножения: " + result;
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
            decimalNumber = baseFrom == 10 ? long.Parse(inputNumber) : NumberSystemConverter.ConvertToDecimal(inputNumber, baseFrom);

            // Генерация линейного объяснения
            linearConversionSteps = baseFrom == 10 ? "" : GenerateStepsToBase(decimalNumber, baseFrom);

            // Переводим из десятичной в целевую систему счисления
            string convertedNumber = NumberSystemConverter.ConvertFromDecimal(decimalNumber, baseTo);
            resultLabel.Text = "Результат: " + convertedNumber;

            // Добавление линейного объяснения к общим шагам
            linearConversionSteps += GenerateStepsToBase(decimalNumber, baseTo);

            // Генерация древовидного объяснения
            treeConversionSteps = NumberSystemConverter.GenerateTreeStepsToBase(decimalNumber, baseTo);
        }
        else if (rbToRoman.Checked)
        {
            if (int.TryParse(inputNumber, out int arabicNumber))
            {
                if (arabicNumber >= 1 && arabicNumber <= 3999) // Ограничение для римских чисел
                {
                    string romanNumber = NumberSystemConverter.ConvertToRoman(arabicNumber);
                    resultLabel.Text = "Римское: " + romanNumber;
                    conversionSteps = "Переводим десятичное число " + arabicNumber + " в римское число:\n" + arabicNumber + " -> " + romanNumber + "\n";
                }
                else
                {
                    MessageBox.Show("Число для перевода в римскую систему должно быть в диапазоне от 1 до 3999.");
                }
            }
            else
            {
                try
                {
                    decimalNumber = NumberSystemConverter.ConvertFromRoman(inputNumber);
                    resultLabel.Text = "Десятичное: " + decimalNumber;
                    conversionSteps = "Переводим римское число " + inputNumber + " в десятичное число:\n" + inputNumber + " -> " + decimalNumber + "\n";
                }
                catch (Exception)
                {
                    MessageBox.Show("Некорректный ввод римского числа.");
                }
            }
        }
        
        linearStepsLabel.Text = linearConversionSteps;
        treeStepsRichTextBox.Text = treeConversionSteps;
    }
    catch (Exception ex)
    {
        MessageBox.Show("Ошибка: " + ex.Message);
    }
}
    
    //Объяснение перевода из одной СС в другую
    private string GenerateStepsToBase(long decimalNumber, int baseTo)
    {
        StringBuilder steps = new StringBuilder();
        steps.AppendLine("Процесс конвертации:");
        steps.AppendLine("Переводим целое число " + decimalNumber + " в систему с основанием " + baseTo + " последовательным делением на " + baseTo + ":");

        List<string> remainders = new List<string>();
        while (decimalNumber > 0)
        {
            long remainder = decimalNumber % baseTo;
            long quotient = decimalNumber / baseTo;
            remainders.Add(remainder.ToString());
            steps.AppendLine($"{decimalNumber} / {baseTo} = {quotient}, остаток: {remainder}");
            decimalNumber = quotient;
        }

        remainders.Reverse();
        steps.AppendLine("Записываем все остатки в обратном порядке (снизу вверх): " + string.Join("", remainders));
        return steps.ToString();
    }
    
    // Обработчик события изменения состояния радиокнопки "Сложение"
    private void rbAddition_CheckedChanged(object sender, EventArgs e)
    {
        // Обновление интерфейса при изменении выбора операции
        UpdateUI();
    }
    
    // Обработчик события изменения состояния радиокнопки "Перевести в другую систему счисления"
    private void rbToNumberSystem_CheckedChanged(object sender, EventArgs e)
    {
        // Обновление интерфейса при изменении выбора операции
        UpdateUI();
    }

    // Обработчик события изменения состояния радиокнопки "Перевести в Римскую систему"
    private void rbToRoman_CheckedChanged(object sender, EventArgs e)
    {
        // Обновление интерфейса при изменении выбора операции
        UpdateUI();
    }
    
    private string ConvertToDecimalSteps(string inputNumber, int baseFrom)
    {
        StringBuilder steps = new StringBuilder();
        // Здесь должен быть ваш код для отображения шагов конвертации
        // Пример вывода:
        steps.Append("Рассмотрим каждый символ числа начиная с конца и умножим его на основание системы в степени его позиции:\n");
        for (int i = 0; i < inputNumber.Length; i++)
        {
            char digit = inputNumber[inputNumber.Length - 1 - i];
            int digitValue = "0123456789ABCDEFGHIJKLMNOPQRSTUVWXYZ".IndexOf(digit);
            steps.Append(digit + " * " + baseFrom + "^" + i + " = " + digitValue * (int)Math.Pow(baseFrom, i) + "\n");
        }
        steps.Append("Сложим все полученные значения.\n");
        return steps.ToString();
    }
    
  private string ConvertToBaseSteps(long decimalNumber, int baseTo)
    {
        StringBuilder steps = new StringBuilder();
        steps.Append("Процесс конвертации:\n");

        long quotient = decimalNumber;
        while (quotient != 0)
        {
            int remainder = (int)(quotient % baseTo);
            quotient /= baseTo;
            steps.Insert(0, $"Делим {quotient * baseTo} на {baseTo}, остаток {remainder}. Число теперь {quotient}.\n");
        }

        return steps.ToString();
    }
  
  // Точка входа в приложение
  [STAThread]
  static void Main()
  {
      // Настройка визуальных стилей и совместимости текстового рендеринга
      Application.EnableVisualStyles();
      Application.SetCompatibleTextRenderingDefault(false);

      // Запуск главной формы приложения
      Application.Run(new ConverterForm());
  }
}