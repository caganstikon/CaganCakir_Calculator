using Microsoft.Extensions.DependencyInjection;

namespace CaganCakir_Calculator;

public partial class App : Application
{
	public App()
	{
		InitializeComponent();
	}

	protected override Window CreateWindow(IActivationState activationState)
	{
		return new Window(new AppShell());
	}
}