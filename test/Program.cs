using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;

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
    
    public static string AddNumbersInBase(string num1, string num2, int baseOfNumbers)
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
    public static string ConvertNumber(string number, int fromBase, int toBase)
    {
        long decimalNumber = ConvertToDecimal(number, fromBase);
        string convertedNumber = ConvertFromDecimal(decimalNumber, toBase);
        return convertedNumber;
    }

}


public class ConverterForm : Form
{
    // Определение элементов управления
    private Label inputLabel;
    private TextBox inputTextBox;
    private Label baseFromLabel;
    private NumericUpDown baseFromUpDown;
    private Label baseToLabel;
    private NumericUpDown baseToUpDown;
    private Button convertButton;
    private Label resultLabel;
    private TextBox processTextBox;
    private TextBox number1TextBox;
    private TextBox number2TextBox;
    private Button addButton;
    
    // Новые кнопки
    private Button convertNumbersButton;
    private Button convertToRomanButton;
    private Button additionButton;
    private Button subtractionButton;
    private Button multiplicationButton;
    
    // Конструктор формы
    public ConverterForm()
    {
        InitializeComponent();
    }

    // Инициализация компонентов формы
    private void InitializeComponent()
    {
        this.Size = new Size(800, 500);
        this.Text = "Конвертер Систем Счисления";
        InitializeControls();
        InitializeButtons();
        LayoutControls();
    }

    // Инициализация стандартных элементов управления
    private void InitializeControls()
    {
        inputLabel = new Label();
        inputTextBox = new TextBox();
        baseFromLabel = new Label();
        baseFromUpDown = new NumericUpDown();
        baseToLabel = new Label();
        baseToUpDown = new NumericUpDown();
        convertButton = new Button();
        resultLabel = new Label();
        processTextBox = new TextBox();
        number1TextBox = new TextBox();
        number2TextBox = new TextBox();
        addButton = new Button();
        
        // Инициализация новых кнопок
        InitializeButtons();

        // Настройка TextBox для первого числа
        number1TextBox.Location = new Point(10, 350);
        number1TextBox.Size = new Size(100, 20);

        // Настройка TextBox для второго числа
        number2TextBox.Location = new Point(120, 350);
        number2TextBox.Size = new Size(100, 20);

        // Настройка кнопки для сложения
        addButton.Text = "Сложить";
        addButton.Location = new Point(230, 350);
        addButton.Size = new Size(75, 23);
        addButton.Click += new EventHandler(AddButton_Click);

        // Настройка inputLabel
        inputLabel.Text = "Введите число:";
        inputLabel.Location = new Point(10, 220);
        inputLabel.Size = new Size(80, 20);

        // Настройка inputTextBox
        inputTextBox.Location = new Point(100, 220);
        inputTextBox.Size = new Size(150, 20);

        // Настройка baseFromLabel
        baseFromLabel.Text = "Из системы (2-50):";
        baseFromLabel.Location = new Point(10, 245);
        baseFromLabel.Size = new Size(80, 20);

        // Настройка baseFromUpDown
        baseFromUpDown.Location = new Point(100, 245);
        baseFromUpDown.Minimum = 2;
        baseFromUpDown.Maximum = 50;
        baseFromUpDown.Size = new Size(50, 20);
        baseFromUpDown.Value = 10;

        // Настройка baseToLabel
        baseToLabel.Text = "В систему (2-50):";
        baseToLabel.Location = new Point(10, 270);
        baseToLabel.Size = new Size(80, 20);

        // Настройка baseToUpDown
        baseToUpDown.Location = new Point(100, 270);
        baseToUpDown.Minimum = 2;
        baseToUpDown.Maximum = 50;
        baseToUpDown.Size = new Size(50, 20);
        baseToUpDown.Value = 2;

        // Настройка convertButton
        convertButton.Text = "Конвертировать";
        convertButton.Location = new Point(160, 245);
        convertButton.Size = new Size(100, 20);
        convertButton.Click += ConvertButton_Click;

        // Настройка resultLabel
        resultLabel.Location = new Point(10, 295);
        resultLabel.Size = new Size(760, 20);

        // Настройка processTextBox
        processTextBox.Multiline = true;
        processTextBox.ScrollBars = ScrollBars.Vertical;
        processTextBox.Location = new Point(250, 10);
        processTextBox.Size = new Size(760, 130);

        // Добавление элементов управления на форму
        this.Controls.Add(inputLabel);
        this.Controls.Add(inputTextBox);
        this.Controls.Add(baseFromLabel);
        this.Controls.Add(baseFromUpDown);
        this.Controls.Add(baseToLabel);
        this.Controls.Add(baseToUpDown);
        this.Controls.Add(convertButton);
        this.Controls.Add(resultLabel);
        this.Controls.Add(processTextBox);
        this.Controls.Add(this.number1TextBox);
        this.Controls.Add(this.number2TextBox);
        this.Controls.Add(this.addButton);
    }

