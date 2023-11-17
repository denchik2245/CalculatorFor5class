using System;
using System.Collections.Generic;
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
            throw new ArgumentException("The base must be between 2 and " + digits.Length);

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
            throw new ArgumentException("The base must be between 2 and " + digits.Length);

        long result = 0;
        int exponent = 0;

        for (int i = number.Length - 1; i >= 0; i--)
        {
            char digit = number[i];
            int digitValue = digits.IndexOf(digit);

            if (digitValue == -1 || digitValue >= baseFrom)
                throw new ArgumentException("The number is not valid in the specified base.");

            result += digitValue * (long)Math.Pow(baseFrom, exponent);
            exponent++;
        }

        return result;
    }
    
    //Перевод в Римскую
    public static string ConvertToRoman(int number)
    {
        if (number < 1 || number > 5000)
            throw new ArgumentOutOfRangeException("Number must be in the range 1-5000.");

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

}

public class ConverterForm : Form
{
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
    

    private RadioButton rbToNumberSystem;
    private RadioButton rbToRoman;
    
    public ConverterForm()
    {
        InitializeComponent();
        UpdateUI();
    }

    private void InitializeComponent()
{
    this.inputLabel = new Label();
    this.inputTextBox = new TextBox();
    this.baseFromLabel = new Label();
    this.baseFromUpDown = new NumericUpDown();
    this.baseToLabel = new Label();
    this.baseToUpDown = new NumericUpDown();
    this.convertButton = new Button();
    this.resultLabel = new Label();
    
    this.processTextBox = new TextBox();
    this.processTextBox.Multiline = true;
    this.processTextBox.ScrollBars = ScrollBars.Vertical;
    this.processTextBox.Location = new System.Drawing.Point(280, 20);
    this.processTextBox.Size = new System.Drawing.Size(500, 210);

    
    this.rbToNumberSystem = new RadioButton();
    this.rbToRoman = new RadioButton();
    
    this.rbToRoman.Text = "Перевести в Римскую систему";
    this.rbToRoman.Location = new System.Drawing.Point(10, 160);
    this.rbToRoman.CheckedChanged += new EventHandler(rbToRoman_CheckedChanged);

    // Настройка TextBox для первого числа
    this.number1TextBox = new TextBox();
    this.number1TextBox.Location = new System.Drawing.Point(10, 250);
    this.number1TextBox.Size = new System.Drawing.Size(100, 20);

    // Настройка TextBox для второго числа
    this.number2TextBox = new TextBox();
    this.number2TextBox.Location = new System.Drawing.Point(120, 250);
    this.number2TextBox.Size = new System.Drawing.Size(100, 20);

    // Настройка кнопки для сложения
    this.addButton = new Button();
    this.addButton.Text = "Сложить";
    this.addButton.Location = new System.Drawing.Point(230, 250);
    this.addButton.Size = new System.Drawing.Size(75, 23);
    this.addButton.Click += new EventHandler(this.AddButton_Click);

    // Добавление элементов управления на форму
    this.Controls.Add(this.number1TextBox);
    this.Controls.Add(this.number2TextBox);
    this.Controls.Add(this.addButton);
    
    // Настройка inputLabel
    this.inputLabel.Text = "Введите число:";
    this.inputLabel.Location = new System.Drawing.Point(10, 20);
    this.inputLabel.Size = new System.Drawing.Size(100, 20);

    // Настройка inputTextBox
    this.inputTextBox.Location = new System.Drawing.Point(120, 10);
    this.inputTextBox.Size = new System.Drawing.Size(150, 20);

    // Настройка baseFromLabel
    this.baseFromLabel.Text = "Из системы счисления (2-50):";
    this.baseFromLabel.Location = new System.Drawing.Point(10, 40);
    this.baseFromLabel.Size = new System.Drawing.Size(180, 20);

    // Настройка baseFromUpDown
    this.baseFromUpDown.Location = new System.Drawing.Point(190, 40);
    this.baseFromUpDown.Minimum = 2;
    this.baseFromUpDown.Maximum = 50;
    this.baseFromUpDown.Size = new System.Drawing.Size(80, 20);
    this.baseFromUpDown.Value = 10;

    // Настройка baseToLabel
    this.baseToLabel.Text = "В систему счисления (2-50):";
    this.baseToLabel.Location = new System.Drawing.Point(10, 70);
    this.baseToLabel.Size = new System.Drawing.Size(180, 20);

    // Настройка baseToUpDown
    this.baseToUpDown.Location = new System.Drawing.Point(190, 70);
    this.baseToUpDown.Minimum = 2;
    this.baseToUpDown.Maximum = 50;
    this.baseToUpDown.Size = new System.Drawing.Size(80, 20);
    this.baseToUpDown.Value = 2;

    // Настройка convertButton
    this.convertButton.Text = "Конвертировать";
    this.convertButton.Location = new System.Drawing.Point(10, 100);
    this.convertButton.Size = new System.Drawing.Size(260, 30);
    this.convertButton.Click += new EventHandler(this.ConvertButton_Click);

    // Настройка resultLabel
    this.resultLabel.Location = new System.Drawing.Point(10, 220);
    this.resultLabel.Size = new System.Drawing.Size(380, 20);
    
    // Радиокнопки
    this.rbToNumberSystem.Text = "Перевести в другую систему счисления";
    this.rbToNumberSystem.Location = new System.Drawing.Point(10, 170);
    this.rbToNumberSystem.Size = new System.Drawing.Size(250, 20);
    this.rbToNumberSystem.Checked = true;
    
    this.rbToRoman.Text = "Перевести в Римскую систему";
    this.rbToRoman.Location = new System.Drawing.Point(10, 190);
    this.rbToRoman.Size = new System.Drawing.Size(250, 20);

    // Добавление компонентов на форму
    this.Controls.Add(this.inputLabel);
    this.Controls.Add(this.inputTextBox);
    this.Controls.Add(this.baseFromLabel);
    this.Controls.Add(this.baseFromUpDown);
    this.Controls.Add(this.baseToLabel);
    this.Controls.Add(this.baseToUpDown);
    this.Controls.Add(this.convertButton);
    this.Controls.Add(this.resultLabel);
    this.Controls.Add(this.rbToNumberSystem);
    this.Controls.Add(this.rbToRoman);
    this.Controls.Add(this.processTextBox);
    
    this.Text = "Конвертер Систем Счисления";
    this.Size = new System.Drawing.Size(800, 500);
}
    
