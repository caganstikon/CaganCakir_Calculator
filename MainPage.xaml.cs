using System.Globalization;

namespace CaganCakir_Calculator;

public partial class MainPage : ContentPage
{
    double firstNumber = 0;
    string operation = "";
    bool newNumber = true;
    bool equalsPressed = false;
    bool errorOnScreen = false;

    public MainPage()
    {
        InitializeComponent();
    }

    // 0-9 butonları
    private void OnDigitClicked(object sender, EventArgs e)
    {
        Button button = (Button)sender;

        if (errorOnScreen == true)
        {
            ClearAll();
        }

        if (equalsPressed == true)
        {
            HistoryLabel.Text = "";
            equalsPressed = false;
        }

        if (newNumber == true)
        {
            DisplayLabel.Text = button.Text;
            newNumber = false;
        }
        else if (DisplayLabel.Text == "0")
        {
            DisplayLabel.Text = button.Text;
        }
        else
        {
            if (DisplayLabel.Text.Length < 16)
            {
                DisplayLabel.Text = DisplayLabel.Text + button.Text;
            }
        }
    }

    // nokta butonu
    private void OnDecimalClicked(object sender, EventArgs e)
    {
        if (errorOnScreen == true)
        {
            ClearAll();
        }

        if (equalsPressed == true)
        {
            HistoryLabel.Text = "";
            equalsPressed = false;
        }

        if (newNumber == true)
        {
            DisplayLabel.Text = "0.";
            newNumber = false;
            return;
        }

        // bir sayıda sadece bir tane nokta olabilir
        if (DisplayLabel.Text.Contains("."))
        {
            return;
        }

        DisplayLabel.Text = DisplayLabel.Text + ".";
    }

    // + - × ÷ butonları
    private void OnOperatorClicked(object sender, EventArgs e)
    {
        if (errorOnScreen == true)
        {
            return;
        }

        Button button = (Button)sender;
        string newOperation = button.Text;

        if (operation != "" && newNumber == true)
        {
            operation = newOperation;
            HistoryLabel.Text = ShowNumber(firstNumber) + " " + operation;
            return;
        }

        double secondNumber = ReadScreen();
        if (errorOnScreen == true)
        {
            return;
        }

        // zincirleme işlem: 5 + 5 - 2 yazınca önce 5 + 5 hesaplanır
        if (operation != "")
        {
            double result = Calculate(firstNumber, secondNumber, operation);
            if (errorOnScreen == true)
            {
                return;
            }
            firstNumber = result;
            DisplayLabel.Text = ShowNumber(result);
        }
        else
        {
            firstNumber = secondNumber;
        }

        operation = newOperation;
        HistoryLabel.Text = ShowNumber(firstNumber) + " " + operation;
        newNumber = true;
        equalsPressed = false;
    }

    // = butonu
    private void OnEqualsClicked(object sender, EventArgs e)
    {
        if (errorOnScreen == true)
        {
            return;
        }

        double secondNumber = ReadScreen();
        if (errorOnScreen == true)
        {
            return;
        }

        if (operation == "")
        {
            HistoryLabel.Text = ShowNumber(secondNumber) + " =";
            newNumber = true;
            equalsPressed = true;
            return;
        }

        HistoryLabel.Text = ShowNumber(firstNumber) + " " + operation + " " + ShowNumber(secondNumber) + " =";

        double result = Calculate(firstNumber, secondNumber, operation);
        if (errorOnScreen == true)
        {
            return;
        }

        DisplayLabel.Text = ShowNumber(result);
        firstNumber = 0;
        operation = "";
        newNumber = true;
        equalsPressed = true;
    }

    // % butonu
    private void OnPercentClicked(object sender, EventArgs e)
    {
        if (errorOnScreen == true)
        {
            return;
        }

        double number = ReadScreen();
        if (errorOnScreen == true)
        {
            return;
        }

        double result;
        if (operation == "+" || operation == "-")
        {
            // 200 + 10% -> 200'ün %10'u = 20
            result = firstNumber * number / 100;
        }
        else
        {
            result = number / 100;
        }

        DisplayLabel.Text = ShowNumber(result);
        newNumber = true;

        if (equalsPressed == true)
        {
            HistoryLabel.Text = "";
            equalsPressed = false;
        }
    }

    // C butonu
    private void OnClearClicked(object sender, EventArgs e)
    {
        ClearAll();
    }

    // ⌫ butonu (son karakteri siler)
    private void OnBackspaceClicked(object sender, EventArgs e)
    {
        if (errorOnScreen == true)
        {
            ClearAll();
            return;
        }

        if (newNumber == true)
        {
            return;
        }

        string text = DisplayLabel.Text;

        if (text.Length > 1)
        {
            text = text.Substring(0, text.Length - 1);
        }
        else
        {
            text = "0";
        }

        DisplayLabel.Text = text;
    }


    void ClearAll()
    {
        firstNumber = 0;
        operation = "";
        newNumber = true;
        equalsPressed = false;
        errorOnScreen = false;
        DisplayLabel.Text = "0";
        HistoryLabel.Text = "";
    }

    double Calculate(double number1, double number2, string op)
    {
        double result = 0;

        try
        {
            if (op == "+")
            {
                result = number1 + number2;
            }
            else if (op == "-")
            {
                result = number1 - number2;
            }
            else if (op == "×")
            {
                result = number1 * number2;
            }
            else if (op == "÷")
            {
                // sıfıra bölme kontrolü
                if (number2 == 0)
                {
                    throw new DivideByZeroException();
                }
                result = number1 / number2;
            }

            if (double.IsInfinity(result))
            {
                throw new OverflowException();
            }
        }
        catch (DivideByZeroException)
        {
            ShowError("Cannot divide by zero");
        }
        catch (OverflowException)
        {
            ShowError("Overflow");
        }

        return result;
    }

    double ReadScreen()
    {
        double number = 0;

        try
        {
            number = double.Parse(DisplayLabel.Text, CultureInfo.InvariantCulture);
        }
        catch (FormatException)
        {
            ShowError("Invalid input");
        }

        return number;
    }

    string ShowNumber(double number)
    {
        // 0.1 + 0.2 = 0.3 çıksın diye yuvarlıyoruz
        number = Math.Round(number, 10);
        return number.ToString(CultureInfo.InvariantCulture);
    }

    void ShowError(string message)
    {
        DisplayLabel.Text = message;
        errorOnScreen = true;
        firstNumber = 0;
        operation = "";
        newNumber = true;
        equalsPressed = false;
    }
}