    // Инициализация кнопок
    private void InitializeButtons()
    {
        convertNumbersButton = new Button();
        convertToRomanButton = new Button();
        additionButton = new Button();
        subtractionButton = new Button();
        multiplicationButton = new Button();

        // Настройка кнопок
        convertNumbersButton.Text = "Перевод чисел из любой системы в любую другую";
        convertToRomanButton.Text = "Перевод чисел в римскую систему";
        additionButton.Text = "Сложение чисел";
        subtractionButton.Text = "Вычитание чисел";
        multiplicationButton.Text = "Умножение чисел";

        // Обработчики событий для кнопок
        convertNumbersButton.Click += ConvertNumbersButton_Click;
        convertToRomanButton.Click += ConvertToRomanButton_Click;
        additionButton.Click += AdditionButton_Click;
        subtractionButton.Click += SubtractionButton_Click;
        multiplicationButton.Click += MultiplicationButton_Click;
        addButton.Click += new EventHandler(AddButton_Click);
        convertButton.Click += new EventHandler(ConvertButton_Click);


        // Добавление кнопок на форму
        this.Controls.Add(convertNumbersButton);
        this.Controls.Add(convertToRomanButton);
        this.Controls.Add(additionButton);
        this.Controls.Add(subtractionButton);
        this.Controls.Add(multiplicationButton);
    }

    // Расположение элементов управления на форме
    private void LayoutControls()
{
    int buttonWidth = 300;
    int buttonHeight = 30;
    int spacing = 10; // Меньшее значение для уменьшения пространства между кнопками
    int startX = 10;
    int startY = 10;

    // Расположение кнопок в верхней части формы
    convertNumbersButton.Size = new Size(buttonWidth, buttonHeight);
    convertNumbersButton.Location = new Point(startX, startY);

    convertToRomanButton.Size = new Size(buttonWidth, buttonHeight);
    convertToRomanButton.Location = new Point(startX, startY + buttonHeight + spacing);

    additionButton.Size = new Size(buttonWidth, buttonHeight);
    additionButton.Location = new Point(startX, startY + 2 * (buttonHeight + spacing));

    subtractionButton.Size = new Size(buttonWidth, buttonHeight);
    subtractionButton.Location = new Point(startX, startY + 3 * (buttonHeight + spacing));

    multiplicationButton.Size = new Size(buttonWidth, buttonHeight);
    multiplicationButton.Location = new Point(startX, startY + 4 * (buttonHeight + spacing));

    // Расположение полей ввода и результатов ниже кнопок
    int controlsStartY = startY + 5 * (buttonHeight + spacing);
    
    inputLabel.Location = new Point(startX, controlsStartY);
    inputTextBox.Location = new Point(startX + inputLabel.Width + spacing, controlsStartY);
    
    baseFromLabel.Location = new Point(startX, controlsStartY + inputTextBox.Height + spacing);
    baseFromUpDown.Location = new Point(startX + baseFromLabel.Width + spacing, controlsStartY + inputTextBox.Height + spacing);
    
    baseToLabel.Location = new Point(startX, controlsStartY + 2 * (inputTextBox.Height + spacing));
    baseToUpDown.Location = new Point(startX + baseToLabel.Width + spacing, controlsStartY + 2 * (inputTextBox.Height + spacing));
    
    convertButton.Location = new Point(startX, controlsStartY + 3 * (inputTextBox.Height + spacing));
    convertButton.Size = new Size(buttonWidth, buttonHeight);

    resultLabel.Location = new Point(startX, convertButton.Bottom + spacing);
    resultLabel.Size = new Size(buttonWidth, buttonHeight);

    processTextBox.Location = new Point(startX, resultLabel.Bottom + spacing);
    processTextBox.Size = new Size(buttonWidth, 100); // Высота больше для отображения всего процесса
}