    private void UpdateUI()
    {
        // Обновление интерфейса в зависимости от выбранной радиокнопки
        bool isToNumberSystem = rbToNumberSystem.Checked;
        this.baseFromLabel.Visible = isToNumberSystem;
        this.baseFromUpDown.Visible = isToNumberSystem;
        this.baseToLabel.Visible = isToNumberSystem;
        this.baseToUpDown.Visible = isToNumberSystem;
    }
    
    private void rbToNumberSystem_CheckedChanged(object sender, EventArgs e)
    {
        UpdateUI();
    }

    private void rbToRoman_CheckedChanged(object sender, EventArgs e)
    {
        UpdateUI();
    }

    private void AddButton_Click(object sender, EventArgs e)
    {
        // Очищаем processTextBox для нового вывода
        processTextBox.Clear();

        // Считываем числа и систему счисления
        string num1 = number1TextBox.Text;
        string num2 = number2TextBox.Text;
        int baseOfNumbers = (int)baseFromUpDown.Value; // Предполагаем, что система счисления для сложения указана в baseFromUpDown

        try
        {
            // Выполняем сложение и получаем пошаговое объяснение
            string additionSteps = NumberSystemConverter.AddNumbersInBase(num1, num2, baseOfNumbers);
            processTextBox.AppendText(additionSteps);
        }
        catch (Exception ex)
        {
            MessageBox.Show("Ошибка: " + ex.Message);
        }
    }

