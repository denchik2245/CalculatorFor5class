using System;
using System.Collections.Generic;
using System.Text;
using System.Windows.Forms;

public class NumberSystemConverter
{
    private static string digits = "0123456789ABCDEFGHIJKLMNOPQRSTUVWXYZabcdefghijklmnopqrst";

    //Перевод из 10 системы
    public static string ConvertFromDecimal(long decimalNumber, int baseTo)
    {
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
            {'I', 1},
            {'V', 5},
            {'X', 10},
            {'L', 50},
            {'C', 100},
            {'D', 500},
            {'M', 1000}
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
    
    this.rbToNumberSystem = new RadioButton();
    this.rbToRoman = new RadioButton();
    
    this.rbToRoman.Text = "Перевести в Римскую систему";
    this.rbToRoman.Location = new System.Drawing.Point(10, 160);
    this.rbToRoman.CheckedChanged += new EventHandler(rbToRoman_CheckedChanged);

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
    
    this.Text = "Конвертер Систем Счисления";
    this.Size = new System.Drawing.Size(800, 800);
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

    private void ConvertButton_Click(object sender, EventArgs e)
    {
        try
        {
            if (rbToNumberSystem.Checked)
            {
                long decimalNumber = NumberSystemConverter.ConvertToDecimal(inputTextBox.Text, (int)baseFromUpDown.Value);
                string convertedNumber = NumberSystemConverter.ConvertFromDecimal(decimalNumber, (int)baseToUpDown.Value);
                resultLabel.Text = "Результат: " + convertedNumber;
            }
            else if (rbToRoman.Checked)
            {
                // Попытка преобразования введенного текста в число
                if (int.TryParse(inputTextBox.Text, out int arabicNumber))
                {
                    // Проверка диапазона для римского числа
                    if (arabicNumber >= 1 && arabicNumber <= 5000)
                    {
                        string romanNumber = NumberSystemConverter.ConvertToRoman(arabicNumber);
                        resultLabel.Text = "Римское: " + romanNumber;
                    }
                    else
                    {
                        MessageBox.Show("Число для перевода в римскую систему должно быть в диапазоне от 1 до 5000.");
                    }
                }
                else
                {
                    // Предположим, что введенная строка - это римское число
                    try
                    {
                        int decimalNumber = NumberSystemConverter.ConvertFromRoman(inputTextBox.Text);
                        resultLabel.Text = "Десятичное: " + decimalNumber.ToString();
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
    
    [STAThread]
    static void Main()
    {
        Application.EnableVisualStyles();
        Application.SetCompatibleTextRenderingDefault(false);
        Application.Run(new ConverterForm());
    }
}