    // Обработчики событий для кнопок
    
private void ConvertNumbersButton_Click(object sender, EventArgs e)
{
    // Скрываем все элементы
    ToggleControlsVisibility(false);
    // Показываем элементы для перевода чисел из любой системы счисления в любую другую
    ShowConversionControls();
    UpdateInterfaceForNumberConversion();
}

private void AddButton_Click(object sender, EventArgs e)
{
    // Считываем числа из текстовых полей
    string number1 = number1TextBox.Text;
    string number2 = number2TextBox.Text;
    int baseOfNumbers = (int)baseFromUpDown.Value; // Система счисления для сложения

    // Выполняем сложение в заданной системе счисления
    try
    {
        string result = NumberSystemConverter.AddNumbersInBase(number1, number2, baseOfNumbers);
        resultLabel.Text = $"Результат сложения: {result}";
    }
    catch (Exception ex)
    {
        // Если возникла ошибка, выводим сообщение
        MessageBox.Show("Ошибка при сложении чисел: " + ex.Message);
    }
}

// Обработчик нажатия кнопки "Конвертировать"
private void ConvertButton_Click(object sender, EventArgs e)
{
    // Считываем число и системы счисления из соответстющих полей
    string numberToConvert = inputTextBox.Text;
    int fromBase = (int)baseFromUpDown.Value;
    int toBase = (int)baseToUpDown.Value;

    // Выполняем конвертацию числа
    try
    {
        string convertedNumber = NumberSystemConverter.ConvertNumber(numberToConvert, fromBase, toBase);
        resultLabel.Text = $"Результат конвертации: {convertedNumber}";
    }
    catch (Exception ex)
    {
        // Если возникла ошибка, выводим сообщение
        MessageBox.Show("Ошибка при конвертации числа: " + ex.Message);
    }
}

private void ConvertToRomanButton_Click(object sender, EventArgs e)
{
    // Скрываем все элементы
    ToggleControlsVisibility(false);
    // Показываем элементы для перевода чисел в римскую систему счисления
    ShowConversionControls();
    UpdateInterfaceForRomanConversion();
}

private void AdditionButton_Click(object sender, EventArgs e)
{
    // Скрываем все элементы
    ToggleControlsVisibility(false);
    // Показываем элементы для операции сложения
    ShowAdditionControls();
    UpdateInterfaceForAddition();
}

private void SubtractionButton_Click(object sender, EventArgs e)
{
    MessageBox.Show("Функция 'Вычитание' ещё не реализована.");
}

private void MultiplicationButton_Click(object sender, EventArgs e)
{
    MessageBox.Show("Функция 'Умножение' ещё не реализована.");
}

// Методы для актуализации интерфейса под каждую функцию

private void UpdateInterfaceForNumberConversion()
{
    inputLabel.Text = "Введите число для конвертации:";
    baseFromLabel.Visible = true;
    baseFromUpDown.Visible = true;
    baseToLabel.Visible = true;
    baseToUpDown.Visible = true;
    convertButton.Visible = true;
    resultLabel.Visible = true;
    processTextBox.Visible = true;
}

private void UpdateInterfaceForRomanConversion()
{
    inputLabel.Text = "Введите десятичное число для конвертации в римское:";
    baseFromLabel.Visible = false;
    baseFromUpDown.Visible = false;
    baseToLabel.Visible = false;
    baseToUpDown.Visible = false;
    convertButton.Visible = true;
    resultLabel.Visible = true;
    processTextBox.Visible = true;
}

private void UpdateInterfaceForAddition()
{
    inputLabel.Text = "Введите числа для сложения:";
    number1TextBox.Visible = true;
    number2TextBox.Visible = true;
    addButton.Visible = true;
    baseFromLabel.Text = "Система счисления:";
    baseFromUpDown.Visible = true;
    resultLabel.Visible = true;
    processTextBox.Visible = true;
}

private void ShowConversionControls()
{
    inputLabel.Visible = true;
    inputTextBox.Visible = true;
    convertButton.Visible = true;
    resultLabel.Visible = true;
    processTextBox.Visible = true;
}

private void ShowAdditionControls()
{
    number1TextBox.Visible = true;
    number2TextBox.Visible = true;
    addButton.Visible = true;
    baseFromLabel.Visible = true;
    baseFromUpDown.Visible = true;
    resultLabel.Visible = true;
    processTextBox.Visible = true;
}

private void ToggleControlsVisibility(bool visible)
{
    // В этом методе вы можете скрыть или показать все элементы управления
    inputLabel.Visible = visible;
    inputTextBox.Visible = visible;
    baseFromLabel.Visible = visible;
    baseFromUpDown.Visible = visible;
    baseToLabel.Visible = visible;
    baseToUpDown.Visible = visible;
    convertButton.Visible = visible;
    resultLabel.Visible = visible;
    processTextBox.Visible = visible;
    number1TextBox.Visible = visible;
    number2TextBox.Visible = visible;
    addButton.Visible = visible;
    // Скрываем или показываем новые кнопки
    convertNumbersButton.Visible = !visible;
    convertToRomanButton.Visible = !visible;
    additionButton.Visible = !visible;
    subtractionButton.Visible = !visible;
    multiplicationButton.Visible = !visible;
}
    
    [STAThread]
    static void Main()
    {
        Application.EnableVisualStyles();
        Application.SetCompatibleTextRenderingDefault(false);
        Application.Run(new ConverterForm());
    }
}