    private void ConvertButton_Click(object sender, EventArgs e)
    {
        processTextBox.Clear(); // Очистка текстового поля перед выводом нового процесса

        try
        {
            int baseFrom = (int)baseFromUpDown.Value;
            int baseTo = (int)baseToUpDown.Value;
            string inputNumber = inputTextBox.Text;

            if (rbToNumberSystem.Checked)
            {
                // Переводим в десятичную систему, если исходная система не десятичная
                long decimalNumber = baseFrom == 10 ? long.Parse(inputNumber) : NumberSystemConverter.ConvertToDecimal(inputNumber, baseFrom);
                string stepsToDecimal = baseFrom == 10 ? "" : GenerateStepsToDecimal(inputNumber, baseFrom);

                // Переводим из десятичной в целевую систему счисления
                string convertedNumber = NumberSystemConverter.ConvertFromDecimal(decimalNumber, baseTo);
                resultLabel.Text = "Результат: " + convertedNumber;

                string stepsToTargetBase = GenerateStepsToBase(decimalNumber, baseTo);

                // Выводим шаги в processTextBox
                processTextBox.AppendText(stepsToDecimal + stepsToTargetBase);
            }
            else if (rbToRoman.Checked)
            {
                if (int.TryParse(inputNumber, out int arabicNumber))
                {
                    if (arabicNumber >= 1 && arabicNumber <= 3999) // Ограничение для римских чисел
                    {
                        string romanNumber = NumberSystemConverter.ConvertToRoman(arabicNumber);
                        resultLabel.Text = "Римское: " + romanNumber;
                        processTextBox.AppendText("Переводим десятичное число " + arabicNumber + " в римское число:\n");
                        processTextBox.AppendText(arabicNumber + " -> " + romanNumber + "\n");
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
                        int decimalNumber = NumberSystemConverter.ConvertFromRoman(inputNumber);
                        resultLabel.Text = "Десятичное: " + decimalNumber;
                        processTextBox.AppendText("Переводим римское число " + inputNumber + " в десятичное число:\n");
                        processTextBox.AppendText(inputNumber + " -> " + decimalNumber + "\n");
                    }
                    catch (Exception)
                    {
                        MessageBox.Show("Некорректный ввод римского числа.");
                    }
                }
            }
        }
        catch (Exception ex)
        {
            MessageBox.Show("Ошибка: " + ex.Message);
        }
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

  
  private string GenerateStepsToDecimal(string number, int baseFrom)
  {
      StringBuilder steps = new StringBuilder();
      steps.AppendLine("Перевод числа из системы с основанием " + baseFrom + " в десятичную систему:");

      long result = 0;
      long multiplier = 1;
      for (int i = number.Length - 1; i >= 0; i--)
      {
          char digit = number[i];
          int digitValue = "0123456789ABCDEFGHIJKLMNOPQRSTUVWXYZ".IndexOf(digit);
          result += digitValue * multiplier;
          steps.AppendLine($"Шаг {number.Length - i}: Умножаем {digit} (значение {digitValue}) на {baseFrom}^{number.Length - 1 - i} (множитель {multiplier}). Промежуточный результат: {result}.");
          multiplier *= baseFrom;
      }

      steps.AppendLine("Результат перевода в десятичную систему: " + result + "\n");
      return steps.ToString();
  }
  
  private string GenerateStepsToBase(long decimalNumber, int baseTo)
  {
      StringBuilder steps = new StringBuilder();
      steps.AppendLine("Перевод десятичного числа " + decimalNumber + " в систему с основанием " + baseTo + ":");

      List<string> remainders = new List<string>();
      int stepNumber = 1;
      while (decimalNumber > 0)
      {
          long remainder = decimalNumber % baseTo;
          remainders.Add(remainder.ToString());
          decimalNumber /= baseTo;
          steps.AppendLine("Шаг " + stepNumber + ": Делим на " + baseTo + ", остаток от деления – " + remainder + ", новое число – " + decimalNumber + ".");
          stepNumber++;
      }

      remainders.Reverse();
      steps.AppendLine("Записываем все остатки в обратном порядке: " + string.Join("", remainders) + "\n");

      return steps.ToString();
  }
  
    [STAThread]
    static void Main()
    {
        Application.EnableVisualStyles();
        Application.SetCompatibleTextRenderingDefault(false);
        Application.Run(new ConverterForm());
    }
}